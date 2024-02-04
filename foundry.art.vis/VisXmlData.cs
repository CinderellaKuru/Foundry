using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace Foundry.Asset
{
    [YAXSerializeAs("Visual")]
    public class VisXmlData
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
                public List<AttachmentClass> Attachments { get; set; }

            }
            [YAXSerializeAs("component")]
            public ComponentClass Component { get; set; }

        }
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "model")]
        public List<Model> Models { get; set; }
    }
}
