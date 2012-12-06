using Blossom.Deployment;
using Blossom.Deployment.Attributes;
using Blossom.Deployment.ContextManagers;
using System;
using System.IO;

namespace Blossom.Examples.Compression
{
    internal class Tasks : IDeployment<Config>
    {
        private IDeploymentContext Context { get; set; }

        private Config Config { get; set; }

        [Task]
        public void CopyFiles()
        {
            foreach (var input in Config.InputDirs)
            using (new Lcd(Context, input.Path))
            foreach (var output in input.OutputDirs)
            {
                Context.RemoteOps.MkDir(output.Path, true);
                foreach (var file in output.Files)
                {
                    Context.Logger.Info(String.Format("Copying file {0} to {1}.", file, output.Path));
                    Context.RemoteOps.PutFile(file, output.Path, new FileTransferHandler(Context.Logger, 
                        Path.GetFileName(file)), true);
                }
            }
        }

        public void InitializeDeployment(IDeploymentContext context, Config config)
        {
            Context = context;
            Config = config;
        }
    }
}