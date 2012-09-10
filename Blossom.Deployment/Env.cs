using Blossom.Deployment.Environments;
using System;
using System.Collections.Generic;

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

        #endregion Properties

        internal Host CurrentHost { get; set; }

        internal Stack<string> Prefixes { get; set; }

        internal Env()
            : this(new Windows()) { }

        internal Env(IEnvironment remote)
            : this(remote, Environments.EnvironmentFinder.AutoDetermineLocalEnvironment(
                AppDomain.CurrentDomain.BaseDirectory)) { }

        internal Env(IEnvironment remote, IEnvironment local)
        {
            Remote = remote;
            Local = local;
            Hosts = new List<Host>();
            InteractionType = InteractionType.AskForInput;
        }
    }
}