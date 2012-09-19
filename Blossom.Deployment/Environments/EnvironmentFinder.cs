using System;

namespace Blossom.Deployment.Environments
{
    internal static class EnvironmentFinder
    {
        internal static IEnvironment AutoDetermineLocalEnvironment(string initialDirectory = null)
        {
            var os = Environment.OSVersion;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return new Linux(initialDirectory);
                default:
                    return new Windows(initialDirectory);
            }
        }
    }
}