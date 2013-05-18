using Blossom.Deployment.Environments;
using System;
using System.Collections.Generic;

namespace Blossom.Deployment
{
    /// <summary>
    /// Control access to both the local and remote environment.
    /// </summary>
    public class Env
    {
        internal IEnvironment Remote { get; set; }

        internal IEnvironment Local { get; set; }

        internal Host Host { get; set; }

        internal Dictionary<string, object> Extras { get; set; } 

        #region Environment Varables

        /// <summary>
        /// Allows for a guaranteed non-interactive session.
        /// If one of the deployment operations attempts to
        /// interact with the user (eg. through stdin),
        /// the deployment will abort with an error message.
        /// </summary>
        public InteractionType InteractionType { get; set; }

        #endregion Properties

        /// <summary>
        /// Set up a Windows remote environment and automatically
        /// determine the local environment.
        /// </summary>
        public Env()
            : this(new Windows()) { }

        /// <summary>
        /// Use the given remote environment and automatically
        /// determine the local environment.
        /// </summary>
        /// <param name="remote">Remote environment.</param>
        public Env(IEnvironment remote)
            : this(remote, EnvironmentFinder.AutoDetermineLocalEnvironment(
                AppDomain.CurrentDomain.BaseDirectory)) { }

        /// <summary>
        /// Use a custom environment both remotely and locally.
        /// </summary>
        /// <param name="remote">Remote environment.</param>
        /// <param name="local">Local environment.</param>
        public Env(IEnvironment remote, IEnvironment local)
        {
            Remote = remote;
            Local = local;
            Extras = new Dictionary<string, object>();
            InteractionType = InteractionType.AskForInput;
        }
    }
}