namespace Blossom.Deployment
{
    public interface IDeployment<T>
    {
        IDeploymentContext Context { get; set; }
        T Config { get; set; }
    }

    public interface IDeployment : IDeployment<NullConfig>{ }
}
