using System;
using System.IO;

namespace Blossom.Deployment.Environments
{
    /// <summary>
    /// Environment where each property can be set indepentently.
    /// </summary>
    public sealed class CustomEnvironment : BaseEnvironment
    {
        public override string LineEnding { get; protected set; }

        public override string ShellStartArguments { get; protected set; }

        public override string SudoPrefix { get; protected set; }

        public override PathSeparator PathSeparator { get; protected set; }

        /// <summary>
        /// Create an environment with custom parameters.
        /// </summary>
        /// <param name="initialDirectory">The initial current directory for the environment.</param>
        /// <param name="lineEnding">Line ending for this environment.</param>
        /// <param name="shellCommand">Shell executable path.</param>
        /// <param name="shellStartArguments">Any shell provided to the shell.</param>
        /// <param name="sudoPrefix">Binary used to elevate permissions.</param>
        /// <param name="pathSeparator">Path separator for this environment.</param>
        public CustomEnvironment(string initialDirectory,
            string lineEnding, string shellCommand, string shellStartArguments,
            string sudoPrefix, PathSeparator pathSeparator)
            : base(shellCommand, initialDirectory)
        {
            LineEnding = lineEnding;
            ShellStartArguments = shellStartArguments;
            SudoPrefix = sudoPrefix;
            PathSeparator = pathSeparator;
        }

        public override string ExpandUser(string path, string username)
        {
            if (path.StartsWith("~"))
            {
                return Path.Combine(
                    Directory.GetParent(Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile)).FullName,
                    username,
                    path.Substring(1));
            }
            return path;
        }
    }
}