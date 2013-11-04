using System.Collections.Generic;
using System.Xml.Serialization;
using Blossom.Logging;
using PathLib;

namespace Blossom.Examples.PushFiles
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
            public List<string> Files { get; set; }
        }
    }
}