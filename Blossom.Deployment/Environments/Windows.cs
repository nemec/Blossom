using System;
using System.IO;

namespace Blossom.Deployment.Environments
{
    /// <summary>
    /// Environment that uses backslashes as path separators and
    /// carriage returns in line endings.
    /// </summary>
    public class Windows : BaseEnvironment, IEnvironment
    {
        public Windows()
            : base() { }

        public Windows(string initialDirectory)
            : base(initialDirectory) { }

        public override string LineEnding
        {
            get { return @"\r\n"; }
        }

        public override PathSeparator PathSeparator
        {
            get { return PathSeparator.Backslash; }
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