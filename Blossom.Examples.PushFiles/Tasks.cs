using Blossom.Deployment;
using Blossom.Deployment.Attributes;
using Blossom.Deployment.ContextManagers;
using System;
using System.IO;

namespace Blossom.Examples.PushFiles
{
    internal class Tasks : IDeployment<Config>
    {
        public Config Config { get; set; }
        public IDeploymentContext Context { get; set; }

        [Task]
        public void CopyFiles()
        {
            foreach (var input in Config.InputDirs)
            using (new Lcd(Context, input.Path))
            foreach (var output in input.OutputDirs)
            {
                Context.RemoteOps.MkDir(output.Path, true);
                using(new Cd(Context, output.Path))
                foreach (var file in output.Files)
                {
                    Context.Logger.Info(String.Format("Copying file {0} to {1}.", file, output.Path));
                    if (!Context.RemoteOps.PutFile(file, output.Path, new FileTransferHandler(Context.Logger,
                        Path.GetFileName(file)), true))
                    {
                        Context.Logger.Info("File already up to date. Not copying.");
                    }
                }
            }
        }
    }
}