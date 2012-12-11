﻿using Blossom.Deployment.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blossom.Deployment.Logging;
using CommandLine;

namespace Blossom.Deployment.Manager
{
    /// <summary>
    /// Manage deployments over all hosts.
    /// Set up the execution plan for each host
    /// and, optionally, provide a <see cref="Main"/>
    /// entry point to handle parsing command line arguments
    /// and displaying help messages and execution plans.
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
        /// </summary>
        /// <param name="config"></param>
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

        /// <summary>
        /// Generate and retrieve all execution plans for the deployment.
        /// </summary>
        /// <returns>
        ///     Iterable of all <see cref="ExecutionPlans"/> that
        ///     will be run by this deployment.
        /// </returns>
        public IEnumerable<ExecutionPlan> GetExecutionPlans()
        {
            return ExecutionPlans ?? (ExecutionPlans = ExecutionPlanner.GetExecutionPlans(
                Config, InitializationMethods, Tasks, CleanupMethods, null));
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
        public virtual void BeginDeployments()
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

        /// <summary>
        /// Convenience entry point to begin a deployment.
        /// Depending on the input argument array, this method
        /// may print the current application's version,
        /// set logging levels, display all <see cref="ExecutionPlans"/>,
        /// perform a dry run of the current <see cref="ExecutionPlans"/>,
        /// and limit which hosts and roles are present in the deployment.
        /// </summary>
        /// <param name="args">
        ///     Command line arguments. See <see cref="CommandLineOptions"/>
        ///     for potential options.
        /// </param>
        /// <param name="initializeConfig">Configuration file to seed each deployment.</param>
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

    /// <summary>
    /// <see cref="DeploymentManager{TDeployment,TTaskConfig}"/> with no TTaskConfig.
    /// </summary>
    /// <typeparam name="TDeployment"></typeparam>
    public class DeploymentManager<TDeployment>
        : DeploymentManager<TDeployment, NullConfig> where TDeployment : IDeployment<NullConfig>, new()
    {
        /// <summary>
        /// Create a new deploymentManager.
        /// </summary>
        /// <param name="config"></param>
        public DeploymentManager(DeploymentConfig<NullConfig> config)
            : base(config) {}
    }
}
