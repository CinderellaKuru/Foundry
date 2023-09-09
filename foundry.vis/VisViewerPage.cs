using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static foundry.unit.UnitModule;
using SharpDX;
using Color = SharpDX.Color;

namespace foundry.vis
{
    public class VisViewerPage : BaseSceneEditorPage
    {
        public VisModule Module { get; private set; }
        public VisViewerPage()
        {
            MaxZoomIn = -5.0f;
            UpdateCameraSetZoom(-75);

            OnPageInit += VisOnInit;
        }

        private void VisOnInit(object o, InitArgs args)
        {
            VisModule module;
            if (Instance.GetModuleByType(out module))
            {
                Module = module;
            }
            else
            {
                throw new Exception("Bad dep. Need a system for this...");
            }


            Module.UnitModuleInstance.SelectedUnitChanged += (sender, e) =>
            {
                Unit unit = Module.UnitModuleInstance.SelectedUnit;
                if (unit != null)
                {
                    if (Module.UnitVisuals.ContainsKey(unit))
                    {
                        if (Module.VisualGeometries.ContainsKey(Module.UnitVisuals[unit]))
                        {
                            SetGeometry("View", Module.VisualGeometries[Module.UnitVisuals[unit]]);
                            Redraw();
                            return;
                        }
                    }
                }
                SetGeometry("View", new MeshGeometry3D());
                Redraw();
            };

            SetGeometry("View", new MeshGeometry3D());
            AddInstance("View", Matrix.Identity);

            MeshBuilder planeBuilder = new MeshBuilder();
            planeBuilder.AddQuad(new Vector3(-15, 0, 15), new Vector3(-15, 0, -15), new Vector3(15, 0, -15), new Vector3(15, 0, 15));
            MeshNode plane = new MeshNode()
            {
                ModelMatrix = Matrix.Identity,
                Material = new DiffuseMaterialCore() { DiffuseColor = new Color4(.5f, .5f, .5f, 1.0f) },
                Geometry = planeBuilder.ToMeshGeometry3D(),
            };
            viewport.Items.AddChildNode(plane);

            Redraw();
        }
    }
}
