using Blossom.Deployment.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Dependencies
{
    public class DependencyResolver
    {
        private List<Node> _nodes;

        private Dictionary<string, MethodInfo> _methods;

        public DependencyResolver(IEnumerable<MethodInfo> methods)
        {
            try
            {
                _methods = methods.ToDictionary(k => k.Name);
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(
                    "Cannot have multiple tasks with the same name (includes method overloads)",
                    exception);
            }
        }

        /// <summary>
        /// Resolves the dependencies in a task block.
        /// </summary>
        /// <returns>
        /// An enumerable of task methods in the order
        /// they should be executed.
        /// </returns>
        public IEnumerable<MethodInfo> OrderTasks()
        {
            BuildDependencyGraph();

            var resolved = new List<Node>();
            foreach (var node in _nodes)
            {
                Resolve(null, node, resolved, new HashSet<Node>());
            }
            return resolved.Select(n => n.Method);
        }

        /// <summary>
        /// Builds the initial dependency graph.
        /// </summary>
        /// <exception cref="TaskDependencyException"></exception>
        private void BuildDependencyGraph()
        {
            var nodeMap = _methods.ToDictionary(
                k => k.Key, 
                v => new Node(this, v.Value));

            // Add edges to all nodes.
            foreach (var pair in nodeMap)
            {
                foreach (var dependency in pair.Value.Dependencies)
                {
                    Node depend;
                    // Check that we have a node for the dependency.
                    if (!nodeMap.TryGetValue(dependency.Name, out depend))
                    {
                        throw new UnknownTaskException(String.Format(
                            "Unable to find task for dependency {0}. Is dependency marked with {1}?",
                            dependency.Name, typeof(TaskAttribute).Name));
                    }
                    pair.Value.Edges.Add(depend.Method.Name, depend);
                }
            }

            _nodes = nodeMap.OrderBy(m => m.Key).Select(m => m.Value).ToList();
        }

        private void Resolve(Node parent, Node curNode,
            List<Node> taskQueue, HashSet<Node> unresolved)
        {
            unresolved.Add(curNode);
            foreach (var edge in curNode.Edges)
            {
                var allowMultipleExecutionEdge = edge.Value.Method.
                    GetCustomAttribute<AllowMultipleExecutionAttribute>();
                if (!taskQueue.Contains(edge.Value) ||
                    allowMultipleExecutionEdge != null)
                {
                    if (unresolved.Contains(edge.Value))
                    {
                        throw new CircularTaskDependencyException(
                            "Circular dependency for " + edge.Value.Method.Name);
                    }
                    Resolve(curNode, edge.Value, taskQueue, unresolved);
                }
            }
            unresolved.Remove(curNode);
            var allowMultipleExecution = curNode.Method.
                    GetCustomAttribute<AllowMultipleExecutionAttribute>();
            if (allowMultipleExecution == null && !taskQueue.Contains(curNode) ||
                (allowMultipleExecution != null && (
                    parent != null || parent == null && allowMultipleExecution.Standalone)))
            {
                taskQueue.Add(curNode);
            }
        }

        /// <summary>
        /// Finds a task in the taskblock with the given name.
        /// </summary>
        /// <param name="taskName">Name of task to find.</param>
        /// <returns>The method for the found task.</returns>
        /// <exception cref="System.AmbiguousMatchException"></exception>
        /// <exception cref="TaskDependencyException"></exception>
        internal MethodInfo GetTaskForName(string taskName)
        {
            MethodInfo method;
            if (!_methods.TryGetValue(taskName, out method))
            {
                throw new UnknownTaskException("Unable to find task " + taskName);
            }
            return method;
        }

        private class Node
        {
            private DependencyResolver _resolver;

            internal MethodInfo Method { get; private set; }

            internal SortedList<string, Node> Edges { get; private set; }

            private List<MethodInfo> _dependencies;
            internal List<MethodInfo> Dependencies
            {
                get
                {
                    if (_dependencies == null)
                    {
                        _dependencies = GenerateDependencies();
                    }
                    return _dependencies;
                }
            }

            internal Node(DependencyResolver resolver, MethodInfo method)
            {
                _resolver = resolver;
                Method = method;
                Edges = new SortedList<string, Node>();
            }

            private List<MethodInfo> GenerateDependencies()
            {
                var dependencies = new List<MethodInfo>();
                foreach (var taskName in Method.
                    GetCustomAttributes<DependsAttribute>().Select(a => a.TaskName)
                    .Distinct())
                {
                    dependencies.Add(_resolver.GetTaskForName(taskName));
                }
                return dependencies;
            }

            public override string ToString()
            {
                return Method != null ? Method.Name : "<Null>";
            }
        }
    }
}
