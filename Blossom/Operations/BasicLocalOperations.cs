using Blossom.Exceptions;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OperationsUnitTest")]

namespace Blossom.Operations
{
    /// <summary>
    /// Defines a number of operations that may be performed on the running
    /// session. Most of the operations are taken directly from Fabric's API.
    /// </summary>
    /// <see cref="http://docs.fabfile.org/en/1.4.3/api/core/operations.html"/>
    internal class BasicLocalOperations : ILocalOperations
    {
        private IDeploymentContext Context { get; set; }

        private readonly object _lock;

        internal BasicLocalOperations(IDeploymentContext context)
        {
            Context = context;
            _lock = new object();
        }

        /// <summary>
        /// Run a command on the local machine in the current working
        /// directory, waiting indefinitely for it to exit.
        /// </summary>
        /// <param name="command">Command (and optional arguments) to run.</param>
        /// <returns>Output of the command.</returns>
        public string Run(string command)
        {
            return Run(command, TimeSpan.MaxValue);
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
        public string Run(string command, TimeSpan timeout)
        {
            var startInfo = new ProcessStartInfo(Context.Environment.Local.ShellCommand)
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

            if (Context.Environment.Local.IsElevated)
            {
                startInfo.Verb = "runas";
            }

            var process = Process.Start(startInfo);
            process.StandardInput.WriteLine(command);
            process.StandardInput.Close();

            if (!process.WaitForExit((int)timeout.TotalMilliseconds))
            {
                process.Kill();
                throw new TimeoutException("Process timed out while waiting for exit.");
            }

            var outputText = process.StandardOutput.ReadToEnd();
            var errorText = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
            {
                Context.Logger.Error(String.Format(
                    "Process executed with nonzero error code [{0}]: {1}",
                    process.ExitCode, errorText));
            }

            if (String.IsNullOrWhiteSpace(errorText))
            {
                return outputText;
            }
            if (String.IsNullOrEmpty(outputText))
            {
                return errorText;
            }
            return String.Format("{0}\n{1}", outputText, errorText);
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
                filestream = File.Open(destination,
                    overwriteDestination ?
                        FileMode.Truncate :
                        FileMode.CreateNew,
                    FileAccess.Write);
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
                var innerStream = outputStream;
                try
                {
                    switch (scheme)
                    {
                        case CompressionScheme.None:
                            // Nothing to do...
                            break;
                        case CompressionScheme.GZip:
                            innerStream = new GZipOutputStream(outputStream)
                                {
                                    IsStreamOwner = false
                                };
                            ((GZipOutputStream) innerStream).SetLevel(compressionLevel);
                            break;
                        default:
                            throw new NotImplementedException(String.Format(
                                "Compression Scheme {0} not yet implemented.", scheme));
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
                }
                finally
                {
                    if (!ReferenceEquals(innerStream, outputStream))
                    {
                        // We own the stream... close it.
                        innerStream.Close();
                    }
                }
            }
        }

        private void RecursiveTarDir(TarOutputStream stream, string subdir, string source, int depth)
        {
            if (depth == 0)
            {
                return;
            }

            if (!Directory.Exists(source))
            {
                throw new IOException("Could not find path " + source);
            }

            subdir = subdir ?? "";
            foreach (var subfile in Directory.EnumerateFiles(source))
            {
                TarFile(
                    stream,
                    Context.Environment.Local.CombinePath(subdir, Path.GetFileName(source)),
                    subfile);
            }
            foreach (var dir in Directory.EnumerateDirectories(source))
            {
                var dirpath = Context.Environment.Local.CombinePath(
                    subdir,
                    Directory.GetParent(dir).Name);
                RecursiveTarDir(stream, dirpath, dir, depth > 0 ? depth - 1 : depth);
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

        public virtual string PromptWithNoValidation(string message, string defaultResponse = null,
            TextWriter displayStream = null, TextReader inputStream = null)
        {
            return PromptWithCallbackValidation(message, s => true,
                defaultResponse, null, displayStream, inputStream);
        }

        public virtual string PromptWithRegexValidation(
            string message, string validateRegex, string defaultResponse,
            string validationFailedMessage,
            TextWriter displayStream, TextReader inputStream)
        {
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

            Func<string, bool> regexCallback = (response =>
                Regex.IsMatch(response, validateRegex ?? ""));

            return PromptWithCallbackValidation(message, regexCallback, defaultResponse,
                validationFailedMessage, displayStream, inputStream);
        }

        public virtual string PromptWithCallbackValidation(
            string message, Func<string, bool> validationCallback, string defaultResponse,
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
                throw new ArgumentException("Message parameter cannot be null");
            }

            displayStream = displayStream ?? Console.Out;
            inputStream = inputStream ?? Console.In;

            string response;

            // Ensure that only one prompt is active at a time.
            lock (_lock)
            {
                displayStream.Write(message + " ");

                if (defaultResponse != null)
                {
                    displayStream.Write("[{0}] ", defaultResponse);
                }

                switch (Context.Environment.InteractionType)
                {
                    case InteractionType.AskForInput:
                        while (true)
                        {
                            response = inputStream.ReadLine();
                            if (validationCallback(response))
                            {
                                break;
                            }

                            displayStream.WriteLine(validationFailedMessage ??
                                "Response did not pass validation. Please try again.");
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
            }

            return response;
        }
    }
}