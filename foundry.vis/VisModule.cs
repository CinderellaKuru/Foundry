using foundry.unit;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using WeifenLuo.WinFormsUI.Docking;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using static foundry.unit.UnitModule;

namespace foundry.vis
{
    public class VisModule : BaseModule
    {
        public override Type PageType { get { return typeof(BaseEditorPage); } }
        public UnitModule UnitModuleInstance { get; private set; }

        protected override void OnInit()
        {
        }
        protected override void OnPostInit()
        {
            UnitModule unitModule = null;
            bool valid = Instance.GetModuleByType<UnitModule>(out unitModule);
            if (!valid)
            {
                //TODO: There should really be a unified system for this in the BaseModule class...
                throw new Exception("Invalid dependencies.");
            }
            UnitModuleInstance = unitModule;

            Operator opOpenVisEditor = new Operator("Edit Visual");
            opOpenVisEditor.OperatorActivated += (sender, e) =>
            {
            };
            UnitModuleInstance.Operators_UnitRightClicked.AddOperator(opOpenVisEditor);
        }
        protected override void OnWorkspaceOpened()
        {
            UnitPickerPage p2 = new UnitPickerPage(UnitModuleInstance);
            p2.Show(Instance.MainDockPanel, DockState.DockLeft);

            UnitModuleInstance.UpdateModule();

            LoadAllVisuals();
            LoadAllVisualGeometry();

            VisViewerPage vvp = new VisViewerPage();
            vvp.Init(Instance);
            vvp.Show(Instance.MainDockPanel, DockState.Document);
        }
        protected override void OnWorkspaceClosed()
        {
        }


        [YAXSerializeAs("Visual")]
        public class Visual
        {
            [YAXSerializeAs("defaultmodel")]
            [YAXAttributeForClass]
            public string DefaultModel { get; set; }


            [YAXSerializeAs("model")]
            public class Model
            {
                [YAXSerializeAs("name")]
                [YAXAttributeForClass]
                public string Name { get; set; }

                public class ComponentClass
                {

                    public class LogicClass
                    {

                    }
                    [YAXSerializeAs("logic")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public LogicClass Logic { get; set; }


                    public class AssetClass
                    {
                        public enum TypeEnum
                        {
                            Model,
                            Particle
                        }
                        [YAXSerializeAs("type")]
                        [YAXAttributeForClass]
                        public TypeEnum Type { get; set; }

                        [YAXSerializeAs("file")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public string File { get; set; }

                        [YAXSerializeAs("damagefile")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public string DamageFile { get; set; }
                    }
                    [YAXSerializeAs("asset")]
                    [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                    public AssetClass Asset { get; set; }


                    public class AttachmentClass
                    {
                        public enum TypeEnum
                        {
                            ModelRef,
                            Model,
                            Particle,
                        }
                        [YAXSerializeAs("type")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public TypeEnum Type { get; set; }

                        [YAXSerializeAs("name")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public string Name { get; set; }

                        [YAXSerializeAs("tobone")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public string ToBone { get; set; }

                        [YAXSerializeAs("frombone")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public string FromBone { get; set; }

                        [YAXSerializeAs("syncanims")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public bool SyncAnims { get; set; }

                        [YAXSerializeAs("disregardorient")]
                        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
                        public bool DisregardOrient { get; set; }
                    }
                    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "attach")]
                    public List<AttachmentClass> AttachmentClasses { get; set; }

                }
                [YAXSerializeAs("component")]
                public ComponentClass Component { get; set; }

            }
            [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "model")]
            public List<Model> Models { get; set; }
        }
        public Dictionary<Unit, Visual> UnitVisuals { get; set; } = new Dictionary<Unit, Visual>();
        private void LoadAllVisuals()
        {
            foreach (Unit unit in UnitModuleInstance.Units.Values)
            {
                if (unit.Visual == null) continue;
                string visfile = Instance.OpenedWorkspaceDir + "art/" + unit.Visual;

                if (File.Exists(visfile))
                {
                    string visxml = File.ReadAllText(visfile);

                    YAXSerializer ser = new YAXSerializer(typeof(Visual));
                    Visual vis = (Visual)ser.Deserialize(visxml);

                    UnitVisuals.Add(unit, vis);
                }
            }
        }

        public Dictionary<Visual, Geometry3D> VisualGeometries { get; set; } = new Dictionary<Visual, Geometry3D>();
        private void LoadAllVisualGeometry()
        {
            foreach(Visual visual in UnitVisuals.Values)
            {
                Visual.Model model = visual.Models.Find(m => m.Name == visual.DefaultModel);
                string file = "";
                if (model.Component.Asset != null &&
                    model.Component.Asset.File != null)
                {
                    file = Instance.OpenedWorkspaceDir + "art/" + model.Component.Asset.File + ".ugx";
                }
                else
                {
                    continue;
                }

                if (File.Exists(file)) {
                    Geometry3D geometry = UGXImporter.ImportUGXGeometry(file);
                    VisualGeometries.Add(visual, geometry);
                    MeshGeometryHelper.CalculateNormals((MeshGeometry3D)geometry);
                }
            }
        }
    }
}