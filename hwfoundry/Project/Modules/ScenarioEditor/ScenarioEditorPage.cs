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

namespace Foundry.Project.Modules
{
    public class ScenarioEditorPage : FoundryPage
    {
        private Timer t;
        private ViewportCore viewport;
        private CameraCore camera;
        private AmbientLightNode ambientLight;
        public ScenarioEditorPage(ContentFile f) : base(f)
        {
            Init();

            t = new Timer();
            t.Interval = 1;
            t.Tick += Tick;
            t.Start();
        }
        private void Tick(object o, EventArgs e)
        {

        }
        void Init()
        {
            viewport = new ViewportCore();
            camera = new PerspectiveCameraCore()
            {
                LookDirection = new Vector3(0, 0, 1),
                Position = new Vector3(0, 0, -10),
                FarPlaneDistance = 5000f,
                NearPlaneDistance = .1f,
                FieldOfView = 90,
                UpDirection = new Vector3(0, 1, 0)
            };
            ambientLight = new AmbientLightNode();

            viewport.CameraCore = camera;
            viewport.Items.AddChildNode(ambientLight);
        }
    }
}
