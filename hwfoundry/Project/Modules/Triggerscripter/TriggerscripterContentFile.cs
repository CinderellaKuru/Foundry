using System.IO;
using System.Collections.Generic;
using static Foundry.Project.ModProject;
using Newtonsoft.Json;

namespace Foundry.Project.Modules.Triggerscripter
{
    public class SerializedVariable
    {
        public string name;
        public string value;
        public string type;
    }
    public class SerializedTrigger
    {
        public string name;
        public bool cndIsOr;
        public bool active;
    }
    public class SerializedNodeLink
    {
        public string sourceType;
        public string sourceSocketName;
        public int sourceId;

        public int targetId;
        public string targetType;
        public string targetSocketName;
    }
    public class SerializableNode
    {
        public int id;
        public int x, y;
        public string handleAs;
        public bool selected;
        public SerializedTrigger trigger;
        public SerializedVariable variable;
        public Effect effect;
        public Condition condition;
    }
    public class SerializedTriggerscripter
    {
        public int lastTrg, lastVar, lastEff, lastCnd;
        public List<SerializableNode> nodes = new List<SerializableNode>();
        public List<SerializedNodeLink> links = new List<SerializedNodeLink>();
    }

    public class TriggerscriptContentFile : ModProjectContentFile
    {
        SerializedTriggerscripter sts;
        TriggerscripterPage page;
        EntryNodeData node;
        
        public TriggerscriptContentFile(string fileName) : base(fileName)
        {
            if (!File.Exists(fileName)) return;

            node = new EntryNodeData();
            node.Text = Path.GetFileName(fileName);
            node.Image = ModProject.images["FILE"];
            node.SubName = "";
            node.FullPath = fileName;
        }

        public override EntryNodeData GetRootNode()
        {
            return node;
        }
        protected override void DoOpen(string subName)
        {
            page = new TriggerscripterPage();
            page.Text = Path.GetFileName(PathOnDisk);

            page.Show(Program.window.Workspace(), WeifenLuo.WinFormsUI.Docking.DockState.Document);            

            string file = File.ReadAllText(PathOnDisk);
            sts = JsonConvert.DeserializeObject<SerializedTriggerscripter>(file);
            page.LoadFromFile(sts);

            Program.window.project.SetActiveFile(this);
        }
        protected override void DoSave()
        {
            page.Text = Path.GetFileName(PathOnDisk);
            string file = JsonConvert.SerializeObject(page.GetSerializedGraph());
            File.WriteAllText(PathOnDisk, file);
        }
    }
}
