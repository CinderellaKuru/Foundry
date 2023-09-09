using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;

namespace foundry.triggerscript
{
    public class EditorNodeDataClass
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    [YAXSerializeAs("TriggerSystem")]
    public class TriggerscriptClass
    {
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

            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public EditorNodeDataClass EditorNodeData { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "TriggerVar")]
        public List<TriggerVarClass> TriggerVars { get; set; }

        public class TriggerClass
        {
            [YAXAttributeForClass()]
            public int ID { get; set; }
            [YAXAttributeForClass()]
            public string Name { get; set; }
            [YAXAttributeForClass()]
            public bool Active { get; set; }
            [YAXAttributeForClass()]
            public int EvaluateFrequency { get; set; }
            [YAXAttributeForClass()]
            public int EvalLimit { get; set; }
            [YAXAttributeForClass()]
            public bool CommentOut { get; set; }
            [YAXAttributeForClass()]
            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public int X { get; set; }
            [YAXAttributeForClass()]
            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public int Y { get; set; }
            [YAXAttributeForClass()]
            public int GroupID { get; set; }


            public class ParameterClass
            {
                [YAXAttributeForClass()]
                public string Name { get; set; }
                [YAXAttributeForClass()]
                public int SigID { get; set; }
                [YAXAttributeForClass()]
                public bool Optional { get; set; }
                [YAXValueForClass()]
                public int Value { get; set; }
            }
            public class ConditionClass
            {
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
                public bool Invert { get; set; }
                [YAXAttributeForClass()]
                public bool Async { get; set; }
                [YAXAttributeForClass()]
                public int AsyncParameterKey { get; set; }

                [YAXValueForClass()]
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Input")]
                public List<ParameterClass> Inputs { get; set; }

                [YAXValueForClass()]
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Output")]
                public List<ParameterClass> Outputs { get; set; }


                [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
                public EditorNodeDataClass EditorNodeData { get; set; }
            }
            public class EffectClass
            {
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

                [YAXValueForClass()]
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Input")]
                public List<ParameterClass> Inputs { get; set; }

                [YAXValueForClass()]
                [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Output")]
                public List<ParameterClass> Outputs { get; set; }

                [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
                public EditorNodeDataClass EditorNodeData { get; set; }
            }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Condition")]
            [YAXElementFor("TriggerConditions")]
            [YAXSerializeAs("Or")]
            public List<ConditionClass> ConditionsOr { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Condition")]
            [YAXElementFor("TriggerConditions")]
            [YAXSerializeAs("And")]
            public List<ConditionClass> ConditionsAnd { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Effect")]
            public List<EffectClass> TriggerEffectsOnTrue { get; set; }

            [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Effect")]
            public List<EffectClass> TriggerEffectsOnFalse { get; set; }


            [YAXErrorIfMissed(YAXLib.Enums.YAXExceptionTypes.Ignore)]
            public EditorNodeDataClass EditorNodeData { get; set; }
        }
        [YAXCollection(YAXLib.Enums.YAXCollectionSerializationTypes.Recursive, EachElementName = "Trigger")]
        public List<TriggerClass> Triggers { get; set; }
    }
}