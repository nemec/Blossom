using Blossom.Deployment;
using Blossom.Deployment.Attributes;
using Blossom.Deployment.ContextManagers;
using System;
using System.IO;

namespace Blossom.Examples.PushFiles
{
    internal class Tasks
    {
        private Config Config { get; set; }

        public Tasks(Config config)
        {
            Config = config;
        }

        [Task]
        public void CopyFiles(IDeploymentContext context)
        {
            foreach (var input in Config.InputDirs)
            using (new Lcd(context, input.Path))
            foreach (var output in input.OutputDirs)
            {
                context.RemoteOps.MkDir(output.Path, true);
                using(new Cd(context, output.Path))
                foreach (var file in output.Files)
                {
                    context.Logger.Info(String.Format("Copying file {0} to {1}.", file, output.Path));
                    if (!context.RemoteOps.PutFile(file, output.Path, new FileTransferHandler(context.Logger,
                        Path.GetFileName(file)), true))
                    {
                        context.Logger.Info("File already up to date. Not copying.");
                    }
                }
            }
        }
    }
}