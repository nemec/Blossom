using System.IO;

namespace Blossom.Deployment.ContextManagers
{
    public class Lcd : ContextManager
    {
        private readonly string NewPath;

        public Lcd(DeploymentContext context, string path)
            : base(context)
        {
            NewPath = path;
            Begin();
        }

        protected override void Enter()
        {
            if (NewPath != null)
            {
                Context.Environment.Local.Pushd(
                    Path.Combine(Context.Environment.Local.CurrentDirectory, NewPath));
            }
        }

        protected override void Exit()
        {
            if (NewPath != null)
            {
                Context.Environment.Local.Popd();
            }
        }
    }
}