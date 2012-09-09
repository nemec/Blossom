using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Environments
{
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
