using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Environments
{
    public static class EnvironmentFinder
    {
        public static IEnvironment AutoDetermineLocalEnvironment(string initialDirectory = null)
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
