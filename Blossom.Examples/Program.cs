using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Blossom.Deployment
{
    //http://docs.fabfile.org/en/1.4.3/#api-documentation
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Need to provide config file as argument.");
                Environment.Exit(1);
            }
            var serializer = new XmlSerializer(typeof(Config));

            var config = (Config)serializer.Deserialize(XmlReader.Create(args[0]));

            var sessionConfig = new Dictionary<string, string>
                {
                    {"StrictHostKeyChecking", "no"}
                };

            var deployment = new DeploymentContext();
            deployment.Environment.Hosts.AddRange(config.Hosts);

            deployment.Environment.Remote = new Environments.Linux();
            deployment.BeginDeployment(args,
                new Tasks(deployment, config),
                sessionConfig);
        }
    }
}