
namespace Blossom.Operations
{
    public interface ILocalOperationsFactory
    {
        ILocalOperations GetOperations();
    }

    public class LocalOperationsFactory : ILocalOperationsFactory
    {
        public IDeploymentConfig DeploymentConfig { get; set; }

        public IDeploymentContext Context { get; set; }

        public virtual ILocalOperations GetOperations()
        {
            if (DeploymentConfig.DryRun)
            {
                return new DryRunLocalOperations(Context.Logger);
            }
            return new BasicLocalOperations(Context);
        }
    }
}
