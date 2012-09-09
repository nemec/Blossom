using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    static class Utils
    {
        // TODO Context sensitive escaping for backslashes
        public static string NormalizePathSeparators(string path, PathSeparator sep)
        {
            if (sep == PathSeparator.ForwardSlash)
            {
                return path.Replace(
                    PathSeparator.Backslash.Value(), 
                    sep.Value());
            }
            else if(sep == PathSeparator.Backslash)
            {
                return path.Replace(
                    PathSeparator.ForwardSlash.Value(),
                    sep.Value());
            }
            return path;
        }

        public static string CombineLocalPath(DeploymentContext context, params string[] paths)
        {
            return NormalizePathSeparators(
                Path.Combine(paths),
                context.Environment.Local.PathSeparator);
        }

        public static string CombineRemotePath(DeploymentContext context, params string[] paths)
        {
            return NormalizePathSeparators(
                Path.Combine(paths),
                context.Environment.Remote.PathSeparator);
        }
    }
}
