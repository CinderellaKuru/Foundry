using hwFoundry.GUI;
using hwFoundry.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace hwFoundry.Modules.TriggerScripter
{
    /// <summary>
    /// Files that can be opened in their own node graphs.
    /// Attaches itself to the nodes of the Project Explorer
    /// that have the .tsp extension.
    /// </summary>
    public class TriggerScriptFile : ContentFile
    {
        // Members
        EntryNode node;
        GUI.TriggerScripter? parentPage = null;

        // Constructor
        public TriggerScriptFile(string filepath) : base(filepath)
        {
            // Check to make sure this file actually exists
            if (!File.Exists(filepath))
                return;

            // Assign this node's visual data
            node = new EntryNode
            {
                Text     = Path.GetFileName(filepath),
                Image    = Properties.Resources.page_white,
                SubName  = string.Empty,
                FullPath = filepath
            };
        }

        // Methods
        public override EntryNode GetRootNode()
            => node;

        protected override void Open(string subName)
        {
            // Check if page needs to be added to document view
            if (parentPage == null || parentPage.IsDead)
                parentPage = new() { Text = Path.GetFileName(PathOnDisk) };

            // Focus on this page in the viewer and set it as the active file
            parentPage.Show(Program.mainWindow.GetWorkspace(), DockState.Document);
            Program.mainWindow.modProject.SetActiveFile(this);
            
            // Populate node graph with saved nodes
            string file = File.ReadAllText(PathOnDisk);
            SerializedGraph graph = JsonConvert.DeserializeObject<SerializedGraph>(file);
            
            if (graph != null)
                parentPage.LoadFromFile(graph);
        }

        protected override void Save()
        {
            parentPage.Text = Path.GetFileName(PathOnDisk);
            string file = JsonConvert.SerializeObject(parentPage.GetSerializedGraph());
            File.WriteAllText(PathOnDisk, file);
        }
    }

    #region Serializable Objects

    // TODO: Maybe make this just one class? Like "Socket"?
    [Serializable]
    public class Input
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }

    [Serializable]
    public class Output
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }

    [Serializable]
    public class SerializedEffect
    {
        public string name;
        public List<Input> inputs   = new();
        public List<Output> outputs = new();
        public List<string> sources = new();
        public int dbid, version;
    }

    [Serializable]
    public class SerializedCondition
    {
        public string name;
        public List<Input> inputs   = new();
        public List<Output> outputs = new();
        public int dbid, version;
    }

    [Serializable]
    public class SerializedTrigger
    {
        public string name;
        public bool cndIsOr, active;
    }

    [Serializable]
    public class SerializedVariable
    {
        public string name, value, type;
    }

    [Serializable]
    public class SerializedNode
    {
        public int id, x, y;
        public string handleAs;
        public bool selected;
        public SerializedTrigger trigger;
        public SerializedVariable variable;
        public SerializedEffect effect;
        public SerializedCondition condition;
    }

    [Serializable]
    public class SerializedNodeLink
    {
        public int sourceId;
        public string sourceType, sourceSocketName;

        public int targetId;
        public string targetType, targetSocketName;
    }

    [Serializable]
    public class SerializedGraph
    {
        public int lastTrg, lastVar, lastEff, lastCnd;
        public List<SerializedNode> nodes = new();
        public List<SerializedNodeLink> links = new();
    }
    #endregion
}
