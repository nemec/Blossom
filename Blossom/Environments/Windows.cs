﻿using System;
using System.IO;

namespace Blossom.Environments
{
    /// <summary>
    /// Environment that uses backslashes as path separators and
    /// carriage returns in line endings.
    /// </summary>
    public class Windows : BaseEnvironment
    {
        private const string DefaultShellCommand = "cmd.exe";

        /// <summary>
        /// Create a new Windows environment with an optional initial directory and shell.
        /// </summary>
        /// <param name="initialDirectory">Initial directory path.</param>
        /// <param name="shellCommand">Shell command to use (or null for default).</param>
        public Windows(string initialDirectory = null, string shellCommand = null)
            : base(shellCommand ?? DefaultShellCommand, initialDirectory)
        {
        }

        public override string LineEnding
        {
            get { return @"\r\n"; }
            protected set { }
        }

        public override string ShellStartArguments
        {
            get { return "/Q /C"; }
            protected set { }
        }

        public override string SudoPrefix
        {
            get { return "runas /user:Administrator"; }
            protected set { }
        } 

        public override PathSeparator PathSeparator
        {
            get { return PathSeparator.Backslash; }
            protected set { }
        }

        public override string ExpandUser(string path, string username)
        {
            if (path.StartsWith("~"))
            {
                return Path.Combine(
                    Directory.GetParent(Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile)).FullName,
                    username,
                    path.Substring(1));
            }
            return path;
        }
    }
}