using Blossom.Deployment;
using Blossom.Scripting;
using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Reflection;
using Blossom.Deployment.Dependencies;

namespace Blossom.Examples.PushFiles
{
    //http://docs.fabfile.org/en/1.4.3/#api-documentation
    public class Program
    {
        // TODO service oriented? https://github.com/fabric/fabric/issues/541
        // http://docs.fabfile.org/en/1.4.3/usage/execution.html#execution-strategy
        // https://github.com/fabric/fabric/issues/26
        private static void Main(string[] args)
        {
            var options = new Blossom.Examples.PushFiles.CommandLineOptions();
            if (!CommandLineParser.Default.ParseArguments(args, options))
            {
                Console.Error.WriteLine(options.GetUsage());
                Environment.Exit(1);
            }

            if (options.PrintVersion)
            {
                Console.WriteLine("Blossom " + 
                    Assembly.GetExecutingAssembly().GetName().Version.ToString());
                return;
            }

            var serializer = new XmlSerializer(typeof(Config));

            var config = (Config)serializer.Deserialize(XmlReader.Create(options.ConfigFile));
            var taskBlock = new Tasks(config);

            DeploymentConfig conf;
            // Gets the subset of hosts specified at the command line.
            if (options.Hosts != null && options.Hosts.Length > 0)
            {
                conf = new DeploymentConfig
                {
                    Hosts = config.Hosts.Where(h =>
                        options.Hosts.Contains(h.Alias) ||
                        options.Hosts.Contains(h.Hostname)).ToList(),
                        DryRun = options.DryRun
                };
            }
            else
            {
                conf = new DeploymentConfig
                {
                    Hosts = config.Hosts,
                    DryRun = options.DryRun
                };
            }

            IDeploymentManager manager = new DeploymentManager(conf, taskBlock);

            if (options.List)
            {
                Console.WriteLine("Planned execution order:");
                foreach (var plan in manager.GetExecutionPlans())
                {
                    Console.WriteLine(plan.Host);
                    foreach (var task in plan.TaskOrder)
                    {
                        Console.WriteLine(String.Format("\t{0}.{1}",
                            task.Method.ReflectedType.Name, task.Method.Name));
                    }
                }
                return;
            }
            manager.BeginDeployments();
        }
    }
}