using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.Project.Util
{
    class ECF
    {
        public static uint CalcAdler32(byte[] barr, int offs, int len)
        {
            const int mod = 65521;
            uint a = 1, b = 0;
            for (int i = offs; i < len + offs; i++)
            {
                byte c = barr[i];
                a = (a + c) % mod;
                b = (b + a) % mod;
            }
            return (b << 16) | a;
        }

		private Dictionary<ulong, List<byte[]>> chunkDatas = new Dictionary<ulong, List<byte[]>>();
		public void Open(string file)
		{
			using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				//ecf header.
				const int ecfHeaderSize = 32;
				byte[] ecfHeader = new byte[ecfHeaderSize];
				fs.Read(ecfHeader, 0, ecfHeaderSize);
				
				//number of chunks.
				ushort numChunks = BitConverter.ToUInt16(ecfHeader, 16);
				numChunks = BinaryPrimitives.ReverseEndianness(numChunks);
				
				//get chunk data and store by id.
				for(int i = 0; i < numChunks; i ++)
				{

				}
			}
		}
    }
}
