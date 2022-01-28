using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using Ionic.Zip;
using SMHEditor.DockingModules.ProjectExplorer;
using static SMHEditor.VFS.Node;

namespace SMHEditor.VFS
{
    public class Node
    {
        private string name;
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
        }
        public class FileNode : Node
        {
            Stream data;
            string onDiskName;
        }
    }




    public class VirtualZipFileSystem
    {
        static char PATH_SEPARATOR = '/';

        public VirtualZipFileSystem(string rootName)
        {
            root = new FolderNode();
            root.SetName(rootName);
        }

        private ZipFile fileStore;
        private FolderNode root;

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
