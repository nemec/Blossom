using clipr;
using clipr.Annotations;
using Blossom.Deployment.Logging;

namespace Blossom.Scripting
{
    internal enum EnvironmentType
    {
        Linux,
        Windows
    }

    internal class CommandLineOptions
    {
        [NamedArgument('e', "env", Description = "Remote Environment (affects things like path separators).")]
        public EnvironmentType RemoteEnvironment { get; set; }

        [NamedArgument("hosts",
            Description = "Limits available hosts to the provided hosts (or aliases)")]
        public string[] Hosts { get; set; }

        [NamedArgument('r', "roles",
            Description = "Limits available hosts to those in the given role or roles.")]
        public string[] Roles { get; set; }

        [NamedArgument('p', "plan", Action = ParseAction.StoreTrue,
            Description = "Display the execution plan and exit (Tasks to be executed " +
                "and the order they will be executed).")]
        public bool List { get; set; }

        [NamedArgument('d', "dryrun",
            Description = "Display the commands that would be run, but don't actually execute them.")]
        public bool DryRun { get; set; }

        [NamedArgument("logging", Description = "Minimum log level to display.")]
        public LogLevel? DisplayLogLevel { get; set; }

        [NamedArgument("abort", Description = "Abort deployment when logs are written to this level.")]
        public LogLevel? AbortLogLevel { get; set; }

        [PositionalArgument(0, Description = "Path to script to run.")]
        public string TaskScriptPath { get; set; }
    }
}
