using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;

namespace Foundry.Data.Triggerscript
{
    public enum ScriptVarType
        {
            Invalid = 0,
            Tech,
            TechStatus,
            Operator,
            ProtoObject,
            ObjectType,
            ProtoSquad,
            Sound,
            Entity,
            EntityList,
            Trigger,
            Time,
            Player,
            UILocation,
            UIEntity,
            Cost,
            AnimType,
            ActionStatus,
            Power,
            Bool,
            Float,
            Iterator,
            Team,
            PlayerList,
            TeamList,
            PlayerState,
            Objective,
            Unit,
            UnitList,
            Squad,
            SquadList,
            UIUnit,
            UISquad,
            UISquadList,
            String,
            MessageIndex,
            MessageJustify,
            MessagePoint,
            Color,
            ProtoObjectList,
            ObjectTypeList,
            ProtoSquadList,
            TechList,
            MathOperator,
            ObjectDataType,
            ObjectDataRelative,
            Civ,
            ProtoObjectCollection,
            Object,
            ObjectList,
            Group,
            RefCountType,
            UnitFlag,
            LOSType,
            EntityFilterSet,
            PopBucket,
            ListPosition,
            RelationType,
            ExposedAction,
            SquadMode,
            ExposedScript,
            KBBase,
            KBBaseList,
            DataScalar,
            KBBaseQuery,
            DesignLine,
            LocStringID,
            Leader,
            Cinematic,
            FlareType,
            CinematicTag,
            IconType,
            Difficulty,
            Integer,
            HUDItem,
            ControlType,
            UIButton,
            MissionType,
            MissionState,
            MissionTargetType,
            IntegerList,
            BidType,
            BidState,
            BuildingCommandState,
            Vector,
            VectorList,
            PlacementRule,
            KBSquad,
            KBSquadList,
            KBSquadQuery,
            AISquadAnalysis,
            AISquadAnalysisComponent,
            KBSquadFilterSet,
            ChatSpeaker,
            RumbleType,
            RumbleMotor,
            TechDataCommandType,
            SquadDataType,
            EventType,
            TimeList,
            DesignLineList,
            GameStatePredicate,
            FloatList,
            UILocationMinigame,
            SquadFlag,
            FlashableUIItem,
            TalkingHead,
            Concept,
            ConceptList,
            UserClassType,
        }

    public class ScriptLogicPrototype
    {
        public string Name { get; set; }
        public int Version { get; set; }
        public int DBID { get; set; }
        public class Param
        {
            public string Name { get; set; }
            public int ID { get; set; }
            public bool Optional { get; set; }
            public bool Output { get; set; }
            public ScriptVarType Type { get; set; }
        }
        public List<Param> Params { get; set; } = new List<Param>();
        [YAXDontSerialize]
        public string Category { get; set; }
    }

    public class TriggerscriptNodeMetadataXml
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class TriggerscriptMetadatasXml
    {
        public TriggerscriptMetadatasXml()
        {
            VariableMetadata = new Dictionary<int, TriggerscriptNodeMetadataXml>();
            TriggerMetadata = new Dictionary<int, TriggerscriptNodeMetadataXml>();
            EffectMetadata = new Dictionary<int, TriggerscriptNodeMetadataXml>();
            ConditionMetadata = new Dictionary<int, TriggerscriptNodeMetadataXml>();
        }

        [YAXDictionary(EachPairName = "Variable",
            KeyName = "ID",         SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            ValueName = "Metadata", SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Content)]
        public Dictionary<int, TriggerscriptNodeMetadataXml> VariableMetadata { get; set; }

        [YAXDictionary(EachPairName = "Trigger",
            KeyName = "ID",         SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            ValueName = "Metadata", SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Content)]
        public Dictionary<int, TriggerscriptNodeMetadataXml> TriggerMetadata { get; set; }

        [YAXDictionary(EachPairName = "Effect",
            KeyName = "ID", SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            ValueName = "Metadata", SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Content)]
        public Dictionary<int, TriggerscriptNodeMetadataXml> EffectMetadata { get; set; }

        [YAXDictionary(EachPairName = "Condition",
            KeyName = "ID", SerializeKeyAs = YAXLib.Enums.YAXNodeTypes.Attribute,
            ValueName = "Metadata", SerializeValueAs = YAXLib.Enums.YAXNodeTypes.Content)]
        public Dictionary<int, TriggerscriptNodeMetadataXml> ConditionMetadata { get; set; }
    }

