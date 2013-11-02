using Blossom.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blossom.Logging;

namespace Blossom.Manager
{
    /// <summary>
    /// Manage deployments over all hosts.
    /// Set up the execution plan for each host.
    /// </summary>
    /// <typeparam name="TDeployment">
    ///     Type of the class holding all tasks for this deployment.
    /// </typeparam>
    /// <typeparam name="TTaskConfig">
    ///     Type of the custom configuration object provided
    ///     to each <see cref="IDeploymentContext"/>.
    /// </typeparam>
    public class DeploymentManager<TDeployment, TTaskConfig>
        where TDeployment : IDeployment<TTaskConfig>, new()
        where TTaskConfig : IConfig
    {
        /// <summary>
        /// Configuration object used to seed each <see cref="IDeploymentContext"/>.
        /// </summary>
        public DeploymentConfig<TTaskConfig> Config { get; set; }

        private IEnumerable<MethodInfo> Tasks { get; set; }

        private IEnumerable<MethodInfo> InitializationMethods { get; set; }

        private IEnumerable<MethodInfo> CleanupMethods { get; set; }

        private IEnumerable<ExecutionPlan> ExecutionPlans { get; set; }

        /// <summary>
        /// Create a new deployment with the given configuration object.
        /// Automatically initializes the
        /// <see cref="DeploymentConfig{TTaskConfig}"/>.
        /// </summary>
        /// <param name="config"></param>
        public DeploymentManager(TTaskConfig config)
        {
            Config = new DeploymentConfig<TTaskConfig>
                {
                    TaskConfig = config
                };

            config.InitializeDeployment(Config);
            InitializeTasks();
        } 

        /// <summary>
        /// Create a new deployment with the given configuration object.
        /// </summary>
        /// <param name="deploymentConfig">
        /// The initialized deployment configuration.
        /// </param>
        public DeploymentManager(DeploymentConfig<TTaskConfig> deploymentConfig)
        {
            Config = deploymentConfig;
            InitializeTasks();
        }

        private void InitializeTasks()
        {
            Tasks = typeof(TDeployment).GetMethods()
                .Where(t =>
                    t.GetCustomAttribute<TaskAttribute>() != null &&
                    t.GetCustomAttribute<DeploymentInitializeAttribute>() == null &&
                    t.GetCustomAttribute<DeploymentCleanupAttribute>() == null);

            InitializationMethods = typeof(TDeployment).GetMethods()
                .Where(t => t.GetCustomAttribute<DeploymentInitializeAttribute>() != null);

            CleanupMethods = typeof(TDeployment).GetMethods()
                .Where(t => t.GetCustomAttribute<DeploymentCleanupAttribute>() != null);
        }

        /// <summary>
        /// Generate and retrieve all execution plans for the deployment.
        /// </summary>
        /// <param name="taskFilter">A filter to exclude certain tasks.</param>
        /// <param name="ignoreDependencies">Ignore dependencies not explicitly included.</param>
        /// <returns>
        ///     Iterable of all <see cref="ExecutionPlans"/> that
        ///     will be run by this deployment.
        /// </returns>
        public IEnumerable<ExecutionPlan> GetExecutionPlans(
            Func<MethodInfo, bool> taskFilter = null, bool ignoreDependencies = false)
        {
            var tasks = Tasks;
            if (taskFilter != null)
            {
                tasks = tasks.Where(taskFilter);
            }
            return ExecutionPlans ?? (ExecutionPlans = ExecutionPlanner.GetExecutionPlans(
                Config, InitializationMethods, tasks, CleanupMethods, ignoreDependencies));
        }

        /// <summary>
        /// Return a list of potential task titles and descriptions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<string, string>> GetAvailableCommands()
        {
            var commands = new List<Tuple<string, string>>();
            foreach (var task in Tasks)
            {
                var taskAttr = task.GetCustomAttribute<TaskAttribute>();
                var taskName = task.Name;
                var taskDescription = String.Empty;
                if (taskAttr != null)
                {
                    if (taskAttr.Name != null)
                    {
                        taskName = taskAttr.Name;
                    }
                    if (taskAttr.Description != null)
                    {
                        taskDescription = taskAttr.Description;
                    }
                }
                commands.Add(Tuple.Create(taskName, taskDescription));
            }
            return commands;
        }

        /// <summary>
        /// Begin executing each <see cref="ExecutionPlan"/>.
        /// </summary>
        public virtual void BeginDeployments(Func<MethodInfo, bool> taskFilter = null, bool ignoreDependencies = false)
        {
            var plans = GetExecutionPlans(taskFilter, ignoreDependencies).ToList();
            if (!plans.Any())
            {
                Config.Logger.AbortLogLevel = LogLevel.None;
                Config.Logger.Fatal("No execution plans to run.");
            }
            foreach (var plan in plans)
            {
                var context = new DeploymentContext<TDeployment, TTaskConfig>(
                    plan.Host,
                    Config,
                    new Environments.Linux());

                Config.TaskConfig.InitializeContext(context);
                context.BeginDeployment(plan.TaskOrder);
            }
        }
    }
}
