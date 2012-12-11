using Blossom.Deployment;
using Blossom.Deployment.Attributes;
using Blossom.Deployment.ContextManagers;
using System;

namespace Blossom.Examples.PushFiles
{
    internal class Tasks : IDeployment<Config>
    {
        private Config Config { get; set; }
        private IDeploymentContext Context { get; set; }

        public void InitializeDeployment(IDeploymentContext context, Config config)
        {
            Config = config;
            Context = context;
        }

        [Task]
        public void CopyFiles()
        {
            foreach (var input in Config.InputDirs)
            using (new Lcd(Context, input.Path))
            foreach (var output in input.OutputDirs)
            {
                Context.Logger.Info(String.Format("Copying files from {0} to {1}.", input.Path, output.Path));
                Context.RemoteOps.MkDir(output.Path, true);
                using (new Cd(Context, output.Path))
                {
                    Context.RemoteOps.PutDir(input.Path, output.Path,
                        new FileTransferHandler(Context.Logger), true, output.Files);
                }
            }
        }
    }
}