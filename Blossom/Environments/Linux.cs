using System;

namespace Blossom.Environments
{
    /// <summary>
    /// Environment that uses forward slashes as path separators.
    /// </summary>
    public class Linux : BaseEnvironment
    {
        private const string DefaultShellCommand = "/bin/sh";

        public Linux()
            :base(DefaultShellCommand)
        {
        }

        public Linux(string initialDirectory)
            : base(initialDirectory, DefaultShellCommand)
        {
        }

        public override string LineEnding
        {
            get { return @"\n"; }
            protected set { }
        }

        public override string ShellStartArguments
        {
            get { return ""; }
            protected set { }
        }

        public override string SudoPrefix
        {
            get { return "sudo"; }
            protected set { }
        }

        public override PathSeparator PathSeparator
        {
            get { return PathSeparator.ForwardSlash; }
            protected set { }
        }

        public override string ExpandUser(string path, string username)
        {
            if (path.StartsWith("~"))
            {
                return String.Join("/", "/home", username, path.Substring(1));
            }
            return path;
        }
    }
}