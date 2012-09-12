using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.Ssh
{
    public interface IShell
    {
        Stream Stream { get; }

        string RunCommand(string command);
    }
}
