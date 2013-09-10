using System.IO;

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
        public Lcd(IDeploymentContext context, string path)
            : base(context)
        {
            Context.Environment.Local.Pushd(
            Path.Combine(Context.Environment.Local.CurrentDirectory, path));
            
        }

        protected override void Exit()
        {
            Context.Environment.Local.Popd();
        }
    }
}