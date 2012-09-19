using Blossom.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Blossom.Examples.Compression
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new XmlSerializer(typeof(Config));
            var config = (Config)serializer.Deserialize(XmlReader.Create(args[0]));

            var deploymentConfig = new DeploymentConfig
            {
                Hosts = config.Hosts
            };

            IDeploymentContext deployment = new DeploymentContext(
                    deploymentConfig, new Blossom.Deployment.Environments.Linux());

            var taskBlock = new Tasks(deployment, config);
            //deployment.BeginDeployment(taskBlock);
        }
    }
}
