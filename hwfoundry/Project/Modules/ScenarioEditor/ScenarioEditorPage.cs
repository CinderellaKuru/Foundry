using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using Vector3 = SharpDX.Vector3;
using static Foundry.FoundryInstance;
using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using System.Runtime.Remoting.Contexts;
using Color = SharpDX.Color;
using System.Diagnostics;

namespace Foundry.Project.Modules.ScenarioEditor
{
    public class ScenarioEditorPage : EditorPage
    {
        private Panel renderControl;
        private ViewportCore viewport;
        private CameraCore camera;
        private CameraController cameraController;
        private EffectsManager effectsManager;

        public ScenarioEditorPage(FoundryInstance i) : base(i)
        {
            renderControl = new Panel();
            renderControl.Width = 600;
            renderControl.Height = 400;
            renderControl.Location = new System.Drawing.Point(0, 0);
            renderControl.Dock = DockStyle.Fill;
            Controls.Add(renderControl);


            //helix
            viewport = new ViewportCore(renderControl.Handle);
            cameraController = new CameraController(viewport);
            cameraController.CameraMode = CameraMode.Inspect;
            cameraController.CameraRotationMode = CameraRotationMode.Turntable;
            camera = new PerspectiveCameraCore()
            {
                LookDirection = new Vector3(0, 0, 1),
                Position = new Vector3(0, 0, -10),
                FarPlaneDistance = 1000f,
                NearPlaneDistance = .1f,
                FieldOfView = 90,
                UpDirection = new Vector3(0, 1, 0)
            };
            viewport.CameraCore = camera;
            cameraController.CameraTarget = new Vector3(0, 0, 0);

            effectsManager = new DefaultEffectsManager();
            effectsManager.AddTechnique(new HelixToolkit.SharpDX.Core.Shaders.TechniqueDescription("technique"));
            viewport.EffectsManager = effectsManager;

            MaterialCore mat = new DiffuseMaterialCore()
            {
                DiffuseColor = Color.White
            };
            var builder = new MeshBuilder(true, false, false);
            builder.AddBox(new Vector3(0, 0, 0), 1, 1, 1);
            MeshNode node = new MeshNode()
            {
                ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
                Geometry = builder.ToMesh(),
                IsTransparent = false,
                Material = mat,
                CullMode = CullMode.Back,
            };
            viewport.Items.AddChildNode(node);

            viewport.StartD3D(Width, Height);
        }
        protected override bool OnLoadFile(string file)
        {
            return true;
        }
        protected override void OnTick()
        {
            MouseState mouseState = GetMouseState();
            Vector2 mousePos = new Vector2(mouseState.x, mouseState.y);

            cameraController.OnTimeStep();
            cameraController.MouseMove(mousePos);
            viewport.MouseMove(mousePos);

            if (mouseState.middleDown && !GetKeyIsDown(Keys.ShiftKey))
            {
                cameraController.StartRotate(mousePos);
            }
            else
            {
                cameraController.EndRotate(mousePos);
            }

            if (mouseState.middleDown && GetKeyIsDown(Keys.ShiftKey))
            {
                cameraController.StartPan(mousePos);
            }
            else
            {
                cameraController.EndPan(mousePos);
            }
        }
        protected override void OnDraw()
        {
            viewport.Render();
        }
        protected override void OnResize()
        {
            viewport.Resize(Width, Height);
        }
        protected override void OnClose()
        {
            viewport.EndD3D();
        }
    }
}
