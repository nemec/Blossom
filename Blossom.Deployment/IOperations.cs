using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment
{
    public interface IOperations : IDisposable
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
        /// Copy a file from the local host to the remote host.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        void PutFile(string sourcePath, string destinationPath);

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

        /// <summary>
        /// Prompt the user (on the local machine) for a response.
        /// </summary>
        /// <param name="message">Prompt message to display.</param>
        /// <param name="defaultResponse">
        /// This string will be substituted in the response if the user
        /// does not enter in a value (or just enters whitespace).
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
        string Prompt(
            string message, string defaultResponse = null,
            Func<string, bool> validateCallable = null, string validateRegex = null,
            string validationFailedMessage = "",
            TextWriter displayStream = null, TextReader inputStream = null);
    }
}
