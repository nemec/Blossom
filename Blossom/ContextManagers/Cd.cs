
namespace Blossom.ContextManagers
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
                Context.Environment.Remote.CurrentDirectory.Join(path));
        }

        /// <inheritdoc />
        protected override void Exit()
        {
            Context.Environment.Remote.Popd();
        }
    }
}