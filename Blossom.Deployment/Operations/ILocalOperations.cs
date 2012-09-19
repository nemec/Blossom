using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Operations
{
    public interface ILocalOperations
    {
        string RunLocal(string command);

        string RunLocal(string command, int timeout);

        /// <summary>
        /// Compress a series of files and folders into a single
        /// destination file.
        /// </summary>
        /// <param name="destination">Compressed file name and path.</param>
        /// <param name="sourcePaths">Files or directories to compress.</param>
        void CompressFiles(string destination, params string[] sourcePaths);

        /// <summary>
        /// Compress a series of files and folders into a single
        /// destination file.
        /// </summary>
        /// <param name="destination">Compressed file name and path.</param>
        /// <param name="sourcePaths">Files or directories to compress.</param>
        /// <param name="overwriteDestination">Overwrite existing compressed file, if exists.</param>
        void CompressFiles(string destination, bool overwriteDestination, params string[] sourcePaths);

        /// <summary>
        /// Compress files and send the compressed output to a stream.
        /// </summary>
        /// <param name="destination">Stream where compressed data is written.</param>
        /// <param name="sourcePaths">Files and directories to compress.</param>
        /// <returns></returns>
        void CompressFiles(Stream destination, params string[] sourcePaths);

        /// <summary>
        /// Compress files and sent the compressed output to a stream.
        /// The compression scheme is customizable along with the compression
        /// level and how deep to recurse down the directory tree.
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="sources"></param>
        /// <param name="scheme"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="pathDepth"></param>
        void CompressFiles(Stream outputStream, string[] sources,
            CompressionScheme scheme, int compressionLevel, int pathDepth);

        /// <summary>
        /// Prompt the user (on the local machine) for a response.
        /// </summary>
        /// <param name="message">Prompt message to display.</param>
        /// <param name="defaultResponse">
        /// This string will be substituted in the response if the user
        /// does not enter in a value (or just enters whitespace).
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
        string PromptWithNoValidation(
            string message, string defaultResponse = null,
            TextWriter displayStream = null, TextReader inputStream = null);

        /// <summary>
        /// Prompt the user (on the local machine) for a response.
        /// Response passes validation if it matches the given
        /// regular expression.
        /// </summary>
        /// <param name="message">Prompt message to display.</param>
        /// <param name="validateCallable">
        /// This callable will be provided with the user's output and must
        /// provide a boolean return value indicating whether or not the
        /// output passes validation.
        /// If validation fails, the validationFailedMessage will be displayed
        /// and the prompt repeated until the user passes validation or
        /// presses Ctrl-C.
        /// </param>
        /// <param name="validationFailedMessage">
        /// The message displayed when validation fails.
        /// </param>
        /// <param name="defaultResponse">
        /// This string will be substituted in the response if the user
        /// does not enter in a value (or just enters whitespace).
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
        /// </returns>
        string PromptWithRegexValidation(string message, string validateRegex,
            string validationFailedMessage = "", string defaultResponse = null,
            TextWriter displayStream = null, TextReader inputStream = null);

        /// <summary>
        /// Prompt the user (on the local machine) for a response.
        /// Validates the response by checking the callback.
        /// </summary>
        /// <param name="message">Prompt message to display.</param>
        /// <param name="validateCallable">
        /// This callable will be provided with the user's output and must
        /// provide a boolean return value indicating whether or not the
        /// output passes validation.
        /// If validation fails, the validationFailedMessage will be displayed
        /// and the prompt repeated until the user passes validation or
        /// presses Ctrl-C.
        /// </param>
        /// <param name="validationFailedMessage">
        /// The message displayed when validation fails.
        /// </param>
        /// <param name="defaultResponse">
        /// This string will be substituted in the response if the user
        /// does not enter in a value (or just enters whitespace).
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
        string PromptWithCallbackValidation(string message, Func<string, bool> validationCallback,
            string validationFailedMessage = "", string defaultResponse = null,
            TextWriter displayStream = null, TextReader inputStream = null);
    }
}
