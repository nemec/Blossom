using CommandLine;
using CommandLine.Text;

namespace Blossom.Scripting
{
    internal enum EnvironmentType
    {
        Linux,
        Windows
    }

    internal class ScriptOptions : CommandLineOptionsBase
    {
        [Option(null, "config", Required = true, 
            HelpText="Deployment task file (C# code).")]
        public string ScriptFile { get; set; }

        [Option("e", "env", 
            DefaultValue = EnvironmentType.Linux, 
            HelpText="Environment (Linux|Windows)")]
        public EnvironmentType RemoteEnvironment { get; set; }

        [OptionArray("h", "hosts", HelpText = "List of hostnames to deploy to.")]
        public string[] Hostnames { get; set; }

        [Option("u", "username", HelpText = "SSH username for hosts.")]
        public string Username { get; set; }

        [Option("p", "password", HelpText = "SSH password for hosts.")]
        public string Password { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, curr => HelpText.DefaultParsingErrorsHandler(this, curr));
        }
    }
}
