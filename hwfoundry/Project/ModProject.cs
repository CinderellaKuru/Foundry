using System;
using System.Collections.Generic;
using System.IO;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;
using System.Drawing;
using Aga.Controls.Tree;
using System.ComponentModel;
using Foundry.Project.Modules.Triggerscripter;

namespace Foundry.Project
{
    public class ModProjectData
    {
        public ModProjectData()
        {
            folderData = new Dictionary<string, FolderData>();
        }

        //folder data
        public class FolderData
        {
            public FolderData()
            {
                folded = true;
            }
            [YAXSerializeAs("Folded")]
            public bool folded { get; set; }
        }
        [YAXDictionary(EachPairName = "Folder", KeyName = "Name", ValueName = "Data", SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Element)]
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

        private ModProject() { }
        public static ModProject Create(string file)
        {
            ModProject mp = new ModProject();
            
            if (Path.GetExtension(file) != PROJ_EXT) throw new Exception("Selected path was not a " + PROJ_EXT + ".");

            mp.projectData = new ModProjectData();
            mp.openedDir = Path.GetDirectoryName(file);
            mp.openedFile = file;

            mp.Save();

            return mp;
        }
        public static ModProject Open(string file)
        {
            ModProject mp = new ModProject();

            if (Path.GetExtension(file) != PROJ_EXT) throw new Exception("Selected path was not a " + PROJ_EXT + ".");

            mp.openedDir = Path.GetDirectoryName(file);
            mp.openedFile = file;

            try
            {
                YAXSerializer ser = new YAXSerializer(typeof(ModProjectData));
                mp.projectData = (ModProjectData)ser.DeserializeFromFile(file);
            }
            catch
            {
                throw new Exception("Failed to parse project file.");
            }

            Program.window.projectExplorer.UpdateHierarchy(mp.DirGetNodeGraph());

            return mp;
        }
        public void Save()
        { 
            YAXSerializer ser = new YAXSerializer(typeof(ModProjectData));
            string serStr = ser.Serialize(projectData);
            File.WriteAllText(openedFile, serStr);
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
            protected virtual void DoImport(string fileName) { }
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
            public  void ImportFile(string fileName)
            {
                DoImport(fileName);
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
        public void                         DirAddFolder    (string localDirLeadingSlash)
        {
            string fullPath = openedDir + localDirLeadingSlash;

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
        }
        public void                         DirAddFile      (ModProjectContentFile mpcf)
        {

        }
        public void                         DirOpenFile     (string fileDir, string subName)
        {
            if (allFiles.ContainsKey(fileDir))
                allFiles[fileDir].OpenFile(subName);
        }
        public void                         DirSelectFile   (string fileDir)
        {
            if (allFiles.ContainsKey(fileDir))
                Program.window.propertyEditor.SetSelectedObject(allFiles[fileDir]);
        }
        #endregion
    }
}
