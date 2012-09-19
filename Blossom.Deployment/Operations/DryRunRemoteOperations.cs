using Blossom.Deployment.Logging;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Operations
{
    internal class DryRunRemoteOperations : IRemoteOperations, IDisposable
    {
        private ILogger Logger { get; set; }

        internal DryRunRemoteOperations(ILogger logger)
        {
            Logger = logger;
        }

        private void LogDryRun(string commandName, string output)
        {
            Logger.Info(
                String.Format("Dry run executing {0}: {1}", commandName, output));
        }

        private Stream _stream = new MemoryStream();
        public Stream ShellStream
        {
            get { return _stream; }
        }

        public string RunCommand(string command)
        {
            LogDryRun("RunCommand", String.Format(
                "Running command [{0}] remotely.", command));
            return "";
        }

        public bool GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            LogDryRun("GetFile", String.Format("Retrieving the file {0} from a remote host " +
                "and placing it at {1}.", sourcePath, destinationPath));
            return false;
        }

        public void GetFile(string sourcePath, System.IO.Stream destination, IFileTransferHandler handler)
        {
            LogDryRun("GetFile", String.Format("Retrieving the file {0} from a remote host " +
                "and writing it to a stream.", sourcePath));
        }

        public bool PutFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            LogDryRun("PutFile", String.Format("Sending the file {0} to a remote host " +
                "and placing it at {1}.", sourcePath, destinationPath));
            return false;
        }

        public void PutFile(System.IO.Stream source, string destinationPath, IFileTransferHandler handler)
        {
            LogDryRun("PutFile", String.Format("Sending contents of a stream to a remote host " +
                            "and placing it at {0}.", destinationPath));
        }

        public void MkDir(string path, bool makeParents = false)
        {
            var output = String.Format("Creating directory {0}", path);
            if(makeParents)
            {
                output += "\nAlso creating all missing subdirectories.";
            }
            LogDryRun("MkDir", output);
        }

        public void Dispose()
        {
        }
    }
}
