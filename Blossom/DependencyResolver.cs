﻿using Blossom.Attributes;
using Blossom.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DeploymentUnitTest")]

namespace Blossom
{
    internal class DependencyResolver
    {
        private List<Node> _nodes;

        private IEnumerable<MethodInfo> RootMethods { get; set; }

        private Dictionary<string, MethodInfo> AllMethods { get; set; }

        internal DependencyResolver(List<MethodInfo> methods)
            : this(methods, methods) { }

        internal DependencyResolver(
            IEnumerable<MethodInfo> rootMethods,
            IEnumerable<MethodInfo> allMethods)
        {
            RootMethods = rootMethods;
            try
            {
                AllMethods = allMethods.ToDictionary(MethodName);
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(
                    "Cannot have multiple tasks with the same name (includes method overloads)",
                    exception);
            }
        }

        private static string MethodName(MethodInfo invokable)
        {
            return invokable.ReflectedType.Namespace + "." + invokable.Name;
        }

        /// <summary>
        /// Resolves the dependencies in a task block.
        /// </summary>
        /// <returns>
        /// An enumerable of task methods in the order
        /// they should be executed.
        /// </returns>
        internal IEnumerable<MethodInfo> OrderTasks()
        {
            BuildDependencyGraph();

            var resolved = new List<Node>();
            // Limit iteration to only the root methods.
            // Only root methods and their dependencies should
            // appear in the final order.
            foreach (var node in _nodes.Where(n => RootMethods.Contains(n.Invokable)))
            {
                Resolve(null, node, resolved, new HashSet<Node>());
            }
            return resolved.Select(n => n.Invokable);
        }

        /// <summary>
        /// Builds the initial dependency graph.
        /// </summary>
        /// <exception cref="TaskDependencyException"></exception>
        private void BuildDependencyGraph()
        {
            var nodeMap = AllMethods.ToDictionary(
                k => k.Key, 
                v => new Node(this, v.Value));

            // Add edges to all nodes.
            foreach (var pair in nodeMap)
            {
                foreach (var dependency in pair.Value.Dependencies)
                {
                    Node depend;
                    // Check that we have a node for the dependency.
                    if (!nodeMap.TryGetValue(MethodName(dependency), out depend))
                    {
                        throw new UnknownTaskException(String.Format(
                            "Unable to find task for dependency {0}. Is dependency marked with {1}?",
                            MethodName(dependency), typeof(TaskAttribute).Name));
                    }
                    pair.Value.Edges.Add(MethodName(depend.Invokable), depend);
                }
            }

            _nodes = nodeMap.OrderBy(m => m.Key).Select(m => m.Value).ToList();
        }

        private static void Resolve(Node parent, Node curNode,
            ICollection<Node> taskQueue, ISet<Node> unresolved)
        {
            unresolved.Add(curNode);
            foreach (var edge in curNode.Edges)
            {
                var allowMultipleExecutionEdge = edge.Value.Invokable.
                    GetCustomAttribute<AllowMultipleExecutionAttribute>();
                if (!taskQueue.Contains(edge.Value) ||
                    allowMultipleExecutionEdge != null)
                {
                    if (unresolved.Contains(edge.Value))
                    {
                        throw new CircularTaskDependencyException(
                            "Circular dependency for " + MethodName(edge.Value.Invokable));
                    }
                    Resolve(curNode, edge.Value, taskQueue, unresolved);
                }
            }
            unresolved.Remove(curNode);
            var allowMultipleExecution = curNode.Invokable.
                    GetCustomAttribute<AllowMultipleExecutionAttribute>();
            if (allowMultipleExecution == null && !taskQueue.Contains(curNode) ||
                (allowMultipleExecution != null && (
                    parent != null || allowMultipleExecution.Standalone)))
            {
                taskQueue.Add(curNode);
            }
        }

        /// <summary>
        /// Finds a task in the taskblock with the given name.
        /// </summary>
        /// <param name="taskName">Name of task to find.</param>
        /// <returns>The method for the found task.</returns>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="TaskDependencyException"></exception>
        internal MethodInfo GetTaskForName(string taskName)
        {
            MethodInfo method;
            if (!AllMethods.TryGetValue(taskName, out method))
            {
                throw new UnknownTaskException("Unable to find task " + taskName);
            }
            return method;
        }

        private class Node
        {
            private readonly DependencyResolver _resolver;

            internal MethodInfo Invokable { get; private set; }

            internal SortedList<string, Node> Edges { get; private set; }

            private List<MethodInfo> _dependencies;
            internal IEnumerable<MethodInfo> Dependencies
            {
                get { return _dependencies ?? (_dependencies = GenerateDependencies()); }
            }

            internal Node(DependencyResolver resolver, MethodInfo method)
            {
                _resolver = resolver;
                Invokable = method;
                Edges = new SortedList<string, Node>();
            }

            private List<MethodInfo> GenerateDependencies()
            {
                return Invokable
                    .GetCustomAttributes<DependsAttribute>()
                    .Select(a => a.TaskName)
                    .Distinct()
                    .Select(taskName => _resolver
                        .GetTaskForName(Invokable.ReflectedType.Namespace + "." + taskName))
                    .ToList();
            }

            public override string ToString()
            {
                return Invokable != null ? MethodName(Invokable) : "<Null>";
            }
        }
    }
}
