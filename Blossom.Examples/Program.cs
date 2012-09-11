using Blossom.Deployment;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Blossom.Examples
{
    //http://docs.fabfile.org/en/1.4.3/#api-documentation
    public class Program
    {
        private static void Main(string[] args)
        {
            var options = new Blossom.Examples.CommandLineOptions();
            if (!CommandLineParser.Default.ParseArguments(args, options))
            {
                Environment.Exit(1);
            }
            var serializer = new XmlSerializer(typeof(Config));

            var config = (Config)serializer.Deserialize(XmlReader.Create(options.ConfigFile));

            var sessionConfig = new Dictionary<string, string>
                {
                    {"StrictHostKeyChecking", "no"}
                };

            IDeploymentContext deployment;            

            if (options.RemoteEnvironment == EnvironmentType.Linux)
            {
                deployment = new DeploymentContext(
                    new Blossom.Deployment.Environments.Linux());
            }
            else
            {
                deployment = new DeploymentContext(
                    new Blossom.Deployment.Environments.Windows());
            }

            deployment.Environment.Hosts.AddRange(config.Hosts);
            deployment.BeginDeployment(args,
                new Tasks(deployment, config),
                sessionConfig);
        }
    }
}