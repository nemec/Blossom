using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blossom.Environments
{
    /// <summary>
    /// Implement common environment operations.
    /// </summary>
    public abstract class BaseEnvironment : IEnvironment
    {
        internal Stack<string> Directories { get; set; }

        internal Stack<string> Prefixes { get; set; }

        /// <inheritdoc />
        public abstract string LineEnding { get; protected set; }

        /// <inheritdoc />
        public abstract string ShellStartArguments { get; protected set; }

        /// <inheritdoc />
        public abstract string SudoPrefix { get; protected set; }

        /// <inheritdoc />
        public abstract PathSeparator PathSeparator { get; protected set; }

        /// <inheritdoc />
        public abstract string ExpandUser(string path, string username);

        /// <summary>
        /// Create some common environment data.
        /// </summary>
        /// <param name="shellCommand"></param>
        /// <param name="initialDirectory"></param>
        protected BaseEnvironment(string shellCommand = null, string initialDirectory = null)
        {
            Directories = new Stack<string>();
            Prefixes = new Stack<string>();
            Pushd(initialDirectory ?? "");
            ShellCommand = shellCommand;
        }

        /// <inheritdoc />
        public string ShellCommand { get; set; }

        /// <inheritdoc />
        public bool IsElevated { get; set; }

        /// <inheritdoc />
        public void Pushd(string newDirectory)
        {
            Directories.Push(newDirectory);
        }

        /// <inheritdoc />
        public string Popd()
        {
            var ret = CurrentDirectory;
            Directories.Pop();
            return ret;
        }

        /// <inheritdoc />
        public string CurrentDirectory { get { return Directories.Peek(); } }

        /// <inheritdoc />
        public string CombinePath(params string[] paths)
        {
            return Utils.NormalizePathSeparators(
                Path.Combine(paths), PathSeparator);
        }

        /// <inheritdoc />
        public void PushPrefix(string prefix)
        {
            Prefixes.Push(prefix);
        }

        /// <inheritdoc />
        public string PopPrefix()
        {
            var ret = Prefixes.Peek();
            Prefixes.Pop();
            return ret;
        }

        /// <inheritdoc />
        public string PrefixString
        {
            get { return String.Join(" && ", Prefixes.Reverse()); }
        }

        /// <inheritdoc />
        public abstract PathLib.IPurePath CreatePath(string initialPath);
    }
}