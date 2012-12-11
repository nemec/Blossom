using System;

namespace Blossom.Deployment.ContextManagers
{
    /// <summary>
    /// Context Manager to elevate all commands to the Superuser.
    /// </summary>
    public class Sudo : ContextManager
    {
        private string _administratorPassword;

        private readonly bool _previousValue;

        /// <summary>
        /// Elevate all commands to the Superuser.
        /// </summary>
        /// <param name="context">Deployment context.</param>
        /// <param name="administratorPassword">Superuser (Admin) password.</param>
        public Sudo(IDeploymentContext context, string administratorPassword)
            : base(context)
        {
            throw new NotImplementedException(); // TODO Figure out how to give password to sudo
            _administratorPassword = administratorPassword;
            _previousValue = Context.Environment.Remote.IsElevated;
            Context.Environment.Remote.IsElevated = true;
        }

        protected override void Exit()
        {
            Context.Environment.Remote.IsElevated = _previousValue;
        }
    }
}
