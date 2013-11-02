using System.Xml;
using System.Xml.Serialization;
using Blossom.QuickStart;

namespace Blossom.Examples.PushFiles
{
    //http://docs.fabfile.org/en/1.4.3/#api-documentation
    public class Program
    {
        public static Config ReadConfig(string filename)
        {
            var serializer = new XmlSerializer(typeof(Config));
            return (Config)serializer.Deserialize(XmlReader.Create(filename));
        } 

        private static void Main(string[] args)
        {
            QuickStart<Tasks, Config>.Main(args, ReadConfig);
        }
    }
}