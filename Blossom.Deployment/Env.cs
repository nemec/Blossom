using Blossom.Deployment.Environments;
using System;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    public class Env
    {
        internal IEnvironment Remote { get; set; }

        internal IEnvironment Local { get; set; }

        internal List<Host> Hosts { get; set; }

        internal Host CurrentHost { get; set; }

        #region Environment Varables

        /// <summary>
        /// Allows for a guaranteed non-interactive session.
        /// If one of the deployment operations attempts to
        /// interact with the user (eg. through stdin),
        /// the deployment will abort with an error message.
        /// </summary>
        public InteractionType InteractionType { get; set; }

        #endregion Properties


        public Env()
            : this(new Windows()) { }

        public Env(IEnvironment remote)
            : this(remote, EnvironmentFinder.AutoDetermineLocalEnvironment(
                AppDomain.CurrentDomain.BaseDirectory)) { }

        public Env(IEnvironment remote, IEnvironment local)
        {
            Remote = remote;
            Local = local;
            Hosts = new List<Host>();
            InteractionType = InteractionType.AskForInput;
        }
    }
}