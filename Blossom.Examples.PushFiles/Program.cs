using Blossom.Deployment;
using Blossom.Deployment.Manager;
using System.Xml;
using System.Xml.Serialization;

namespace Blossom.Examples.PushFiles
{
    //http://docs.fabfile.org/en/1.4.3/#api-documentation
    public class Program
    {
        public static void ReadConfig(DeploymentConfig<Config> deployment, string filename)
        {
            var serializer = new XmlSerializer(typeof(Config));
            var config = (Config)serializer.Deserialize(XmlReader.Create(filename));
            deployment.Hosts = config.Hosts;
            deployment.TaskConfig = config;
        } 

        // TODO service oriented? https://github.com/fabric/fabric/issues/541
        // http://docs.fabfile.org/en/1.4.3/usage/execution.html#execution-strategy
        // https://github.com/fabric/fabric/issues/26
        private static void Main(string[] args)
        {
            DeploymentManager<Tasks, Config>.Main(args, ReadConfig);
        }
    }
}