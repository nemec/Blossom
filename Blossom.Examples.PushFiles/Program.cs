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
        // Comment
        private static void Main(string[] args)
        {
            var options = new Blossom.Examples.PushFiles.CommandLineOptions();
            if (!CommandLineParser.Default.ParseArguments(args, options))
            {
                Console.Error.WriteLine(options.GetUsage());
                Environment.Exit(1);
            }
            var serializer = new XmlSerializer(typeof(Config));

            var config = (Config)serializer.Deserialize(XmlReader.Create(options.ConfigFile));

            IDeploymentConfig conf;
            // Gets the subset of hosts specified at the command line.
            if (options.Hosts != null && options.Hosts.Length > 0)
            {
                conf = new DeploymentConfig(
                    config.Hosts.Where(h =>
                        options.Hosts.Contains(h.Alias) ||
                        options.Hosts.Contains(h.Hostname)).ToList());
            }
            else
            {
                conf = new DeploymentConfig(config.Hosts);
            }
            IDeploymentContext deployment;            

            if (options.RemoteEnvironment == EnvironmentType.Linux)
            {
                deployment = new DeploymentContext(
                    conf,
                    new Blossom.Deployment.Environments.Linux());
            }
            else
            {
                deployment = new DeploymentContext(
                    conf,
                    new Blossom.Deployment.Environments.Windows());
            }
            
            deployment.BeginDeployment(new Tasks(deployment, config));
        }
    }
}