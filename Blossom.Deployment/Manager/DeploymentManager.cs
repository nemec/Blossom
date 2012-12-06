using Blossom.Deployment.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blossom.Deployment.Logging;
using CommandLine;

namespace Blossom.Deployment.Manager
{
    public class DeploymentManager<TDeployment, TTaskConfig>
        where TDeployment : IDeployment<TTaskConfig>, new()
    {
        public DeploymentConfig<TTaskConfig> Config { get; set; }

        private IEnumerable<MethodInfo> Tasks { get; set; }

        private IEnumerable<MethodInfo> InitializationMethods { get; set; }

        private IEnumerable<MethodInfo> CleanupMethods { get; set; }

        private IEnumerable<ExecutionPlan> ExecutionPlans { get; set; }

        public DeploymentManager(DeploymentConfig<TTaskConfig> config)
        {
            Config = config;
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

        public IEnumerable<ExecutionPlan> GetExecutionPlans()
        {
            return ExecutionPlans ?? (ExecutionPlans = ExecutionPlanner.GetExecutionPlans(
                Config, InitializationMethods, Tasks, CleanupMethods, null));
        }

        public IEnumerable<Tuple<string, string>> GetAvailableCommands()
        {
            var commands = new List<Tuple<string, string>>();
            foreach (var task in Tasks)
            {
                var taskAttr = task.GetCustomAttribute<TaskAttribute>();
                if (taskAttr != null && taskAttr.Description != null)
                {
                    commands.Add(Tuple.Create(task.Name, taskAttr.Description));
                }
                else
                {
                    commands.Add(Tuple.Create(task.Name, ""));
                }
            }
            return commands;
        }

        public void BeginDeployments()
        {
            var plans = GetExecutionPlans();
            if (!plans.Any())
            {
                Config.Logger.AbortLogLevel = LogLevel.None;
                Config.Logger.Fatal("No execution plans to run.");
            }
            foreach (var plan in plans)
            {
                var deployment = new DeploymentContext<TDeployment, TTaskConfig>(
                    Config,
                    new Environments.Linux());

                deployment.BeginDeployment(plan.TaskOrder);
            }
        }

        public static void Main(string[] args,
            Action<DeploymentConfig<TTaskConfig>, string> initializeConfig)
        {
            var options = new CommandLineOptions();
            if (!CommandLineParser.Default.ParseArguments(args, options))
            {
                Console.Error.WriteLine(options.GetUsage());
                Environment.Exit(1);
            }

            if (options.PrintVersion)
            {
                Console.WriteLine("Blossom " +
                    Assembly.GetExecutingAssembly().GetName().Version);
                return;
            }

            var configFile = options.ConfigFileList.FirstOrDefault();
            if (configFile == null)
            {
                Console.Error.WriteLine("Please provide config file path.");
                Environment.Exit(1);
            }

            var config = new DeploymentConfig<TTaskConfig>
                {
                    Logger = new SimpleConsoleLogger()
                };
            initializeConfig(config, configFile);

            #region Set DeploymentConfig from command line options

            // Gets the subset of hosts specified at the command line.
            if (options.Hosts != null && options.Hosts.Length > 0)
            {
                config.Hosts = config.Hosts.Where(h =>
                    options.Hosts.Contains(h.Alias) ||
                    options.Hosts.Contains(h.Hostname)).ToList();
            }

            if (options.Roles != null && options.Roles.Length > 0)
            {
                config.Hosts = config.Hosts.Where(
                    h => h.Roles != null && h.Roles.Split(';').Intersect(options.Roles).Any()).ToList();

                config.Roles = options.Roles.ToList();
            }

            if (options.DryRun)
            {
                config.DryRun = options.DryRun;
            }

            // Overwrite only if provided at command line
            if(options.DisplayLogLevel.HasValue)
            {
                config.Logger.DisplayLogLevel = options.DisplayLogLevel.Value;
            }

            // Overwrite only if provided at command line
            if (options.AbortLogLevel.HasValue)
            {
                config.Logger.AbortLogLevel = options.AbortLogLevel.Value;
            }

            #endregion

            var manager = new DeploymentManager<TDeployment, TTaskConfig>(config);

            if (options.List)
            {
                Console.WriteLine("Planned execution order:");
                foreach (var plan in manager.GetExecutionPlans())
                {
                    Console.WriteLine(plan.Host);
                    foreach (var task in plan.TaskOrder)
                    {
                        Console.WriteLine("\t{0}.{1}",
                            task.ReflectedType.Name, task.Name);
                    }
                }
                return;
            }
            manager.BeginDeployments();
        }
    }

    public class DeploymentManager<TDeployment>
        : DeploymentManager<TDeployment, NullConfig> where TDeployment : IDeployment<NullConfig>, new()
    {
        public DeploymentManager(DeploymentConfig<NullConfig> config)
            : base(config) {}
    }
}
