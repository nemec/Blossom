namespace Blossom
{
    /// <summary>
    /// File transfer handler that adds support for notifying when a file is
    /// older than the destination and thus was not transferred or the
    /// transfer was canceled mid-copy.
    /// </summary>
    public interface IFileTransferHandler
    {
        /// <summary>
        /// Gets the number of uploaded bytes.
        /// </summary>
        ulong BytesTransferred { get; }

        /// <summary>
        /// Triggered every time bytes have been copied.
        /// </summary>
        /// <param name="count"></param>
        void IncrementBytesTransferred(ulong count);

        /// <summary>
        /// Marks the completion of the file transfer.
        /// </summary>
        void TransferCompleted();

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
