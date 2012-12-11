using System.IO;

namespace Blossom.Deployment.ContextManagers
{
    /// <summary>
    /// Context manager for changing directories on a remote machine.
    /// </summary>
    public class Cd : ContextManager
    {
        /// <summary>
        /// Change to the given directory on the remote machine.
        /// </summary>
        /// <param name="context">Deployment context.</param>
        /// <param name="path">New current directory.</param>
        public Cd(IDeploymentContext context, string path)
            : base(context)
        {
            Context.Environment.Remote.Pushd(
                Path.Combine(Context.Environment.Remote.CurrentDirectory, path));
        }

        protected override void Exit()
        {
            Context.Environment.Remote.Popd();
        }
    }
}