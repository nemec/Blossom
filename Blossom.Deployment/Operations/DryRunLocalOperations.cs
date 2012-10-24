using Blossom.Deployment.Logging;
using System;

namespace Blossom.Deployment.Operations
{
    internal class DryRunLocalOperations : ILocalOperations
    {
        private ILogger Logger { get; set; }

        internal DryRunLocalOperations(ILogger logger)
        {
            Logger = logger;
        }

        private void LogDryRun(string commandName, string output)
        {
            Logger.Info(
                String.Format("Dry run executing {0}: {1}", commandName, output));
        }

        public string RunLocal(string command)
        {
            LogDryRun("RunLocal", String.Format(
                "Running command [{0}] locally with no timeout.", command));
            return "";
        }

        public string RunLocal(string command, int timeout)
        {
            LogDryRun("RunLocal", String.Format(
                "Running command [{0}] locally with timeout {1}.", command, timeout));
            return "";
        }

        public void CompressFiles(string destination, params string[] sourcePaths)
        {
            LogDryRun("CompressFiles", String.Format(
                "Compressing [{0}] into {1}", String.Join(", ", sourcePaths), destination));
        }

        public void CompressFiles(string destination, bool overwriteDestination, params string[] sourcePaths)
        {
            LogDryRun("CompressFiles", String.Format(
                "Compressing [{0}] into {1}, overwriting destination if exists.",
                String.Join(", ", sourcePaths), destination));
        }

        public void CompressFiles(System.IO.Stream destination, params string[] sourcePaths)
        {
            LogDryRun("CompressFiles", String.Format(
                "Compressing [{0}] into a stream.", String.Join(", ", sourcePaths)));
        }

        public void CompressFiles(System.IO.Stream outputStream, string[] sources, CompressionScheme scheme, int compressionLevel, int pathDepth)
        {
            LogDryRun("CompressFiles", String.Format(
                "Compressing [{0}] into a stream using the compression scheme {1}",
                    String.Join(", ", sources), scheme.ToString()));
        }

        public string PromptWithNoValidation(string message, string defaultResponse = null, System.IO.TextWriter displayStream = null, System.IO.TextReader inputStream = null)
        {
            LogDryRun("PromptWithNoValidation", String.Format(
                @"Creating a prompt with message ""{0}"", default response ""{1}"", {2} output stream, and {3} input stream.",
                message, defaultResponse,
                inputStream == null ? "no" : "an", displayStream == null ? "no" : "an"));
            return "";
        }

        public string PromptWithRegexValidation(string message, string validateRegex, string validationFailedMessage = "", string defaultResponse = null, System.IO.TextWriter displayStream = null, System.IO.TextReader inputStream = null)
        {
            LogDryRun("PromptWithRegexValidation", String.Format(
                @"Creating a prompt with message ""{0}"", default response ""{1}"", " +
                @"{2} output stream, and {3} input stream. Response would validate against " +
                @"the regular expression ""{4}"".",
                message, defaultResponse,
                inputStream == null ? "no" : "an", displayStream == null ? "no" : "an",
                validateRegex));
            return "";
        }

        public string PromptWithCallbackValidation(string message, Func<string, bool> validationCallback, string validationFailedMessage = "", string defaultResponse = null, System.IO.TextWriter displayStream = null, System.IO.TextReader inputStream = null)
        {
            LogDryRun("PromptWithRegexValidation", String.Format(
                @"Creating a prompt with message ""{0}"", default response ""{1}"", " +
                @"{2} output stream, and {3} input stream. Response would validate against " +
                @"a callback.",
                message, defaultResponse,
                inputStream == null ? "no" : "an", displayStream == null ? "no" : "an"));
            return "";
        }
    }
}
