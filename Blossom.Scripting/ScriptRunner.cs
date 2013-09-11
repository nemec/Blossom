using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Blossom.Manager;
using Roslyn.Scripting.CSharp;
using clipr;

namespace Blossom.Scripting
{
    public class ScriptRunner
    {
        private static void InitializeWithAction(IDeploymentConfig deployment, IConfig config)
        {
            config.Initialize(deployment);
        }

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
        public static void Main(string[] args)
        {
            var options = CliParser.StrictParse<CommandLineOptions>(args);

            if (!File.Exists(options.TaskScriptPath))
            {
                Console.Error.WriteLine(
                    "Cannot find script '{0}'", options.TaskScriptPath);
                Environment.Exit(1);
            }

            var engine = new ScriptEngine();
            var session = engine.CreateSession();

            // Add a reference to our framework.
            session.AddReference(typeof(IDeployment).Assembly);
            session.AddReference(typeof(Renci.SshNet.BaseClient).Assembly);

            var submission = session.CompileSubmission<object>(
                File.ReadAllText(options.TaskScriptPath));
            var assemblyStream = new MemoryStream();
            submission.Compilation.Emit(assemblyStream);

            var assembly = Assembly.Load(assemblyStream.ToArray());
            var deploymentClasses = assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    i.GetGenericTypeDefinition() == typeof(IDeployment<>)));

            foreach (var deploymentClass in deploymentClasses)
            {

                var configType = deploymentClass
                    .GetInterfaces()
                    .First(i => i.GetGenericTypeDefinition() == typeof(IDeployment<>))
                    .GenericTypeArguments[0];

                var deploymentConfigType = typeof (DeploymentConfig<>)
                    .MakeGenericType(configType);
                var deploymentConfig = (IDeploymentConfig)Activator
                    .CreateInstance(deploymentConfigType);

                var configInstance = Activator.CreateInstance(configType);
                var iConfig = configInstance as IConfig;
                if (iConfig != null)
                {
                    InitializeWithAction(deploymentConfig, iConfig);
                }
                else
                {
                    Console.Error.WriteLine(
                        "Configuration class must implement `{0}`",
                        typeof(IConfig).FullName);
                }

                deploymentConfigType
                    .GetProperty("TaskConfig")
                    .SetValue(deploymentConfig, configInstance);

                #region Set DeploymentConfig from command line options

                // Gets the subset of hosts specified at the command line.
                if (options.Hosts != null && options.Hosts.Count > 0)
                {
                    deploymentConfig.Hosts = deploymentConfig.Hosts.Where(h =>
                        options.Hosts.Contains(h.Alias) ||
                        options.Hosts.Contains(h.Hostname)).ToArray();
                }

                if (options.Roles != null && options.Roles.Count > 0)
                {
                    deploymentConfig.Hosts = deploymentConfig.Hosts.Where(
                        h => h.Roles != null && h.Roles.Split(';').Intersect(options.Roles).Any()).ToArray();

                    deploymentConfig.Roles = options.Roles.ToArray();
                }

                if (options.DryRun)
                {
                    deploymentConfig.DryRun = options.DryRun;
                }

                // Overwrite only if provided at command line
                if (options.DisplayLogLevel.HasValue)
                {
                    deploymentConfig.Logger.DisplayLogLevel = options.DisplayLogLevel.Value;
                }

                // Overwrite only if provided at command line
                if (options.AbortLogLevel.HasValue)
                {
                    deploymentConfig.Logger.AbortLogLevel = options.AbortLogLevel.Value;
                }

                #endregion

                var types = new[] { deploymentClass, configType };
                dynamic manager = Activator
                    .CreateInstance(
                        typeof(DeploymentManager<,>).MakeGenericType(types), 
                        new object[] { deploymentConfig });

                if (options.ListExecutionPlan)
                {
                    Console.WriteLine("Planned execution order:");
                    foreach (var plan in (IEnumerable<ExecutionPlan>)manager
                        .GetExecutionPlans(options.IgnoreDependencies))
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
}
