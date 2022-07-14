using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ComponentFactory.Krypton.Toolkit;
using Foundry.DockingModules.ProjectExplorer;
using System.Runtime.Serialization;
using System.Xml;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using Foundry.DockingModules.ObjectEditor;
using Foundry.Project.FileTypes;
using Foundry.DockingModules;
using YAXLib.Options;
using ComponentFactory.Krypton.Workspace;
using Foundry.DockingModules.MapEditor;
using Foundry.Project.FileTypes.Scripts;
using System.Windows.Forms;
using System.Drawing;
using Aga.Controls.Tree;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel;

namespace Foundry.Project
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
        public static string PROJ_EXT                   = ".hwfp";

        public static string DATA_DIR                   = "\\data";
        public static string DATA_SCRIPTS_DIR           = "\\data\\triggerscripts";
        public static string DATA_PREFABS_DIR           = "\\data\\triggerscripts\\prefabs";
        
        public static string ART_DIR                    = "\\art";

        public static Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>()
        {
            {"FILE", Properties.Resources.page_white},
            {"FOLDER", Properties.Resources.folder},
        };

        #region Project
        private string openedDir;
        private string openedFile;
        private ModProjectData projectData;
        private ModProjectContentFile activeFile;

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

            Program.window.projectExplorer.UpdateHierarchy(DirGetNodeGraph());
        }
        public void Save()
        { 
            YAXSerializer ser = new YAXSerializer(typeof(ModProjectData));
            ser.SerializeToFile(projectData, openedDir +"\\"+ projectData.name + PROJ_EXT);
        }
        
        public void SetActiveFile(ModProjectContentFile file)
        {
            activeFile = file;
        }
        public void SaveActiveFile()
        {
            if(activeFile != null)
                activeFile.SaveFile();
        }
        #endregion

        #region Explorer
        public class ModProjectContentFile
        {
            private string fileName;
            [Category("File"), Description("Location of the file on disk.")]
            public string PathOnDisk
            {
                get { return fileName; }
                set { fileName = value; }
            }
            private bool export;
            [Category("File"), Description("Include this file in the project export.")]
            public bool IncludeInExport
            {
                get { return export; }
                set { export = value; }
            }


            protected virtual void DoSave() { }
            protected virtual void DoOpen(string subName) { }
            public    virtual EntryNodeData GetRootNode()
            {
                EntryNodeData end = new EntryNodeData();
                end.Text = Path.GetFileName(fileName);
                end.Image = ModProject.images["FILE"];
                return end;
            }

            public ModProjectContentFile(string fileName)
            {
                this.fileName = fileName;
            }
            private bool markedEdited = false;
            public  void MarkEdited()
            {
                markedEdited = true;
            }
            public  void SaveFile()
            {
                if(markedEdited) DoSave();
                markedEdited = false;
            }
            public  void OpenFile(string subName)
            {
                DoOpen(subName);
            }
        }
        public class EntryNodeData : Node
        {
            private string _fullPath;
            public string FullPath
            {
                get { return _fullPath; }
                set { _fullPath = value; }
            }

            private string _subName;
            public string SubName
            {
                get { return _subName; }
                set { _subName = value; }
            }
        }
        
        private Dictionary<string, ModProjectContentFile>    allFiles            = new Dictionary<string, ModProjectContentFile>();
        private Dictionary<string, TriggerscriptContentFile> triggerscriptFiles  = new Dictionary<string, TriggerscriptContentFile>();
        private EntryNodeData LoadContentFile(string dir)
        {
            if (!File.Exists(dir)) return new EntryNodeData();
            
            ModProjectContentFile cf;

            switch(Path.GetExtension(dir))
            {
                case ".tsp":
                    cf = new TriggerscriptContentFile(dir);
                    triggerscriptFiles.Add(dir, (TriggerscriptContentFile)cf);
                    allFiles.Add(dir, cf);
                    break;
                default:
                    cf = new ModProjectContentFile(dir);
                    allFiles.Add(dir, cf);
                    break;
            }

            return cf.GetRootNode();
        }

        public IEnumerable<EntryNodeData>   DirGetNodeGraph()
        {
            List<EntryNodeData> roots = new List<EntryNodeData>();
            Dictionary<string, EntryNodeData> folders = new Dictionary<string, EntryNodeData>();

            foreach (string path in Directory.EnumerateDirectories(openedDir, "*", SearchOption.AllDirectories))
            {
                string[] entries = path.Substring(openedDir.Length + 1).Split('\\');

                string concat = "";
                EntryNodeData last = null;

                foreach (string e in entries)
                {
                    concat += "\\" + e;

                    if (!folders.ContainsKey(concat))
                    {
                        EntryNodeData n = new EntryNodeData();
                        n.Text = e;
                        n.Image = images["FOLDER"];
                        n.FullPath = concat;
                        if (last != null) last.Nodes.Add(n);
                        else roots.Add(n);
                        folders.Add(concat, n);
                        last = n;
                    }
                    else
                    {
                        last = folders[concat];
                    }
                }
            }
            foreach (var v in folders)
            {
                foreach(var f in Directory.EnumerateFiles(openedDir + v.Key))
                {
                    v.Value.Nodes.Add(LoadContentFile(f));
                }
            }

            return roots;
        }
        public void                         DirAddFolder(string localDirLeadingSlash)
        {
            string fullPath = openedDir + localDirLeadingSlash;

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
        public void                         DirAddFile  (ModProjectContentFile mpcf)
        {

        }
        public void                         DirOpenFile (string fileDir, string subName)
        {
            if (allFiles.ContainsKey(fileDir))
                allFiles[fileDir].OpenFile(subName);
        }
        #endregion
    }
}
