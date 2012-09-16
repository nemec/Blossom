namespace Blossom.Deployment.Environments
{
    public interface IEnvironment
    {
        string LineEnding { get; }

        PathSeparator PathSeparator { get; }

        string ShellCommand { get; }

        string ShellStartArguments { get; }

        string SudoPrefix { get; }

        string CombinePath(params string[] paths);

        string ExpandUser(string path, string username);

        #region State-affecting Members

        bool IsElevated { get; set; }

        void Pushd(string newDirectory);

        string Popd();

        string CurrentDirectory { get; }

        void PushPrefix(string prefix);

        string PopPrefix();

        string PrefixString { get; }

        #endregion
    }
}