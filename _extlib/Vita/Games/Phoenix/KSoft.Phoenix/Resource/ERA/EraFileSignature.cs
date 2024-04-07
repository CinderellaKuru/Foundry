using KSoft.IO;
using System;
using System.IO;

#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif
using SHA1CryptoServiceProvider = System.Security.Cryptography.SHA1CryptoServiceProvider;
using PhxHash = KSoft.Security.Cryptography.PhxHash;

namespace KSoft.Phoenix.Resource
{
    /*public*/
    internal sealed class EraFileSignature
        : IEndianStreamSerializable
    {
        private const uint kSignature = 0x05ABDBD8;
        private const uint kSignatureMarker = 0xAAC94350;
        private const byte kDefaultSizeBit = 0x13;
        private const int kNonSignatureBytesSize = sizeof(uint) + sizeof(byte) + sizeof(uint);
        private const uint kSha1Salt = 0xA7F95F9C;

        public byte SizeBit = kDefaultSizeBit;
        public byte[] SignatureData;

        #region IEndianStreamSerializable Members
        public void Serialize(EndianStream s)
        {
            bool reading = s.IsReading;

            int sig_data_length = reading || SignatureData == null
                ? 0
                : SignatureData.Length;
            int size = reading
                ? 0
                : kNonSignatureBytesSize + sig_data_length;

            _ = s.StreamSignature(kSignature);
            _ = s.Stream(ref size);
            if (size < kNonSignatureBytesSize)
            {
                throw new InvalidDataException(size.ToString("X8"));
            }

            _ = s.Pad64();

            _ = s.StreamSignature(kSignatureMarker);
            _ = s.Stream(ref SizeBit);
            if (reading)
            {
                Array.Resize(ref SignatureData, size - kNonSignatureBytesSize);
                sig_data_length = SignatureData.Length;
            }
            if (sig_data_length > 0)
            {
                _ = s.Stream(SignatureData);
            }
            _ = s.StreamSignature(kSignatureMarker);
        }
        #endregion

        internal static byte[] ComputeSignatureDigest(Stream chunksStream
            , long chunksOffset
            , long chunksLength
            , ECF.EcfHeader header)
        {
            Contract.Requires(chunksStream != null);
            Contract.Requires(chunksStream.CanSeek && chunksStream.CanRead);
            Contract.Requires(chunksOffset >= 0);
            Contract.Requires(chunksLength > 0);

            using (SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider())
            {
                PhxHash.UInt32(sha, kSha1Salt);
                PhxHash.UInt32(sha, (uint)header.HeaderSize);
                PhxHash.UInt32(sha, (uint)header.ChunkCount);
                PhxHash.UInt32(sha, header.ExtraDataSize);
                PhxHash.UInt32(sha, (uint)header.TotalSize);

                PhxHash.Stream(sha,
                    chunksStream, chunksOffset, chunksLength,
                    isFinal: true);

                return sha.Hash;
            }
        }
    };
}
