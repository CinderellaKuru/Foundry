using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using Ionic.Zip;
using SMHEditor.DockingModules.ProjectExplorer;
using Newtonsoft.Json;
using static SMHEditor.VFS.Node;
using static SMHEditor.VFS.Node.FolderNode;

namespace SMHEditor.VFS
{
    public class Node
    {
        public string name;
        private Node parent;
        private List<Node> children = new List<Node>();
        
        public Node FindChildWithName(string _name)
        {
            bool found = false;
            foreach (Node n in children)
            {
                if (n.name == _name)
                {
                    found = true;
                    return n;
                }
            }

            if (found == false) throw new FileNotFoundException(_name + " was not found in " + this.name + ".");
            return null;
        }
        public FolderNode AddNewChildFolder(string _name)
        {
            FolderNode f = new FolderNode();
            f.name = _name;
            f.parent = this;
            children.Add(f);
            return f;
        }
        public FolderNode AddNewChildFolder(SerializedFolder ser)
        {
            FolderNode f = new FolderNode();
            f.name = ser.name;
            f.parent = this;
            children.Add(f);
            return f;
        }

        public void SetName(string newName) { name = newName; }

        protected string iconName;
        public KryptonTreeNode GetTreeView()
        {
            KryptonTreeNode tn = new KryptonTreeNode();
            tn.ImageIndex = ProjectExplorerControl.ImageIndex[iconName];
            tn.SelectedImageIndex = ProjectExplorerControl.ImageIndex[iconName];
            tn.Text = name;
            tn.Name = name;
            foreach(Node n in children)
            {
                tn.Nodes.Add(n.GetTreeView());
            }
            return tn;
        }

        public class FolderNode : Node
        {
            public FolderNode()
            {
                iconName = "folder";
            }
            public void AddExistingFileToThis(FileNode file)
            {
                file.parent.children.Remove(file);
                file.parent = this;
                children.Add(this);
            }

            public SerializedFolder Serialize()
            {
                SerializedFolder sf = new SerializedFolder();
                sf.name = name;
                foreach(var c in children)
                {
                    if(c is FolderNode)
                    {
                        sf.children.Add(((FolderNode)c).Serialize());
                    }
                    if(c is FileNode)
                    {
                        SerializedFolder.SerializedFile sfn = new SerializedFolder.SerializedFile();
                        sfn.name = c.name;
                        sfn.diskName = ((FileNode)c).onDiskName;
                        sf.childFiles.Add(sfn);
                    }
                }
                return sf;
            }
            public void ApplySerialization(SerializedFolder ser)
            {
                foreach(var v in ser.children)
                {
                    FolderNode n = new FolderNode();
                    AddNewChildFolder(ser).ApplySerialization(ser);
                }
                foreach(var v in ser.childFiles)
                {
                    FileNode fn = new FileNode();
                    fn.name = v.name;
                    fn.parent = this;
                    fn.onDiskName = v.diskName;
                    children.Add(fn);
                }
            }
            public class SerializedFolder
            {
                public class SerializedFile { public string name; public string diskName; }
                public string name;
                public List<SerializedFolder> children = new List<SerializedFolder>();
                public List<SerializedFile> childFiles = new List<SerializedFile>();
            }
        }
        public class FileNode : Node
        {
            public string onDiskName;
            public object data;
        }
    }




    public class VirtualZipFileSystem
    {
        static char PATH_SEPARATOR = '/';

        string vfsID;
        public VirtualZipFileSystem(string _vfsID)
        {
            root = new FolderNode();
            vfsID = _vfsID;
            root.SetName(vfsID + " [None]");

            vfsInfoFile = new ZipEntry();
            vfsInfoFile.FileName = "." + _vfsID;

            fileStore = new ZipFile();
            fileStore.AddEntry(vfsInfoFile);
            //root.AddNewChildFolder("folder 1").AddNewChildFolder("folder 2");
        }

        string loadedDir;
        public void Open(string dir)
        {
            string name = Path.GetFileNameWithoutExtension(dir);
            root.SetName(name);

            if (File.Exists(dir))
            {
                if (!ZipFile.IsZipFile(dir)) throw new Exception("Selected file was not a zip file.");
                fileStore = ZipFile.Read(dir);
                
                //if it doesnt have an id file, its not correct.
                if (!fileStore.ContainsEntry("." + vfsID)) throw new Exception("Selected zip was not a valid file system.");
                else vfsInfoFile = fileStore.SelectEntries("." + vfsID).First();

                loadedDir = dir;
                Save(dir);
                ApplyFileSystem();
                WalkFS();
            }
            else throw new Exception("Bad path.");
        }
        public void Save(string dir)
        {
            UpdateFileSystem();
            
            fileStore.Save(dir);
        }
        public void UpdateFileSystem()
        {
            fileStore.UpdateEntry("." + vfsID,
                JsonConvert.SerializeObject(root.Serialize()));
        }
        public void ApplyFileSystem()
        {
            byte[] buff = new byte[vfsInfoFile.Info.Length];
            vfsInfoFile.OpenReader().Read(buff, 0, vfsInfoFile.Info.Length);
            root.ApplySerialization(JsonConvert.DeserializeObject<SerializedFolder>(System.Text.Encoding.UTF8.GetString(buff)));
        }

        public void WalkFS()
        {
            List<string[]> entryNames = new List<string[]>();
            int max = 0;
            foreach (var e in fileStore.Entries)
            {
                string[] split = e.FileName.Split('/');
                entryNames.Add(split);
                if (split.Count() > max) max = split.Count();
            };
            
            for(int i = 0; i < max; i++)
            {
                foreach(string[] s in entryNames)
                {
                    if(s.Count() > i)
                    {

                    }
                }
            }
        }


        private ZipFile fileStore;
        private FolderNode root;
        ZipEntry vfsInfoFile;

        public Node Root() { return root; }
        public Node GetByPath(string path)
        {
            string[] split = path.Split(PATH_SEPARATOR);
            Node current = root;
            foreach(string s in split)
            {
                current = (FolderNode)current.FindChildWithName(s);
            }
            return current;
        }
        public KryptonTreeNode GetTreeView()
        {
            return root.GetTreeView();
        }
    }



}
