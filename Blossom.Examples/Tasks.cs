using Blossom.Deployment.ContextManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    class Tasks
    {
        private DeploymentContext Context { get; set; }

        private Config Config { get; set; }

        public Tasks(DeploymentContext context, Config config)
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

        //[Task]
        public void Test()
        {
            Console.WriteLine(Context.Operations.Prompt("What?"));
            Console.WriteLine(Context.Operations.Prompt("Okay.", "Nothing."));
            Console.WriteLine(Context.Operations.Prompt("Valid.",
                validateCallable: (r => r == "hello"),
                validationFailedMessage: "Please enter hello."));
            /*Console.WriteLine(Context.Operations.Prompt("Regex.",
                validateRegex: @"\d+", validationFailedMessage: "Please enter a number."));*/
        }
    }
}
