namespace Blossom.Deployment.ContextManagers
{
    /// <summary>
    /// Context manager to prefix all commands with
    /// the given prefix command. Commands will be
    /// run as: 'prefix && command'.
    /// </summary>
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