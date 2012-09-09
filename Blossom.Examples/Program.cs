using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    //http://docs.fabfile.org/en/1.4.3/#api-documentation
    //Rhyble
    public class Program
    {
        static void Main(string[] args)
        {
            var serializer = new XmlSerializer(typeof(Config));

            var config = (Config)serializer.Deserialize(XmlReader.Create("testdeploy.config"));

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
