using Blossom.Deployment;
using Blossom.Deployment.ContextManagers;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blossom.Examples.PushFiles
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

        //[Task]
        public void TryCommand()
        {
            byte[] bytes = new byte[1024];
            var stream = Context.RemoteOps.ShellStream;

            var send = Encoding.ASCII.GetBytes("dir\r\n");
            //stream.Write(send, 0, 0);
            //stream.Flush();
            stream.Read(bytes, 0, 1024); // Welcome message
            Context.Logger.Info(Encoding.ASCII.GetString(bytes));
            send = Encoding.ASCII.GetBytes("dir\r\n");
            stream.Write(send, 0, send.Length);
            stream.Flush();
            Thread.Sleep(100);
            var total = 0;
            var read = 0;
            do
            {
                read = stream.Read(bytes, total, 1024 - total);
                total += read;
                
            } while (read > 0 && total < 1024);
            if (total > 0)
            {
                Context.Logger.Info(Encoding.ASCII.GetString(bytes, 0, total));
            }
        }

        [Task]
        public void CompressAndSend()
        {
            foreach (var input in Config.InputDirs)
            {
                var filename = "out.tar.gz";
                var destinationFolder = "dest";
                foreach (var output in input.OutputDirs)
                {
                    using (var filestream = File.OpenWrite(filename))
                    using (new Lcd(Context, input.Path))
                    {
                        Context.LocalOps.CompressFiles(filestream, output.Files.ToArray());
                    }
                    using (new Cd(Context, "/home/dan/"))
                    {
                        Context.RemoteOps.PutFile(filename, filename,
                            new FileTransferHandler(Context.Logger, filename), false);
                        Context.LocalOps.RunLocal("del " + filename);

                        Context.RemoteOps.MkDir(destinationFolder);
                        Context.Logger.Info(
                            Context.RemoteOps.RunCommand(String.Format(
                            "tar -C {0} -zxvf {1}", destinationFolder, filename)));
                        Context.RemoteOps.RunCommand("rm " + filename);
                    }
                }
            }
        }

        //[Task]
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
    }
}