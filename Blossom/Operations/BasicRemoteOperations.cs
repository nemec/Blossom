using System.Collections.Generic;
using System.Linq;
using GlobDir;
using PathLib;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OperationsUnitTest")]

namespace Blossom.Operations
{
    internal class BasicRemoteOperations : IRemoteOperations
    {
        private IDeploymentContext Context { get; set; }

        private SshClient Shell { get; set; }

        private SftpClient Sftp { get; set; }

        public Stream ShellStream { get; private set; }

        internal BasicRemoteOperations(IDeploymentContext context, IHost host)
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
            var prefix = Context.RemoteEnv.PrefixString;
            var fullCommand = !String.IsNullOrWhiteSpace(prefix) ?
                prefix + " && " + command :
                command;
            if (Context.RemoteEnv.IsElevated)
            {
                return Shell.RunCommand(String.Format("{0} {1}",
                    Context.RemoteEnv.SudoPrefix,
                    fullCommand)).Result;
            }
            
            return Shell.RunCommand(fullCommand).Result;
        }

        public bool GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            if (ifNewer)
            {
                var modified = File.GetLastWriteTimeUtc(destinationPath);
                var remoteModified = DateTime.MinValue;
                try
                {
                    remoteModified = Sftp.GetLastWriteTimeUtc(sourcePath);
                }
                catch (SftpPathNotFoundException)
                {
                }
                if (remoteModified <= modified)
                {
                    if (handler != null)
                    {
                        handler.FileUpToDate();
                    }
                    return false;
                }
            }
            using (var file = File.OpenWrite(destinationPath))
            {
                var result = Sftp.BeginDownloadFile(
                    sourcePath, 
                    file, 
                    r => handler.TransferCompleted(), 
                    new object(), 
                    handler.IncrementBytesTransferred);
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
            var source = Context.LocalEnv.CreatePurePath(sourcePath);
            var dest = Context.RemoteEnv.CreatePurePath(destinationPath);
            return PutFile(source, dest, handler, ifNewer);
        }

