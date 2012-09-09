using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment
{
    /// <summary>
    /// Defines a number of operations that may be performed on the running
    /// session. Most of the operations are taken directly from Fabric's API.
    /// </summary>
    /// <see cref="http://docs.fabfile.org/en/1.4.3/api/core/operations.html"/>
    public class Operations : IDisposable
    {
        private DeploymentContext Context { get; set; }
        private ChannelSftp Sftp { get; set; }
        private ChannelShell Shell { get; set; }

        public Operations(DeploymentContext context)
        {
            Context = context;
            Sftp = (ChannelSftp)Context.Environment.CurrentSession.openChannel("sftp");
            Sftp.connect();
            /*Shell = (ChannelShell)Context.Environment.CurrentSession.openChannel("shell");
            Shell.connect();*/
        }

        ~Operations()
        {
            Dispose(false);
        }

        /// <summary>
        /// Run a shell command on the remote host.
        /// </summary>
        /// <param name="command"></param>
        public void Run(string command)
        {
            throw new NotImplementedException("Running a command not implemented.");
            /*command = Utils.NormalizePathSeparators(
                command, Context.Environment.Remote.PathSeparator);

            using (var writer = new StreamWriter(Shell.getInputStream()))
            {
                writer.Write(command);
            }
            using (var reader = new StreamReader(Shell.getOutputStream()))
            {
                //Shell.
            }*/
        }

        /// <summary>
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public void PutFile(string sourcePath, string destinationPath)
        {
            var filename = Path.GetFileName(sourcePath);
            var source = Utils.CombineLocalPath(
                Context,
                Context.Environment.Local.CurrentDirectory,
                sourcePath);
            var dest= Utils.CombineRemotePath(
                Context,
                Context.Environment.Remote.CurrentDirectory,
                destinationPath);

            if (Path.GetFileName(destinationPath) == "")
            {
                dest = Utils.CombineRemotePath(Context, dest, filename);
            }
            
            Sftp.put(source, dest, new ProgressMonitor(Context.Logger), ChannelSftp.OVERWRITE);
        }

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
                    try
                    {
                        Sftp.stat(pathToCheck);
                    }
                    catch (SftpException exception)
                    {
                        if (exception.id == ChannelSftp.SSH_FX_NO_SUCH_FILE)
                        {
                            Sftp.mkdir(pathToCheck);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    Sftp.stat(path);
                }
                catch (SftpException exception)
                {
                    if (exception.id == ChannelSftp.SSH_FX_NO_SUCH_FILE)
                    {
                        Sftp.mkdir(path);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Prompt the user (on the local machine) for a response.
        /// </summary>
        /// <param name="message">Prompt message to display.</param>
        /// <param name="defaultResponse">
        /// If the user does not enter in a value (or just enters whitespace),
        /// this string will be substituted in the response.
        /// </param>
        /// <param name="validateCallable">
        /// This callable will be provided with the user's output and must
        /// provide a boolean return value indicating whether or not the 
        /// output passes validation.
        /// If validation fails, the validationFailedMessage will be displayed
        /// and the prompt repeated until the user passes validation or
        /// presses Ctrl-C.
        /// 
        /// If both Regex and Callable matching are provided, only the Callable
        /// will be compared with the output.
        /// </param>
        /// <param name="validateRegex">
        /// A regular expression used to validate the response.
        /// If validation fails, the validationFailedMessage will be displayed
        /// and the prompt repeated until the user passes validation or
        /// presses Ctrl-C. The regular expression must match the input
        /// *exactly* (eg. the rx will be bounded by `^` and `$` if necessary).
        /// 
        /// If both Regex and Callable matching are provided, only the Callable
        /// will be compared with the output.
        /// </param>
        /// <param name="validationFailedMessage">
        /// The message displayed when validation fails.
        /// </param>
        /// <param name="displayStream">
        /// An optional TextWriter containing the output stream to display
        /// prompt messages on. Defaults to Console.Out.
        /// </param>
        /// <param name="inputStream">
        /// An optional TextReader containing the input stream to read
        /// responses from. Defaults to Console.In.
        /// </param>
        /// <returns>
        /// The user's response (or default value, if provided).
        /// </returns>
        public virtual string Prompt(
            string message, string defaultResponse = null,
            Func<string, bool> validateCallable = null, string validateRegex = null,
            string validationFailedMessage = "",
            TextWriter displayStream = null, TextReader inputStream = null)
        {
            if (Context.Environment.InteractionType == InteractionType.NonInteractive ||
                (Context.Environment.InteractionType == InteractionType.UseDefaults &&
                    defaultResponse == null))
            {
                throw new NonInteractiveSessionException("Task asked for prompt during non-interactive session.");
            }

            if (message == null)
            {
                return null;
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
                throw new NotImplementedException("Regex not implemented.");
                if (!validateRegex.StartsWith("^"))
                {
                    validateRegex = "^" + validateRegex;
                }
                if (!validateRegex.EndsWith("$"))
                {
                    validateRegex = validateRegex + "$";
                }
            }

            displayStream.Write(message);
            displayStream.Write(" ");

            if (defaultResponse != null)
            {
                displayStream.Write(String.Format("[{0}] ", defaultResponse));
            }

            string response = null;
            if (Context.Environment.InteractionType == InteractionType.AskForInput)
            {
                while (true)
                {
                    response = inputStream.ReadLine();
                    if ((validateCallable == null && validateRegex == null) ||
                        (validateCallable != null && validateCallable(response)))
                    {
                        break;
                    }
                    else
                    {
                        displayStream.WriteLine(validationFailedMessage ??
                            "Response did not pass validation. Please try again.");
                    }
                }
            }
            if (defaultResponse != null && String.IsNullOrWhiteSpace(response))
            {
                response = defaultResponse;
            }

            return response;
        }

        protected virtual void Dispose(bool freeManagedObjects)
        {
            if (Sftp != null && Sftp.isConnected())
            {
                Sftp.disconnect();
                Sftp = null;
            }
            if (Shell != null && Shell.isConnected())
            {
                Shell.disconnect();
                Shell = null;
            }

        }
    
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