    [YAXSerializeAs("TriggerSystem")]
    public class TriggerscriptXmlData
    {
        public TriggerscriptXmlData()
        {
            Metadata = new TriggerscriptMetadatasXml();
            TriggerGroups = new List<TriggerGroupClass>();
            TriggerVars = new List<TriggerVarClass>();
            Triggers = new List<TriggerClass>();
            Name = "";
            Type = "TriggerScript";
        }

        [YAXAttributeForClass()]
        public string Name { get; set; }
        [YAXAttributeForClass()]
        public string Type { get; set; }
        [YAXAttributeForClass()]
        public int NextTriggerVarID { get; set; }
        [YAXAttributeForClass()]
        public int NextTriggerID { get; set; }
        [YAXAttributeForClass()]
        public int NextConditionID { get; set; }
        [YAXAttributeForClass()]
        public int NextEffectID { get; set; }
        [YAXAttributeForClass()]
        public bool External { get; set; }

        public class TriggerGroupClass
        {
            [YAXAttributeForClass()]
            public int ID { get; set; }

            [YAXAttributeForClass()]
            public string Name { get; set; }

            [YAXValueForClass()]
            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Serially, SeparateBy = ",")]
            public List<int> Values { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Group")]
        public List<TriggerGroupClass> TriggerGroups { get; set; }

        public class TriggerVarClass
        {
            public TriggerVarClass()
            {
            }
            public TriggerVarClass(TriggerVarClass copy) : this()
            {
                this.ID = copy.ID;
                this.Type = copy.Type;
                this.Name = copy.Name; 
                this.IsNull = copy.IsNull;
                this.Value = copy.Value;
            }


            [YAXAttributeForClass()]
            public int ID { get; set; }

            [YAXAttributeForClass()]
            public string Type { get; set; }

            [YAXAttributeForClass()]
            public string Name { get; set; }

            [YAXAttributeForClass()]
            public bool IsNull { get; set; }

            [YAXValueForClass()]
            public string Value { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "TriggerVar")]
        public List<TriggerVarClass> TriggerVars { get; set; }

        public class TriggerClass
        {
            public TriggerClass()
            {
                TriggerEffectsOnTrue = new List<EffectClass>();
                TriggerEffectsOnFalse = new List<EffectClass>();
                Conditions = new List<ConditionClass>();
                _ConditionsAreAND = true;
            }
            public TriggerClass(TriggerClass copy) : this()
            {
                this.ID = copy.ID;
                this.Name = copy.Name;
                this.Active = copy.Active;
                this.EvalLimit = copy.EvalLimit;
                this.EvaluateFrequency = copy.EvaluateFrequency;
                this.CommentOut = copy.CommentOut;
                this.X = copy.X;
                this.Y = copy.Y;
                this.GroupID = copy.GroupID;
                this.Conditions = copy.Conditions.ConvertAll(cnd => new ConditionClass(cnd));
                this.TriggerEffectsOnTrue = copy.TriggerEffectsOnTrue.ConvertAll(eff => new EffectClass(eff));
                this.TriggerEffectsOnFalse = copy.TriggerEffectsOnFalse.ConvertAll(eff => new EffectClass(eff));
            }

            [YAXAttributeForClass()]
            public int ID { get; set; }
            [YAXAttributeForClass()]
            public string Name {get; set;}
            [YAXAttributeForClass()]
            public bool Active { get; set; }
            [YAXAttributeForClass()]
            public int EvaluateFrequency { get; set; }
            [YAXAttributeForClass()]
            public int EvalLimit { get; set; }
            [YAXAttributeForClass()]
            public bool CommentOut { get; set; }
            [YAXAttributeForClass()]
            public bool ConditionalTrigger { get; set; }
            [YAXAttributeForClass()]
            public int GroupID { get; set; }

            [YAXAttributeForClass()]
            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public float X { get; set; }
            [YAXAttributeForClass()]
            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public float Y { get; set; }

            public class ParameterClass
            {
                public ParameterClass()
                {

                }
                public ParameterClass(ParameterClass copy) : this()
                {
                    this.Name = copy.Name;
                    this.SigID = copy.SigID;
                    this.Optional = copy.Optional;
                    this.Value = copy.Value;
                }

                [YAXAttributeForClass()]
                public string Name { get; set; }
                [YAXAttributeForClass()]
                public int SigID { get; set; }
                [YAXAttributeForClass()]
                public bool Optional { get; set; }
                [YAXValueForClass()]
                public int Value { get; set; }
            }
            public class LogicClass
            {
                public LogicClass()
                {
                    Inputs = new List<ParameterClass>();
                    Outputs = new List<ParameterClass>();
                }
                public LogicClass(LogicClass copy) : this()
                {
                    this.ID = copy.ID;
                    this.Type = copy.Type;
                    this.DBID = copy.DBID;
                    this.Version = copy.Version;
                    this.CommentOut = copy.CommentOut;
                    this.X = copy.X;
                    this.Y = copy.Y;

                    this.Inputs = copy.Inputs.ConvertAll(p => new ParameterClass(p));
                    this.Outputs = copy.Outputs.ConvertAll(p => new ParameterClass(p));
                }

                [YAXAttributeForClass()]
                public int ID { get; set; }
                [YAXAttributeForClass()]
                public string Type { get; set; }
                [YAXAttributeForClass()]
                public int DBID { get; set; }
                [YAXAttributeForClass()]
                public int Version { get; set; }
                [YAXAttributeForClass()]
                public bool CommentOut { get; set; }

                [YAXAttributeForClass()]
                [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
                public float X { get; set; }
                [YAXAttributeForClass()]
                [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
                public float Y { get; set; }

                [YAXValueForClass()]
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Input")]
                public List<ParameterClass> Inputs { get; set; }

                [YAXValueForClass()]
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Output")]
                public List<ParameterClass> Outputs { get; set; }

                public int GetValueOfParam(int sigID, bool output = false)
                {
                    if (Inputs != null)
                    {
                        foreach (ParameterClass param in Inputs)
                        {
                            if (param.SigID == sigID) return param.Value;
                        }
                    }
                    if (Outputs != null)
                    {
                        foreach (ParameterClass param in Outputs)
                        {
                            if (param.SigID == sigID) return param.Value;
                        }
                    }
                    return -1;
                }
                public void SetValueOfParam(int sigID, int value)
                {
                    if (Inputs != null)
                    {
                        foreach (ParameterClass param in Inputs)
                        {
                            if (param.SigID == sigID) param.Value = value;
                        }
                    }
                    if (Outputs != null)
                    {
                        foreach (ParameterClass param in Outputs)
                        {
                            if (param.SigID == sigID) param.Value = value;
                        }
                    }
                }
            }
            public class EffectClass : LogicClass
            {
                public EffectClass() : base()
                {

                }
                public EffectClass(EffectClass copy) : base(copy)
                {

                }
            }
            public class ConditionClass : LogicClass
            {
                public ConditionClass() : base()
                {

                }
                public ConditionClass(ConditionClass copy) : base(copy)
                {
                    this.Invert = copy.Invert;
                    this.Async = copy.Async;
                    this.AsyncParameterKey = copy.AsyncParameterKey;
                }

                [YAXAttributeForClass()]
                public bool Invert { get; set; }
                [YAXAttributeForClass()]
                public bool Async { get; set; }
                [YAXAttributeForClass()]
                public int AsyncParameterKey { get; set; }
            }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Condition")]
            [YAXElementFor("TriggerConditions")]
            [YAXSerializeAs("Or")]
            [YAXDontSerializeIfNull()]
            public List<ConditionClass> ConditionsOr
            {
                get
                {
                    if (!ConditionsAreAND) return Conditions;
                    else return null;
                }
                set
                {
                    Conditions = value;
                    _ConditionsAreAND = false;
                }
            }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Condition")]
            [YAXElementFor("TriggerConditions")]
            [YAXSerializeAs("And")]
            [YAXDontSerializeIfNull()]
            public List<ConditionClass> ConditionsAnd
            {
                get
                {
                    if (ConditionsAreAND) return Conditions;
                    else return null;
                }
                set
                {
                    Conditions = value;
                    _ConditionsAreAND = true;
                }
            }

            [YAXDontSerialize()]
            public bool ConditionsAreAND
            {
                get
                {
                    return _ConditionsAreAND;
                }
                set
                {
                    if (value == true)
                    {
                        ConditionsAnd = ConditionsOr;
                        ConditionsOr = null;
                    }
                    if (value == false)
                    {
                        ConditionsOr = ConditionsAnd;
                        ConditionsAnd = null;
                    }
                    _ConditionsAreAND = value;
                }
            }
            [YAXDontSerialize()]
            private bool _ConditionsAreAND;
            [YAXDontSerialize()]
            public List<ConditionClass> Conditions { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Effect")]
            public List<EffectClass> TriggerEffectsOnTrue { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Effect")]
            public List<EffectClass> TriggerEffectsOnFalse { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Trigger")]
        public List<TriggerClass> Triggers { get; set; }


        public TriggerscriptMetadatasXml Metadata { get; set; }
    }
}