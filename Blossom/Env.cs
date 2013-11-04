using Blossom.Environments;
using System;

namespace Blossom
{
    /// <summary>
    /// Control access to both the local and remote environment.
    /// </summary>
    public class Env
    {
        internal IEnvironment Remote { get; set; }

        internal IEnvironment Local { get; set; }

        /// <summary>
        /// Environment's current host
        /// </summary>
        public IHost Host { get; internal set; }

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
            InteractionType = InteractionType.AskForInput;
        }
    }
}