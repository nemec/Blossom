using Blossom.Logging;
using clipr;

namespace Blossom.QuickStart
{
    internal enum EnvironmentType
    {
        Linux,
        Windows
    }

    [ApplicationInfo(Name = "Blossom")]
    internal class CommandLineOptions
    {
        public CommandLineOptions()
        {
            RemoteEnvironment = EnvironmentType.Linux;

        }

        [NamedArgument('e', "env", 
            Description = "Environment (Linux|Windows)")]
        public EnvironmentType RemoteEnvironment { get; set; }

        [NamedArgument('h', "hosts", 
            Description = "Limits available hosts to the provided hosts (or aliases)")]
        public string[] Hosts { get; set; }

        [NamedArgument('r', "roles",
            Description = "Limits available hosts to those in the given role or roles.")]
        public string[] Roles { get; set; }

        [NamedArgument('p', "plan",
            Description = "Display the execution plan and exit (Tasks to be executed " +
                "and the order they will be executed).")]
        public bool List { get; set; }

        [NamedArgument('d', "dryrun",
            Description = "Display the commands that would be run, but don't actually execute them.")]
        public bool DryRun { get; set; }

        [NamedArgument("version",
            Description = "Print the current version and exit.")]
        public bool PrintVersion { get; set; }

        [NamedArgument("logging", 
            Description = "Minimum log level to display.")]
        public LogLevel? DisplayLogLevel { get; set; }

        [NamedArgument("abort", 
            Description = "Abort deployment when logs are written to this level.")]
        public LogLevel? AbortLogLevel { get; set; }

        [PositionalArgument(0)]
        public string ConfigFile { get; set; }
    }
}
