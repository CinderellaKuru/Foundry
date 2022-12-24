using System;
using System.Windows.Forms;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;
using Foundry.Project.Modules.ScenarioEditor;
using Foundry.Project.Modules.Triggerscripter;
using System.Collections.Generic;
using System.IO;
using YAXLib.Attributes;
using YAXLib.Enums;
using YAXLib;
using System.Drawing;
using System.Linq;
using Foundry.Project.Modules;

namespace Foundry
{
    public partial class FoundryInstance : Form
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // foundry instance
        public FoundryInstance()
        {
            InitializeComponent();
            versionReadout.Text = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
            workspace.Theme = new VS2015LightTheme();
        }
        private void FoundryInstance_Load(object sender, EventArgs e)
        {
            AddProjectExplorer(workspace, DockState.DockLeft);

            ProjectOpen("workingProject/workingproj.fproject");
        }
        private void ToolStripMenu_OpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ProjectOpen(ofd.FileName);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // pages
        public class FoundryPage : DockContent
        {
            private string fileName = null;
            private FoundryInstance instance;
            private enum Status
            {
                Closed,
                Opened,
                Opened_Edited,
            }
            private Status status;

            public FoundryPage(FoundryInstance i)
            {
                instance = i;
                status = Status.Closed;

                mouseState = new MouseState();
                downKeys = new List<Keys>();

                GotFocus += new EventHandler(Internal_GotFocus);
                LostFocus += new EventHandler(Internal_LostFocus);

                MouseMove += new MouseEventHandler(Internal_MouseMoved);
                MouseWheel += new MouseEventHandler(Internal_MouseWheelMoved);
                MouseDown += new MouseEventHandler(Internal_MouseButtonDown);
                MouseUp += new MouseEventHandler(Internal_MouseButtonUp);
            }
            public FoundryPage(FoundryInstance i, string file) : this(i)
            {
                fileName = file;
            }


