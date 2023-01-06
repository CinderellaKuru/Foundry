using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

static class ClassConstructor
{
    public static void Stitch()
    {
        List<string> vars = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText("out\\var.json"));
        string final = "";
        final += "public class VarNode { public int id; }\n";
        foreach (string s in vars)
        {
            final += "public class " + s + "Var : VarNode { };\n";
        }

        List<Effect> effs = JsonConvert.DeserializeObject<List<Effect>>(File.ReadAllText("out\\eff.json"));
        List<Condition> cnds = JsonConvert.DeserializeObject<List<Condition>>(File.ReadAllText("out\\cnd.json"));

        foreach (Effect e in effs)
        {
            if (e.name.Contains("Debug")) continue;

            final +=
"[Node(name:\"" + e.name + "\", menu:\"Debug\")]" +
"public void " + e.name + "Effect(";
            int ind = 0;
            foreach (Input inp in e.inputs)
            {
                final += inp.valueType + "Var inVar"+ inp.name;
                if (ind < e.inputs.Count -1) final += ", ";
                if (ind == e.inputs.Count -1 && e.outputs.Count > 0) final += ", ";
                ind++;
            }
            ind = 0;
            foreach (Output outp in e.outputs)
            {
                final += outp.valueType + "Var outVar" + outp.name;
                if (ind < e.outputs.Count -1) final += ", ";
                ind++;
            }
            final += 
@") {

}
";
        }

        File.WriteAllText("out\\out.txt", final);
    }
}
