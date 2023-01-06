using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundry.Project.Modules.Base;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using SharpDX;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Reflection;

namespace Foundry.Project.Modules.ScenarioEditor
{
    public enum TerrainSize
    {
        x512  = 8,
        x768  = 12,
        x1024 = 16,
        x1536 = 24,
        x2048 = 32,
        x3072 = 48,
        x4096 = 64,
    }
    public class ScenarioEditorPage : SceneEditorPage
    {
        public ScenarioEditorPage(FoundryInstance i) : base(i)
        {
            InitTerrainMesh(TerrainSize.x4096);
        }

		private class TerrainChunk
		{
			public MeshNode meshNode;

			private ScenarioEditorPage owner;
			public TerrainChunk(ScenarioEditorPage owner, int worldX, int worldY, IEnumerable<Vector3> positions, IEnumerable<int> indices)
			{
				this.owner = owner;

				MeshBuilder builder = new MeshBuilder();
				builder.Positions.AddRange(positions);
				builder.TriangleIndices.AddRange(indices);

				meshNode = new MeshNode()
				{
					Geometry = builder.ToMesh(),
					Material = new DiffuseMaterialCore() { DiffuseColor = Color.White },
					ModelMatrix = Matrix.Translation(new Vector3(worldX, 0, worldY))
				};
				owner.viewport.Items.AddChildNode(meshNode);
			}
		}
        private TerrainChunk[,] terrainChunks;
        public void InitTerrainMesh(TerrainSize size)
        {
            const int numXVerts = 64;
            terrainChunks = new TerrainChunk[(int)size, (int)size];

            for (int chunkX = 0; chunkX < (int)size; chunkX++)
            {
                for (int chunkZ = 0; chunkZ < (int)size; chunkZ++)
                {
					List<Vector3> positions = new List<Vector3>();
					List<int> indices = new List<int>();

					//vertices
					for (int x = 0; x < numXVerts; x++)
                    {
                        for (int z = 0; z < numXVerts; z++)
                        {
							positions.Add(new Vector3(x, 0, z));

                            float uvu = (1f / (numXVerts - 1) * x);
                            float uvv = 1 - (1f / (numXVerts - 1) * z);
                            //builder.TextureCoordinates.Add(new Vector2(uvu, uvv));
                        }
                    }

                    //indices
                    for (int x = 0; x < numXVerts - 1; x++)
                    {
                        for (int z = 0; z < numXVerts - 1; z++)
                        {
                            int row0 = z * numXVerts;
                            int row1 = (z + 1) * numXVerts;

                            int i0 = row0 + x;
                            int i1 = row0 + x + 1;
                            int i2 = row1 + x;
                            int i3 = row1 + x + 1;
                            int i4 = row1 + x;
                            int i5 = row0 + x + 1;

                            indices.Add(i0);
                            indices.Add(i1);
                            indices.Add(i2);
                            indices.Add(i3);
                            indices.Add(i4);
                            indices.Add(i5);
                        }
                    }

                    int chunkWorldX = chunkX * (numXVerts - 1);
                    int chunkWorldY = chunkZ * (numXVerts - 1);
					terrainChunks[chunkX, chunkZ] = new TerrainChunk(this, chunkWorldX, chunkWorldY, positions, indices);
                }
            }
        }
    }
}
