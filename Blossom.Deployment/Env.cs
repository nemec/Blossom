using Blossom.Deployment.Environments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Blossom.Deployment
{
    public class Env
    {
        public IEnvironment Remote { get; set; }
        public IEnvironment Local { get; set; }
        public List<Host> Hosts { get; set; }

        #region Properties
        /// <summary>
        /// Allows for a guaranteed non-interactive session.
        /// If one of the deployment operations attempts to
        /// interact with the user (eg. through stdin),
        /// the deployment will abort with an error message.
        /// </summary>
        public InteractionType InteractionType { get; set; }

        #endregion

        internal Host CurrentHost { get; set; }
        internal Session CurrentSession { get; set; }
        internal Stack<string> Prefixes { get; set; }

        internal Env()
            : this(Environments.EnvironmentFinder.AutoDetermineLocalEnvironment(
                AppDomain.CurrentDomain.BaseDirectory), new Windows())
        {
        }

        internal Env(IEnvironment local, IEnvironment remote)
        {
            Local = local;
            Remote = remote;
            Hosts = new List<Host>();
            InteractionType = InteractionType.AskForInput;
        }
    }
}
