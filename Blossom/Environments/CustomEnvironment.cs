using System;
using System.IO;
using PathLib;

namespace Blossom.Environments
{
    /// <summary>
    /// Environment where each property can be set indepentently.
    /// </summary>
    public sealed class CustomEnvironment : BaseEnvironment
    {
        /// <inheritdoc />
        public override string LineEnding { get; protected set; }

        /// <inheritdoc />
        public override string ShellStartArguments { get; protected set; }

        /// <inheritdoc />
        public override string SudoPrefix { get; protected set; }

        private Func<string, IPurePath> PurePathFactory { get; set; }

        /// <summary>
        /// Create an environment with custom parameters.
        /// </summary>
        /// <param name="initialDirectory">The initial current directory for the environment.</param>
        /// <param name="lineEnding">Line ending for this environment.</param>
        /// <param name="shellCommand">Shell executable path.</param>
        /// <param name="shellStartArguments">Any shell provided to the shell.</param>
        /// <param name="sudoPrefix">Binary used to elevate permissions.</param>
        /// <param name="purePathFactory">Factory method for generating an <see cref="IPurePath"/></param>
        public CustomEnvironment(string initialDirectory,
            string lineEnding, string shellCommand, string shellStartArguments,
            string sudoPrefix,
            Func<string, IPurePath> purePathFactory)
            : base(shellCommand, initialDirectory)
        {
            LineEnding = lineEnding;
            ShellStartArguments = shellStartArguments;
            SudoPrefix = sudoPrefix;
            PurePathFactory = purePathFactory;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override IPurePath CreatePurePath(string initialPath)
        {
            return PurePathFactory(initialPath);
        }
    }
}