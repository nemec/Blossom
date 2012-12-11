using System.Linq;
using GlobDir;
using Renci.SshNet.Sftp;
using System;
using System.IO;

namespace Blossom.Deployment.Operations
{
    internal class LoopbackRemoteOperations : IRemoteOperations
    {
        private IDeploymentContext Context { get; set; }

        // Already implements a few operations that we need.
        private BasicLocalOperations LocalOps { get; set; }

        internal LoopbackRemoteOperations(IDeploymentContext context)
        {
            Context = context;
            LocalOps = new BasicLocalOperations(context);
        }

        public Stream ShellStream
        {
            get { throw new NotImplementedException(); }
        }

        public string RunCommand(string command)
        {
            return LocalOps.RunLocal(command);
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
            if (!makeParents &&
                !Directory.Exists(path.Substring(0,
                path.LastIndexOf(Context.Environment.Local.PathSeparator.Value(),
                    StringComparison.Ordinal))))
            {
                throw new IOException("Parent directory does not exist.");
            }
            Directory.CreateDirectory(path);
        }

        public void Dispose()
        {
        }


        public void PutDir(string sourceDir, string destinationDir, IFileTransferHandler handler, bool ifNewer)
        {
            PutDir(sourceDir, destinationDir, handler, ifNewer, new[] { "*" });
        }

        public void PutDir(string sourceDir, string destinationDir, IFileTransferHandler handler, bool ifNewer, System.Collections.Generic.IEnumerable<string> fileFilters)
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
    }
}
