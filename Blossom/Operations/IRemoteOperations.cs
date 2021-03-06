﻿using System.Collections.Generic;
using System;
using System.IO;
using PathLib;

namespace Blossom.Operations
{
    /// <summary>
    /// Operations tha interact with a remote host.
    /// </summary>
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
        /// Copy a file from the remote host to a stream.
        /// </summary>
        /// <param name="sourcePath">Path and filename of the file to copy on the remote server.</param>
        /// <param name="destination">Destination stream.</param>
        /// <param name="handler">Display file transfer information.</param>
        void GetFile(string sourcePath, Stream destination, IFileTransferHandler handler);

        /// <summary>
        /// Copy a file from the remote host to the local host.
        /// </summary>
        /// <param name="sourcePath">Path and filename of the file to copy on the remote server.</param>
        /// <param name="destinationPath">Destination for the file on the local machine.</param>
        /// <param name="handler">Display file transfer information.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        /// <returns>True if the file was copied, false otherwise.</returns>
        bool GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer);

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationPath"></param>
        /// <param name="handler"></param>
        [Obsolete("Use the version with IPurePath parameters instead.")]
        void PutFile(Stream source, string destinationPath, IFileTransferHandler handler);

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationPath"></param>
        /// <param name="handler"></param>
        void PutFile(Stream source, IPurePath destinationPath, IFileTransferHandler handler);

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="sourcePath">Path and filename of the file to copy on the local machine.</param>
        /// <param name="destinationPath">Destination for the file on the remote server.</param>
        /// <param name="handler">Display file transfer information.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        [Obsolete("Use the version with IPurePath parameters instead.")]
        bool PutFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer);

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="sourcePath">Path and filename of the file to copy on the local machine.</param>
        /// <param name="destinationPath">Destination for the file on the remote server.</param>
        /// <param name="handler">Display file transfer information.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        bool PutFile(IPurePath sourcePath, IPurePath destinationPath, IFileTransferHandler handler, bool ifNewer);

        /// <summary>
        ///     Copy a directory (and all files, recursively)
        ///     from the local host to the remote host.
        /// 
        ///     Will create the destination directory and any
        ///     parents, if necessary.
        /// </summary>
        /// <param name="sourceDir">Directory to copy.</param>
        /// <param name="destinationDir">Destination directory where contents are copied.</param>
        /// <param name="handlerFactory">Produces <see cref="IFileTransferHandler"/>s for each copied file.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        [Obsolete("Use the version with IPurePath parameters instead.")]
        void PutDir(string sourceDir, string destinationDir,
                    Func<IFileTransferHandler> handlerFactory, bool ifNewer);

        /// <summary>
        ///     Copy a directory (and all files, recursively)
        ///     from the local host to the remote host.
        /// 
        ///     Will create the destination directory and any
        ///     parents, if necessary.
        /// </summary>
        /// <param name="sourceDir">Directory to copy.</param>
        /// <param name="destinationDir">Destination directory where contents are copied.</param>
        /// <param name="handlerFactory">Produces <see cref="IFileTransferHandler"/>s for each copied file.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        void PutDir(IPurePath sourceDir, IPurePath destinationDir,
                    Func<IFileTransferHandler> handlerFactory, bool ifNewer);

        /// <summary>
        ///     Copy a directory (and all files, recursively)
        ///     from the local host to the remote host.
        /// 
        ///     Will create the destination directory and any
        ///     parents, if necessary.
        /// </summary>
        /// <param name="sourceDir">Directory to copy.</param>
        /// <param name="destinationDir">Destination directory where contents are copied.</param>
        /// <param name="handlerFactory">Produces <see cref="IFileTransferHandler"/>s for each copied file.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        /// <param name="fileFilters">
        ///     List of filenames or globbed filenames that are allowed to be copied.
        ///     Glob characters allowed are * for any number of characters and 
        ///     ? for one arbitrary character.
        /// </param>
        [Obsolete("Use the version with IPurePath parameters instead.")]
        void PutDir(string sourceDir, string destinationDir,
                    Func<IFileTransferHandler> handlerFactory, bool ifNewer, IEnumerable<string> fileFilters);

        /// <summary>
        ///     Copy a directory (and all files, recursively)
        ///     from the local host to the remote host.
        /// 
        ///     Will create the destination directory and any
        ///     parents, if necessary.
        /// </summary>
        /// <param name="sourceDir">Directory to copy.</param>
        /// <param name="destinationDir">Destination directory where contents are copied.</param>
        /// <param name="handlerFactory">Produces <see cref="IFileTransferHandler"/>s for each copied file.</param>
        /// <param name="ifNewer">
        ///     If file exists on destination, only copy if that file is
        ///     older than the one being copied.
        /// </param>
        /// <param name="fileFilters">
        ///     List of filenames or globbed filenames that are allowed to be copied.
        ///     Glob characters allowed are * for any number of characters and 
        ///     ? for one arbitrary character.
        /// </param>
        void PutDir(IPurePath sourceDir, IPurePath destinationDir,
                    Func<IFileTransferHandler> handlerFactory, bool ifNewer, IEnumerable<string> fileFilters);

        /// <summary>
        /// Creates a directory on the remote computer.
        /// </summary>
        /// <param name="path">
        /// Directory path to create.
        /// </param>
        /// <param name="makeParents">
        ///     If true, go through each parent dir and make sure each
        ///     child directory exists before creating the final directory.
        /// </param>
        [Obsolete("Use the version with IPurePath parameters instead.")]
        void MkDir(string path, bool makeParents = false);

        /// <summary>
        /// Creates a directory on the remote computer.
        /// </summary>
        /// <param name="path">
        /// Directory path to create.
        /// </param>
        /// <param name="makeParents">
        ///     If true, go through each parent dir and make sure each
        ///     child directory exists before creating the final directory.
        /// </param>
        void MkDir(IPurePath path, bool makeParents = false);

        /// <summary>
        /// Remove a directory on the remote computer. Fails if
        /// <paramref name="recursive"/> is false and the directory is not
        /// empty.
        /// </summary>
        /// <param name="path">Directory to remove.</param>
        /// <param name="recursive">
        /// Whether to remove files and folders inside recursively.
        /// </param>
        [Obsolete("Use the version with IPurePath parameters instead.")]
        void RmDir(string path, bool recursive = false);

        /// <summary>
        /// Remove a directory on the remote computer. Fails if
        /// <paramref name="recursive"/> is false and the directory is not
        /// empty.
        /// </summary>
        /// <param name="path">Directory to remove.</param>
        /// <param name="recursive">
        /// Whether to remove files and folders inside recursively.
        /// </param>
        void RmDir(IPurePath path, bool recursive = false);
    }
}
