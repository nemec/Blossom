using Blossom.Deployment;
using Blossom.Deployment.Manager;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blossom.Scripting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new ScriptOptions();
            if (!CommandLineParser.Default.ParseArguments(args, options))
            {
                Console.Error.WriteLine(options.GetUsage());
                Environment.Exit(1);
            }

            List<Host> hosts = null;
            if (options.Hostnames != null && options.Hostnames.Length > 0 && options.Username != null)
            {
                hosts = options.Hostnames.Select(h => new Host
                {
                    Hostname = h,
                    Username = options.Username,
                    Password = options.Password ?? ""
                }).ToList();
            }

            var config = new DeploymentConfig
            {
                Hosts = hosts
            };

            var assemblyPath = RuntimeAssembly.BuildAssembly(options.ScriptFile);
            if (assemblyPath == null)
            {
                Console.Error.WriteLine("Could not build script file. Aborting.");
                Environment.Exit(1);
            }
            var assembly = RuntimeAssembly.LoadAssembly(assemblyPath);

            DeploymentConfig assemblyConf = RuntimeAssembly.GetDeploymentConfigFromAssembly(assembly);
            if (assemblyConf != null)
            {
                config.MergeFrom(assemblyConf);
            }

            if (!config.Hosts.Any())
            {
                Console.Error.WriteLine("Need to provide at least one hostname (with username).");
                Environment.Exit(1);
            }

            IDeploymentContext deployment;
            if (options.RemoteEnvironment == EnvironmentType.Linux)
            {
                deployment = new DeploymentContext(config,
                    new Deployment.Environments.Linux());
            }
            else
            {
                deployment = new DeploymentContext(config,
                    new Deployment.Environments.Windows());
            }

            var deploymentObjects = RuntimeAssembly.LoadTaskInstancesFromAssembly(assembly, deployment);
            var manager = new DeploymentManager(config, deploymentObjects);
            manager.BeginDeployments();
        }
    }
}