            //external file interactions
            public bool TrySetEdited()
            {
                if (fileName != null)
                {
                    if (status == Status.Opened)
                    {
                        status = Status.Opened_Edited;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            public bool TryOpen(DockPanel workspace, DockState state)
            {
                if (status == Status.Closed)
                {
                    OnOpen();

                    Show(workspace, state);

                    GotFocus += new EventHandler(Internal_GotFocus);
                    LostFocus += new EventHandler(Internal_LostFocus);

                    status = Status.Opened;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public bool TryClose()
            {
                if (status == Status.Opened)
                {
                    Close();
                    return true;
                }
                else if (status == Status.Opened_Edited)
                {
                    DialogResult r = MessageBox.Show("Are you sure you want to close this file? Any unsaved progress will be lost.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (r == DialogResult.Yes)
                    {
                        Close();
                        status = Status.Closed;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            public bool TrySave(string file)
            {
                if (fileName != null)
                {
                    if (status == Status.Opened_Edited)
                    {
                        OnSave(fileName);
                        status = Status.Opened;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }


            //internal page callbacks
            private void Internal_GotFocus(object o, EventArgs e)
            {

            }
            private void Internal_LostFocus(object o, EventArgs e)
            {

            }

            protected struct MouseState
            {
                public bool leftDown, rightDown;
                public int mX, mY;
                public int deltaX, deltaY, deltaScroll;
            }
            MouseState mouseState;
            private void Internal_MouseMoved(object o, MouseEventArgs e)
            {
                mouseState.deltaScroll = 0;
                mouseState.deltaX = mouseState.mX - e.X;
                mouseState.deltaY = mouseState.mY - e.Y;
                mouseState.mX = e.X;
                mouseState.mY = e.Y;
                OnInput();
            }
            private void Internal_MouseWheelMoved(object o, MouseEventArgs e)
            {
                mouseState.deltaScroll = e.Delta;
                OnInput();
            }
            private void Internal_MouseButtonDown(object o, MouseEventArgs e)
            {
                mouseState.deltaScroll = 0;
                if (e.Button == MouseButtons.Left)
                {
                    mouseState.leftDown = true;
                }
                if (e.Button == MouseButtons.Right)
                {
                    mouseState.rightDown = true;
                }
                OnInput();
            }
            private void Internal_MouseButtonUp(object o, MouseEventArgs e)
            {
                mouseState.deltaScroll = 0;
                if (e.Button == MouseButtons.Left)
                {
                    mouseState.leftDown = false;
                }
                if (e.Button == MouseButtons.Right)
                {
                    mouseState.rightDown = false;
                }
                OnInput();
            }
            private List<Keys> downKeys;
            private void Internal_KeyDown(object o, KeyEventArgs e)
            {
                if (!downKeys.Contains(e.KeyCode))
                    downKeys.Add(e.KeyCode);
                OnInput();
            }
            private void Internal_KeyUp(object o, KeyEventArgs e)
            {
                if (downKeys.Contains(e.KeyCode))
                    downKeys.RemoveAll(x => x == e.KeyCode);
                OnInput();
            }


            //protected getters
            protected FoundryInstance Instance()
            {
                return instance;
            }
            protected MouseState GetMouseState()
            {
                return mouseState;
            }
            protected bool GetKeyIsDown(Keys k)
            {
                return downKeys.Contains(k);
            }


            //page overridable functions
            protected virtual void OnOpen() { }
            protected virtual void OnSave(string file) { }
            protected virtual void OnInput() { }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // log
        public enum LogEntryType
        {
            Info,
            Warning,
            Error,
        }
        public void AppendLog(LogEntryType type, string message, bool displayAsStatus, string secondaryMessage = "")
        {
            string prefix;
            switch(type)
            {
                case LogEntryType.Info:
                    prefix = "[Info] ";
                    break;
                case LogEntryType.Warning:
                    prefix = "[Warning] ";
                    break;
                case LogEntryType.Error:
                    prefix = "[Error] ";
                    break;
                default:
                    prefix = "";
                    break;
            }
            string print = prefix + message;

            if(displayAsStatus)
                logStatus.Text = print;

            Console.WriteLine(print);
            if(secondaryMessage != "")
            {
                Console.WriteLine(secondaryMessage);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // property editors
        private List<PropertyEditor> propertyEditors = new List<PropertyEditor>();
        public void AddPropertyEditor(DockState state)
        {
            PropertyEditor pe = new PropertyEditor();
            propertyEditors.Add(pe);
            pe.Show(workspace, DockState.Float);
        }
        public void SetSelectedObject(object o)
        {

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // projectfiles
        private const string FOUNDRYPROJECT_EXT = ".fproject";
        private const string FOUNDRYTRIGGERSCRIPT_EXT = ".fts";
        private const string FOUNDRYSCENARIO_EXT = ".fsc";

        private class ProjectData
        {
            public ProjectData()
            {
            }

            //folder data
            public class FolderDataEntry
            {
                public FolderDataEntry()
                {
                    folded = true;
                }
                [YAXSerializeAs("Folded")]
                public bool folded { get; set; }
            }
            [YAXDictionary(EachPairName = "Folder", KeyName = "Name", ValueName = "Data", SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Element)]
            [YAXSerializeAs("FolderData")]
            public Dictionary<string, FolderDataEntry> FolderData { get; set; }
        }
        private ProjectData openedData;
        private string openedDir;
        private string openedFile;
        private string openedName;
        private bool projectOpened = false;


        private Dictionary<string, ContentFile> files = new Dictionary<string, ContentFile>();


        public void ProjectCreate(string file)
        {
            //TODO
        }
        public void ProjectOpen(string file)
        {
            if (projectOpened)
                ProjectClose();
            projectOpened = true;

            if (!File.Exists(file))
                return;

            if (Path.GetExtension(file) != FOUNDRYPROJECT_EXT)
                return;

            openedDir = Path.GetDirectoryName(file);
            openedFile = Path.GetFullPath(file);
            openedName = Path.GetFileNameWithoutExtension(file);

            try
            {
                YAXSerializer ser = new YAXSerializer(typeof(ProjectData));
                openedData = (ProjectData)ser.DeserializeFromFile(openedFile);
            }
            catch(Exception e)
            {
                ProjectClose();
                AppendLog(LogEntryType.Error, "Project '" + openedName + "' failed to load. Open the log for more info.", true, e.Message);
                return;
            }

            UpdateAllProjectExplorers(UpdateContent());

            AppendLog(LogEntryType.Info, "Project '" + openedName + "' loaded.", true);
        }
        public void ProjectSave()
        {
            YAXSerializer ser = new YAXSerializer(typeof(ProjectData));
            string serStr = ser.Serialize(openedData);
            File.WriteAllText(openedFile, serStr);
        }
        public void ProjectClose()
        {
            ContentFileCloseAll();
            ClearAllProjectExplorers();
            files.Clear();
            projectOpened = false;
            openedDir = null;
            openedFile = null;
            openedName = null;
        }


        public struct DiskEntryNode
        {
            public DiskEntryNode(string name, string path, Image icon)
            {
                _name = name;
                _path = path;
                _icon = icon;
                _children = new List<DiskEntryNode>();
            }
            public string _name;
            public string _path;
            public Image _icon;
            public List<DiskEntryNode> _children;
        }
        private void AddChildrenRecursive(DiskEntryNode node, List<string> foundFiles)
        {
            if (File.GetAttributes(node._path).HasFlag(FileAttributes.Directory))
            {
                //Get child folders first
                foreach (string dir in Directory.GetDirectories(node._path))
                {
                    DiskEntryNode child = new DiskEntryNode(Path.GetDirectoryName(dir), dir, Properties.Resources.folder);
                    node._children.Add(child);
                    AddChildrenRecursive(child, foundFiles);
                }

                //Get child files second
                foreach (string file in Directory.GetFiles(node._path))
                {
                    Image image = Properties.Resources.page_white;

                    //If this file is not represented by a ContentFile, add it.
                    //We need to probe the files extension here already for the nodes image, so we create the ContentFile while were here.
                    if (!ContentFileExists(file))
                    {
                        switch (Path.GetExtension(file))
                        {
                            case FOUNDRYPROJECT_EXT:
                                //TODO: add content file for ProjectData
                                break;
                            case FOUNDRYTRIGGERSCRIPT_EXT:
                                files.Add(file, new TriggerscriptContentFile(this, file));
                                image = Properties.Resources.page_white;
                                break;
                            case FOUNDRYSCENARIO_EXT:
                                files.Add(file, new ScenarioEditorContentFile(this, file));
                                image = Properties.Resources.page_white;
                                break;
                            default:
                                files.Add(file, new ContentFile(this, file));
                                image = Properties.Resources.page_white;
                                break;
                        }
                    }

                    //Add this file to the manifest to use to remove files that are no longer present on disk.
                    foundFiles.Add(file);

                    //Create a node for this file.
                    DiskEntryNode child = new DiskEntryNode(Path.GetFileName(file), file, image);
                    node._children.Add(child);
                }
            }
        }
        public DiskEntryNode UpdateContent()
        {
            DiskEntryNode root = new DiskEntryNode(openedName, openedDir, Properties.Resources.box);
            List<string> foundFiles = new List<string>() { root._path };
            AddChildrenRecursive(root, foundFiles);

            //Remove unused files.
            foreach(var pair in files)
            {
                if(!foundFiles.Contains(pair.Key))
                {
                    ContentFileDelete(pair.Key);
                }
            }

            return root;
        }


        //content files
        private string activeFile;
        public void ContentFileOpen(string file)
        {
            if (files.ContainsKey(file))
            {
                files[file].TryOpen(workspace, DockState.Document);
            }
        }
        public void ContentFileSave(string file)
        {
            if (files.ContainsKey(file))
            {
                files[file].TrySave();
            }
        }
        public void ContentFileClose(string file)
        {

        }
        public void ContentFileSaveAll()
        {
            foreach (ContentFile f in files.Values)
            {
                f.TrySave();
            }
        }
        public void ContentFileCloseAll()
        {
            foreach (ContentFile f in files.Values)
            {
                f.TryClose();
            }
        }
        public bool ContentFileExists(string file)
        {
            return files.Keys.Contains(file);
        }
        public void ContentFileDelete(string file)
        {
            if(ContentFileExists(file))
            {
                files[file].TryClose();
                files.Remove(file);
                if (file == activeFile)
                {
                    activeFile = null;
                }
            }
        }
        public void ContentFileSetActive(string file)
        {
            if (ContentFileExists(file))
            {
                activeFile = file;
            }
        }


        //project explorers
        private List<ProjectExplorer> projectExplorers = new List<ProjectExplorer>();
        private void ProjectExplorer_OnClose(object o, FormClosedEventArgs e)
        {
            if (o is ProjectExplorer)
            {
                projectExplorers.Remove((ProjectExplorer)o);
            }
        }
        public void AddProjectExplorer(DockPanel workspace, DockState state)
        {
            ProjectExplorer pe = new ProjectExplorer(this);
            pe.FormClosed += new FormClosedEventHandler(ProjectExplorer_OnClose);
            projectExplorers.Add(pe);
            pe.Show(workspace, state);
        }
        public void UpdateAllProjectExplorers(DiskEntryNode root)
        {
            foreach (ProjectExplorer explorer in projectExplorers)
            {
                explorer.UpdateNodes(root);
            }
        }
        public void ClearAllProjectExplorers()
        {
            foreach (ProjectExplorer explorer in projectExplorers)
            {
                explorer.ClearNodes();
            }
        }
    }
}
