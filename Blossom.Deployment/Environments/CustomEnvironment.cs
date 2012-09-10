using System;
using System.IO;

namespace Blossom.Deployment.Environments
{
    public class CustomEnvironment : BaseEnvironment, IEnvironment
    {
        private string _lineEnding;

        public override string LineEnding { get { return _lineEnding; } }

        private PathSeparator _pathSeparator;

        public override PathSeparator PathSeparator { get { return _pathSeparator; } }

        public CustomEnvironment(
            string lineEnding, PathSeparator pathSeparator, string initialDirectory)
            : base(initialDirectory)
        {
            _lineEnding = lineEnding;
            _pathSeparator = pathSeparator;
        }

        public CustomEnvironment(string lineEnding, PathSeparator pathSeparator)
            : this(lineEnding, pathSeparator, null)
        {
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