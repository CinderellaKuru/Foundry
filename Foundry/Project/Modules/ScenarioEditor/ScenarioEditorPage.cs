using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;
using System.IO;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model;
using SharpDX;
using Color = SharpDX.Color;
using Foundry.Project.Modules.Base;
using MessagePack;
using System.Diagnostics;
using Foundry.Project.Util;

namespace Foundry.Project.Modules.ScenarioEditor
{
	public class ScenarioEditorPage : SceneEditorPage
	{
		public ScenarioEditorPage(FoundryInstance i, int numXChunks = 16): base(i)
		{
			chunks = new TerrainChunk[0, 0];
			SetTerrainSize(numXChunks);
		}
		protected override string GetSaveExtension()
		{
			return FoundryInstance.SaveScenarioExt;
		}
		protected override string GetImportExtension()
		{
			return FoundryInstance.ImportTerrainExt;
		}

		public class TerrainChunk
		{
			private const int numXVerts = 64;

			public MeshNode meshNode;
			private ScenarioEditorPage owner;
			public TerrainChunk(ScenarioEditorPage owner, int chunkX, int chunkZ)
			{
				this.owner = owner;
				MeshBuilder builder = new MeshBuilder();

				//vertices
				for (int x = 0; x < numXVerts; x++)
				{
					for (int z = 0; z < numXVerts; z++)
					{
						builder.Positions.Add(new Vector3(x, 0, z));

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

						builder.TriangleIndices.Add(i0);
						builder.TriangleIndices.Add(i1);
						builder.TriangleIndices.Add(i2);
						builder.TriangleIndices.Add(i3);
						builder.TriangleIndices.Add(i4);
						builder.TriangleIndices.Add(i5);
					}
				}

				meshNode = new MeshNode()
				{
					Geometry = builder.ToMesh(),
					Material = new DiffuseMaterialCore() { DiffuseColor = Color.White },
					ModelMatrix = Matrix.Translation(new Vector3(chunkX * (numXVerts - 1), 0, chunkZ * (numXVerts - 1)))
				};
				owner.viewport.Items.AddChildNode(meshNode);
			}
		}
		private TerrainChunk[,] chunks;
		private int NumXChunks { get { return chunks != null ? chunks.GetLength(0) : 0; } }
		private int NumYChunks { get { return chunks != null ? chunks.GetLength(1) : 0; } }
		public void SetTerrainSize(int numXChunks)
		{
			//terrain is guaranteed to always be square right now. this might change in the future.
			int currentChunksX = chunks.GetLength(0);
			int currentChunksZ = chunks.GetLength(1);
			int desiredChunksX = numXChunks;
			int desiredChunksZ = numXChunks;

			TerrainChunk[,] newChunks = new TerrainChunk[desiredChunksX, desiredChunksZ];
			for (int x = 0; x < desiredChunksX; x++)
			{
				for (int z = 0; z < desiredChunksZ; z++)
				{
					if (x < currentChunksX && z < currentChunksZ)
					{
						newChunks[x, z] = chunks[x, z];
					}
					else
					{
						newChunks[x, z] = new TerrainChunk(this, x, z);
					}
				}
			}
			chunks = newChunks;
		}


		#region serialization
		[MessagePackObject(keyAsPropertyName: true)]
		public class SerializedTerrainChunk
		{
			public List<float> Positions { get; set; }

			[SerializationConstructor]
			internal SerializedTerrainChunk(List<float> positions)
			{
				Positions = positions;
			}
			public SerializedTerrainChunk(TerrainChunk chunk)
			{
				Positions = new List<float>();
				foreach (Vector3 v in chunk.meshNode.Geometry.Positions)
				{
					Positions.Add(v.X);
					Positions.Add(v.Y);
					Positions.Add(v.Z);
				}
			}
		}
		[MessagePackObject(keyAsPropertyName: true)]
		public class SerializedTerrain
		{
			public SerializedTerrainChunk[,] Chunks { get; set; }

			[SerializationConstructor]
			internal SerializedTerrain(SerializedTerrainChunk[,] chunks)
			{
				Chunks = chunks;
			}
			public SerializedTerrain(ScenarioEditorPage page)
			{
				Chunks = new SerializedTerrainChunk[page.NumXChunks, page.NumYChunks];
				for (int x = 0; x < page.chunks.GetLength(0); x++)
				{
					for (int z = 0; z < page.chunks.GetLength(0); z++)
					{
						Chunks[x, z] = new SerializedTerrainChunk(page.chunks[x, z]);
					}
				}
			}
		}

		protected override bool OnLoadFile(string file)
		{
			return true;
		}
		protected override bool OnSaveFile(string file)
		{
			File.WriteAllBytes(file, MessagePackSerializer.Serialize(typeof(SerializedTerrain), new SerializedTerrain(this)));
			return true;
		}
		protected override bool OnImportFile(string file)
		{
			ECF ecf = new ECF();
			ecf.Open(file);
			return true;
		}
		#endregion
	}
}
