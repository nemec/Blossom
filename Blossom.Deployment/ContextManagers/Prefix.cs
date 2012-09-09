using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blossom.Deployment.ContextManagers
{
    public class Prefix : ContextManager
    {
        private string Pf { get; set; }

        public Prefix(DeploymentContext context, string prefix)
            : base(context)
        {
            Pf = prefix;
            base.Begin();
        }

        protected override void Enter()
        {
            Context.Environment.Remote.PushPrefix(Pf);
        }

        protected override void Exit()
        {
            Context.Environment.Remote.PopPrefix();
        }
    }
}
