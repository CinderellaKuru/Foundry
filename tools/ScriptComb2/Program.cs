
using System.Text.RegularExpressions;
using YAXLib;
using YAXLib.Attributes;
using static foundry.triggerscript.TriggerscriptModule;
using static Program;

class Program
{
    public static void Main(string[] args)
    {
        List<Item> effects = new List<Item>();
        effects.AddRange(GetItems(File.ReadAllText("R:\\foundry\\tools\\ScriptComb2\\triggereffect.cpp"), "void BTriggerEffect::te"));
        effects.AddRange(GetItems(File.ReadAllText("R:\\foundry\\tools\\ScriptComb2\\triggereffectai.cpp"), "void BTriggerEffect::te"));
        effects.AddRange(GetItems(File.ReadAllText("R:\\foundry\\tools\\ScriptComb2\\triggereffectcopy.cpp"), "void BTriggerEffect::te"));

        List<Item> conditions = new List<Item>();
        conditions.AddRange(GetItems(File.ReadAllText("R:\\foundry\\tools\\ScriptComb2\\triggercondition.cpp"), "bool BTriggerCondition::tc"));

        YAXSerializer ser = new YAXSerializer(typeof(List<Item>));
        ser.SerializeToFile(effects, "../../effects.tsdef");
        ser.SerializeToFile(conditions, "../../conditions.tsdef");
    }

    public static List<Item> GetItems(string file, string prefix)
    {
        List<Item> items = new List<Item>();

        int offset = 0;
        while (file.IndexOf(prefix, offset) != -1)
        {
            offset = file.IndexOf(prefix, offset);
            offset += prefix.Length;
            string name = file.Substring(offset, file.IndexOf("()", offset) - offset);
            offset = file.IndexOf("{", offset) + 1;
            

            int start = offset;
            int len = 0;
            int opens = 1;
            while (opens != 0)
            {
                if (file[offset] == '{') opens++;
                if (file[offset] == '}') opens--;

                offset++;
                len++;
            }

            string body = file.Substring(start, len);

            Match match = Regex.Match(body, @"enum[\s]*[\n]*{([\s\S]*)};");
            if (match.Success)
            {
                Item item = new Item() { Name = name };

                string enumstr = Regex.Replace(match.Groups[1].Value, @"[^\w=,]*", "");
                string[] enums = enumstr.Split(",");
                int tracker = 0;
                foreach (string e in enums)
                {
                    if (e == "") continue;

                    string[] keyval = e.Split("=");

                    string pname = keyval[0];
                    int pid = keyval.Length > 1 ? int.Parse(keyval[1]) : tracker;
                    tracker = pid + 1;

                    bool poptional = Regex.Match(body, string.Format("{0}\\)->isUsed", pname)).Success;
                    bool poutput = Regex.Match(body, string.Format("{0}\\)->as[\\w]*\\(\\)->writeVar", pname)).Success;

                    item.Params.Add(new Item.Param()
                    {
                        Name = pname.Substring(1),
                        ID = pid,
                        Optional = poptional,
                        Output = poutput
                    });
                }

                items.Add(item);
            }
        }

        return items;
    }
}