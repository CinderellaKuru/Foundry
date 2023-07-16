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
using System.Xml;
using static Foundry.Project.FoundryInstance.Project.ProjectData;
using SharpDX.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;

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
			//NewEditorFromImportedFile("S:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\scenario\\skirmish\\design\\chasms\\chasms.xtd");
#endif
		}
		private void OnClose(object o, EventArgs e)
		{
			if (ProjectIsOpen())
			{
				ProjectClose();
			}
			Controls.Clear();
		}
		private Timer memoryMonitorTicker;
		private void ToolStrip_File_OpenProjectClicked(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Foundry Project (.fproject)|*.fproject";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				ProjectOpen(ofd.FileName);
			}
		}
		private void ToolStrip_File_ImportTerrainClicked(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Terrain Visual (.xtd)|*.xtd";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				NewEditorFromImportedFile(ofd.FileName);
			}
		}
		private void ToolStrip_File_ImportObjectClicked(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Object (.xml)|*.xml";
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
			DebugInfo,
			DebugError,
		}
		/// <summary>
		/// Writes text to the window status bar and console.
		/// </summary>
		/// <param name="type">The type of message to send. Affects prefix.</param>
		/// <param name="mainMessage">The main text to be written.</param>
		/// <param name="displayAsStatus">If true, mainMessage will be written to the footer status bar, until another message overwrites it.</param>
		/// <param name="secondaryMessage">If supplied, this will be written to the console, but not displayed on the status bar. Useful for things like exception info.</param>
		public void AppendLog(LogEntryType type, string mainMessage, bool displayAsStatus, string secondaryMessage = null)
		{
#if !DEBUG
			if(type == LogEntryType.DebugInfo || type == LogEntryType.DebugWarning || type == LogEntryType.DebugError) return;
#endif
			string prefix;
			switch (type)
			{
				case LogEntryType.Info:
				case LogEntryType.DebugInfo:
					prefix = "[Info] ";
					break;
				case LogEntryType.Warning:
					prefix = "[Warning] ";
					break;
				case LogEntryType.Error:
				case LogEntryType.DebugError:
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

			//just for debug error:
			if(type == LogEntryType.DebugError)
			{
				MessageBox.Show(mainMessage, "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
		//extension for project file.
		public const string ExtProject = ".fproject";

		//extensions for serialized editors' files.
		public const string ExtSerializeTriggerscript = ".ftsr";
		public const string ExtSerializeTerrain = ".ftrn";
		public const string ExtSerializeScenario = ".fscn";
		public const string ExtSerializeObject = ".fobj";
		public const string ExtSerializeSquad = ".fsqd";
		public const string ExtSerializeTech = ".ftch";

		//extensions for imported files.
		public const string ExtImportTriggerscript = ".triggerscript";
		public const string ExtImportTerrain = ".xtd";
		public const string ExtImportScenario = ".scn";
		public const string ExtImportXml = ".xml";

		//full file names for specific source files.
		public const string FileSourceObjectsXml = "objects.xml";
		public const string FileSourceSquadsXml = "squads.xml";
		public const string FileSourceTechsXml = "techs.xml";
		public const string FileSourceObjectsUpdateXml = "objects_update.xml";
		public const string FileSourceSquadsUpdateXml = "squads_update.xml";
		public const string FileSourceTechsUpdateXml = "techs_update.xml";

		//folder names for special directories.
		public const string DirReferenceName = ".ref"; //this is supposed to have a dot before it.

		public class Project
		{
			//private ctor
			private Project(){ }

			public enum OpenResult
			{
				Success = 0,
				IncorrectExt,
				FileNull,
				BadFileData,
			}
			public static OpenResult Open(string file, out Project project)
			{
				if (!File.Exists(file))
				{
					project = null;
					return OpenResult.FileNull;
				}

				if (!Path.HasExtension(file) || Path.GetExtension(file) != ExtProject)
				{
					project = null;
					return OpenResult.IncorrectExt;
				}

				string fullFile = Path.GetFullPath(file);
				project = new Project();
				project.FilePath = fullFile;
				project.Name = Path.GetFileNameWithoutExtension(fullFile);
				project.FileDir = Path.GetDirectoryName(fullFile);

				//read project file.
				YAXSerializer ser = new YAXSerializer(typeof(ProjectData),
					new YAXLib.Options.SerializerOptions() { ExceptionBehavior = YAXExceptionTypes.Ignore });
				project.Data = (ProjectData)ser.DeserializeFromFile(project.FilePath);

				return OpenResult.Success;
			}
			public void Save()
			{
				YAXSerializer ser = new YAXSerializer(typeof(ProjectData));
				string xml = ser.Serialize(Data);
				File.WriteAllText(FilePath, xml);
			}

			public class ProjectData
			{
				public ProjectData() { }

				//folder info
				public class FolderDataEntry
				{
					[YAXAttributeForClass()]
					public bool Folded { get; set; }
				}
				[YAXDictionary(EachPairName = "Folder", KeyName = "name", ValueName = "Data", SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Element)]
				[YAXDontSerializeIfNull()]
				//dont use auto property so that if the value is null, we can create a list.
				private Dictionary<string, FolderDataEntry> _folderData;
				public Dictionary<string, FolderDataEntry> FolderData
				{
					get
					{
						if (_folderData == null)
							_folderData = new Dictionary<string, FolderDataEntry>();
						return _folderData;
					}
					set
					{
						_folderData = value;
					}
				}
			}
			public ProjectData Data { get; private set; }

			public string Name { get; private set; }
			public string FilePath { get; private set; }
			public string FileDir { get; private set; }
			public string RefFolderDir { get { return string.Format("{0}\\{1}", FileDir, DirReferenceName); } }

			/// <summary>
			/// Returns the full path on-disk from the relative directory.
			/// </summary>
			/// <returns>The full path of the specified relative path. Returns null for invalid paths.</returns>
			public string TryGetFullPathFromRelativePath(string relativePath)
			{
				string tryFull = FileDir + relativePath;
				if (Directory.Exists(tryFull))
				{
					return tryFull;
				}

				return null;
			}
		}
		public Project OpenedProject { get; private set; }

		public bool ProjectIsOpen()
		{
			return OpenedProject != null;
		}
		public void ProjectCreate(string file)
		{
			//TODO:
		}
		public void ProjectOpen(string file)
		{
			if (ProjectIsOpen())
			{
				ProjectClose();
			}

			Project info;
			Project.OpenResult result = Project.Open(file, out info);
			if(result != Project.OpenResult.Success)
			{
				AppendLog(LogEntryType.Warning, string.Format("The project file could not be opened: {0}", result.ToString()), true);
				ProjectClose();
				return;
			}
			OpenedProject = info;
			//save it in case the file does not have all of the required attributes.
			ProjectSave();

			AppendLog(LogEntryType.Info, string.Format("Project '{0}' loaded.", OpenedProject.Name), true);

			//get all present files and update the explorers.
			UpdateDirectory();
		}
		public void ProjectSave()
		{
			if(ProjectIsOpen())
			{
				OpenedProject.Save();
			}
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
			OpenedProject = null;

			if(ProjectIsOpen())
			{
				AppendLog(LogEntryType.DebugError, "The project is still considered open after calling ProjectClose()!", true);
			}
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
		private void UpdateDirectory_ScanDirRecursive(DiskEntryNode node)
		{
			if (File.GetAttributes(node.path).HasFlag(FileAttributes.Directory))
			{
				//get child folders first
				foreach (string dir in Directory.GetDirectories(node.path))
				{
					DiskEntryNode child = new DiskEntryNode(Path.GetFileName(dir), dir, true);
					node.children.Add(child);
					UpdateDirectory_ScanDirRecursive(child);
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
		public void UpdateDirectory()
		{
			DiskEntryNode root = new DiskEntryNode(OpenedProject.Name, OpenedProject.FileDir, true);
			UpdateDirectory_ScanDirRecursive(root);

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
				case ExtSerializeTriggerscript:
					page = new TriggerscriptEditorPage(this);
					break;
				case ExtSerializeScenario:
					page = new ScenarioEditorPage(this);
					break;
				case ExtSerializeXml:
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
		public void NewTerrainEditorImport(string file)
		{
			ScenarioEditorPage page = new ScenarioEditorPage(this);
			page.TryImport(file);
		}
#endregion
	}
}
