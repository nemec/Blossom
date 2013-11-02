using System;
using System.Security;

namespace Blossom.ContextManagers
{
    /// <summary>
    /// Context Manager to elevate all commands to the Superuser.
    /// </summary>
    public class Sudo : ContextManager
    {
        private readonly SecureString _administratorPassword;

        private readonly bool _previousValue;

        /// <summary>
        /// Elevate all commands to the Superuser.
        /// </summary>
        /// <param name="context">Deployment context.</param>
        /// <param name="administratorPassword">Superuser (Admin) password.</param>
        public Sudo(IDeploymentContext context, SecureString administratorPassword)
            : base(context)
        {
            throw new NotImplementedException(); // TODO Figure out how to give password to sudo
            administratorPassword.MakeReadOnly();
            _administratorPassword = administratorPassword;
            _previousValue = Context.Environment.Remote.IsElevated;
            Context.Environment.Remote.IsElevated = true;
        }

        /// <inheritdoc />
        protected override void Exit()
        {
            Context.Environment.Remote.IsElevated = _previousValue;
            _administratorPassword.Dispose();
        }
    }
}
