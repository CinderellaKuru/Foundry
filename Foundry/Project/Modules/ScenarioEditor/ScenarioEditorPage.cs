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
using System.Buffers.Binary;
using Newtonsoft.Json.Linq;
using SharpDX.Direct2D1;
using ScintillaNET;
using Foundry.Project.Util.ECF;

namespace Foundry.Project.Modules.ScenarioEditor
{
	public class ScenarioTool
	{
		private ScenarioEditorPage page;
		public ScenarioTool(ScenarioEditorPage p)
		{
			page = p;
		}

		public class ToolParams
		{
			float x, y;
		}
		public virtual void Use(ToolParams p)
		{
			
		}
	}

	public class ScenarioEditorPage : SceneEditorPage
	{
		//page
		private const int numXVerts = 64;
		public ScenarioEditorPage(FoundryInstance i): base(i)
		{
		}
		protected override string GetSaveExtension()
		{
			return FoundryInstance.ExtSerializeScenario;
		}
		protected override string GetImportExtension()
		{
			return FoundryInstance.ExtImportTerrain;
		}

		//chunk
		public class TerrainChunk
		{
			public MeshNode meshNode;
			private ScenarioEditorPage owner;
			public TerrainChunk(ScenarioEditorPage owner, int chunkX, int chunkZ)
			{
				this.owner = owner;
				MeshBuilder builder = new MeshBuilder();

				//vertices
				for (int z = 0; z < numXVerts; z++)
				{
					for (int x = 0; x < numXVerts; x++)
					{
						builder.Positions.Add(new Vector3(x + (chunkX * numXVerts), 0, z + (chunkZ * numXVerts)));

						float uvu = (1f / (numXVerts - 1) * x);
						float uvv = 1 - (1f / (numXVerts - 1) * z);
						//builder.TextureCoordinates.Add(new Vector2(uvu, uvv));
					}
				}
				//indices
				for (int z = 0; z < numXVerts - 1; z++)
				{
					for (int x = 0; x < numXVerts - 1; x++)
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
					Geometry = builder.ToMeshGeometry3D(),
					Material = new PhongMaterialCore() { DiffuseColor = Color.White },
					ModelMatrix = Matrix.Translation(new Vector3(chunkX * (numXVerts - 1), 0, chunkZ * (numXVerts - 1)))
				};

				owner.viewport.Items.AddChildNode(meshNode);
			}

