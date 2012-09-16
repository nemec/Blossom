using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.SshNet.Sftp
{
    public interface IFileTransferHandler
    {
        /// <summary>
        /// Gets the number of uploaded bytes.
        /// </summary>
        ulong BytesTransferred { get; }

        void IncrementBytesTransferred(ulong count);

        void TransferCompleted();
    }
}
