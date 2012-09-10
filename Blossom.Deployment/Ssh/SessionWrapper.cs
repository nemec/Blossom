using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment.Ssh
{
    public class SessionWrapper : ISession
    {
        private UserInfo _userInfo;
        public UserInfo UserInfo
        {
            get
            { 
                return _userInfo;
            }
            set
            {
                _userInfo = value;
                Session.setUserInfo(value);
            }
        }

        private Session Session { get; set; }

        public bool Connected { get { return Session.isConnected(); } }

        public SessionWrapper(Host host)
        {
            var jsch = new JSch(); // Constructor just creates a static config on first run
            Session = jsch.getSession(
                    host.Username ?? System.Environment.UserName,
                    host.Hostname,
                    host.Port != 0 ? host.Port : 22);
        }

        public void Connect()
        {
            Session.connect();
        }

        public void Disconnect()
        {
            Session.disconnect();
        }

        public T GetChannel<T>() where T : Tamir.SharpSsh.jsch.Channel
        {
            // Not pretty, but the code's gotta come from somewhere.
            // Better to abstract it here so that when we get a sane
            // ssh implementation we don't have to change as much.
            if (typeof(T) == typeof(ChannelSftp))
            {
                return (T)Session.openChannel("sftp");
            }
            else if (typeof(T) == typeof(ChannelShell))
            {
                return (T)Session.openChannel("shell");
            }
            return null;
        }


        public void SetConfig(Dictionary<string, string> config)
        {
            Session.setConfig(new System.Collections.Hashtable(config));
        }
    }
}
