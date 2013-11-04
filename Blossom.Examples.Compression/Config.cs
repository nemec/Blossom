using System.Collections.Generic;
using System.Xml.Serialization;
using Blossom.Logging;
using PathLib;
using System.Linq;

namespace Blossom.Examples.Compression
{
    [XmlRoot("Deployment")]
    public class Config : BaseConfig
    {
        public XmlHost[] Hosts;

        public override void InitializeDeployment(IDeploymentConfig deploymentConfig)
        {
            deploymentConfig.Hosts = Hosts;
            deploymentConfig.Logger = new ColorizedConsoleLogger();
        }

        public List<InputDir> InputDirs;

        public class InputDir
        {
            public IPurePath Path { get; set; }

            [XmlAttribute("path")]
            public string PathStr
            {
                set
                {
                    Path = new PureNtPath(value);
                }
            }

            public List<OutputDir> OutputDirs;
        }

        public class OutputDir
        {
            public IPurePath Path { get; set; }

            [XmlAttribute("path")]
            public string PathStr
            {
                set
                {
                    Path = new PureNtPath(value);
                }
            }

            [XmlArray]
            [XmlArrayItem("File")]
            public List<IPurePath> Files;

            public List<string> FilesStr
            {
                set
                {
                    Files = value
                        .Select(f => new PureNtPath(f))
                        .Cast<IPurePath>()
                        .ToList();
                }
            } 
        }
    }
}