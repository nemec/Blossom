using Blossom.Deployment.Ssh;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace Blossom.Deployment.Operations
{
    /// <summary>
    /// Defines a number of operations that may be performed on the running
    /// session. Most of the operations are taken directly from Fabric's API.
    /// </summary>
    /// <see cref="http://docs.fabfile.org/en/1.4.3/api/core/operations.html"/>
    public class BasicOperations : ILocalOperations, IRemoteOperations, IDisposable
    {
        private IDeploymentContext Context { get; set; }

        private IShell Shell { get; set; }

        private ISftp Sftp { get; set; }

        public BasicOperations(IDeploymentContext context, IShell shell, ISftp sftp)
        {
            Context = context;
            Sftp = sftp;
            Sftp.Connect();

            Shell = shell;
        }

        ~BasicOperations()
        {
            Dispose(false);
        }

        #region IRemoteOperations Members

        public Stream ShellStream { get { return Shell.Stream; } }
        
        public string RunCommand(string command)
        {
            var prefix = Context.Environment.Remote.PrefixString;
            var fullCommand = !String.IsNullOrWhiteSpace(prefix) ?
                prefix + " && " + command :
                command;
            if (Context.Environment.Remote.IsElevated)
            {
                return Shell.RunCommand(
                    Context.Environment.Remote.SudoPrefix +
                    " " +
                    fullCommand);
            }
            else
            {
                return Shell.RunCommand(fullCommand);
            }
        }

        public void GetFile(string sourcePath, string destinationPath, IFileTransferHandler handler)
        {
            Sftp.Get(sourcePath, destinationPath, handler);
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

            return Sftp.Put(source, dest, handler, ifNewer);
        }

        public void PutFile(Stream source, string destinationPath, IFileTransferHandler handler)
        {
            Sftp.Put(source, destinationPath, handler);
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
                    if(!Sftp.Exists(pathToCheck))
                    {
                        Sftp.Mkdir(pathToCheck);
                    }
                }
            }
            else
            {
                if(!Sftp.Exists(path))
                {
                    Sftp.Mkdir(path);
                }
            }
        }

        #endregion

        #region ILocalOperations Members

        /// <summary>
        /// Run a command on the local machine in the current working
        /// directory, waiting indefinitely for it to exit.
        /// </summary>
        /// <param name="command">Command (and optional arguments) to run.</param>
        /// <returns>Output of the command.</returns>
        public string RunLocal(string command)
        {
            return RunLocal(command, -1);
        }

        /// <summary>
        /// Runs a command on the local machine in the current working
        /// directory and return the output.
        /// 
        /// If the timeout is reached, the process will be terminated
        /// and an exception thrown.
        /// </summary>
        /// <param name="command">Command and arguments (separated by a space) to run.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException">Thrown when the timeout is reached before the process exits.</exception>
        /// <returns>Output of the command.</returns>
        public string RunLocal(string command, int timeout)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(Context.Environment.Local.ShellCommand)
            {
                Arguments = String.Format(@"{0} ""{1}""",
                    Context.Environment.Local.ShellStartArguments,
                    command),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                WorkingDirectory = Context.Environment.Local.CurrentDirectory,
            };

            var process = Process.Start(startInfo);
            process.StandardInput.WriteLine(command);
            process.StandardInput.Close();

            if (!process.WaitForExit(timeout))
            {
                process.Kill();
                throw new TimeoutException("Process timed out while waiting for exit.");
            }

            var outputText = process.StandardOutput.ReadToEnd();
            var errorText = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
            {
                // TODO display error message
            }

            if (String.IsNullOrWhiteSpace(errorText))
            {
                return outputText;
            }
            else if (String.IsNullOrEmpty(outputText))
            {
                return errorText;
            }
            else
            {
                return String.Format("{0}\n{1}", outputText, errorText);
            }
        }

        public void CompressFiles(string destination, params string[] sources)
        {
            CompressFiles(destination, true, sources);
        }

        public void CompressFiles(string destination, bool overwriteDestination, params string[] sources)
        {
            Stream filestream = null;
            try
            {
                if (overwriteDestination)
                {
                    filestream = File.Open(destination, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    filestream = File.Open(destination, FileMode.CreateNew, FileAccess.Write);
                }
                CompressFiles(filestream, sources);
            }
            finally
            {
                if (filestream != null)
                {
                    filestream.Dispose();
                }
            }
        }

        public void CompressFiles(Stream outputStream, params string[] sources)
        {
            CompressFiles(outputStream, sources, CompressionScheme.GZip, 5, -1);
        }

        public void CompressFiles(Stream outputStream, string[] sources,
            CompressionScheme scheme, int compressionLevel, int pathDepth)
        {
            if (scheme == CompressionScheme.None || scheme == CompressionScheme.GZip)
            {
                Stream innerStream = outputStream;
                if (scheme == CompressionScheme.GZip)
                {
                    innerStream = new GZipOutputStream(outputStream)
                    {
                        IsStreamOwner = false
                    };
                    ((GZipOutputStream)innerStream).SetLevel(compressionLevel);
                }

                using (var output = new TarOutputStream(innerStream))
                {
                    output.IsStreamOwner = false;
                    foreach (var source in sources)
                    {
                        if (String.IsNullOrWhiteSpace(source))
                        {
                            continue;
                        }
                        var fullpath = Context.Environment.Local.CombinePath(
                            Context.Environment.Local.CurrentDirectory, source);
                        if (File.Exists(fullpath))
                        {
                            TarFile(output, null, fullpath);
                        }
                        else
                        {
                            RecursiveTarDir(output, null, fullpath, pathDepth);
                        }
                    }
                }

                if (innerStream is GZipOutputStream)
                {
                    innerStream.Close();
                }
            }
            else
            {
                throw new NotImplementedException(String.Format(
                    "Compression Scheme {0} not yet implemented.", scheme.ToString()));
            }
        }

        private void RecursiveTarDir(TarOutputStream stream, string subdir, string source, int depth)
        {
            if (depth == 0)
            {
                return;
            }

            if (Directory.Exists(source))
            {
                subdir = subdir ?? "";
                foreach (var subfile in Directory.EnumerateFiles(source))
                {
                    TarFile(
                        stream,
                        Context.Environment.Local.CombinePath(subdir, Path.GetFileName(source)),
                        subfile);
                }
                foreach(var dir in Directory.EnumerateDirectories(source)){
                    string dirpath = Context.Environment.Local.CombinePath(
                        subdir,
                        Directory.GetParent(dir).Name);
                    RecursiveTarDir(stream, dirpath, dir, depth > 0 ? depth - 1 : depth);
                }
            }
            else
            {
                throw new IOException("Could not find path " + source);
            }
        }

        private void TarFile(TarOutputStream stream, string baseDir, string sourcePath)
        {
            var tarName = Context.Environment.Local.CombinePath(
                baseDir ?? "", Path.GetFileName(sourcePath));
            var entry = TarEntry.CreateTarEntry(tarName);
            using (var file = File.OpenRead(sourcePath))
            {
                entry.Size = file.Length;
                stream.PutNextEntry(entry);
                file.CopyTo(stream);
            }
            stream.CloseEntry();
        }

        public virtual string Prompt(
            string message, string defaultResponse,
            Func<string, bool> validateCallable, string validateRegex,
            string validationFailedMessage,
            TextWriter displayStream, TextReader inputStream)
        {
            if (Context.Environment.InteractionType == InteractionType.NonInteractive ||
                (Context.Environment.InteractionType == InteractionType.UseDefaults &&
                    defaultResponse == null))
            {
                throw new NonInteractiveSessionException("Task asked for prompt during non-interactive session.");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message parameter cannot be null");
            }

            if (displayStream == null)
            {
                displayStream = Console.Out;
            }
            if (inputStream == null)
            {
                inputStream = Console.In;
            }

            if (validateRegex != null)
            {
                if (!validateRegex.StartsWith("^"))
                {
                    validateRegex = "^" + validateRegex;
                }
                if (!validateRegex.EndsWith("$"))
                {
                    validateRegex = validateRegex + "$";
                }
            }

            displayStream.Write(message + " ");

            if (defaultResponse != null)
            {
                displayStream.Write(String.Format("[{0}] ", defaultResponse));
            }

            string response = null;
            switch(Context.Environment.InteractionType)
            {
                case InteractionType.AskForInput:
                    while (true)
                    {
                        response = inputStream.ReadLine();
                        if ((validateCallable == null && validateRegex == null) ||
                            (validateCallable != null && validateCallable(response)) ||
                            validateRegex != null && Regex.IsMatch(response, validateRegex))
                        {
                            break;
                        }
                        else
                        {
                            displayStream.WriteLine(validationFailedMessage ??
                                "Response did not pass validation. Please try again.");
                        }
                    }
                    if (defaultResponse != null && String.IsNullOrWhiteSpace(response))
                    {
                        response = defaultResponse;
                    }
                    break;
                case InteractionType.UseDefaults:
                    response = defaultResponse;
                    break;
                default:
                    throw new ArgumentException(
                        "Invalid InteractionType " + 
                        Context.Environment.InteractionType.ToString());
            }

            return response;
        }

        #endregion

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