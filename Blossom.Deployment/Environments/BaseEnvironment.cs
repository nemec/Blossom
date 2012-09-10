using System.Collections.Generic;
using System.IO;

namespace Blossom.Deployment.Environments
{
    public abstract class BaseEnvironment : IEnvironment
    {
        internal Stack<string> Directories { get; set; }

        internal Stack<string> Prefixes { get; set; }

        public abstract string LineEnding { get; }

        public abstract PathSeparator PathSeparator { get; }

        public abstract string ExpandUser(string path, string username);

        public BaseEnvironment()
            : this(null)
        {
        }

        public BaseEnvironment(string initialDirectory)
        {
            Directories = new Stack<string>();
            Prefixes = new Stack<string>();
            Pushd(initialDirectory ?? "");
        }

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
    }
}