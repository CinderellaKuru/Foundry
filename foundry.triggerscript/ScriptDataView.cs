using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Newtonsoft.Json;
using Foundry;
using System.Numerics;
using Foundry.Util;
using Foundry.util;
using static Foundry.Triggerscript.ScriptModule;
using System.Text.RegularExpressions;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;

namespace Foundry.Triggerscript
{
    public class ScriptDataView : NodeView
    {
        private OperatorRegistrantToolstrip OperatorRegistrant { get; set; }
        private PointF CapturedLocation { get; set; }

        public ScriptDataView(FoundryInstance i) : base(i)
        {
            Form.ContextMenuStrip = new ContextMenuStrip();
            Form.ContextMenuStrip.Opened += (sender, e) =>
            {
                CapturedLocation = GetTransformedMousePos();
            };
            OperatorRegistrant = new OperatorRegistrantToolstrip();
            CapturedLocation = new PointF(0, 0);

            Operator opAddTrigger = new Operator("Add Trigger");
            opAddTrigger.OperatorActivated += (sender, e) =>
            {
                ((ScriptData)NodeData).AddTrigger(string.Format("NewTrigger{0}", Random.Shared.Next()), CapturedLocation);
            };
            OperatorRegistrant.Operators.Add(opAddTrigger);

            Dictionary<string, Operator> opConditionCategories = new Dictionary<string, Operator>();
            Operator opAddCondition = new Operator("Add Condition");
            foreach (var pair in ConditionItems.Values)
            {
                foreach (var versions in pair.Values)
                {
                    foreach (var version in pair.Values)
                    {
                        string category = "";//ConditionCategories[version.Name];
                        string running = "";
                        Operator last = opAddCondition;
                        foreach (string entry in category.Split("|"))
                        {
                            running += "|" + entry;
                            if (!opConditionCategories.ContainsKey(running))
                            {
                                Operator entryOp = new Operator(entry);
                                entryOp.Parent = last;
                                last = entryOp;
                                opConditionCategories.Add(running, entryOp);
                            }
                            else
                            {
                                last = opConditionCategories[running];
                            }
                        }

                        string ver = version.Version == -1 ? "" : " v" + version.Version.ToString();
                        Operator opEffect = new Operator(string.Format("{0}{1}", version.Name, ver));
                        opEffect.OperatorActivated += (sender, e) =>
                        {
                            ((ScriptData)NodeData).AddCondition(version.DBID, version.Version, CapturedLocation);
                        };
                        opEffect.Parent = opConditionCategories["|" + category];
                    }
                }
            }
            OperatorRegistrant.AddOperator(opAddCondition);

            Dictionary<string, Operator> opEffectCategories = new Dictionary<string, Operator>();
            Operator opAddEffect = new Operator("Add Effect");
            foreach (var pair in EffectItems.Values)
            {
                foreach (var version in pair.Values)
                {
                    string category = EffectCategories[version.Name];
                    string running = "";
                    Operator last = opAddEffect;
                    foreach (string entry in category.Split("|"))
                    {
                        running += "|" + entry;
                        if (!opEffectCategories.ContainsKey(running))
                        {
                            Operator entryOp = new Operator(entry);
                            entryOp.Parent = last;
                            last = entryOp;
                            opEffectCategories.Add(running, entryOp);
                        }
                        else
                        {
                            last = opEffectCategories[running];
                        }
                    }

                    string ver = version.Version == -1 ? "" : " v" + version.Version.ToString();
                    Operator opEffect = new Operator(string.Format("{0}{1}", version.Name, ver));
                    opEffect.OperatorActivated += (sender, e) =>
                    {
                        ((ScriptData)NodeData).AddEffect(version.DBID, version.Version, CapturedLocation);
                    };
                    opEffect.Parent = opEffectCategories["|" + category];
                }
            }
            OperatorRegistrant.AddOperator(opAddEffect);

            Operator opAddVar = new Operator("Add Variable");
            foreach (string type in Enum.GetNames<ScriptVarType>())
            {
                Operator opVar = new Operator(type);
                opVar.OperatorActivated += (sender, e) =>
                {
                    ((ScriptData)NodeData).AddVariable(Enum.Parse<ScriptVarType>(type), CapturedLocation);
                };
                opVar.Parent = opAddVar;
            }
            OperatorRegistrant.AddOperator(opAddVar);

            Form.ContextMenuStrip.Items.AddRange(OperatorRegistrant.GetRootMenuItems().ToArray());

            ViewTick += (sender, e) =>
            {
                if (GetKeyIsDown(Keys.F5) && !GetKeyWasDown(Keys.F5))
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Script(*.triggerscript)|*.triggerscript";
                    if (sfd.ShowDialog(Instance) == DialogResult.OK)
                    {
                        var ser = new YAXSerializer<ScriptXml>(new SerializerOptions() { ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow });
                        ser.SerializeToFile(((ScriptData)NodeData).TriggerscriptData, sfd.FileName);
                    }
                }
            };
        }
    }
}