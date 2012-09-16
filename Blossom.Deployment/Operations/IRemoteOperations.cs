using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Operations
{
    public interface IRemoteOperations
    {
        /// <summary>
        /// A stream enabling access to the shell's input and output.
        /// </summary>
        Stream ShellStream { get; }

        /// <summary>
        /// Run a shell command on the remote host.
        /// </summary>
        /// <param name="command"></param>
        string RunCommand(string command);

        void GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler);

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="handler"></param>
        /// <param name="ifNewer"></param>
        bool PutFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer);

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationPath"></param>
        /// <param name="handler"></param>
        void PutFile(Stream source, string destinationPath, IFileTransferHandler handler);

        /// <summary>
        /// Creates a directory on the remote computer.
        /// </summary>
        /// <param name="path">
        /// Directory path to create.
        /// </param>
        /// <param name="makeParents">
        /// If true, go through each parent dir and make sure each
        /// child directory exists before creating the final directory.
        /// </param>
        void MkDir(string path, bool makeParents = false);
    }
}
