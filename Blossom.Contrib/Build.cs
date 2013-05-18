using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using ILogger = Blossom.Deployment.Logging.ILogger;
using BetterDotNet;

namespace Blossom.Contrib
{
    public class Build
    {
        private LoggerWrapper Logger { get; set; }

        public Build(ILogger logger)
        {
            Logger = new LoggerWrapper(logger);
        }

        public bool BuildProject(string projectFile, string outputDir)
        {
            return BuildProject(projectFile, outputDir, 
                new Dictionary<string, string>());
        }

        public bool BuildProject(string projectFile, string outputDir, 
            Dictionary<string, string> extraParams)
        {
            extraParams.Add("OutputPath", outputDir);

            var coll = new ProjectCollection();
            var parameters = new BuildParameters(coll)
                {
                    Loggers = new[] {Logger}
                };
            var request = new BuildRequestData(projectFile, extraParams,
                "4.0", new[] {"Build"}, null);

            var result = BuildManager.DefaultBuildManager.Build(parameters, request);
            return result.OverallResult == BuildResultCode.Success;
        }

        private class LoggerWrapper : Microsoft.Build.Framework.ILogger
        {
            private ILogger Logger { get; set; }

            private Dictionary<string, string> BuildProperties { get; set; } 

            public LoggerWrapper(ILogger logger)
            {
                Logger = logger;
            }

            public void Initialize(IEventSource eventSource)
            {
                Logger.Info("Build started");
                eventSource.ProjectStarted += LogProjectStarted;

                eventSource.ProjectFinished += LogProjectFinished;

                eventSource.WarningRaised += LogProjectWarning;

                eventSource.ErrorRaised += LogProjectError;
            }

            private static Dictionary<string, string> FormProperties(IEnumerable props)
            {
                return props.Cast<DictionaryEntry>().ToDictionary(
                    k => k.Key as string, v => v.Value as string);
            } 

            private void LogProjectStarted(object sender, ProjectStartedEventArgs args)
            {
                BuildProperties = FormProperties(args.Properties);

                Logger.Info(String.Format("Project: {0}, Configuration: {1} {2}",
                    BuildProperties.Get("MSBuildProjectName",
                        Path.GetFileNameWithoutExtension(args.ProjectFile)),
                    BuildProperties.Get("Configuration", "Unknown"),
                    BuildProperties.Get("Platform", String.Empty)));
            }

            private void LogProjectFinished(object sender, ProjectFinishedEventArgs args)
            {
                Logger.Info(String.Format("{0} -> {1}", args.SenderName, args.ProjectFile));
            }

            private void LogProjectWarning(object sender, BuildWarningEventArgs args)
            {
                Logger.Warn(String.Format("[{0}:{1},{2}] {3}",
                    args.File, args.LineNumber, args.ColumnNumber,
                    args.Message));
            }

            private void LogProjectError(object sender, BuildErrorEventArgs args)
            {
                Logger.Error(String.Format("[{0}:{1},{2}] {3}",
                    args.File, args.LineNumber, args.ColumnNumber,
                    args.Message));
            }

            public string Parameters { get; set; }

            public LoggerVerbosity Verbosity { get; set; }

            public void Shutdown()
            {
                Logger.Info("Build complete.");
            }
        }
    }
}
