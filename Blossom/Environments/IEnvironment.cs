using PathLib;

namespace Blossom.Environments
{
    /// <summary>
    /// Operations that are specific to a certain environment
    /// or operating system.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Line ending for this environment.
        /// </summary>
        string LineEnding { get; }

        /// <summary>
        /// Create an <see cref="IPurePath"/> for this environment
        /// based on the input path.
        /// </summary>
        /// <param name="initialPath"></param>
        /// <returns></returns>
        IPurePath CreatePurePath(string initialPath);

        /// <summary>
        /// Path to the current shell.
        /// </summary>
        string ShellCommand { get; set; }

        /// <summary>
        /// Arguments to provide to the shell when executing.
        /// </summary>
        string ShellStartArguments { get; }

        /// <summary>
        /// Path to binary that elevates the current process to root.
        /// </summary>
        string SudoPrefix { get; }

        /// <summary>
        /// Identify and expand a shortcut to the user's home directory.
        /// Usually identified by a tilde (~).
        /// </summary>
        /// <param name="path">Path to search and expand.</param>
        /// <param name="username">Expand this user's home directory.</param>
        /// <returns></returns>
        string ExpandUser(string path, string username);

        #region State-affecting Members

        /// <summary>
        /// Identify if the current environment is elevated to root.
        /// </summary>
        bool IsElevated { get; set; }

        /// <summary>
        /// Push a new directory onto the stack and change
        /// to it.
        /// </summary>
        /// <param name="newDirectory">New current directory.</param>
        void Pushd(IPurePath newDirectory);

        /// <summary>
        /// Remove the current directory and change to the previous
        /// directory.
        /// </summary>
        /// <returns>The directory that was just removed.</returns>
        IPurePath Popd();

        /// <summary>
        /// Current directory, as a string.
        /// </summary>
        IPurePath CurrentDirectory { get; }

        /// <summary>
        /// Add an arbitrary prefix to all commands run in this environment.
        /// Each pushed prefix will be added to every command run.
        /// </summary>
        /// <param name="prefix">Prefix to prepend to each command run.</param>
        void PushPrefix(string prefix);

        /// <summary>
        /// Remove and return the last prefix from the stack.
        /// </summary>
        /// <returns>The prefix that was removed.</returns>
        string PopPrefix();

        /// <summary>
        /// The whole prefix string prepended to each command run.
        /// </summary>
        string PrefixString { get; }

        #endregion
    }
}