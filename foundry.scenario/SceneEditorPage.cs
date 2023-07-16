using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using Vector3 = SharpDX.Vector3;
using SharpDX;
using Color = SharpDX.Color;
using Foundry.Util;
using HelixToolkit.SharpDX.Core.Core;
using Foundry;
using Foundry.Util;

namespace hwfoundry.scenario
{
    public abstract class SceneEditorPage : BaseEditorPage
    {
        public SceneEditorPage()
        {
			renderControl = new Panel
			{
				Width = 600,
				Height = 400,
				Location = new System.Drawing.Point(0, 0),
				Dock = DockStyle.Fill
			};
			Controls.Add(renderControl);

			//helix
			viewport = new ViewportCore(renderControl.Handle);
			viewport.EnableRenderFrustum = false; //TODO: culling is broken.

			camera = new PerspectiveCameraCore()
			{
				LookDirection = new Vector3(0, 0, 1),
				Position = new Vector3(0, 0, 0),
                FarPlaneDistance = 7500f,
                NearPlaneDistance = .1f,
                FieldOfView = 90,
                UpDirection = new Vector3(0, 1, 0),
            };
            viewport.CameraCore = camera;

			directionalLight = new SpotLightNode()
			{
				Color = Color.White.ToColor4().ChangeIntensity(.35f),
				ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
				Direction = new Vector3(0, 0, 1),
				Position = new Vector3(0, 600, 0),
				OuterAngle = 4.0f,
				Range = 2500.0f
			};
			viewport.Items.AddChildNode(directionalLight);

			ambientLight = new AmbientLightNode()
			{
				Color = Color.White.ToColor4().ChangeIntensity(.25f),
				ModelMatrix = Matrix.Translation(new Vector3(512, 100, 512)),
			};
			viewport.Items.AddChildNode(ambientLight);

			effectsManager = new DefaultEffectsManager();
			viewport.EffectsManager = effectsManager;

			viewport.StartD3D(Width, Height);

			viewport.ShowRenderDetail = true;
        }

        protected Panel renderControl;
        protected ViewportCore viewport;
        protected PerspectiveCameraCore camera;
		protected AmbientLightNode ambientLight;
		protected SpotLightNode directionalLight;
		protected EffectsManager effectsManager;


		//Camera
        private bool allowPan = true;
        public void SetAllowPan(bool pan)
        {
            allowPan = pan;
        }
		Vector2 cameraDir = new Vector2();
		Vector3 cameraTarget = new Vector3();
		float cameraZoom = -150.0f;
		private void UpdateCamera(float panX, float panY, float zoom, float rotX, float rotY)
		{
			cameraDir += new Vector2(rotX, rotY);
			cameraDir.X = cameraDir.X.Clamp(-1.56f, 1.56f); //clamp to almost exactly up/down (radians).

			cameraZoom += zoom;
			cameraZoom = cameraZoom.Clamp(-2000.0f, -30.0f); //clamp to these arbitrary values.


			var pitch = Quaternion.RotationAxis(Vector3.UnitX, cameraDir.X);
			var yaw = Quaternion.RotationAxis(Vector3.UnitY, cameraDir.Y);
			var rotation = yaw * pitch;

			var zoomOffset = Vector3.Transform(new Vector3(0, 0, cameraZoom), rotation);
			var rotationVec = Vector3.Transform(Vector3.UnitZ, rotation);


			cameraTarget += (Vector3.Transform(new Vector3(panX, panY, 0), rotation) * (-cameraZoom / 200.0f));

			camera.LookDirection = rotationVec;
			camera.Position = cameraTarget + zoomOffset;

			viewport.InvalidateRender();
		}


		//Geometry
		class FoundryInstancingMeshNode : InstancingMeshNode
		{
			public void UpdateInstances()
			{
				Instances = Instances.ToList();
				InstanceParamArray = InstanceParamArray.ToList();
			}
		}
		private Dictionary<string, FoundryInstancingMeshNode> instancedGeometry = new Dictionary<string, FoundryInstancingMeshNode>();
		public void SetGeometry(string name, Geometry3D geometry)
		{
			if (!instancedGeometry.ContainsKey(name))
			{
				instancedGeometry.Add(name,
				new FoundryInstancingMeshNode()
				{
					ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
					Material = new PhongMaterialCore() { DiffuseColor = Color.White.ToColor4() },
					InstanceIdentifiers = new List<Guid>(),
					InstanceParamArray = new List<InstanceParameter>(), // { new InstanceParameter() { DiffuseColor = Color.Red.ToColor4()} },
					Instances = new List<Matrix>(), // { Matrix.Translation(new Vector3(0,0,0)) },
				});

				viewport.Items.AddChildNode(instancedGeometry[name]);
			}

			instancedGeometry[name].Geometry = geometry;
		}
		public Guid AddInstance(string name, Matrix matrix)
		{
			if(instancedGeometry.ContainsKey(name))
			{
				Guid guid = Guid.NewGuid();
				var a = instancedGeometry[name];
				instancedGeometry[name].Instances.Add(matrix);
				instancedGeometry[name].InstanceParamArray.Add(new InstanceParameter() { DiffuseColor = Color.Red.ToColor4() });
				instancedGeometry[name].InstanceIdentifiers.Add(guid);
				instancedGeometry[name].UpdateInstances();
				return guid;
			}
			return Guid.Empty;
		}
		public void RemoveInstance(string name, Guid guid)
		{
			if (instancedGeometry.ContainsKey(name))
			{
				int index = instancedGeometry[name].InstanceIdentifiers.IndexOf(guid);
				if (index >= 0)
				{
					instancedGeometry[name].Instances.RemoveAt(index);
					instancedGeometry[name].InstanceParamArray.RemoveAt(index);
					instancedGeometry[name].InstanceIdentifiers.RemoveAt(index);
					instancedGeometry[name].UpdateInstances();
				}
			}
		}
		public void SetInstanceMatrix(string name, Guid guid, Matrix matrix)
		{
			if(instancedGeometry.ContainsKey(name))
			{
				int index = instancedGeometry[name].InstanceIdentifiers.IndexOf(guid);
				if(index >= 0)
				{
					instancedGeometry[name].Instances[index] = matrix;
					instancedGeometry[name].Instances.RemoveAt(index);
					instancedGeometry[name].Instances.Insert(index, matrix);
					instancedGeometry[name].UpdateInstances();
				}
			}
		}


		protected override void OnTick()
		{
			MouseState mouseState = GetMouseState();

			if (mouseState.middleDown)
			{
				if (GetKeyIsDown(Keys.ShiftKey))
				{
					//Pan
					UpdateCamera(-mouseState.deltaX, -mouseState.deltaY, 0, 0, 0);
				}
				else
				{
					//Rotate
					UpdateCamera(0, 0, 0, -mouseState.deltaY / 100.0f, mouseState.deltaX / 100.0f);
				}
			}
			else
			{
				//Zoom
				UpdateCamera(0, 0, mouseState.deltaScroll / 10.0f, 0, 0);
			}


		}
		protected override void OnDraw()
        {
			viewport.Render();
        }
        protected override void OnResize()
        {
            viewport.Resize(Width, Height);
			viewport.Render();
        }
        protected override void OnClose()
        {
            viewport.EndD3D();
        }
    }
}
