using Blossom.Deployment.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blossom.Deployment.Manager
{
    public class DeploymentManager : IDeploymentManager
    {
        private DeploymentConfig Config { get; set; }

        private IEnumerable<Invokable> Tasks { get; set; }

        private IEnumerable<Invokable> InitializationMethods { get; set; }

        private IEnumerable<Invokable> CleanupMethods { get; set; }

        private IEnumerable<ExecutionPlan> ExecutionPlans { get; set; }

        public DeploymentManager(DeploymentConfig config, string taskNamespace)
        {
            Config = config;

            var taskBlocks = FindNamespace(taskNamespace);
            InitializeTasks(taskBlocks);
        }

        public DeploymentManager(DeploymentConfig config, params object[] taskBlocks)
        {
            Config = config;
            InitializeTasks(taskBlocks);
        }

        private static object[] FindNamespace(string ns)
        {
            throw new NotImplementedException(); // TODO find and initialize all classes in namespace
        }

        private void InitializeTasks(object[] taskBlocks)
        {
            Tasks = taskBlocks.SelectMany(
                b => b.GetType().GetMethods(),
                (o, m) => new Invokable
                {
                    Base = o,
                    Method = m
                }).
                Where(t =>
                    t.Method.GetCustomAttribute<TaskAttribute>() != null &&
                    t.Method.GetCustomAttribute<DeploymentInitializeAttribute>() == null &&
                    t.Method.GetCustomAttribute<DeploymentCleanupAttribute>() == null);

            var uncallableTasks = Tasks.Where(t => {
                var param = t.Method.GetParameters().FirstOrDefault();
                return param == null || param.ParameterType != typeof(IDeploymentContext);
            });
            if (uncallableTasks.Any())
            {
                throw new ArgumentException(
                    "Task methods [{0}] must take a DeploymentContext as its sole parameter.",
                    String.Join(", ", uncallableTasks));

            }

            InitializationMethods = taskBlocks.SelectMany(
                b => b.GetType().GetMethods(),
                (o, m) => new Invokable
                {
                    Base = o,
                    Method = m
                }).
                Where(t => t.Method.GetCustomAttribute<DeploymentInitializeAttribute>() != null);

            CleanupMethods = taskBlocks.SelectMany(
                b => b.GetType().GetMethods(),
                (o, m) => new Invokable
                {
                    Base = o,
                    Method = m
                }).
                Where(t => t.Method.GetCustomAttribute<DeploymentCleanupAttribute>() != null);
        }

        public IEnumerable<ExecutionPlan> GetExecutionPlans()
        {
            return ExecutionPlans ?? (ExecutionPlans = ExecutionPlanner.GetExecutionPlans(
                Config, InitializationMethods, Tasks, CleanupMethods));
        }

        public IEnumerable<Tuple<string, string>> GetAvailableCommands()
        {
            var commands = new List<Tuple<string, string>>();
            foreach (var task in Tasks)
            {
                var taskAttr = task.Method.GetCustomAttribute<TaskAttribute>();
                if (taskAttr != null && taskAttr.Description != null)
                {
                    commands.Add(Tuple.Create(task.Method.Name, taskAttr.Description));
                }
                else
                {
                    commands.Add(Tuple.Create(task.Method.Name, ""));
                }
            }
            return commands;
        }

        public void BeginDeployments()
        {
            foreach (var plan in GetExecutionPlans())
            {
                IDeploymentContext deployment = new DeploymentContext(
                    Config,
                    new Environments.Linux());

                deployment.BeginDeployment(plan.TaskOrder);
            }
        }
    }
}
