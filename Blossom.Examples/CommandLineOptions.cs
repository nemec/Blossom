using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Examples
{
    internal enum EnvironmentType
    {
        Linux,
        Windows
    }

    internal class CommandLineOptions : CommandLineOptionsBase
    {
        [Option(null, "config", Required = true, 
            HelpText="Deployment config file.")]
        public string ConfigFile { get; set; }

        [Option("e", "env", 
            DefaultValue = EnvironmentType.Windows, 
            HelpText="Environment (Linux|Windows)")]
        public EnvironmentType RemoteEnvironment { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, curr => HelpText.DefaultParsingErrorsHandler(this, curr));
        }
    }
}
