﻿using System;
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

namespace Foundry.Project.Modules.Base
{
    public abstract class SceneEditorPage : BaseEditorPage
    {
        protected Panel renderControl;
        protected ViewportCore viewport;
        protected CameraCore camera;
        protected CameraController cameraController;
		protected AmbientLightNode ambientLight;
		protected LightNode directionalLight;
		protected EffectsManager effectsManager;

        public SceneEditorPage(FoundryInstance i) : base(i)
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
			cameraController.SpinReleaseTime = 0;
            camera = new PerspectiveCameraCore()
            {
                LookDirection = new Vector3(0, 0, 1),
                Position = new Vector3(0, 50, -10),
                FarPlaneDistance = 7500f,
                NearPlaneDistance = .1f,
                FieldOfView = 90,
                UpDirection = new Vector3(0, 1, 0),
            };
            viewport.CameraCore = camera;

			directionalLight = new PointLightNode()
			{
				Color = Color.White.ToColor4(),
				ModelMatrix = Matrix.Translation(new Vector3(0, 0, 0)),
			};
			viewport.Items.AddChildNode(directionalLight);

			ambientLight = new AmbientLightNode()
			{
				Color = Color.White.ToColor4().ChangeIntensity(.2f),
				ModelMatrix = Matrix.Translation(new Vector3(512, 100, 512)),
			};
			viewport.Items.AddChildNode(ambientLight);

			effectsManager = new DefaultEffectsManager();
			viewport.EffectsManager = effectsManager;

			viewport.StartD3D(Width, Height);
        }

        private bool allowPan = true;
        public void SetAllowPan(bool pan)
        {
            allowPan = pan;
        }
		protected override void OnTick()
		{
			MouseState mouseState = GetMouseState();
			Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

			if (mouseState.middleDown)
			{
				if (GetKeyIsDown(Keys.ShiftKey))
				{
					if (cameraController.IsRotating)
					{
						cameraController.EndRotate(mousePos);
					}
					if (!cameraController.IsPanning)
					{
						if (allowPan)
						{
							cameraController.StartPan(mousePos);
						}
					}
				}
				if (!GetKeyIsDown(Keys.ShiftKey))
				{
					if (cameraController.IsPanning)
					{
						cameraController.EndPan(mousePos);
					}
					if (!cameraController.IsRotating)
					{
						cameraController.StartRotate(mousePos);
					}
				}
			}
			else
			{
				if (cameraController.IsPanning)
				{
					cameraController.EndPan(mousePos);
				}
				if (cameraController.IsRotating)
				{
					cameraController.EndRotate(mousePos);
				}
			}

			cameraController.OnTimeStep();
			cameraController.MouseMove(mousePos);
			viewport.MouseMove(mousePos);
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
