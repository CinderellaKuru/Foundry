using System;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;
using YAXLib.Attributes;
using YAXLib.Enums;
using YAXLib;
using Foundry.Project.Modules.Base;
using Foundry.Project.Modules.TriggerscriptEditor;
using Foundry.Project.Modules.ScenarioEditor;
using Foundry.Project.Modules.Workspace;
using Foundry.Project.Modules.XmlEditor;

namespace Foundry.Project
{
	public partial class FoundryInstance : Form
	{
		//////////////////////////////////////////////////////////////////////////////////////
		#region  foundry instance
		public FoundryInstance()
		{
			Load += new EventHandler(OnLoad);
			FormClosed += new FormClosedEventHandler(OnClose);

			InitializeComponent();
			versionReadout.Text = "v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
			workspace.Theme = new VS2015LightTheme();

			memoryMonitorTicker = new Timer();
			memoryMonitorTicker.Tick += (object o, EventArgs e) =>
			{
				memoryReadout.Text = (Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024)).ToString() + "mb";
			};
			memoryMonitorTicker.Interval = 1000;
			memoryMonitorTicker.Start();
		}

		//callbacks
		private void OnLoad(object o, EventArgs e)
		{
			AddProjectExplorer(workspace, DockState.DockLeft);

#if DEBUG
			ProjectOpen("workingProject/workingproj.fproject");

#endif
		}
		private void OnClose(object o, EventArgs e)
		{
			if (projectOpened)
			{
				ProjectClose();
			}
			Controls.Clear();
		}
		private Timer memoryMonitorTicker;
		private void ToolStrip_File_OpenProjectClicked(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				ProjectOpen(ofd.FileName);
			}
		}
		private void ToolStrip_File_ImportClicked(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				NewEditorFromImportedFile(ofd.FileName);
			}
		}
		private void Footer_DiscordImageLink_Click(object sender, EventArgs e)
		{
			Process.Start("https://discord.gg/kfrCNUTaSc");
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region config
		private void LoadConfig()
		{

		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region  log
		public enum LogEntryType
		{
			Info,
			Warning,
			Error,
		}
		/// <summary>
		/// Writes text to the console.
		/// </summary>
		/// <param name="type">The type of message to send. Affects prefix.</param>
		/// <param name="mainMessage">The main text to be written.</param>
		/// <param name="displayAsStatus">If true, mainMessage will be written to the footer status bar, until another message overwrites it.</param>
		/// <param name="secondaryMessage">If supplied, this will be written to the console, but not displayed on the status bar. Useful for things like exception info.</param>
		public void AppendLog(LogEntryType type, string mainMessage, bool displayAsStatus, string secondaryMessage = null)
		{
			string prefix;
			switch (type)
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
			string print = prefix + mainMessage;

			if (displayAsStatus)
				logStatus.Text = print;

			Console.WriteLine(print);
			if (secondaryMessage != null)
			{
				Console.WriteLine(secondaryMessage);
			}
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region property editors
		private List<PropertyEditor> propertyEditorPages = new List<PropertyEditor>();
		/// <summary>
		/// Adds a new property editor to the workspace.
		/// </summary>
		public void AddPropertyEditor()
		{
			PropertyEditor pe = new PropertyEditor();
			propertyEditorPages.Add(pe);
			pe.Show(workspace, DockState.Float);
		}
		public void SetSelectedObject(object o)
		{

		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region project explorers
		private List<ProjectExplorer> projectExplorerPages = new List<ProjectExplorer>();
		/// <summary>
		/// Callback to remove the explorer from the FoundryInstance upon closing.
		/// </summary>
		private void ProjectExplorer_OnClose(object o, FormClosedEventArgs e)
		{
			if (o is ProjectExplorer)
			{
				projectExplorerPages.Remove((ProjectExplorer)o);
			}
		}
		/// <summary>
		/// Adds a new project explorer to the desired panel, in the desired state.
		/// </summary>
		public void AddProjectExplorer(DockPanel workspace, DockState state)
		{
			ProjectExplorer pe = new ProjectExplorer(this);
			pe.FormClosed += new FormClosedEventHandler(ProjectExplorer_OnClose);
			projectExplorerPages.Add(pe);
			pe.Show(workspace, state);
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region project
		public const string SaveFoundryProjectExt = ".fproject";
		public const string SaveTriggerscriptExt = ".fts";
		public const string SaveTerrainExt = ".ftr";
		public const string SaveScenarioExt = ".fsc";
		public const string SaveXmlExt = ".xml";

		public const string ImportTriggerscriptExt = ".triggerscript";
		public const string ImportTerrainExt = ".xtd";
		public const string ImportScenarioExt = ".scn";


		/// <summary>
		/// Contains info about project state, such as folded folders etc.
		/// </summary>
		private class ProjectData
		{
			#region classes
			public class FolderDataEntry
			{
				[YAXSerializeAs("Folded")]
				public bool Folded { get; set; }
			}
			#endregion
			public ProjectData()
			{
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

			if (Path.GetExtension(file) != SaveFoundryProjectExt)
				return;

			openedDir = Path.GetDirectoryName(file);
			openedFile = Path.GetFullPath(file);
			openedName = Path.GetFileNameWithoutExtension(file);
			projectOpened = true;

			//load the ProjectData from within the project file.
			try
			{
				YAXSerializer ser = new YAXSerializer(typeof(ProjectData));
				openedData = (ProjectData)ser.DeserializeFromFile(openedFile);
			}
			catch (Exception e)
			{
				ProjectClose();
				AppendLog(LogEntryType.Error, "Project '" + openedName + "' failed to load. Open the log for more info.", true, e.Message);
				return;
			}

			//get all present files and update the explorers.
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
			foreach (BaseEditorPage p in openEditors)
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

		/// <summary>
		/// Nodes representing a file or folder on disk.
		/// </summary>
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
		/// <summary>
		/// Scans the project directory for present files, and updates the explorers with this information.
		/// </summary>
		public void ScanProjectDirectoryAndUpdate()
		{
			DiskEntryNode root = new DiskEntryNode(openedName, openedDir, false);
			ScanProjectDirectoryRecursive(root);

			foreach (var explorer in projectExplorerPages)
			{
				explorer.UpdateNodes(root);
			}
		}

		//editors that are currently open.
		private List<BaseEditorPage> openEditors = new List<BaseEditorPage>();
		/// <summary>
		/// Opens an editor from a file. If a file of the name file is already open, (TODO: bring the page to the front).
		/// </summary>
		public void NewEditorFromProjectFile(string file)
		{
			BaseEditorPage page;
			switch (Path.GetExtension(file))
			{
				case SaveTriggerscriptExt:
					page = new TriggerscriptEditorPage(this);
					break;
				case SaveScenarioExt:
					page = new ScenarioEditorPage(this);
					break;
				case SaveXmlExt:
					page = new XmlEditorPage(this);
					break;
				default:
					return;
			}
			if (page.TryOpen(file))
			{
				openEditors.Add(page);
				page.Text = Path.GetFileName(file);
				page.TryShow(workspace, DockState.Document);
			}
		}
		/// <summary>
		/// Opens an editor from an imported file.
		/// </summary>
		public void NewEditorFromImportedFile(string file)
		{
			BaseEditorPage page;
			switch (Path.GetExtension(file))
			{
				case ImportTriggerscriptExt:
					page = new TriggerscriptEditorPage(this);
					break;
				case ImportTerrainExt:
					page = new ScenarioEditorPage(this);
					break;
				default:
					return;
			}
			if (page.TryImport(file))
			{
				openEditors.Add(page);
				page.Text = Path.GetFileName(file);
				page.TryShow(workspace, DockState.Document);
			}
		}

		#endregion

	}
}
