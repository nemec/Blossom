namespace Blossom.Deployment
{
    public interface IDeployment<in T>
    {
        void InitializeDeployment(IDeploymentContext context, T config);
    }

    public interface IDeployment : IDeployment<NullConfig>{ }
}
