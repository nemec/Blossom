using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Examples.PushFiles
{
    internal enum EnvironmentType
    {
        Linux,
        Windows
    }

    internal class CommandLineOptions : CommandLineOptionsBase
    {
        [Option(null, "config", Required = true, 
            HelpText = "Deployment config file.")]
        public string ConfigFile { get; set; }

        [Option("e", "env", 
            DefaultValue = EnvironmentType.Linux, 
            HelpText = "Environment (Linux|Windows)")]
        public EnvironmentType RemoteEnvironment { get; set; }

        [OptionArray("h", "hosts",
            HelpText = "Limits available hosts to the provided hosts (or aliases)")]
        public string[] Hosts { get; set; }

        [Option("p", "plan",
            HelpText = "Display the execution plan and exit (Tasks to be executed " +
                "and the order they will be executed).")]
        public bool List { get; set; }

        [Option("d", "dryrun",
            HelpText = "Display the commands that would be run, but don't actually execute them.")]
        public bool DryRun { get; set; }

        [Option(null, "version",
            HelpText = "Print the current version and exit.")]
        public bool PrintVersion { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                curr => HelpText.DefaultParsingErrorsHandler(this, curr));
        }
    }
}