        public bool PutFile(IPurePath sourcePath, IPurePath destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            var filename = sourcePath.Filename;
            var source = Context.LocalEnv.CurrentDirectory.Join(sourcePath);
            var dest = Context.RemoteEnv.CurrentDirectory.Join(destinationPath);

            if (handler != null)
            {
                handler.Filename = filename;
            }

            if (!File.Exists(source.ToString()))
            {
                if (handler != null)
                {
                    handler.FileDoesNotExist();
                }
                return false;
            }

            if (String.IsNullOrEmpty(dest.Filename) ||
                Sftp.GetAttributes(dest.ToString()).IsDirectory)
            {
                dest = dest.WithFilename(filename);
            }

            if (ifNewer)
            {
                var modified = File.GetLastWriteTimeUtc(source.ToString());
                var remoteModified = DateTime.MinValue;
                try
                {
                    remoteModified = Sftp.GetLastWriteTimeUtc(dest.ToString());
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
            using (var file = File.OpenRead(source.ToString()))
            {
                PutFile(file, dest, handler);
            }
            return true;
        }

        public void PutFile(Stream source, string destinationPath, IFileTransferHandler handler)
        {
            PutFile(source, Context.RemoteEnv.CreatePurePath(destinationPath), handler);
        }

        public void PutFile(Stream source, IPurePath destinationPath, IFileTransferHandler handler)
        {
            var result = Sftp.BeginUploadFile(
                source, 
                destinationPath.ToString(), 
                r => handler.TransferCanceled(), 
                new object(),
                handler.IncrementBytesTransferred);
            Sftp.EndUploadFile(result);
        }

        public void MkDir(string path, bool makeParents = false)
        {
            MkDir(Context.RemoteEnv.CreatePurePath(path), makeParents);
        }

        public void MkDir(IPurePath path, bool makeParents = false)
        {
            // Iteratively check whether or not each directory in the path exists
            // and create them if they do not.
            if (makeParents)
            {
                foreach (var parent in path.Parents().Reverse())
                {
                    var pathStr = parent.ToString();
                    if (!Sftp.Exists(pathStr))
                    {
                        Sftp.CreateDirectory(pathStr);
                    }
                }
            }
            else
            {
                var pathStr = path.ToString();
                if (!Sftp.Exists(pathStr))
                {
                    Sftp.CreateDirectory(pathStr);
                }
            }
        }

        public void PutDir(string sourceDir, string destinationDir,
            Func<IFileTransferHandler> handlerFactory, bool ifNewer)
        {
            var source = Context.LocalEnv.CreatePurePath(sourceDir);
            var dest = Context.RemoteEnv.CreatePurePath(destinationDir);
            PutDir(source, dest, handlerFactory, ifNewer);
        }

        public void PutDir(IPurePath sourceDir, IPurePath destinationDir,
            Func<IFileTransferHandler> handlerFactory, bool ifNewer)
        {
            const string pattern = "**"; // All files, recursive
            PutDir(sourceDir, destinationDir, handlerFactory, ifNewer, new []{ pattern });
        }

        public void PutDir(string sourceDir, string destinationDir,
            Func<IFileTransferHandler> handlerFactory, bool ifNewer,
            IEnumerable<string> fileFilters)
        {
            var source = Context.LocalEnv.CreatePurePath(sourceDir);
            var dest = Context.RemoteEnv.CreatePurePath(destinationDir);
            PutDir(source, dest, handlerFactory, ifNewer, fileFilters);
        }

        public void PutDir(IPurePath sourceDir, IPurePath destinationDir,
            Func<IFileTransferHandler> handlerFactory, bool ifNewer, 
            IEnumerable<string> fileFilters)
        {
            MkDir(destinationDir, true);
            foreach (var filter in fileFilters)
            {
                var pattern = sourceDir.Join(filter);
                var matches = Glob.GetMatches(pattern.AsPosix()).ToList();
                if (!matches.Any())
                {
                    var handler = handlerFactory();
                    handler.Filename = Path.GetFileName(filter);
                    handler.FileDoesNotExist();
                }
                else
                {
                    foreach (var file in matches)
                    {
                        var tmpPath = file;
                        if (sourceDir.Drive.StartsWith("//") || sourceDir.Drive.StartsWith(@"\\"))
                        {
                            // TODO fix bug in GlobDir that trims the leading slash off UNC paths
                            // https://github.com/giggio/globdir/issues/2
                            tmpPath = "/" + file;
                        }

                        var subdir = Context.RemoteEnv
                            .CreatePurePath(tmpPath)
                            .RelativeTo(sourceDir);

                        var newDestinationDir = destinationDir.Join(subdir);

                        if (Directory.Exists(tmpPath))
                        {
                            MkDir(newDestinationDir);
                        }
                        else if (File.Exists(tmpPath))
                        {
                            PutFile(tmpPath, newDestinationDir.ToString(), handlerFactory(), ifNewer);
                        }
                        else
                        {
                            Context.Logger.Error(String.Format(
                                "File or directory {0} does not exist locally.", tmpPath));
                        }
                    }
                }
            }
        }

        public void RmDir(string path, bool recursive)
        {
            RmDir(Context.RemoteEnv.CreatePurePath(path), recursive);
        }

        public void RmDir(IPurePath path, bool recursive)
        {
            var pathStr = path.ToString();
            if (!Sftp.GetAttributes(pathStr).IsDirectory)
            {
                throw new InvalidOperationException(String.Format(
                    "'{0}' is not a directory.", path));
            }
            if (!recursive && Sftp.ListDirectory(pathStr).Any())
            {
                throw new InvalidOperationException(String.Format(
                    "Directory '{0}' is not empty.", path));
            }
            Sftp.DeleteDirectory(pathStr);
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
