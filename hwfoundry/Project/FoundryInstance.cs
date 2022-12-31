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
using System.ComponentModel;
using System.Diagnostics;

namespace Foundry.Project
{
    public partial class FoundryInstance : Form
    {
        #region  foundry instance
        public FoundryInstance()
        {
            Load += new System.EventHandler(FoundryInstance_OnLoad);
            FormClosed += new FormClosedEventHandler(FoundryInstance_OnClose);

            InitializeComponent();
            versionReadout.Text = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
            workspace.Theme = new VS2015LightTheme();
        }
        private void FoundryInstance_OnLoad(object o, EventArgs e)
        {
            AddProjectExplorer(workspace, DockState.DockLeft);

#if DEBUG
            ProjectOpen("workingProject/workingproj.fproject");
#endif
        }
        private void FoundryInstance_OnClose(object o, EventArgs e)
        {
            if (projectOpened)
            {
                ProjectClose();
            }
            Controls.Clear();
        }
        private void ToolStripMenu_OpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ProjectOpen(ofd.FileName);
            }
        }
        #endregion


        #region  log
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
        #endregion


        #region property editors
        private List<PropertyEditor> propertyEditorPages = new List<PropertyEditor>();
        public void AddPropertyEditor(DockState state)
        {
            PropertyEditor pe = new PropertyEditor();
            propertyEditorPages.Add(pe);
            pe.Show(workspace, DockState.Float);
        }
        public void SetSelectedObject(object o)
        {

        }
        #endregion


        #region project explorers
        private List<ProjectExplorer> projectExplorerPages = new List<ProjectExplorer>();
        private void ProjectExplorer_OnClose(object o, FormClosedEventArgs e)
        {
            if (o is ProjectExplorer)
            {
                projectExplorerPages.Remove((ProjectExplorer)o);
            }
        }
        public void AddProjectExplorer(DockPanel workspace, DockState state)
        {
            ProjectExplorer pe = new ProjectExplorer(this);
            pe.FormClosed += new FormClosedEventHandler(ProjectExplorer_OnClose);
            projectExplorerPages.Add(pe);
            pe.Show(workspace, state);
        }
        #endregion


        #region project
        public const string FoundryProjectExt = ".fproject";
        public const string TriggerscriptProjectExt = ".fts";
        public const string ScenarioProjectExt = ".fsc";

        //contains info about project state, such as folded folders etc.
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

        public void ProjectCreate(string file)
        {
            //TODO
        }
        public void ProjectOpen(string file)
        {
            if (projectOpened)
                ProjectClose();

            if (!File.Exists(file))
                return;

            if (Path.GetExtension(file) != FoundryProjectExt)
                return;

            openedDir = Path.GetDirectoryName(file);
            openedFile = Path.GetFullPath(file);
            openedName = Path.GetFileNameWithoutExtension(file);
            projectOpened = true;

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

            ScanProjectDirectoryAndUpdate();

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
            foreach (EditorPage p in openEditors.Values)
            {
                p.TryClose(true);
                //TODO: check for edited editors.
                //if(p.IsEdited()))
                //{
                //    return;
                //}
            }
            openEditors.Clear();
            projectOpened = false;
            openedDir = null;
            openedFile = null;
            openedName = null;
        }

        //the editor page is the base class for all editors. handles input, rendering, content file saving, etc.
        public class EditorPage : DockContent
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            private FoundryInstance instance;
            bool loaded = false;
            bool edited = false;
            public EditorPage(FoundryInstance i)
            {
                instance = i;

                mouseState = new MouseState();
                downKeys = new List<Keys>();

                ControlAdded += new ControlEventHandler(InternalOnly_ControlAdded);
                FormClosing += new FormClosingEventHandler(Internal_Closed);
                Resize += new EventHandler(Internal_Resize);

                MouseMove += new MouseEventHandler(Internal_MouseMoved);
                MouseWheel += new MouseEventHandler(Internal_MouseWheelMoved);
                MouseDown += new MouseEventHandler(Internal_MouseButtonDown);
                MouseUp += new MouseEventHandler(Internal_MouseButtonUp);

                KeyDown += new KeyEventHandler(Internal_KeyDown);
                KeyUp += new KeyEventHandler(Internal_KeyUp);

                renderTimer.Start();
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////
            //internal events
            private void InternalOnly_ControlAdded(object o, ControlEventArgs e)
            {
                e.Control.MouseMove += new MouseEventHandler(Internal_MouseMoved);
                e.Control.MouseWheel += new MouseEventHandler(Internal_MouseWheelMoved);
                e.Control.MouseDown += new MouseEventHandler(Internal_MouseButtonDown);
                e.Control.MouseUp += new MouseEventHandler(Internal_MouseButtonUp);

                e.Control.KeyDown += new KeyEventHandler(Internal_KeyDown);
                e.Control.KeyUp += new KeyEventHandler(Internal_KeyUp);
            }
            private void Internal_Closed(object o, FormClosingEventArgs e)
            {
                Controls.Clear();
                OnClose();
            }
            private void Internal_Resize(object o, EventArgs e)
            {
                if (!Disposing)
                {
                    OnResize();
                }
            }

            //mouse
            protected struct MouseState
            {
                public bool leftDown, rightDown, middleDown;
                public int x, y;
                public int deltaX, deltaY, deltaScroll;
            }
            private MouseState mouseState;
            private void Internal_MouseMoved(object o, MouseEventArgs e)
            {
                mouseState.deltaScroll = 0;
                mouseState.deltaX = mouseState.x - e.X;
                mouseState.deltaY = mouseState.y - e.Y;
                mouseState.x = e.X;
                mouseState.y = e.Y;
                Internal_Tick();
            }
            private void Internal_MouseWheelMoved(object o, MouseEventArgs e)
            {
                mouseState.deltaScroll = e.Delta;
                Internal_Tick();
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
                if (e.Button == MouseButtons.Middle)
                {
                    mouseState.middleDown = true;
                }
                Internal_Tick();
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
                if (e.Button == MouseButtons.Middle)
                {
                    mouseState.middleDown = false;
                }
                Internal_Tick();
            }

            //keyboard
            private List<Keys> downKeys;
            private void Internal_KeyDown(object o, KeyEventArgs e)
            {
                if (!downKeys.Contains(e.KeyCode))
                {
                    downKeys.Add(e.KeyCode);
                }
                Internal_Tick();
            }
            private void Internal_KeyUp(object o, KeyEventArgs e)
            {
                if (downKeys.Contains(e.KeyCode))
                {
                    downKeys.RemoveAll(x => x == e.KeyCode);
                }
                Internal_Tick();
            }

            //tick - this function calls OnTick, and when enough time has passed, OnDraw().
            private const int renderIntervalMilliseconds = 16;
            private Stopwatch renderTimer = new Stopwatch();
            private void Internal_Tick()
            {
                OnTick();
                if (renderTimer.ElapsedMilliseconds > renderIntervalMilliseconds)
                {
                    OnDraw();
                    renderTimer.Restart();
                }
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////
            //external file interactions
            public bool TrySetEdited()
            {
                if (loaded)
                {
                    if (!edited)
                    {
                        edited = true;
                        return true;
                    }
                }
                return false;
            }
            public bool TryOpen(string file, DockPanel location, DockState state)
            {
                if (!loaded)
                {
                    if (File.Exists(file))
                    {
                        edited = false;

                        if (OnLoadFile(file))
                        {
                            Show(location, state);
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
            public bool TryClose(bool force = false)
            {
                if (loaded)
                {
                    if (edited && !force)
                    {
                        if (MessageBox.Show("Are you sure you want to close this page? Any unsaved progress will be lost.", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            edited = false;
                            Close();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Close();
                        return true;
                    }
                }
                return false;
            }
            public bool TrySave(string file)
            {
                if (loaded)
                {
                    if (edited)
                    {
                        if (OnSaveFile(file))
                        {
                            edited = false;
                            return true;
                        }
                    }
                }
                return false;
            }
            public bool TrySaveAs(string file)
            {
                if (loaded)
                {
                    if (OnSaveFile(file))
                    {
                        edited = false;
                        return true;
                    }
                }
                return false;
            }
            public bool IsEdited()
            {
                return edited;
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////
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


            //////////////////////////////////////////////////////////////////////////////////////////////////
            //page overridable functions
            protected virtual bool OnLoadFile(string file) { return true; }
            protected virtual bool OnSaveFile(string file) { return true; }
            protected virtual void OnResize() { }
            protected virtual void OnClose() { }
            protected virtual void OnTick() { }
            protected virtual void OnDraw() { }
        }

        public struct DiskEntryNode
        {
            public DiskEntryNode(string name, string path, bool isFolder)
            {
                this.isFolder = isFolder;
                this.name = name;
                this.path = path;
                children = new List<DiskEntryNode>();
            }
            public bool isFolder;
            public string name;
            public string path;
            public List<DiskEntryNode> children;
        }
        private void ScanProjectDirectoryRecursive(DiskEntryNode node)
        {
            if (File.GetAttributes(node.path).HasFlag(FileAttributes.Directory))
            {
                //get child folders first
                foreach (string dir in Directory.GetDirectories(node.path))
                {
                    DiskEntryNode child = new DiskEntryNode(Path.GetDirectoryName(dir), dir, true);
                    node.children.Add(child);
                    ScanProjectDirectoryRecursive(child);
                }

                //get child files second
                foreach (string file in Directory.GetFiles(node.path))
                {
                    DiskEntryNode child = new DiskEntryNode(Path.GetFileName(file), file, false);
                    node.children.Add(child);
                }
            }
        }
        public void ScanProjectDirectoryAndUpdate()
        {
            DiskEntryNode root = new DiskEntryNode(openedName, openedDir, false);
            ScanProjectDirectoryRecursive(root);

            foreach(var explorer in projectExplorerPages)
            {
                explorer.UpdateNodes(root);
            }
        }

        //editors that are currently open.
        private Dictionary<string, EditorPage> openEditors = new Dictionary<string, EditorPage>();
        public bool EditorIsOpen(string file)
        {
            return openEditors.ContainsKey(file);
        }
        public void EditorOpen(string file)
        {
            if (!EditorIsOpen(file))
            {
                if (File.Exists(file))
                {
                    EditorPage page;
                    switch (Path.GetExtension(file))
                    {
                        case TriggerscriptProjectExt:
                            page = new TriggerscriptEditorPage(this);
                            break;
                        case ScenarioProjectExt:
                            page = new ScenarioEditorPage(this);
                            break;
                        default:
                            return;
                    }
                    if (page.TryOpen(file, workspace, DockState.Document))
                    {
                        page.Text = Path.GetFileName(file);
                        openEditors.Add(file, page);
                    }
                }
            }
        }
        public void EditorPageClose(string file)
        {
            if (EditorIsOpen(file))
            {
                if(openEditors[file].TryClose())
                {
                    openEditors.Remove(file);
                }
            }
        }
        public void EditorPageSave(string file)
        {
            if (EditorIsOpen(file))
            {
                openEditors[file].TrySave(file);
            }
        }
        #endregion
    }
}
