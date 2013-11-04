using System;
using System.Linq;
using System.Reflection;
using Blossom.Logging;
using Blossom.Manager;

namespace Blossom.QuickStart
{
    public static class QuickStart<TDeploymentTasks, TTaskConfig>
        where TDeploymentTasks : IDeploymentTasks<TTaskConfig>, new()
        where TTaskConfig : IConfig
    {
        /// <summary>
        /// Convenience entry point to begin a deployment.
        /// Depending on the input argument array, this method
        /// may print the current application's version,
        /// set logging levels, display all <see cref="ExecutionPlan"/>s,
        /// perform a dry run of the current <see cref="ExecutionPlan"/>s,
        /// and limit which hosts and roles are present in the deployment.
        /// </summary>
        /// <param name="args">
        ///     Command line arguments. See <see cref="CommandLineOptions"/>
        ///     for potential options.
        /// </param>
        /// <param name="initializeConfig">
        /// Callback to initialize the config object from the given
        /// configuration file name.
        /// deployment.
        /// </param>
        /// <param name="taskFilter"></param>
        /// <param name="ignoreDependencies"></param>
        public static void Main(string[] args,
                Func<string, TTaskConfig> initializeConfig,
                Func<MethodInfo, bool> taskFilter = null,
                bool ignoreDependencies = false)
        {
            var options = clipr.CliParser.StrictParse<CommandLineOptions>(args);

            if (options.ConfigFile == null)
            {
                Console.Error.WriteLine("Please provide a config file.");
                Environment.Exit(1);
            }

            var config = new DeploymentConfig<TTaskConfig>
            {
                Logger = new SimpleConsoleLogger()
            };

            var taskConfig = initializeConfig(options.ConfigFile);
            taskConfig.InitializeDeployment(config);

            #region Set DeploymentConfig from command line options

            // Gets the subset of hosts specified at the command line.
            if (options.Hosts != null && options.Hosts.Length > 0)
            {
                config.Hosts = config.Hosts.Where(h =>
                    options.Hosts.Contains(h.Alias) ||
                    options.Hosts.Contains(h.Hostname)).ToArray();
            }

            if (options.Roles != null && options.Roles.Length > 0)
            {
                config.Hosts = config.Hosts.Where(
                    h => h.Roles != null && h.Roles.Intersect(options.Roles).Any()).ToArray();

                config.Roles = options.Roles.ToArray();
            }

            if (options.DryRun)
            {
                config.DryRun = options.DryRun;
            }

            // Overwrite only if provided at command line
            if (options.DisplayLogLevel.HasValue)
            {
                config.Logger.DisplayLogLevel = options.DisplayLogLevel.Value;
            }

            // Overwrite only if provided at command line
            if (options.AbortLogLevel.HasValue)
            {
                config.Logger.AbortLogLevel = options.AbortLogLevel.Value;
            }

            #endregion

            var manager = new DeploymentManager<TDeploymentTasks, TTaskConfig>(config);

            if (options.List)
            {
                Console.WriteLine("Planned execution order:");
                foreach (var plan in manager.GetExecutionPlans(
                    taskFilter, ignoreDependencies))
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
}
