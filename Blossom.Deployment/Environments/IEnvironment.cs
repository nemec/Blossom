namespace Blossom.Deployment.Environments
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
        /// Path separator used in this environment.
        /// </summary>
        PathSeparator PathSeparator { get; }

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
        /// Combine multiple paths into a single path using
        /// this environment's <see cref="PathSeparator"/>.
        /// 
        /// Final path is not guaranteed to exist.
        /// </summary>
        /// <param name="paths">Paths to combine.</param>
        /// <returns>A single path containing the joined paths, in order.</returns>
        string CombinePath(params string[] paths);

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
        void Pushd(string newDirectory);

        /// <summary>
        /// Remove the current directory and change to the previous
        /// directory.
        /// </summary>
        /// <returns>The directory that was just removed.</returns>
        string Popd();

        /// <summary>
        /// Current directory, as a string.
        /// </summary>
        string CurrentDirectory { get; }

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