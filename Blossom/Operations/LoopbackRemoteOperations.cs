using System.Collections.Generic;
using System.Linq;
using GlobDir;
using System;
using System.IO;
using PathLib;

namespace Blossom.Operations
{
    internal class LoopbackRemoteOperations : IRemoteOperations
    {
        private IDeploymentContext Context { get; set; }

        // Already implements a few operations that we need.
        private ILocalOperations LocalOps { get; set; }

        internal LoopbackRemoteOperations(IDeploymentContext context)
            : this(context, new BasicLocalOperations(context))
        {
        }

        internal LoopbackRemoteOperations(IDeploymentContext context, ILocalOperations localOperations)
        {
            Context = context;
            LocalOps = localOperations;
        }

        public Stream ShellStream
        {
            get { throw new NotImplementedException(); }
        }

        public string RunCommand(string command)
        {
            return LocalOps.Run(command);
        }

        private static void CopyFile(Stream source, string destination, int bytesPerChunk, IFileTransferHandler handler)
        {
            using (var br = new BinaryReader(source))
            {
                using (var fsDest = new FileStream(destination, FileMode.Create))
                {
                    var bw = new BinaryWriter(fsDest);
                    var buffer = new byte[bytesPerChunk];
                    int bytesRead;

                    for (long i = 0; i < source.Length; i += bytesRead)
                    {
                        bytesRead = br.Read(buffer, 0, bytesPerChunk);
                        bw.Write(buffer, 0, bytesRead);
                        handler.IncrementBytesTransferred((ulong)bytesRead);
                    }
                }
            }
            handler.TransferCompleted();
        }

        public bool GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            return PutFile(destinationPath, sourcePath, handler, ifNewer);
        }

        public void GetFile(string sourcePath, Stream destination, IFileTransferHandler handler)
        {
            PutFile(destination, sourcePath, handler);
        }

        public bool PutFile(string sourcePath, string destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            var source = Context.LocalEnv.CreatePurePath(sourcePath);
            var dest = Context.LocalEnv.CreatePurePath(destinationPath);
            return PutFile(source, dest, handler, ifNewer);
        }

        public bool PutFile(IPurePath sourcePath, IPurePath destinationPath, IFileTransferHandler handler, bool ifNewer)
        {
            var filename = sourcePath.Filename;
            var source = Context.LocalEnv.CurrentDirectory.Join(sourcePath);
            var dest = Context.LocalEnv.CurrentDirectory.Join(destinationPath);

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
                Directory.Exists(dest.ToString()))
            {
                dest = dest.WithFilename(filename);
            }

            var sourceStr = source.ToString();
            var destStr = dest.ToString();

            if (ifNewer && File.Exists(destStr) && 
                File.GetLastWriteTimeUtc(sourceStr) <= File.GetLastWriteTimeUtc(destStr))
            {
                if (handler != null)
                {
                    handler.FileUpToDate();
                }
                return false;
            }
            using (var file = File.OpenRead(sourceStr))
            {
                
                PutFile(file, destStr, handler);
            }
            return true;
        }

        public void PutFile(Stream source, string destinationPath, IFileTransferHandler handler)
        {
            PutFile(source, Context.LocalEnv.CreatePurePath(destinationPath), handler);
        }

        public void PutFile(Stream source, IPurePath destinationPath, IFileTransferHandler handler)
        {
            CopyFile(source, destinationPath.ToString(), 1024, handler);
        }

        public void MkDir(string path, bool makeParents = false)
        {
            MkDir(Context.LocalEnv.CreatePurePath(path), makeParents);
        }

        public void MkDir(IPurePath path, bool makeParents = false)
        {
            if (!makeParents &&
                !Directory.Exists(path.Parent().ToString()))
            {
                throw new IOException("Parent directory does not exist.");
            }
            Directory.CreateDirectory(path.ToString());
        }

        public void PutDir(string sourceDir, string destinationDir, Func<IFileTransferHandler> handlerFactory,
            bool ifNewer)
        {
            var source = Context.LocalEnv.CreatePurePath(sourceDir);
            var dest = Context.LocalEnv.CreatePurePath(destinationDir);
            PutDir(source, dest, handlerFactory, ifNewer);
        }

        public void PutDir(IPurePath sourceDir, IPurePath destinationDir, Func<IFileTransferHandler> handlerFactory, bool ifNewer)
        {
            const string filter = "**"; // All files, recursive
            PutDir(sourceDir, destinationDir, handlerFactory, ifNewer, new[] { filter });
        }

        public void PutDir(string sourceDir, string destinationDir,
            Func<IFileTransferHandler> handlerFactory, bool ifNewer,
            IEnumerable<string> fileFilters)
        {
            var source = Context.LocalEnv.CreatePurePath(sourceDir);
            var dest = Context.LocalEnv.CreatePurePath(destinationDir);
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

                        var subdir = Context.LocalEnv
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
            RmDir(Context.LocalEnv.CreatePurePath(path), recursive);
        }

        public void RmDir(IPurePath path, bool recursive)
        {
            Directory.Delete(path.ToString(), recursive);
        }

        public void Dispose()
        {
        }
    }
}
