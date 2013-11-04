
using System;
using PathLib;

namespace Blossom.ContextManagers
{
    /// <summary>
    /// Context manager for changing directories on a local machine.
    /// </summary>
    public class Lcd : ContextManager
    {
        /// <summary>
        /// Change to the given directory on the local machine.
        /// </summary>
        /// <param name="context">Deployment context.</param>
        /// <param name="path">New current directory.</param>
        [Obsolete]
        public Lcd(IDeploymentContext context, string path)
            : base(context)
        {
            Context.LocalEnv.Pushd(
                Context.LocalEnv.CurrentDirectory.Join(path));
            
        }
        /// <summary>
        /// Change to the given directory on the local machine.
        /// </summary>
        /// <param name="context">Deployment context.</param>
        /// <param name="path">New current directory.</param>
        public Lcd(IDeploymentContext context, IPurePath path)
            : base(context)
        {
            Context.LocalEnv.Pushd(
                Context.LocalEnv.CurrentDirectory.Join(path));
            
        }

        /// <inheritdoc />
        protected override void Exit()
        {
            Context.LocalEnv.Popd();
        }
    }
}