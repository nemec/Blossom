using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment.Ssh
{
    public interface ISession
    {
        bool Connected { get; }

        UserInfo UserInfo { get; set; }

        void Connect();

        void Disconnect();

        void SetConfig(Dictionary<string, string> config);

        T GetChannel<T>() where T : Channel;
    }
}
