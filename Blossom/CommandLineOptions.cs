using System.Linq;
using Blossom.Logging;
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Blossom
{
    internal enum EnvironmentType
    {
        Linux,
        Windows
    }

    internal class CommandLineOptions : CommandLineOptionsBase
    {
        [Option("e", "env", 
            DefaultValue = EnvironmentType.Linux, 
            HelpText = "Environment (Linux|Windows)")]
        public EnvironmentType RemoteEnvironment { get; set; }

        [OptionArray("h", "hosts",
            HelpText = "Limits available hosts to the provided hosts (or aliases)")]
        public string[] Hosts { get; set; }

        [OptionArray("r", "roles",
            HelpText = "Limits available hosts to those in the given role or roles.")]
        public string[] Roles { get; set; }

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

        [Option(null, "logging", HelpText = "Minimum log level to display.")]
        public LogLevel? DisplayLogLevel { get; set; }

        [Option(null, "abort", HelpText = "Abort deployment when logs are written to this level.")]
        public LogLevel? AbortLogLevel { get; set; }

        [ValueList(typeof(List<string>), MaximumElements = 1)]
        public IList<string> ConfigFileList { get; set; }

        internal string ConfigFile
        {
            get { return ConfigFileList.FirstOrDefault(); }
        }
        
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                curr => HelpText.DefaultParsingErrorsHandler(this, curr));
        }
    }
}
