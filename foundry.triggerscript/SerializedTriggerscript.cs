using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace foundry.triggerscript
{
    public class Input
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }
    public class Output
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }
    public class SerializedEffect
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public List<string> sources = new List<string>();
        public int dbid;
        public int version;
    }
    public class SerializedCondition
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public int dbid;
        public int version;
    }
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
        public int sourceId;
        public string sourceType;
        public string sourceSocketName;

        public int targetId;
        public string targetType;
        public string targetSocketName;
    }
    public class SerializedNode
    {
        public int id;
        public float x, y;
        public string handleAs;
        public bool selected;
        public SerializedTrigger trigger;
        public SerializedVariable variable;
        public SerializedEffect effect;
        public SerializedCondition condition;
    }
    public class SerializedTriggerscript
    {
        public int lastTrg, lastVar, lastEff, lastCnd;
        public List<SerializedNode> nodes = new List<SerializedNode>();
        public List<SerializedNodeLink> links = new List<SerializedNodeLink>();
    }
}
