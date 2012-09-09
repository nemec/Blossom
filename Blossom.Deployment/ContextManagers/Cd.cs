using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.ContextManagers
{
    public class Cd : ContextManager
    {
        readonly string NewPath;

        public Cd(DeploymentContext context, string path)
            : base(context)
        {
            NewPath = path;
            Begin();
        }

        protected override void Enter()
        {
            Context.Environment.Remote.Pushd(
                Path.Combine(Context.Environment.Remote.CurrentDirectory, NewPath));
        }

        protected override void Exit()
        {
            Context.Environment.Remote.Popd();
        }
    }
}
