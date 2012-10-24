using Renci.SshNet.Sftp;
using System;
using System.IO;

namespace Blossom.Deployment.Operations
{
    public interface IRemoteOperations : IDisposable
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

        /// <summary>
        /// Copy a file from the remote host to the local host.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="handler"></param>
        /// <param name="ifNewer"></param>
        /// <returns></returns>
        bool GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer);

        /// <summary>
        /// Copy a file from the remote host to a stream.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destination"></param>
        /// <param name="handler"></param>
        void GetFile(string sourcePath, Stream destination, IFileTransferHandler handler);

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
