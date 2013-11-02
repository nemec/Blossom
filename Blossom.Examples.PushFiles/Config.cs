using System.Collections.Generic;
using System.Xml.Serialization;
using Blossom.Logging;

namespace Blossom.Examples.PushFiles
{
    [XmlRoot("Deployment")]
    public class Config : IConfig
    {
        public void InitializeDeployment(IDeploymentConfig deploymentConfig)
        {
            deploymentConfig.Hosts = Hosts;
            deploymentConfig.Logger = new ColorizedConsoleLogger();
        }

        public void InitializeContext(IDeploymentContext context)
        {
        }
     
        public Host[] Hosts;

        public List<InputDir> InputDirs;

        public class InputDir
        {
            [XmlAttribute("path")]
            public string Path
            {
                get
                {
                    return _path;
                }
                set
                {
                    if (value.Contains(@"\") && !value.EndsWith(@"\"))
                    {
                        _path = value + @"\";
                    }
                    else if (value.Contains("/") && !value.EndsWith("/") ||
                        !value.EndsWith("/") && !value.EndsWith(@"\"))
                    {
                        _path = value + "/";
                    }
                    else
                    {
                        _path = value;
                    }
                }
            }
            private string _path;

            public List<OutputDir> OutputDirs;
        }

        public class OutputDir
        {
            [XmlAttribute("path")]
            public string Path;

            [XmlArray]
            [XmlArrayItem("File")]
            public List<string> Files;
        }
    }
}