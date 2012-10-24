using System.IO;

namespace Blossom.Deployment.ContextManagers
{
    public class Lcd : ContextManager
    {
        private readonly string _newPath;

        public Lcd(IDeploymentContext context, string path)
            : base(context)
        {
            _newPath = path;
            Begin();
        }

        protected override void Enter()
        {
            if (_newPath != null)
            {
                Context.Environment.Local.Pushd(
                    Path.Combine(Context.Environment.Local.CurrentDirectory, _newPath));
            }
        }

        protected override void Exit()
        {
            if (_newPath != null)
            {
                Context.Environment.Local.Popd();
            }
        }
    }
}