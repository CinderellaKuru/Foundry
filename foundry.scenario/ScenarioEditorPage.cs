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
using System.Diagnostics;
using System.Buffers.Binary;
using Newtonsoft.Json.Linq;
using SharpDX.Direct2D1;
using Foundry.Util;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using HelixToolkit.SharpDX.Core.Core;
using Foundry.Modules.UGX;
using System.Windows.Forms;
using System.Xml;
using System.Drawing.Drawing2D;
using Matrix = SharpDX.Matrix;
using Foundry;

namespace hwfoundry.scenario
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
		public ScenarioEditorPage()
		{
			ToolStrip toolStrip = new ToolStrip();
			Controls.Add(toolStrip);

			toolStrip.Items.Add(null, Foundry.Properties.Resources.icon_skirt, new EventHandler((object o, EventArgs e) =>
			{
				terrain.ToggleSkirt();
				viewport.InvalidateRender();
				OnDraw();
			}));

			SetGeometry("cube", UGXImporter.ImportUGXGeometry("S:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\mods\\Sarc0.5.7.5\\art\\banished\\vehicle\\scarab_01\\mesh_scarab01.ugx"));

			temp = new Object(this, "cube");
		}
		protected override string GetSaveExtension()
		{
			return FoundryInstance.ExtSerializeScenario;
		}
		protected override string GetImportExtension()
		{
			return FoundryInstance.ExtImportScenario;
		}


		[Flags]
		enum HitMask
		{
			TERRAIN = 1,
			OBJECT = 2,
		}
		List<HitTestResult> HitTest(HitMask mask)
		{
			List<HitTestResult> ret = new List<HitTestResult>();
			foreach (var h in viewport.FindHits(new Vector2(GetMouseState().X, GetMouseState().Y)))
			{
				if(h.ModelHit is MeshNode)
				{
					if (((MeshNode)h.ModelHit).Tag is HitMask)
					{
						if ((mask & (HitMask)((MeshNode)h.ModelHit).Tag) > 0)
						{
							ret.Add(h);
						}
					}
				}
			}
			return ret;
		}

		public class Object
		{
			string geometryName;
			Guid instance;
			private ScenarioEditorPage owner;
			public Object(ScenarioEditorPage owner, string geometryName)
			{
				this.owner = owner;
				this.geometryName = geometryName;

				instance = owner.AddInstance(geometryName, Matrix.Translation(new Vector3(0,0,0)));
			}

			public void SetPosition(Vector3 pos)
			{
				owner.SetInstanceMatrix(geometryName, instance, Matrix.Translation(pos));
			}
			public void SetMatrix(Matrix m)
			{
				owner.SetInstanceMatrix(geometryName, instance, m);
			}
		}
		Object temp;
		List<Object> objects = new List<Object>();

		public class Terrain
		{
			private InstancingMeshNode meshNode;
			private ScenarioEditorPage owner;
			private int sizeX, sizeY;
			public Terrain(ScenarioEditorPage owner, int sizeX, int sizeY)
			{
				this.owner = owner;
				this.sizeX = sizeX;
				this.sizeY = sizeY;
				MeshBuilder builder = new MeshBuilder();

				//vertices
				for (int z = 0; z < sizeY; z++)
				{
					for (int x = 0; x < sizeX; x++)
					{
						builder.Positions.Add(new Vector3(x, 0, z));
					}
				}
				//indices
				for (int z = 0; z < sizeY - 1; z++)
				{
					for (int x = 0; x < sizeX - 1; x++)
					{
						int row0 = z * (sizeY);
						int row1 = (z + 1) * (sizeY);

						int i0 = row0 + x;
						int i1 = row0 + x + 1;
						int i2 = row1 + x;
						int i3 = row1 + x + 1;
						int i4 = row1 + x;
						int i5 = row0 + x + 1;

						builder.TriangleIndices.Add(i2);
						builder.TriangleIndices.Add(i1);
						builder.TriangleIndices.Add(i0);
						builder.TriangleIndices.Add(i5);
						builder.TriangleIndices.Add(i4);
						builder.TriangleIndices.Add(i3);
					}
				}

				meshNode = new InstancingMeshNode()
				{
					Geometry = builder.ToMeshGeometry3D(),
					Material = new PhongMaterialCore() { DiffuseColor = Color.White },
					ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
					Tag = HitMask.TERRAIN,
				};

				meshNode.Instances = new List<Matrix>() { meshNode.ModelMatrix };
				meshNode.InstanceParamArray = new List<InstanceParameter>() { new InstanceParameter() { DiffuseColor = Color4.White } };

				owner.viewport.Items.AddChildNode(meshNode);
			}


			public void SetVertex(int x, int z, Vector3 v)
			{
				meshNode.Geometry.Positions[(z * (sizeY)) + x] = new Vector3(x + v.X, v.Y, z + v.Z);
			}
			public Vector3 GetVertex(int x, int z)
			{
				return meshNode.Geometry.Positions[(z * (sizeY)) + x] - new Vector3(x, 0, z);
			}
			public void UpdateNormals()
			{
				((MeshGeometry3D)meshNode.Geometry).Normals = MeshGeometryHelper.CalculateNormals((MeshGeometry3D)meshNode.Geometry);
			}
			public void UpdateOctree()
			{
				meshNode.Geometry.UpdateOctree(true);
			}
			public void UpdateBounds()
			{
				meshNode.Geometry.UpdateBounds();
			}

			private bool skirt = false;
			public void ToggleSkirt()
			{
				skirt = !skirt;

				if(!skirt)
				{
					meshNode.Instances = new List<Matrix>() { meshNode.ModelMatrix };
					meshNode.InstanceParamArray = new List<InstanceParameter>() { new InstanceParameter() { DiffuseColor = Color4.White } };
				}
				else
				{
					List<Matrix> matrices = new List<Matrix>() { meshNode.ModelMatrix };
					List<InstanceParameter> parameters = new List<InstanceParameter>() { new InstanceParameter() { DiffuseColor = Color4.White } };

					//Right
					matrices.Add(
						Matrix.Scaling(-1, 1, 1) *
						Matrix.Translation(0, 0, 0));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });
					//Left
					matrices.Add(
						Matrix.Scaling(-1, 1, 1) *
						Matrix.Translation(sizeX * 2, 0, 0));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });
					//Bottom
					matrices.Add(
						Matrix.Scaling(1, 1, -1) *
						Matrix.Translation(0, 0, 0));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });
					//Top
					matrices.Add(
						Matrix.Scaling(1, 1, -1) *
						Matrix.Translation(0, 0, sizeY * 2));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });

					//Bottom Right
					matrices.Add(
						Matrix.Scaling(-1, 1, -1) *
						Matrix.Translation(0, 0, 0));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });
					//Bottom Left
					matrices.Add(
						Matrix.Scaling(-1, 1, -1) *
						Matrix.Translation(sizeX * 2, 0, 0));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });
					//Top Left
					matrices.Add(
						Matrix.Scaling(-1, 1, -1) *
						Matrix.Translation(0, 0, sizeY * 2));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });
					//Top Right
					matrices.Add(
						Matrix.Scaling(-1, 1, -1) *
						Matrix.Translation(sizeX * 2, 0, sizeY * 2));
					parameters.Add(new InstanceParameter() { DiffuseColor = Color.CornflowerBlue.ToColor4() });

					meshNode.Instances = matrices;
					meshNode.InstanceParamArray = parameters;
				}
			}
		}
		private Terrain terrain;

		protected override void OnTick()
		{
			base.OnTick();

			var hits = HitTest(HitMask.TERRAIN);
			if (hits.Count > 0)
			{
				temp.SetPosition(hits[0].PointHit);
			}
			viewport.InvalidateRender();
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
			return false;
		}
		protected override bool OnImportFile(string file)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(file);
			XmlNode scenarioNode = doc.SelectSingleNode("Scenario");

			XmlDocument doc2 = new XmlDocument();
			doc2.Load(Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".sc2");
			XmlNode scenarioNode2 = doc2.SelectSingleNode("Scenario");

			#region Objects
			XmlNode[] objectsNodes =
				{
					scenarioNode.SelectSingleNode("Objects"),
					scenarioNode2.SelectSingleNode("Objects")
				};


			XmlDocument objectsXmlDoc = new XmlDocument();
			objectsXmlDoc.Load("S:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\data\\objects.xml");
			XmlNode objectsXmlDocNode = objectsXmlDoc.SelectSingleNode("Objects");

			foreach (XmlNode objectsNode in objectsNodes)
			{
				foreach (XmlNode child in objectsNode.ChildNodes)
				{
					string objectName = child.LastChild.Value;

					foreach (XmlNode obj in objectsXmlDocNode.ChildNodes)
					{
						if (obj.Attributes.GetNamedItem("name").Value == objectName)
						{
							XmlNode visNode = obj.SelectSingleNode("Visual");
							if (visNode != null)
							{
								string visPath = "";
								try
								{
									visPath = visNode.LastChild.Value;
									string fullVisPath = "S:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\art\\" + visPath;

									XmlDocument visDoc = new XmlDocument();
									visDoc.Load(fullVisPath);

									string ugx = "S:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\art\\" +
										visDoc.SelectSingleNode("visual")
										.SelectSingleNode("model")
										.SelectSingleNode("component")
										.SelectSingleNode("asset")
										.SelectSingleNode("file").LastChild.Value + ".ugx";

									SetGeometry(visPath, UGXImporter.ImportUGXGeometry(ugx));
									Object o = new Object(this, visPath);

									string posStr = child.Attributes.GetNamedItem("Position").Value;
									string[] posStrElems = posStr.Split(',');
									float x = float.Parse(posStrElems[0]);
									float y = float.Parse(posStrElems[1]);
									float z = float.Parse(posStrElems[2]);
									Vector3 pos = new Vector3(x, y, z);

									string fwdStr = child.Attributes.GetNamedItem("Forward").Value;
									string[] fwdStrElems = fwdStr.Split(',');
									float fx = float.Parse(fwdStrElems[0]);
									float fy = float.Parse(fwdStrElems[1]);
									float fz = float.Parse(fwdStrElems[2]);
									Vector3 fwd = new Vector3(fx, fy, fz);

									string rgtStr = child.Attributes.GetNamedItem("Right").Value;
									string[] rgtStrElems = rgtStr.Split(',');
									float rx = float.Parse(rgtStrElems[0]);
									float ry = float.Parse(rgtStrElems[1]);
									float rz = float.Parse(rgtStrElems[2]);
									Vector3 rgt = new Vector3(rx, ry, rz);

									Matrix m1 = Matrix.Identity;
									m1.Forward = fwd;
									m1.Right = rgt;
									m1.Up = new Vector3(0, 1, 0);
									//mr.ScaleVector = new Vector3(1, 1, 1);

									Matrix m2 = Matrix.Translation(pos);


									Matrix m = m1 * m2;
									o.SetMatrix(m);
								}
								catch { Console.WriteLine(visPath); }
							}
						}
					}
				}
			}
			#endregion

			#region XTD
			string xtdPath = 
				Path.GetDirectoryName(file) + "\\" +
				scenarioNode.SelectSingleNode("Terrain").LastChild.Value +
				".xtd";

			var ecfChunks = ECF.ReadChunks(xtdPath);

			byte[] xtdHeader = ecfChunks[XTDHeaderId][0];
			int thisNumXVerts = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(xtdHeader, 4));
			int thisNumXChunks = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(xtdHeader, 8));

			//TerrainSetTotalSize(thisNumXChunks);
			terrain = new Terrain(this, thisNumXVerts, thisNumXVerts);

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
			const uint kBitMask10 = (1 << 10) - 1;
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
				//row and col order is intentional based on objects.
				terrain.SetVertex(row, col, new Vector3(fx, fy, fz));
			}

			terrain.ToggleSkirt();
			terrain.UpdateBounds();
			terrain.UpdateNormals();
			terrain.UpdateOctree();

			return true;
			#endregion
		}
	}
}
