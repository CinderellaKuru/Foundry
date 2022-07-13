using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using OpenTK;
using SMHEditor.DockingModules.MapEditor;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace SMHEditor.Project.FileTypes
{
    public class TerrainFileChunk
    {
        public int X { get; set; }
        public int Y { get; set; }
        public JVector[] positionData = new JVector[64 * 64];
    }
    public class TerrainFile
    {
        public static int maxVStride = 64;
        public TerrainFile()
        {

        }

        public enum TerrainSize
        {
            Small512 = 8,
            Small768 = 12,
            Medium1024 = 16,
            Medium1536 = 24,
            Large2048 = 32
        }
        public static TerrainFile Create(TerrainSize size)
        {
            TerrainFile trn = new TerrainFile();
            trn.Size = size;
            trn.chunks = new List<TerrainFileChunk>();

            for (int y = 0; y < (int)size; y++)
            {
                for (int x = 0; x < (int)size; x++)
                {
                    TerrainFileChunk tfc = new TerrainFileChunk();
                    tfc.X = x;
                    tfc.Y = y;

                    for (int i = 0; i < maxVStride; i++)
                    {
                        for (int j = 0; j < maxVStride; j++)
                        {
                            tfc.positionData[i * maxVStride + j] = new JVector(i, 0, j);
                        }
                    }

                    trn.chunks.Add(tfc);
                }
            }
            return trn;
        }

        public TerrainSize Size { get; set; }
        public List<TerrainFileChunk> chunks { get; set; }
    }
}
