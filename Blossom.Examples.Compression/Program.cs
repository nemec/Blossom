using Blossom.Deployment;
using System.Xml;
using System.Xml.Serialization;
using Blossom.Deployment.Manager;

namespace Blossom.Examples.Compression
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new XmlSerializer(typeof(Config));
            var config = (Config)serializer.Deserialize(XmlReader.Create(args[0]));

            var deploymentConfig = new DeploymentConfig<Config>
            {
                Hosts = config.Hosts
            };

            var manager = new DeploymentManager<Tasks, Config>(deploymentConfig);
            manager.BeginDeployments();
        }
    }
}
