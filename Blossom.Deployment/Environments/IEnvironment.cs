using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Environments
{
    public interface IEnvironment
    {
        string LineEnding { get; }
        PathSeparator PathSeparator { get; }

        string CombinePath(params string[] paths);
        string ExpandUser(string path, string username);

        void Pushd(string newDirectory);
        string Popd();
        string CurrentDirectory { get; }

        void PushPrefix(string prefix);
        string PopPrefix();
    }
}
