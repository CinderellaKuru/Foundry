using foundry;
using SharpDX;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace foundry.triggerscript
{
    public class TriggerscriptModule : BaseModule
    {
        public override string ImportExt { get { return ".triggerscript"; } }
        public override string SaveExt { get { return ".fts"; } }
        public override Type PageType { get { return typeof(TriggerscriptEditorPage); } }

        public class Item
        {
            public string Name { get; set; }
            public class Param
            {
                public string Name { get; set; }
                public int ID { get; set; }
                public bool Optional { get; set; }
                public bool Output { get; set; }
            }
            public List<Param> Params { get; set; } = new List<Param>();
        }
        public IReadOnlyCollection<Item> EffectItems {get;private set;}
        public IReadOnlyCollection<Item> ConditionItems {get;private set; }

        protected override void OnInit()
        {
            YAXSerializer tsdefSerializer = new YAXSerializer(typeof(List<Item>));
            EffectItems = (List<Item>)tsdefSerializer.DeserializeFromFile("effects.tsdef");
            ConditionItems = (List<Item>)tsdefSerializer.DeserializeFromFile("conditions.tsdef");

            YAXSerializer ser = new YAXSerializer(typeof(TriggerscriptClass), new YAXLib.Options.SerializerOptions()
            {
                SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects,
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors
            });

            TriggerscriptClass aa = (TriggerscriptClass)ser.Deserialize(
                File.ReadAllText(
                    "R:\\foundry\\_resources\\New folder (2)\\data\\triggerscripts\\abilitylockdown.triggerscript")
                );

            TriggerscriptEditorPage page = new TriggerscriptEditorPage();
            page.Show(Instance.MainDockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            page.Init(Instance);
            page.LoadFromSerializedGraph(aa);
        }
        protected override void OnPostInit()
        {
        }
        protected override void OnWorkspaceOpened()
        {

        }
        protected override void OnWorkspaceClosed()
        {
        }

        private void LoadAllTriggerscripts()
        {
            
        }
    }
}