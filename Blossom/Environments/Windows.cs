using System;
using System.IO;
using PathLib;

namespace Blossom.Environments
{
    /// <summary>
    /// Environment that uses backslashes as path separators and
    /// carriage returns in line endings.
    /// </summary>
    public class Windows : BaseEnvironment
    {
        private const string DefaultShellCommand = "cmd.exe";

        /// <summary>
        /// Create a new Windows environment with an optional initial directory and shell.
        /// </summary>
        /// <param name="initialDirectory">Initial directory path.</param>
        /// <param name="shellCommand">Shell command to use (or null for default).</param>
        public Windows(string initialDirectory = null, string shellCommand = null)
            : base(shellCommand ?? DefaultShellCommand, initialDirectory)
        {
        }

        /// <inheritdoc />
        public override string LineEnding
        {
            get { return @"\r\n"; }
            protected set { }
        }

        /// <inheritdoc />
        public override string ShellStartArguments
        {
            get { return "/Q /C"; }
            protected set { }
        }

        /// <inheritdoc />
        public override string SudoPrefix
        {
            get { return "runas /user:Administrator"; }
            protected set { }
        }

        /// <inheritdoc />
        public override PathSeparator PathSeparator
        {
            get { return PathSeparator.Backslash; }
            protected set { }
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

        public override IPurePath CreatePath(string initialPath)
        {
            return new PureNtPath(initialPath);
        }
    }
}