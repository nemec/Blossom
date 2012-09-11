﻿using Blossom.Deployment;
using Blossom.Deployment.ContextManagers;
using System;
using System.Threading.Tasks;

namespace Blossom.Examples
{
    internal class Tasks
    {
        private IDeploymentContext Context { get; set; }

        private Config Config { get; set; }

        public Tasks(IDeploymentContext context, Config config)
        {
            Context = context;
            Config = config;
        }

        [Task]
        public void CopyFiles()
        {
            foreach (var input in Config.InputDirs)
            {
                using (new Lcd(Context, input.Path))
                {
                    foreach (var output in input.OutputDirs)
                    {
                        Context.Operations.MkDir(output.Path, true);
                        foreach (var file in output.Files)
                        {
                            Context.Operations.PutFile(file, output.Path);
                        }
                    }
                }
            }
        }
    }
}