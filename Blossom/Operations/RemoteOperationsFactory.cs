
namespace Blossom.Operations
{
    public interface IRemoteOperationsFactory
    {
        IRemoteOperations GetOperations();
    }

    public class RemoteOperationsFactory : IRemoteOperationsFactory
    {
        public IDeploymentConfig DeploymentConfig { get; set; }

        public IDeploymentContext Context { get; set; }

        public virtual IRemoteOperations GetOperations()
        {
            if (DeploymentConfig.DryRun)
            {
                return new DryRunRemoteOperations(Context.Logger);
            }
            // If host is loopback, short circuit the network
            if (Context.Host.Hostname == Host.LoopbackHostname)
            {
                return new LoopbackRemoteOperations(Context);
            }
            return new BasicRemoteOperations(Context, Context.Host);
        }
    }
}
