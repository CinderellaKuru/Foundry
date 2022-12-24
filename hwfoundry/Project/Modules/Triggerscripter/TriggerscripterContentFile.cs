using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using static Foundry.FoundryInstance;
using WeifenLuo.WinFormsUI.Docking;

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

    public class TriggerscriptContentFile : ContentFile
    {
        SerializedTriggerscripter sts;
        TriggerscripterPage page;

        public TriggerscriptContentFile(FoundryInstance i, string fileName) : base(i, fileName)
        {
            if (!File.Exists(fileName)) return;
        }
        protected override DockContent DoOpenFile()
        {
            page = new TriggerscripterPage(Instance());
            page.Text = Path.GetFileName(FileName());

            string file = File.ReadAllText(FileName());
            sts = JsonConvert.DeserializeObject<SerializedTriggerscripter>(file);
            page.Load(sts);

            return page;
        }
        protected override void DoSaveFile()
        {
            page.Text = Path.GetFileName(FileName());
            string file = JsonConvert.SerializeObject(page.GetSerializedGraph());
            File.WriteAllText(FileName(), file);
        }
    }
}
