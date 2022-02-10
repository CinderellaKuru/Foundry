using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.DockingModules.ProjectExplorer;
using System.Runtime.Serialization;
using System.Xml;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using SMHEditor.DockingModules.ObjectEditor;
using SMHEditor.Project.FileTypes;
using SMHEditor.DockingModules;
using YAXLib.Options;
using ComponentFactory.Krypton.Workspace;

namespace SMHEditor.Project
{
    public class ModProjectData
    {
        public ModProjectData()
        {
            folderData = new Dictionary<string, FolderData>();
        }

        // Project info
        [YAXAttributeForClass]
        public string name { get; set; }

        // Folder data
        public class FolderData
        {
            public FolderData()
            {
                folded = true;
            }
            [YAXSerializeAs("Folded")]
            public bool folded { get; set; }
        }
        [YAXDictionary(EachPairName = "Folder", KeyName = "Name", ValueName = "Data",
                   SerializeKeyAs = YAXNodeTypes.Attribute,
                   SerializeValueAs = YAXNodeTypes.Element)]
        [YAXSerializeAs("FolderData")]
        public Dictionary<string, FolderData> folderData { get; set; }
    }

    public class ModProject
    {
        public enum FileType
        {
            ROOT,
            FOLDER,
            OBJECT,
            SQUAD,
            TACTICS
        };
        public static Dictionary<string, FileType> extToType = new Dictionary<string, FileType>()
        {
            {".obj", FileType.OBJECT },
            {".sqd", FileType.SQUAD },
            {".tct", FileType.TACTICS }
        };
        public static Dictionary<FileType, string> typeToExt = new Dictionary<FileType, string>()
        {
            {FileType.OBJECT, ".obj" },
            {FileType.SQUAD, ".sqd" },
            {FileType.TACTICS, ".tct" }
        };
        public static Dictionary<FileType, string> typeToImage = new Dictionary<FileType, string>()
        {
            {FileType.FOLDER, "folder" },
            {FileType.OBJECT, "object" },
            {FileType.SQUAD, "squad" },
            {FileType.TACTICS, "tactics" }
        };
        public static string PROJ_EXT = ".hwproj";
        public static string DATA_DIR = "\\data";
        public static string ART_DIR = "\\art";

        // Project
        private string openedDir;
        private string openedFile;
        public ModProjectData projectData;
        
        public ModProject(string file)
        {
            if (Path.GetExtension(file) != PROJ_EXT) throw new Exception("Selected path was not a " + PROJ_EXT + ".");

            openedDir = Path.GetDirectoryName(file);
            openedFile = file;

            if (!File.Exists(file))
            {
                projectData = new ModProjectData();
                projectData.name = Path.GetFileNameWithoutExtension(file);
                Save();
            }
            else
            {
                try
                {
                    YAXSerializer ser = new YAXSerializer(typeof(ModProjectData));
                    projectData = (ModProjectData)ser.DeserializeFromFile(file);
                }
                catch
                {
                    throw new Exception("Failed to parse project file.");
                }
            }

            if (!Directory.Exists(openedDir + DATA_DIR))
                Directory.CreateDirectory(openedDir + DATA_DIR);
            if (!Directory.Exists(openedDir + ART_DIR))
                Directory.CreateDirectory(openedDir + ART_DIR);
        }
        public void Save()
        { 
            YAXSerializer ser = new YAXSerializer(typeof(ModProjectData));
            ser.SerializeToFile(projectData, openedDir +"\\"+ projectData.name + PROJ_EXT);
        }

