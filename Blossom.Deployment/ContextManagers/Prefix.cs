namespace Blossom.Deployment.ContextManagers
{
    public class Prefix : ContextManager
    {
        private string Pf { get; set; }

        public Prefix(IDeploymentContext context, string prefix)
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