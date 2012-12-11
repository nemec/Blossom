namespace Blossom.Deployment
{
    /// <summary>
    /// File transfer handler that adds support for notifying when a file is
    /// older than the destination and thus was not transferred or the
    /// transfer was canceled mid-copy.
    /// </summary>
    public interface IFileTransferHandler : Renci.SshNet.Sftp.IFileTransferHandler
    {
        /// <summary>
        /// Name of the file being transferred.
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// Notify the user that the file cannot be transferred because it does not exist.
        /// </summary>
        void FileDoesNotExist();

        /// <summary>
        /// Notify the user that the file being copied is up to data and thus
        /// was not transferred.
        /// </summary>
        void FileUpToDate();

        /// <summary>
        /// Trigger when the file transfer is canceled for any reason.
        /// </summary>
        void TransferCanceled();
    }
}
