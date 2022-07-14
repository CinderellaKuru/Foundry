using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.ECF
{
    class ECFChunk
    {
        public long id;
        public int padding = 0;
        public List<byte> data = new List<byte>();
        public ECFChunk(long name)
        {
            id = name;
        }
    }
    class ECFFile
    {
        private List<ECFChunk> chunks = new List<ECFChunk>();
        public void AddChunk(ECFChunk chunk)
        {
            chunks.Add(chunk);
        }
        public void Save(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            
            #region Header
            List<byte> header = new List<byte>()
            {
                0xDA, 0xBA, 0x77, 0x37, //magic
                0x00, 0x00, 0x00, 0x20, //headerSize
                0x00, 0x00, 0x00, 0x00, //adler32
                0x00, 0x00, 0x00, 0x00, //fileSize placeholder
                0x00, 0x00,             //numChunks placeholder
                0x00, 0x00,             //flags
                0x00, 0x07, 0x78, 0x26, //id
                0x00, 0x00,             //chunkExtraDataSize
                0x00, 0x00,             //PAD0
                0x00, 0x00, 0x00, 0x00, //PAD1
            };

            int length = 32;
            foreach(ECFChunk c in chunks)
            {
                length += 24;
                length += c.data.Count;
            }
            byte[] size = BitConverter.GetBytes(length);
            header[12] = size[3];
            header[13] = size[2];
            header[14] = size[1];
            header[15] = size[0];
            byte[] chunkCount = BitConverter.GetBytes((short)chunks.Count);
            header[16] = chunkCount[1];
            header[17] = chunkCount[0];

            uint adler = Util.CalcAdler32(header.ToArray(), 12, 20);
            header[8] = BitConverter.GetBytes(adler)[3];
            header[8] = BitConverter.GetBytes(adler)[2];
            header[8] = BitConverter.GetBytes(adler)[1];
            header[8] = BitConverter.GetBytes(adler)[0];

            fs.Write(header.ToArray(), 12, 20);
            
            int chunkDataOffset = 32 + (chunks.Count * 24);
            foreach (var c in chunks)
            {
                List<byte> chunkHeader = new List<byte>();
                chunkHeader.AddRange(BitConverter.GetBytes(c.id).Reverse());
                chunkHeader.AddRange(BitConverter.GetBytes(chunkDataOffset).Reverse());
                chunkHeader.AddRange(BitConverter.GetBytes(c.data.Count).Reverse());
                chunkHeader.AddRange(BitConverter.GetBytes(0)); //adler
                chunkHeader.Add(0x00); //flags
                chunkHeader.Add(0x02); //allignment
                chunkHeader.Add(0x00); //resource flags x2
                chunkHeader.Add(0x00);
                fs.Write(chunkHeader.ToArray(), 0, 24);
                chunkDataOffset += c.data.Count + c.padding;
            }
            #endregion

            foreach(ECFChunk c in chunks)
            {
                fs.Write(c.data.ToArray(), 0, c.data.Count);
                for (int i = 0; i < c.padding; i++) { fs.Write(new byte[1] { 0x00 }, 0, 1); }
            }

            fs.Close();
        }
    }
}
