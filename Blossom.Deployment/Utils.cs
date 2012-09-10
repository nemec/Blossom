using System;
using System.IO;

namespace Blossom.Deployment
{
    internal static class Utils
    {
        public static string HumanizeBytes(long bytes)
        {
            double tmpbytes = bytes;
            int byteRadix = 1000;
            foreach (var suffix in new string[] { "B", "kB", "MB", "GB", "TB" })
            {
                if (tmpbytes < byteRadix)
                {
                    return String.Format("{0:0.##}{1}", tmpbytes, suffix);
                }
                tmpbytes = tmpbytes / byteRadix;
            }
            return String.Format("{0}B", bytes);
        }

        // TODO Context sensitive escaping for backslashes
        public static string NormalizePathSeparators(string path, PathSeparator sep)
        {
            if (sep == PathSeparator.ForwardSlash)
            {
                return path.Replace(
                    PathSeparator.Backslash.Value(),
                    sep.Value());
            }
            else if (sep == PathSeparator.Backslash)
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