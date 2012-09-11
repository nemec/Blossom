
namespace Blossom.Deployment.Ssh
{
    public interface ISftp
    {
        bool IsConnected { get; }

        void Connect();

        void Disconnect();

        void Mkdir(string path);

        bool Exists(string path);

        void Put(string source, string destination);
    }
}