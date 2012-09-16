﻿using System;
using System.IO;

namespace Blossom.Deployment
{
    internal static class Utils
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
            else if (sep == PathSeparator.Backslash)
            {
                return path.Replace(
                    PathSeparator.ForwardSlash.Value(),
                    sep.Value());
            }
            return path;
        }
    }
}