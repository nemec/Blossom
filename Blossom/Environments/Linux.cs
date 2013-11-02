using System;
using PathLib;

namespace Blossom.Environments
{
    /// <summary>
    /// Environment that uses forward slashes as path separators.
    /// </summary>
    public class Linux : BaseEnvironment
    {
        private const string DefaultShellCommand = "/bin/sh";

        /// <summary>
        /// Create a new linux environment.
        /// </summary>
        public Linux()
            :base(DefaultShellCommand)
        {
        }

        /// <summary>
        /// Create a new linux environment.
        /// </summary>
        /// <param name="initialDirectory"></param>
        public Linux(string initialDirectory)
            : base(initialDirectory, DefaultShellCommand)
        {
        }

        /// <inheritdoc />
        public override string LineEnding
        {
            get { return @"\n"; }
            protected set { }
        }

        /// <inheritdoc />
        public override string ShellStartArguments
        {
            get { return ""; }
            protected set { }
        }

        /// <inheritdoc />
        public override string SudoPrefix
        {
            get { return "sudo"; }
            protected set { }
        }

        /// <inheritdoc />
        public override PathSeparator PathSeparator
        {
            get { return PathSeparator.ForwardSlash; }
            protected set { }
        }

        /// <inheritdoc />
        public override string ExpandUser(string path, string username)
        {
            if (path.StartsWith("~"))
            {
                return String.Join("/", "/home", username, path.Substring(1));
            }
            return path;
        }

        public override IPurePath CreatePurePath(string initialPath)
        {
            return new PurePosixPath(initialPath);
        }
    }
}