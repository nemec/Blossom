﻿using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OperationsUnitTest")]

namespace Blossom.Deployment.Operations
{
    internal class BasicRemoteOperations : IRemoteOperations, IDisposable
    {
        private IDeploymentContext Context { get; set; }

        private SshClient Shell { get; set; }

        private SftpClient Sftp { get; set; }

        public Stream ShellStream { get; private set; }

        internal BasicRemoteOperations(IDeploymentContext context, Host host)
        {
            Context = context;

            Sftp = new SftpClient(host.Hostname, host.Username, host.Password);
            Sftp.OperationTimeout = TimeSpan.FromMinutes(2);
            Sftp.ConnectionInfo.Timeout = TimeSpan.FromMinutes(2);
            Sftp.BufferSize = 1024 * 16;
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
            else
            {
                return Shell.RunCommand(fullCommand).Result;
            }
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