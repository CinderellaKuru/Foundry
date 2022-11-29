using Aga.Controls.Tree;
using hwFoundry.Modules.TriggerScripter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;

namespace hwFoundry.Project
{
    public class ModProject
    {
        #region Constants
        public static string PROJ_EXT = ".hwfp";

        public static string ART_DIR = @"\art";
        public static string DATA_DIR = @"\data";
        public static string DATA_SCRIPTS_DIR = @$"{DATA_DIR}\triggerscripts";
        public static string DATA_PREFABS_DIR = @$"{DATA_SCRIPTS_DIR}\prefabs";
        #endregion

        private ModProject() { }

        #region Project Management
        public ContentFile activeFile;
        private string? openedDir, openedFile;
        private SerializableModProject? projectData;
        private Dictionary<string, ContentFile> allFiles = new();
        private Dictionary<string, TriggerScriptFile> triggerScriptFiles = new();
        
        #region Project Management
        public static ModProject OpenProject(string file)
        {
            // Secondary extension check
            if (Path.GetExtension(file) != PROJ_EXT)
                throw new Exception($"Selected path does not lead to a project file (*{PROJ_EXT}).");

            // Make a new ModProject object and set directory data
            ModProject modProject = new()
            {
                openedDir = Path.GetDirectoryName(file),
                openedFile = file
            };

            // Try to deserialize the mod project
            try
            {
                YAXSerializer ser = new(typeof(SerializableModProject));
                modProject.projectData = (SerializableModProject)ser.DeserializeFromFile(file);
            }
            catch { throw new Exception("Failed to parse project file! Malformed project file!"); }

            // Populate the visual hierarchy with included items
            Program.mainWindow.projectExplorer.UpdateProjectHierarchy(modProject.LoadProjectHierarchy());

            return modProject;
        }

        public static ModProject CreateProject(string filePath)
        {
            // Check filepath extension
            if (Path.GetExtension(filePath) != PROJ_EXT)
                throw new Exception($"Selected path was not a {PROJ_EXT}.");

            // Create the new project
            ModProject mp = new()
            {
                projectData = new(),
                openedDir = Path.GetDirectoryName(filePath),
                openedFile = filePath
            };

            // Save the project
            mp.SaveProject();
            return mp;
        }

        public void SaveProject()
        {
            YAXSerializer ser = new(typeof(SerializableModProject));
            string serStr = ser.Serialize(projectData);
            File.WriteAllText(openedFile, serStr);
        }
        #endregion

        #region Utility
        private IEnumerable<EntryNode> LoadProjectHierarchy()
        {
            // Temporary Containers
            List<EntryNode> roots = new();
            Dictionary<string, EntryNode> folders = new();

            // Go through all folders in project dir
            foreach (string path in Directory.EnumerateDirectories(openedDir, "*", SearchOption.AllDirectories))
            {
                // Containers
                StringBuilder concat = new();
                EntryNode? lastNode = null;

                // Split directory paths into substrings
                string[] entries = path.Substring(openedDir.Length + 1).Split("\\");

                // Go through each entry and add a
                // new folder to the treeview if one doesn't exist
                foreach (string entry in entries)
                {
                    // Append folder name and determine if
                    // the new folder path needs to be made
                    concat.Append($@"\{entry}");
                    if (!folders.ContainsKey(concat.ToString()))
                    {
                        // The folder doesn't exist, add a new folder node
                        EntryNode tempNode = new()
                        {
                            Text = entry,
                            Image = Properties.Resources.folder,
                            FullPath = concat.ToString()
                        };

                        // Check if this is the first folder of the directory tree
                        if (lastNode != null) lastNode.Nodes.Add(tempNode);
                        else roots.Add(tempNode);

                        // Add the folder to the hierarchy, track the last appended folder
                        folders.Add(concat.ToString(), tempNode);
                        lastNode = tempNode;
                        continue;
                    }

                    // The folder exists, don't add a new one
                    lastNode = folders[concat.ToString()];
                }
            }

            // Iterate through each folder found, load each file's content
            foreach (var folder in folders)
                foreach (string file in Directory.EnumerateFiles(openedDir + folder.Key))
                    folder.Value.Nodes.Add(LoadContentFile(file));

            // Return all roots of the directory tree
            return roots;
        }

        private EntryNode LoadContentFile(string filepath)
        {
            // If an invalid directory is attempting to be loaded, return an empty EntryNodeData obj
            if (!File.Exists(filepath))
                return new EntryNode();

            // Figure out what kind of content file we're reading
            ContentFile cf;
            switch (Path.GetExtension(filepath))
            {
                // Trigger Scripts
                case ".tsp":
                    cf = new TriggerScriptFile(filepath);
                    triggerScriptFiles.Add(filepath, (TriggerScriptFile)cf);
                    allFiles.Add(filepath, cf);
                    break;

                // All other files
                default:
                    cf = new ContentFile(filepath);
                    allFiles.Add(filepath, cf);
                    break;
            }

            // Return the root node 
            return cf.GetRootNode();
        }

        internal void DirSelectFile(string fullPath)
            => Program.mainWindow.propertyEditor.SetSelectedObject(allFiles.ContainsKey(fullPath) ? allFiles[fullPath] : null);

        internal void DirOpenFile(string fullPath, string subName)
        {
            if (allFiles.ContainsKey(fullPath))
                allFiles[fullPath].OpenFile(subName);
        }

        internal void SetActiveFile(TriggerScriptFile triggerScriptFile)
            => activeFile = triggerScriptFile;

        internal void SaveActiveFile()
        {
            if (activeFile != null)
                activeFile.SaveFile();
        }
        #endregion

        #endregion
    }
}
