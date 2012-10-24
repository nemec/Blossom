using System;
using System.IO;

namespace Blossom.Deployment.Environments
{
    public sealed class CustomEnvironment : BaseEnvironment
    {
        public override string LineEnding { get; protected set; }

        public override string ShellCommand { get; protected set; }

        public override string ShellStartArguments { get; protected set; }

        public override string SudoPrefix { get; protected set; }

        public override PathSeparator PathSeparator { get; protected set; }

        public CustomEnvironment(string initialDirectory,
            string lineEnding, string shellCommand, string shellStartArguments,
            string sudoPrefix, PathSeparator pathSeparator)
            : base(initialDirectory)
        {
            LineEnding = lineEnding;
            ShellCommand = shellCommand;
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