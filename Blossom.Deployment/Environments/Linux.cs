using System;

namespace Blossom.Deployment.Environments
{
    /// <summary>
    /// Environment that uses forward slashes as path separators.
    /// </summary>
    public class Linux : BaseEnvironment, IEnvironment
    {
        public Linux()
            : base() { }

        public Linux(string initialDirectory)
            : base(initialDirectory) { }

        public override string LineEnding
        {
            get { return @"\n"; }
        }

        public override PathSeparator PathSeparator
        {
            get { return PathSeparator.ForwardSlash; }
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