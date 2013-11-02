using System;
using Blossom;
using Blossom.Attributes;
using Blossom.ContextManagers;
using Blossom.Logging;

// ReSharper disable CheckNamespace
public class ScriptingTasks
// ReSharper restore CheckNamespace
{
    public class Config : IConfig
    {
        public void InitializeDeployment(IDeploymentConfig config)
        {
            config.Hosts = new[] {
                new Host
                {
                    Alias = "localhost",
                    Hostname = "loopback",
                    Roles = "itg;pro"
                }
            };
            config.Logger = new ColorizedConsoleLogger();
        }

        public void InitializeContext(IDeploymentContext context)
        {
        }

        public InputDir[] InputDirs = {
            new InputDir
            {
                Path= 
                    @"C:\Users\nemecd\prg\AutoUpdateSoftwareDownload\AutoUpdateSoftwareDownload\bin\Release",
                OutputDirs = new []{ 
                    new OutputDir
                    {
                        Path = @"C:\Users\nemecd\tmp\testfiles",
                        Files = new[]
                        {
                            "AutoUpdateSoftwareDownload.exe",
                            "AutoUpdateSoftwareDownload.exe.config",
                            "CDS_SOAR.cmd",
                            "*.dll"
                        }
                    }
                }
            }
        };
    }

    public class InputDir
    {
        public string Path { get; set; }
        public OutputDir[] OutputDirs { get; set; }
    }

    public class OutputDir
    {
        public string Path { get; set; }
        public string[] Files { get; set; }
    }

    public class Tasks : IDeployment<Config>
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
