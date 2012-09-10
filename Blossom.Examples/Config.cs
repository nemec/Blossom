﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Blossom.Deployment
{
    [XmlRoot("Deployment")]
    public class Config
    {
        public List<Host> Hosts;

        public List<InputDir> InputDirs;

        public class InputDir
        {
            [XmlAttribute("path")]
            public string Path;

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