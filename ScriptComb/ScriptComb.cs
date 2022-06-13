using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

class Input
{
    public string name;
    public string valueType;
    public bool optional;
    public int sigId;
}
class Output
{
    public string name;
    public string valueType;
    public bool optional;
    public int sigId;
}
class Effect
{
    public string name;
    public List<Input> inputs = new List<Input>();
    public List<Output> outputs = new List<Output>();
    public List<string> sources = new List<string>();
    public int dbid;
    public int version;
}
class Condition
{
    public string name;
    public List<Input> inputs = new List<Input>();
    public List<Output> outputs = new List<Output>();
    public int dbid;
    public int version;
}

class ScriptComb
{
    public static void Comb()
    {
        Dictionary<string, Effect> effects = new Dictionary<string, Effect>();
        List<string> varTypes = new List<string>();
        Dictionary<string, Condition> conditions = new Dictionary<string, Condition>();

        string[] files = Directory.GetFiles("res");
        foreach (string s in files)
        {
           // try
            {
                Dictionary<int, string> vars = new Dictionary<int, string>();
                Dictionary<int, string> varValues = new Dictionary<int, string>();

                XmlDocument doc = new XmlDocument();
                doc.Load(s);
                XmlNode root = doc.ChildNodes[1];

                //vars
                XmlNode variables = root.ChildNodes[1];
                for (int i = 0; i < variables.ChildNodes.Count; i++)
                {
                    vars.Add(int.Parse(variables.ChildNodes[i].Attributes[0].Value), variables.ChildNodes[i].Attributes[1].Value);
                    varValues.Add(int.Parse(variables.ChildNodes[i].Attributes[0].Value), variables.ChildNodes[i].InnerText);
                }
                
                //effects & conditons
                XmlNode triggers = root.ChildNodes[2];
                for (int i = 0; i < triggers.ChildNodes.Count; i++)
                {
                    #region effects
                    XmlNode effect;
                    for (int inOut = 1; inOut < 3; inOut++)
                    {
                        for (int z = 0; z < triggers.ChildNodes[i].ChildNodes[inOut].ChildNodes.Count; z++)
                        {
                            effect = triggers.ChildNodes[i].ChildNodes[inOut].ChildNodes[z];
                            Effect e = new Effect();
                            e.name = effect.Attributes[1].Value;
                            int.TryParse(effect.Attributes[2].Value, out e.dbid);
                            int.TryParse(effect.Attributes[3].Value, out e.version);

                            for (int j = 0; j < effect.ChildNodes.Count; j++)
                            {
                                if (effect.ChildNodes[j].Name == "Input")
                                {
                                    Input input = new Input();
                                    input.name = effect.ChildNodes[j].Attributes[0].Value.Replace(" ", "");
                                    input.optional = bool.Parse(effect.ChildNodes[j].Attributes[2].Value);
                                    input.valueType = vars[int.Parse(effect.ChildNodes[j].InnerText)].Replace(" ", "");
                                    int.TryParse(effect.ChildNodes[j].Attributes[1].Value, out input.sigId);

                                    e.inputs.Add(input);
                                }

                                if (effect.ChildNodes[j].Name == "Output")
                                {
                                    Output output = new Output();
                                    output.name = effect.ChildNodes[j].Attributes[0].Value.Replace(" ", "");
                                    output.optional = bool.Parse(effect.ChildNodes[j].Attributes[2].Value);
                                    output.valueType = vars[int.Parse(effect.ChildNodes[j].InnerText)].Replace(" ", "");
                                    int.TryParse(effect.ChildNodes[j].Attributes[1].Value, out output.sigId);

                                    e.outputs.Add(output);
                                }

                                //collect all var types
                                if (!varTypes.Contains(vars[int.Parse(effect.ChildNodes[j].InnerText)]))
                                {
                                    varTypes.Add(vars[int.Parse(effect.ChildNodes[j].InnerText)]);
                                }
                            }
                            if (!effects.ContainsKey(e.name) &&
                                !e.name.Contains("(") &&
                                e.name != "")
                            {
                                effects.Add(e.name, e);
                                //if (e.name.ToLower() == "triggeractivate") throw new Exception();
                            }
                        }
                    }
                    #endregion

                    #region conditions

                    XmlNode condition = triggers.ChildNodes[i].ChildNodes[0].ChildNodes[0];
                    for (int j = 0; j < condition.ChildNodes.Count; j++)
                    {
                        Condition c = new Condition();
                        c.name = condition.ChildNodes[j].Attributes[1].Value;
                        int.TryParse(condition.ChildNodes[j].Attributes[2].Value, out c.dbid);
                        int.TryParse(condition.ChildNodes[j].Attributes[3].Value, out c.version);

                        for (int k = 0; k < condition.ChildNodes[j].ChildNodes.Count; k++)
                        {
                            if (condition.ChildNodes[j].ChildNodes[k].Name == "Input")
                            {
                                Input input = new Input();
                                input.name = condition.ChildNodes[j].ChildNodes[k].Attributes[0].Value;
                                input.optional = bool.Parse(condition.ChildNodes[j].ChildNodes[k].Attributes[2].Value);
                                input.valueType = vars[int.Parse(condition.ChildNodes[j].ChildNodes[k].InnerText)];
                                int.TryParse(condition.ChildNodes[j].ChildNodes[k].Attributes[1].Value, out input.sigId);
                                if (!varTypes.Contains(input.valueType)) varTypes.Add(input.valueType);

                                c.inputs.Add(input);
                            }

                            if (condition.ChildNodes[j].ChildNodes[k].Name == "Output")
                            {
                                Output output = new Output();
                                output.name = condition.ChildNodes[j].ChildNodes[k].Attributes[0].Value;
                                output.optional = bool.Parse(condition.ChildNodes[j].ChildNodes[k].Attributes[2].Value);
                                output.valueType = vars[int.Parse(condition.ChildNodes[j].ChildNodes[k].InnerText)];
                                int.TryParse(condition.ChildNodes[j].ChildNodes[k].Attributes[1].Value, out output.sigId);
                                if (!varTypes.Contains(output.valueType)) varTypes.Add(output.valueType);

                                c.outputs.Add(output);
                            }
                        }
                        if (!conditions.ContainsKey(c.name) &&
                            c.name != "") conditions.Add(c.name, c);
                    }

                    #endregion

                }
            }
            //catch { Console.WriteLine("Error: " + s); }
            Console.WriteLine("Loaded: " + s);
        }
        Console.WriteLine("Loading... Done.");

        varTypes.Remove("Trigger");
        string jsonvar = JsonConvert.SerializeObject(varTypes, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("out\\var.txt", jsonvar);
        
        string jsoneff = JsonConvert.SerializeObject(effects.Values.ToList(), Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("out\\eff.txt", jsoneff);

        string jsoncnd = JsonConvert.SerializeObject(conditions.Values.ToList(), Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText("out\\cnd.txt", jsoncnd);


        //Console.WriteLine(varTypes.Count);
        //Console.WriteLine(effects.Values.Count);
        //Console.WriteLine(conditions.Values.Count);

        //Thread.Sleep(1000);
        Console.ReadLine();
        
        #region Old
        /*
        Loop:
        Console.ReadLine();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Vars
        int varNum = 0;
        int varPage = 0;
        List<string> varSorted = varTypes.ToList().OrderBy(x => x).ToList();
        Console.WriteLine(varSorted.Count);
        string strVar = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System;

public class VarStatic { public static int idTracker = 0; }
public class VarColors
{
    public static Color requiredColor = new Color(0.0f, 1.0f, 0.51f, 1.0f);
    public static Color optionalColor = new Color(1.0f, 1.0f, 0.01f, 1.0f);
}

";
        foreach (string v in varSorted)
        {
            if (v == "Trigger") continue;


strVar+=@"public class Var" + v + @"_Type { }
public class Var" + v + @"_Knob : ValueConnectionType
{
    public override string Identifier => ""Var" + v + @""";
    public override Type Type => typeof(Var" + v + @"_Type);
    public override Color Color => VarColors.requiredColor;
}
public class Var" + v + @"_KnobOptional : ValueConnectionType
{
    public override string Identifier => ""Var" + v + @"Optional"";
    public override Type Type => typeof(Var" + v + @"_Type);
    public override Color Color => VarColors.optionalColor;
}
[Node(false, ""Vars/Page " + varPage.ToString() + "/" + v + @""")]
class Var" + v + @"Node : Node
{
    public const string ID = ""Var" + v + @"Node"";
    public override string GetID { get { return ID; } }
    string title = ""[Var] "" + """ + v + @""";
    public override string Title { get { return title; } }
    public override Vector2 DefaultSize => new Vector2(215, 65);

    [ValueConnectionKnob(""Set"", Direction.In, ""Var" + v + @""")]
    public ValueConnectionKnob set;

    [ValueConnectionKnob(""Use"", Direction.Out, ""Var" + v + @""")]
    public ValueConnectionKnob use;

    public string initialValue = """";
    public int id;

    protected override void OnCreate()
    {
        id = VarStatic.idTracker;
        VarStatic.idTracker++;
    }
    public override void NodeGUI()
    {
        initialValue = RTEditorGUI.TextField(GUIContent.none, initialValue);
        base.NodeGUI();
    }
}" + "\n\n";
            varNum++;
            if (varNum >= 15) { varPage++; varNum = 0; }
        }
        File.WriteAllText("I:\\StumpyTriggerscripterUnity\\Assets\\NewNodes\\VarNode.cs", strVar);
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Effects
        int effNum = 0;
        int effPage = 0;
        List<Effect> effSorted = effects.Values.ToList().OrderBy(x => x.name).ToList();
        Console.WriteLine(effSorted.Count);
        string streff =@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System;

public class EffectCall_Type { }
public class EffectCall_Knob : ValueConnectionType
{
    public override string Identifier => ""EffectCall"";
    public override Type Type => typeof(EffectCall_Type);
    public override Color Color => Color.magenta;
}

";
        foreach (Effect e in effSorted)
        {
            if (e.name.Contains("(") || e.name == "") continue;
streff+=
@"[Node(false, ""Effects/Page " + effPage.ToString() + "/" + e.name + @""")]
class Effect" + e.name + @"Node : Node
{
    public const string ID = ""Effect" + e.name + @"Node"";
    public override string GetID { get { return ID; } }
    string title = ""[Effect] "" + """ + e.name + @""";
    public override string Title { get { return title; } }
    public override bool AutoLayout => true;

    [ValueConnectionKnob(""Caller"", Direction.In, ""EffectCall"")]
    public ValueConnectionKnob caller;
";

            if (e.name != "TriggerActivate") streff += @"    [ValueConnectionKnob(""Call Effect"", Direction.Out, ""EffectCall"")]
    public ValueConnectionKnob call;" + "\n";

            if (e.name != "TriggerActivate")
            {
                foreach (Input i in e.inputs)
                {
                    if (i.valueType == "Hook Type") i.valueType = "HookType";
                    if (i.name == "Hook Type") i.name = "HookType";
                    string op = ""; if (i.optional) op = "Optional";
                    streff +=
    @"    
    [ValueConnectionKnob(""[" + i.valueType + "] " + i.name + @""", Direction.In, ""Var" + i.valueType + op + @""")]
    public ValueConnectionKnob " + "knobIn_" + i.name + @";";
                }

                foreach (Output o in e.outputs)
                {
                    string op = ""; if (o.optional) op = "Optional";
                    streff +=
    @"    
    [ValueConnectionKnob(""[" + o.valueType + "] " + o.name + @""", Direction.Out, ""Var" + o.valueType + op + @""")]
    public ValueConnectionKnob " + "knobOut_" + o.name + @";";
                }
            }
            else if (e.name == "TriggerActivate")
            {
                streff += @"
    [ValueConnectionKnob(""Execute"", Direction.Out, ""TriggerCall"")]
    public ValueConnectionKnob callKnob;";
            }

            streff += "\n";

            streff += @"
    protected override void OnCreate()
    {

    }
}" + "\n\n";
            effNum++;
            if (effNum >= 15) { effPage++; effNum = 0; }
        }
        File.WriteAllText("I:\\StumpyTriggerscripterUnity\\Assets\\NewNodes\\EffectNode.cs", streff);
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Conditions
        int cndNum = 0;
        int cndPage = 0;
        List<Condition> cndSorted = conditions.Values.ToList().OrderBy(x => x.name).ToList();
        Console.WriteLine(cndSorted.Count);
        string strcnd =@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System;


class Condition_Type { }
public class Condition_Knob : ValueConnectionType
{
    public override string Identifier => ""Condition"";
    public override Type Type => typeof(Condition_Type);
    public override Color Color => Color.red;
}

";
        foreach (Condition c in cndSorted)
        {
            if (c.name == "") continue;
strcnd+=
@"[Node(false, ""Conditions/Page " + cndPage.ToString() + "/" + c.name + @""")]
class Condition" + c.name + @"Node : Node
{
    public const string ID = ""Condition" + c.name + @"Node"";
    public override string GetID { get { return ID; } }
    string title = ""[Condition] "" + """ + c.name + @""";
    public override string Title { get { return title; } }
    public override bool AutoLayout => true;

    [ValueConnectionKnob("""", Direction.Out, ""Condition"")]
    public ValueConnectionKnob knobOut_ToTrigger;
";

            foreach (Input i in c.inputs)
            {
                if (i.valueType == "Hook Type") i.valueType = "HookType";
                if (i.name == "Hook Type") i.name = "HookType";
                string op = ""; if (i.optional) op = "Optional";
                strcnd +=
@"    
    [ValueConnectionKnob(""[" + i.valueType + "] " + i.name + @""", Direction.In, ""Var" + i.valueType + op + @""")]
    public ValueConnectionKnob " + "knobIn_" + i.name + @";";
            }

            foreach (Output o in c.outputs)
            {
                string op = ""; if (o.optional) op = "Optional";
                strcnd +=
@"    
    [ValueConnectionKnob(""[" + o.valueType + "] " + o.name + @""", Direction.Out, ""Var" + o.valueType + op + @""")]
    public ValueConnectionKnob " + "knobOut_" + o.name + @";";
            }
            
            strcnd += "\n";
            strcnd += @"
    protected override void OnCreate()
    {

    }
}" + "\n\n";
            cndNum++;
            if (cndNum >= 15) { cndPage++; cndNum = 0; }
        }
        File.WriteAllText("I:\\StumpyTriggerscripterUnity\\Assets\\NewNodes\\ConditionNode.cs", strcnd);
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        Console.WriteLine("Functions Created.");
        goto Loop;
        */
        #endregion
    }
}