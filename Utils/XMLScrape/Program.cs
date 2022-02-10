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
            string xmlFile = "C:/users/jaken/desktop/objects.xsd";
            
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);

            Dictionary<string, List<string>> tags = new Dictionary<string, List<string>>();
            foreach (XmlNode n in doc.ChildNodes[1].ChildNodes)
            {
                foreach (XmlNode c in n.ChildNodes)
                {
                    if (!tags.Keys.Contains(c.Name)) tags.Add(c.Name, new List<string>());
                    foreach (XmlAttribute a in c.Attributes)
                    {
                        if (!tags[c.Name].Contains(a.Name))
                            tags[c.Name].Add(a.Name);
                    }
                }
            }
            FileStream fs = File.Create("C:/users/jaken/desktop/out.txt");
            foreach (var p in tags)
            {
                fs.Write(Encoding.UTF8.GetBytes(p.Key + "\n"), 0, (p.Key + "\n").Length);
                foreach (string a in p.Value)
                {
                    fs.Write(Encoding.UTF8.GetBytes(a + "\n"), 0, (a + "\n").Length);
                }
                fs.Write(Encoding.UTF8.GetBytes("\n"), 0, 1);
            }
        }
    }
}
