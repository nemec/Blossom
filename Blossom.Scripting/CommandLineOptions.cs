using System.Collections.Generic;
using clipr;
using Blossom.Logging;

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
            Constraint = NumArgsConstraint.AtLeast, NumArgs = 1,
            Description = "Limits available hosts to the provided hosts (or aliases)")]
        public List<string> Hosts { get; set; }

        [NamedArgument('r', "roles", 
            Constraint = NumArgsConstraint.AtLeast, NumArgs = 1,
            Description = "Limits available hosts to those in the given role or roles.")]
        public List<string> Roles { get; set; }

        [NamedArgument('p', "plan", Action = ParseAction.StoreTrue,
            Description = "Display the execution plan and exit (Tasks to be executed " +
                "and the order they will be executed).")]
        public bool ListExecutionPlan { get; set; }

        [NamedArgument('d', "dryrun",
            Description = "Display the commands that would be run, but don't actually execute them.")]
        public bool DryRun { get; set; }

        [NamedArgument('i', "ignoreDependencies", Action = ParseAction.StoreTrue,
            Description = "Execute only the tasks specified with -t, ignoring any dependencies.")]
        public bool IgnoreDependencies { get; set; }

        [NamedArgument('t', "tasks",
            Action = ParseAction.Append, Constraint = NumArgsConstraint.AtLeast, NumArgs = 1,
            Description = "List of specific tasks to execute. Will execute these and their dependencies.")]
        public List<string> Tasks { get; set; }
            
        [NamedArgument("logging", Description = "Minimum log level to display.")]
        public LogLevel? DisplayLogLevel { get; set; }

        [NamedArgument("abort", Description = "Abort deployment when logs are written to this level.")]
        public LogLevel? AbortLogLevel { get; set; }

        [PositionalArgument(0, Description = "Path to script to run.")]
        public string TaskScriptPath { get; set; }
    }
}
