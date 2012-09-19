using Blossom.Deployment.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
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
            throw new NotImplementedException(); // TODO find and initialize all classes in namespace
            //InitializeTasks(taskBlocks);
        }

        public DeploymentManager(DeploymentConfig config, params object[] taskBlocks)
        {
            Config = config;
            InitializeTasks(taskBlocks);
        }

        private void InitializeTasks(object[] taskBlocks)
        {
            Tasks = taskBlocks.SelectMany<object, MethodInfo, Invokable>(
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

            InitializationMethods = taskBlocks.SelectMany<object, MethodInfo, Invokable>(
                b => b.GetType().GetMethods(),
                (o, m) => new Invokable
                {
                    Base = o,
                    Method = m
                }).
                Where(t => t.Method.GetCustomAttribute<DeploymentInitializeAttribute>() != null);

            CleanupMethods = taskBlocks.SelectMany<object, MethodInfo, Invokable>(
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
            if (ExecutionPlans == null)
            {
                ExecutionPlans = ExecutionPlanner.GetExecutionPlans(
                    Config, InitializationMethods, Tasks, CleanupMethods);
            }
            return ExecutionPlans;
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
                    new Blossom.Deployment.Environments.Linux());

                deployment.BeginDeployment(plan.TaskOrder);
            }
        }
    }
}
