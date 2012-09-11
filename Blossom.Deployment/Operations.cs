using Blossom.Deployment.Ssh;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Blossom.Deployment
{
    /// <summary>
    /// Defines a number of operations that may be performed on the running
    /// session. Most of the operations are taken directly from Fabric's API.
    /// </summary>
    /// <see cref="http://docs.fabfile.org/en/1.4.3/api/core/operations.html"/>
    public class Operations : IOperations, IDisposable
    {
        private IDeploymentContext Context { get; set; }

        private ISftp Sftp { get; set; }

        public Operations(IDeploymentContext context, ISftp sftp)
        {
            Context = context;
            Sftp = sftp;
            Sftp.Connect();
        }

        ~Operations()
        {
            Dispose(false);
        }
        
        public void Run(string command)
        {
            throw new NotImplementedException("Running a command not implemented.");
        }

        public void PutFile(string sourcePath, string destinationPath)
        {
            var filename = Path.GetFileName(sourcePath);
            var source = Utils.CombineLocalPath(
                Context,
                Context.Environment.Local.CurrentDirectory,
                sourcePath);
            var dest = Utils.CombineRemotePath(
                Context,
                Context.Environment.Remote.CurrentDirectory,
                destinationPath);

            if (Path.GetFileName(destinationPath) == "")
            {
                dest = Utils.CombineRemotePath(Context, dest, filename);
            }

            Sftp.Put(source, dest);
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
            if (Context.Environment.InteractionType == InteractionType.AskForInput)
            {
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
            }
            if (defaultResponse != null && String.IsNullOrWhiteSpace(response))
            {
                response = defaultResponse;
            }

            return response;
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