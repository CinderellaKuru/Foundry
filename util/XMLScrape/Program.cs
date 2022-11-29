using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Text;

namespace XMLScrape
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlFile = "D:\\StumpyHWDEMod\\Lom1.6.2\\data\\objects.xml";
            
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);

            Dictionary<string, List<string>> tags = new Dictionary<string, List<string>>();
            List<string> flags = new List<string>();
            foreach (XmlNode n in doc.ChildNodes[1].ChildNodes)
            {
                foreach (XmlNode c in n.ChildNodes)
                {
                    //if (!tags.Keys.Contains(c.Name)) tags.Add(c.Name, new List<string>());
                    //foreach (XmlAttribute a in c.Attributes)
                    //{
                    //    if (!tags[c.Name].Contains(a.Name))
                    //        tags[c.Name].Add(a.Name);
                    //}
                    if(c.Name == "Flag")
                    {
                        if(c.InnerText != null)
                            if(!flags.Contains(c.InnerText))
                                flags.Add(c.InnerText);
                    }
                }
            }
            FileStream fs = File.Create("C:/users/jaken/desktop/out.txt");
            foreach (var p in flags)
            {
                fs.Write(Encoding.UTF8.GetBytes(p + '\n'), 0, p.Length + 1);
                //fs.Write(Encoding.UTF8.GetBytes(p.Key + "\n"), 0, (p.Key + "\n").Length);
                //foreach (string a in p.Value)
                //{
                //    fs.Write(Encoding.UTF8.GetBytes(a + "\n"), 0, (a + "\n").Length);
                //}
                //fs.Write(Encoding.UTF8.GetBytes("\n"), 0, 1);
            }
        }
    }
}
