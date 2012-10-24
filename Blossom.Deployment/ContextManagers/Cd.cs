using System.IO;

namespace Blossom.Deployment.ContextManagers
{
    public class Cd : ContextManager
    {
        private readonly string _newPath;

        public Cd(IDeploymentContext context, string path)
            : base(context)
        {
            _newPath = path;
            Begin();
        }

        protected override void Enter()
        {
            Context.Environment.Remote.Pushd(
                Path.Combine(Context.Environment.Remote.CurrentDirectory, _newPath));
        }

        protected override void Exit()
        {
            Context.Environment.Remote.Popd();
        }
    }
}