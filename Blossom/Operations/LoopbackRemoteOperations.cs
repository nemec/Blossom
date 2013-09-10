using System.Collections.Generic;
using System.Linq;
using GlobDir;
using System;
using System.IO;

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
            handler.Filename = Path.GetFileName(sourcePath);
            sourcePath = Context.Environment.Local.CombinePath(
                Context.Environment.Local.CurrentDirectory, sourcePath);
            destinationPath = Context.Environment.Local.CombinePath(
                Context.Environment.Local.CurrentDirectory, destinationPath);

            if (!File.Exists(sourcePath))
            {
                handler.FileDoesNotExist();
                return true;
            }

            if (Directory.Exists(destinationPath))
            {
                destinationPath = Context.Environment.Local.CombinePath(
                    destinationPath, Path.GetFileName(sourcePath));
            }

            if (ifNewer && File.Exists(destinationPath) && 
                File.GetLastWriteTimeUtc(sourcePath) <= File.GetLastWriteTimeUtc(destinationPath))
            {
                handler.FileUpToDate();
                return false;
            }
            using (var file = File.OpenRead(sourcePath))
            {
                
                PutFile(file, destinationPath, handler);
            }
            return true;
        }

        public void PutFile(Stream source, string destinationPath, IFileTransferHandler handler)
        {
            CopyFile(source, destinationPath, 1024, handler);
        }

        public void MkDir(string path, bool makeParents = false)
        {
            var lastSlash = path.LastIndexOf(Context.Environment.Local.PathSeparator.Value(),
                                             StringComparison.Ordinal);
            if (!makeParents &&
                !Directory.Exists(path.Substring(0, 
                    lastSlash >= 0 ? lastSlash : path.Length)))
            {
                throw new IOException("Parent directory does not exist.");
            }
            Directory.CreateDirectory(path);
        }

        public void PutDir(string sourceDir, string destinationDir, Func<IFileTransferHandler> handlerFactory, bool ifNewer)
        {
            const string filter = "**"; // All files, recursive
            PutDir(sourceDir, destinationDir, handlerFactory, ifNewer, new[] { filter });
        }

        private delegate string CombinePathDelegate(params string[] paths);

        public void PutDir(string sourceDir, string destinationDir, 
            Func<IFileTransferHandler> handlerFactory, bool ifNewer, 
            IEnumerable<string> fileFilters)
        {
            MkDir(destinationDir, true);
            foreach (var filter in fileFilters)
            {
                CombinePathDelegate combineLocal = Context.Environment.Local.CombinePath;
                var pattern = combineLocal(sourceDir, filter);
                var matches = Glob.GetMatches(Utils.NormalizePathSeparators(pattern, PathSeparator.ForwardSlash)).ToList();
                if (!matches.Any())
                {
                    var handler = handlerFactory();
                    handler.Filename = Path.GetFileName(filter);
                    handler.FileDoesNotExist();
                }
                else
                {
                    foreach (var filePath in matches)
                    {
                        var tmpPath = filePath;
                        if (sourceDir.StartsWith("//") || sourceDir.StartsWith(@"\\"))
                        {
                            // TODO fix bug in GlobDir that trims the leading slash off
                            // https://github.com/giggio/globdir/issues/2
                            tmpPath = "/" + filePath;
                        }

                        var subdir = tmpPath.Substring(sourceDir.Length).TrimStart('/', '\\');  // Make relative
                        var newDestinationDir = combineLocal(destinationDir, subdir);

                        if (Directory.Exists(tmpPath))
                        {
                            MkDir(newDestinationDir);
                        }
                        else if (File.Exists(tmpPath))
                        {
                            PutFile(tmpPath, newDestinationDir, handlerFactory(), ifNewer);
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
            Directory.Delete(path, recursive);
        }

        public void Dispose()
        {
        }
    }
}
