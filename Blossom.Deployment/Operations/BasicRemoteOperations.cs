using System.Collections.Generic;
using System.Linq;
using GlobDir;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("OperationsUnitTest")]

namespace Blossom.Deployment.Operations
{
    internal class BasicRemoteOperations : IRemoteOperations
    {
        private IDeploymentContext Context { get; set; }

        private SshClient Shell { get; set; }

        private SftpClient Sftp { get; set; }

        public Stream ShellStream { get; private set; }

        internal BasicRemoteOperations(IDeploymentContext context, Host host)
        {
            Context = context;

            Sftp = new SftpClient(host.Hostname, host.Username, host.Password)
                {
                    OperationTimeout = TimeSpan.FromMinutes(2),
                    ConnectionInfo = {Timeout = TimeSpan.FromMinutes(2)},
                    BufferSize = 1024*16
                };
            Sftp.Connect();

            Shell = new SshClient(host.Hostname, host.Username, host.Password);
            Shell.Connect();
            ShellStream = Shell.CreateShellStream(
                String.Format("{0}@{1}", host.Username, host.Hostname), 0, 0, 0, 0, 1024);
        }

        ~BasicRemoteOperations()
        {
            Dispose(false);
        }

        public string RunCommand(string command)
        {
            var prefix = Context.Environment.Remote.PrefixString;
            var fullCommand = !String.IsNullOrWhiteSpace(prefix) ?
                prefix + " && " + command :
                command;
            if (Context.Environment.Remote.IsElevated)
            {
                return Shell.RunCommand(String.Format("{0} {1}",
                    Context.Environment.Remote.SudoPrefix,
                    fullCommand)).Result;
            }
            
            return Shell.RunCommand(fullCommand).Result;
        }

        public bool GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            if (ifNewer)
            {
                throw new NotImplementedException(); // TODO check remote modified time.
            }
            using (var file = File.OpenWrite(destinationPath))
            {
                var result = Sftp.BeginDownloadFile(sourcePath, file, handler, new object());
                Sftp.EndDownloadFile(result);
            }
            return true;
        }

        public void GetFile(string sourcePath, Stream destination, IFileTransferHandler handler)
        {
            throw new NotImplementedException(); // TODO SFTP get to a stream.
        }


        public bool PutFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            var filename = Path.GetFileName(sourcePath);
            var source = Context.Environment.Local.CombinePath(
                Context.Environment.Local.CurrentDirectory,
                sourcePath);
            var dest = Context.Environment.Remote.CombinePath(
                Context.Environment.Remote.CurrentDirectory,
                destinationPath);

            if (handler != null)
            {
                handler.Filename = filename;
            }

            if (!File.Exists(source))
            {
                if (handler != null)
                {
                    handler.FileDoesNotExist();
                }
                return false;
            }

            if (Path.GetFileName(destinationPath) == "")
            {
                dest = Context.Environment.Remote.CombinePath(dest, filename);
            }

            if (ifNewer)
            {
                var modified = File.GetLastWriteTimeUtc(source);
                var remoteModified = DateTime.MinValue;
                try
                {
                    remoteModified = Sftp.GetLastWriteTimeUtc(dest);
                }
                catch (SftpPathNotFoundException)
                {
                }
                if (modified <= remoteModified)
                {
                    if (handler != null)
                    {
                        handler.FileUpToDate();
                    }
                    return false;
                }
            }
            using (var file = File.OpenRead(source))
            {
                PutFile(file, dest, handler);
            }
            return true;
        }

        public void PutFile(Stream source, string destinationPath, IFileTransferHandler handler)
        {
            var result = Sftp.BeginUploadFile(source, destinationPath, handler, new object());
            Sftp.EndUploadFile(result);
        }

        public void MkDir(string path, bool makeParents = false)
        {
            // Iteratively check whether or not each directory in the path exists
            // and create them if they do not.
            if (makeParents)
            {
                var currentPath = new StringBuilder();
                foreach (var dir in path.Split(
                    Context.Environment.Remote.PathSeparator.Value().ToCharArray()))
                {
                    currentPath.Append(dir);
                    currentPath.Append(Context.Environment.Remote.PathSeparator.Value());
                    var pathToCheck = currentPath.ToString();
                    if (!Sftp.Exists(pathToCheck))
                    {
                        Sftp.CreateDirectory(pathToCheck);
                    }
                }
            }
            else
            {
                if (!Sftp.Exists(path))
                {
                    Sftp.CreateDirectory(path);
                }
            }
        }

        public void PutDir(string sourceDir, string destinationDir,
            IFileTransferHandler handler, bool ifNewer)
        {
            PutDir(sourceDir, destinationDir, handler, ifNewer, new []{"*"});
        }

        public void PutDir(string sourceDir, string destinationDir,
            IFileTransferHandler handler, bool ifNewer, IEnumerable<string> fileFilters)
        {
            MkDir(destinationDir, true);
            foreach (var filter in fileFilters)
            {
                var pattern = Context.Environment.Local.CombinePath(sourceDir, filter);
                var matches = Glob.GetMatches(Utils.NormalizePathSeparators(pattern, PathSeparator.ForwardSlash));
                if (!matches.Any())
                {
                    handler.Filename = Path.GetFileName(filter);
                    handler.FileDoesNotExist();
                    continue;
                }
                foreach (var file in matches)
                {
                    PutFile(file, destinationDir, handler, ifNewer);
                }
            }
        }

        protected virtual void Dispose(bool freeManagedObjects)
        {
            if (Sftp != null && Sftp.IsConnected)
            {
                Sftp.Disconnect();
                Sftp = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
