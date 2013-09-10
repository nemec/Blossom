namespace Blossom.ContextManagers
{
    /// <summary>
    /// Context manager to prefix all commands with
    /// the given prefix command. Commands will be
    /// run as: 'prefix &amp;&amp; command'.
    /// </summary>
    public class Prefix : ContextManager
    {
        /// <summary>
        /// Add a new prefix to all commands.
        /// </summary>
        /// <param name="context">Deployment context.</param>
        /// <param name="prefix">Prefix content.</param>
        public Prefix(IDeploymentContext context, string prefix)
            : base(context)
        {
            Context.Environment.Remote.PushPrefix(prefix);
        }

        protected override void Exit()
        {
            Context.Environment.Remote.PopPrefix();
        }
    }
}