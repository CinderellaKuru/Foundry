using KSoft.IO;
using KSoft.Phoenix.Resource.ECF;
using System.IO;

namespace KSoft.Phoenix.Resource
{
    /*public*/
    internal sealed class EraFileHeader
        : IEndianStreamSerializable
    {
        private const uint kSiganture = 0x17FDBA9C;
        private const int kHeaderSize = 0x1E00;

        public static int CalculateHeaderSize() { return kHeaderSize; }

        private ECF.EcfHeader mHeader;
        private readonly EraFileSignature mSignature = new EraFileSignature();

        public int FileCount => mHeader.ChunkCount;

        public EraFileHeader()
        {
            mHeader.InitializeChunkInfo(kSiganture, EraFileEntryChunk.kExtraDataSize);
            mHeader.HeaderSize = kHeaderSize;
        }

        public void UpdateFileCount(int count)
        {
            mHeader.ChunkCount = (short)count;
        }

        #region IEndianStreamSerializable Members
        public void Serialize(EndianStream s)
        {
            EraFileUtil eraFile = s.Owner as EraFileUtil;

            if (s.IsWriting)
            {
                mHeader.UpdateTotalSize(s.BaseStream);
            }

            long header_position = s.BaseStream.CanSeek
                ? s.BaseStream.Position
                : -1;

            // write the header, but it won't have the correct CRC if things have changed,
            // or if this is a fresh new archive
            mHeader.Serialize(s);
            mSignature.Serialize(s);

            long leftovers_size = mHeader.HeaderSize - s.BaseStream.Position;
            _ = s.Pad((int)leftovers_size);

            // verify or update the header checksum
            if (s.IsReading)
            {
                if (header_position != -1 &&
                    !eraFile.Options.Test(EraFileUtilOptions.SkipVerification))
                {
                    uint actual_adler = mHeader.ComputeAdler32(s.BaseStream, header_position);
                    if (actual_adler != mHeader.Adler32)
                    {
                        throw new InvalidDataException(string.Format(
                            "ERA header adler32 {0} does not match actual adler32 {1}",
                            mHeader.Adler32.ToString("X8"),
                            actual_adler.ToString("X8")
                            ));
                    }
                }
            }
            else if (s.IsWriting)
            {
                if (header_position != -1)
                {
                    mHeader.ComputeAdler32AndWrite(s, header_position);
                }
            }
        }
        #endregion

        public static bool VerifyIsEraAndDecrypted(EndianReader s)
        {
            return EcfHeader.VerifyIsEcf(s);
        }
    };
}