        //Explorer
        public KryptonTreeNode GetTreeView(string fromDir, string name)
        {
            KryptonTreeNode root = new KryptonTreeNode();
            root.Text = name;
            root.Name = fromDir;
            root.ImageIndex = ProjectExplorerControl.ImageIndex["data_dir"];
            root.SelectedImageIndex = ProjectExplorerControl.ImageIndex["data_dir"];
            root.Tag = FileType.ROOT;

            Dictionary<string, KryptonTreeNode> nodes = new Dictionary<string, KryptonTreeNode>();

            if (!projectData.folderData.Keys.Contains(fromDir))
                projectData.folderData.Add(fromDir, new ModProjectData.FolderData());

            //Folders
            foreach (var p in Directory.GetDirectories(openedDir + fromDir))
            {
                string local = p.Substring((openedDir + fromDir).Length);
                string[] spl = local.Split('\\').Where(e => e != "").ToArray();

                string concat = fromDir;
                KryptonTreeNode last = root;
                foreach (string s in spl)
                {
                    concat += "\\" + s;

                    //get existing or create a new node for the next part of the path.
                    if (nodes.Keys.Contains(concat))
                    {
                        if (!last.Nodes.Contains(nodes[concat]))
                        {
                            last.Nodes.Add(nodes[concat]);
                        }

                        //fold or unfold?
                        if (projectData.folderData[concat].folded) nodes[concat].Expand();
                        else nodes[concat].Collapse();

                        last = nodes[concat];
                    }
                    else
                    {
                        KryptonTreeNode newNode = new KryptonTreeNode();
                        newNode.Name = concat;
                        newNode.Text = s;
                        nodes.Add(concat, newNode);
                        newNode.Tag = FileType.FOLDER;
                        newNode.ImageIndex = ProjectExplorerControl.ImageIndex["folder"];
                        newNode.SelectedImageIndex = ProjectExplorerControl.ImageIndex["folder"];
                        newNode.Expand();

                        if (!projectData.folderData.Keys.Contains(concat))
                            projectData.folderData.Add(concat, new ModProjectData.FolderData());

                        //fold or unfold?
                        if (projectData.folderData[concat].folded) nodes[concat].Expand();
                        else nodes[concat].Collapse();

                        last.Nodes.Add(newNode);
                        last = newNode;
                    }
                }
            }

            //files
            foreach (var f in Directory.GetFiles(openedDir + fromDir, "*", SearchOption.AllDirectories))
            {
                string parent = Path.GetDirectoryName(f).Substring((openedDir + fromDir).Length);
                KryptonTreeNode fn = new KryptonTreeNode(Path.GetFileName(f));

                string ext = Path.GetExtension(f);
                if (extToType.Keys.Contains(ext))
                {
                    fn.Tag = extToType[ext];
                }
                else continue;

                fn.Name = fromDir + parent + "\\" + fn.Text;
                fn.ImageIndex = ProjectExplorerControl.ImageIndex[typeToImage[(FileType)fn.Tag]];
                fn.SelectedImageIndex = ProjectExplorerControl.ImageIndex[typeToImage[(FileType)fn.Tag]];

                nodes[fromDir + parent].Nodes.Add(fn);
            }
            return root;
        }

        public void OpenFile(string fileDir)
        {
            string ext = Path.GetExtension(fileDir);
            if (!extToType.Keys.Contains(ext)) return; //is not a recognized file type.

            string name = Path.GetFileNameWithoutExtension(fileDir);
            EditorPage page;

            // already open
            if (openPages.Keys.Contains(fileDir))
            {
                page = openPages[fileDir];
            }

            // not open, open it
            else
            {
                if (ext == typeToExt[FileType.OBJECT])
                {
                    ObjectFile obj;
                    try
                    {
                        var op = new SerializerOptions();
                        op.SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects;
                        YAXSerializer ser = new YAXSerializer(typeof(ObjectFile), op);
                        obj = (ObjectFile)ser.DeserializeFromFile(openedDir + fileDir);
                    }
                    catch
                    {
                        Console.WriteLine("Could not deserialize " + fileDir + ".");
                        return;
                    }
                    page = new ObjectEditorPage(obj, fileDir, name);
                    Program.window.dockingManager.AddToWorkspace(MainWindow.WORKSPACE_NAME,
                        new ComponentFactory.Krypton.Navigator.KryptonPage[] { page });
                }
                else page = null;
            }

            // set focus on this file
            if (page.KryptonParentContainer is KryptonWorkspaceCell)
            {
                if (!page.Visible) page.Visible = true;
                ((KryptonWorkspaceCell)page.KryptonParentContainer).SelectedPage = page;
            }
            // parent was not a cell. what could it be?
            else throw new Exception("Look into this.");
        }
        public void SaveFile(string fileDir)
        {

        }

        public void Create(string folderDir, string nameNoExt, FileType type)
        {
            if(type == FileType.FOLDER)
            {
                Directory.CreateDirectory(folderDir + "\\" + nameNoExt);
            }

            if(type == FileType.OBJECT)
            {
                //File.Create()
            }
        }

        private void RenameNodeRecursive(KryptonTreeNode n, string prevPath, string newPath)
        {
            string suffix = n.Name.Substring(n.Name.IndexOf(prevPath) + prevPath.Length);
            string newName = newPath + suffix;
            n.Name = newName;

            foreach(KryptonTreeNode tn in n.Nodes)
            {
                RenameNodeRecursive(tn, prevPath, newPath);
            }
        }
        public  void RenameNode(KryptonTreeNode n, string newLabel)
        {
            string prevName = n.Name;
            string parentName = n.Name.Substring(0, n.Name.LastIndexOf("\\") + 1);

            if((FileType)n.Tag == FileType.FOLDER)
            {
                RenameNodeRecursive(n, prevName, parentName + newLabel);
                n.Text = newLabel;
                Directory.Move(openedDir + prevName, openedDir + n.Name);
            }
            else
            {
                File.Move(openedDir + prevName, openedDir + n.Name);
            }
        }

        //Workspace
        public Dictionary<string, EditorPage> openPages = new Dictionary<string, EditorPage>();
        public EditorPage activePage;
    }
}
