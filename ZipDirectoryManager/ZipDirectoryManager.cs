using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.DockingModules.ProjectExplorer;
using Newtonsoft.Json;
using Ionic.Zip;

namespace SMHEditor.ZipDir
{

    public class ZipDirectoryManager
    {
        class PhysicalNode
        {
            public string name;
            public List<PhysicalNode> children = new List<PhysicalNode>();
        }

        string typeID;
        public ZipDirectoryManager(string _typeID)
        {
            typeID = _typeID;

            vfsInfoFile = new ZipEntry();
            vfsInfoFile.FileName = "." + typeID;

            fileStore = new ZipFile();
            fileStore.AddEntry(vfsInfoFile);
        }

        string loadedDir;
        public void Open(string dir)
        {
            string name = Path.GetFileNameWithoutExtension(dir);

            if (File.Exists(dir))
            {
                if (!ZipFile.IsZipFile(dir)) throw new Exception("Selected file was not a zip file.");
                fileStore = ZipFile.Read(dir);
                
                //if it doesnt have an id file, its not correct.
                if (!fileStore.ContainsEntry("." + typeID)) throw new Exception("Selected zip did not have valid ID file.");
                else vfsInfoFile = fileStore.SelectEntries("." + typeID).First();

                loadedDir = dir;
				fileStore.
                Save(dir);
                WalkDirectory("");
            }
            else throw new Exception("Bad path.");
        }
        public void Save(string dir)
        {
            fileStore.Save(dir);
        }
		
		private PhysicalNode GetOrCreateNode(string dir, string name, Dictionary<string, PhysicalNode> nodes)
        {
			if(nodes.Keys.Contains(dir))
            {
                return nodes[dir];
            }
			else
            {
                PhysicalNode n = new PhysicalNode();
                n.name = name;
                nodes.Add(dir, n);
                return n;
            }
        }
		private PhysicalNode BuildNode(string[] path, int cur, Dictionary<string, PhysicalNode> nodes)
        {
            string concat = "";
            for (int i = 0; i <= cur; i++)
            {
                concat += "/" + path[i];
            }

            PhysicalNode n = GetOrCreateNode(concat, path[cur], nodes);
			if(cur < path.Count() - 1)
            {
                n.children.Add(BuildNode(path, cur + 1, nodes));
            }

            return n;
        }
        public void WalkDirectory(string dir)
        {
            List<string[]> entryNames = new List<string[]>();
            int max = 0;
            foreach (var e in fileStore.Entries)
            {
                string[] split = e.FileName.Split('/');
                if (split.Count() > max) max = split.Count();
                split = split.Where(s => s != "").ToArray();
                entryNames.Add(split);
            };

			foreach(string[] s in entryNames)
            {
                BuildNode(s, 0);
            }
        }

        
        ZipFile fileStore;
        ZipEntry vfsInfoFile;
    }



}
