using Blossom.Deployment;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Blossom.Examples.Compression
{
    [XmlRoot("Deployment")]
    public class Config
    {
        public List<Host> Hosts;

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