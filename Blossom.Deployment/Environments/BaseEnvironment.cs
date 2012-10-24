using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blossom.Deployment.Environments
{
    public abstract class BaseEnvironment : IEnvironment
    {
        internal Stack<string> Directories { get; set; }

        internal Stack<string> Prefixes { get; set; }

        public abstract string LineEnding { get; protected set; }

        public abstract string ShellCommand { get; protected set; }

        public abstract string ShellStartArguments { get; protected set; }

        public abstract string SudoPrefix { get; protected set; }

        public abstract PathSeparator PathSeparator { get; protected set; }

        public abstract string ExpandUser(string path, string username);

        protected BaseEnvironment()
            : this(null)
        {
        }

        protected BaseEnvironment(string initialDirectory)
        {
            Directories = new Stack<string>();
            Prefixes = new Stack<string>();
            Pushd(initialDirectory ?? "");
        }

        public bool IsElevated { get; set; }

        public void Pushd(string newDirectory)
        {
            Directories.Push(newDirectory);
        }

        public string Popd()
        {
            var ret = CurrentDirectory;
            Directories.Pop();
            return ret;
        }

        public string CurrentDirectory { get { return Directories.Peek(); } }

        public string CombinePath(params string[] paths)
        {
            return Utils.NormalizePathSeparators(
                Path.Combine(paths), PathSeparator);
        }

        public void PushPrefix(string prefix)
        {
            Prefixes.Push(prefix);
        }

        public string PopPrefix()
        {
            var ret = Prefixes.Peek();
            Prefixes.Pop();
            return ret;
        }


        public string PrefixString
        {
            get { return String.Join(" && ", Prefixes.Reverse()); }
        }
    }
}