			public void SetVertex(int x, int z, Vector3 v)
			{
				meshNode.Geometry.Positions[(z * numXVerts) + x] = new Vector3(x + v.X, v.Y, z + v.Z);
			}
			public Vector3 GetVertex(int x, int z)
			{
				return meshNode.Geometry.Positions[(z * numXVerts) + x] - new Vector3(x, 0 , z);
			}
			public void UpdateNormals()
			{
				((MeshGeometry3D)meshNode.Geometry).Normals = MeshGeometryHelper.CalculateNormals((MeshGeometry3D)meshNode.Geometry);
			}
		}
		private TerrainChunk[,] chunks;
		private int NumXChunks { get { return chunks != null ? chunks.GetLength(0) : 0; } }
		private int NumZChunks { get { return chunks != null ? chunks.GetLength(1) : 0; } }
		public void TerrainSetTotalSize(int numXChunks)
		{
			//terrain is guaranteed to always be square right now. this might change in the future.
			TerrainChunk[,] newChunks = new TerrainChunk[numXChunks, numXChunks];
			for (int x = 0; x < numXChunks; x++)
			{
				for (int z = 0; z < numXChunks; z++)
				{
					if (x < NumXChunks && z < NumZChunks)
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
		/// <summary>
		/// Sets a vertex's value at selected world-space coords.
		/// </summary>
		public void TerrainSetVertex(int x, int z, Vector3 value)
		{
			int chunkX = x / numXVerts;
			int chunkZ = z / numXVerts;
			int vertexX = x % numXVerts;
			int vertexZ = z % numXVerts;

			if (chunkX >= NumXChunks) return;
			if (chunkZ >= NumZChunks) return;
			if (vertexX >= numXVerts) return;
			if (vertexZ >= numXVerts) return;

			TerrainChunk chunk = chunks[chunkX, chunkZ];
			chunk.SetVertex(vertexX, vertexZ, value);

			//if (vertexX == 0 && chunkX > 0)
			//{
			//	TerrainChunk neighbor = chunks[chunkX - 1, chunkZ];
			//	neighbor.SetVertex(numXVerts - 1, vertexZ, value);
			//}
			//if (vertexZ == 0 && chunkZ > 0)
			//{
			//	TerrainChunk neighbor = chunks[chunkX, chunkZ - 1];
			//	neighbor.SetVertex(vertexX, numXVerts - 1 , value);
			//}
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
				Chunks = new SerializedTerrainChunk[page.NumXChunks, page.NumZChunks];
				for (int x = 0; x < page.chunks.GetLength(0); x++)
				{
					for (int z = 0; z < page.chunks.GetLength(0); z++)
					{
						Chunks[x, z] = new SerializedTerrainChunk(page.chunks[x, z]);
					}
				}
			}
		}

		private const long XTDHeaderId	  = 0x1111;
		private const long TerrainChunkId = 0x2222;
		private const long AtlasChunkId   = 0x8888;
		private const long AOChunkID	  = 0xCCCC;
		private const long AlphaChunkID	  = 0xDDDD;
		private const long TessChunkID    = 0xAAAA;
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
			var ecfChunks = ECF.ReadChunks(file);

			byte[] xtdHeader = ecfChunks[XTDHeaderId][0];
			int thisNumXVerts = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(xtdHeader, 4));
			int thisNumXChunks = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(xtdHeader, 8));
			
			TerrainSetTotalSize(thisNumXChunks);
			
			byte[] atlas = ecfChunks[AtlasChunkId][0];
			Vector3 posCompMin = new Vector3(
				BitConverter.ToSingle(atlas.Skip(0).Take(4).Reverse().ToArray(), 0),
				BitConverter.ToSingle(atlas.Skip(4).Take(4).Reverse().ToArray(), 0),
				BitConverter.ToSingle(atlas.Skip(8).Take(4).Reverse().ToArray(), 0));
			Vector3 posCompRange = new Vector3(
				BitConverter.ToSingle(atlas.Skip(16).Take(4).Reverse().ToArray(), 0),
				BitConverter.ToSingle(atlas.Skip(20).Take(4).Reverse().ToArray(), 0),
				BitConverter.ToSingle(atlas.Skip(24).Take(4).Reverse().ToArray(), 0));

			const int positionsOffset = 32;
			const uint  kBitMask10 = (1 << 10) - 1;
			const float kBitMask10Rcp = 1.0f / kBitMask10;
			for (int i = 0; i < thisNumXVerts * thisNumXVerts; i++)
			{
				uint v = BitConverter.ToUInt32(atlas, (i * 4) + positionsOffset);
				
				uint x = (v >> 20) & kBitMask10;
				uint y = (v >> 10) & kBitMask10;
				uint z = (v >> 00) & kBitMask10;
				float fx = (x * kBitMask10Rcp * posCompRange.X) - posCompMin.X;
				float fy = (y * kBitMask10Rcp * posCompRange.Y) - posCompMin.Y;
				float fz = (z * kBitMask10Rcp * posCompRange.Z) - posCompMin.Z;

				int row = i / (thisNumXVerts);
				int col = i % (thisNumXVerts);
				TerrainSetVertex(col, row, new Vector3(fx, fy, fz));
			}

			foreach(TerrainChunk c in chunks)
			{
				c.UpdateNormals();
				c.meshNode.ForceUpdateTransformsAndBounds();
			}

			directionalLight.ModelMatrix = Matrix.Translation(new Vector3(thisNumXVerts / 2, -100, thisNumXVerts / 2));

			return true;
		}
		#endregion
	}
}
