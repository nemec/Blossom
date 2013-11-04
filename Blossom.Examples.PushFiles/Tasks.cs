using Blossom;
using Blossom.Attributes;
using Blossom.ContextManagers;
using System;

namespace Blossom.Examples.PushFiles
{
    internal class Tasks : IDeploymentTasks<Config>
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
                Context.Logger.Info(String.Format("Copying files from {0} to {1}.", input.Path, output.Path));
                Context.RemoteOps.MkDir(output.Path, true);
                using (new Cd(Context, output.Path))
                {
                    Context.RemoteOps.PutDir(input.Path, output.Path,
                        () => new FileTransferHandler(Context.Logger), false, output.Files);
                }
            }
        }
    }
}