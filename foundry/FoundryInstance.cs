using System;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.IO.Compression;
using WeifenLuo.WinFormsUI.Docking;
using Timer = System.Windows.Forms.Timer;
using File = System.IO.File;
using YAXLib;
using System.ComponentModel;
using foundry;
using IniParser.Model;
using System.Runtime.Loader;

namespace foundry
{
	public partial class FoundryInstance : Form
	{
		//////////////////////////////////////////////////////////////////////////////////////
		#region  foundry instance
		public FoundryInstance()
		{
			Load += new EventHandler(OnLoad);
			FormClosed += new FormClosedEventHandler(OnClose);

			ThreadPool.SetMinThreads(16, 16);

			InitializeComponent();

			versionReadout.Text = "v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
			dockpanel.Theme = new VS2015LightTheme();

			memoryMonitorTicker = new Timer();
			memoryMonitorTicker.Tick += (object o, EventArgs e) =>
			{
				memoryReadout.Text = (Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024)).ToString() + "mb";
			};
			memoryMonitorTicker.Interval = 1000;
			memoryMonitorTicker.Start();

			StartPage = new StartPage();
			StartPage.Show(MainDockPanel, DockState.Document);
        }
		public StartPage StartPage { get; private set; }

		//callbacks
		private void OnLoad(object o, EventArgs e)
        {
            InitConfig();
			InitToolstrip();
            InitModules();

#if DEBUG
			OpenWorkspace("R:\\foundry\\_resources\\New folder (2)\\NewWorkspace1.fworkspace");
#endif

			//ImportAsset("S:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\extract\\scenario\\skirmish\\design\\baron_1_swe\\baron_1_swe.xtd");


			foreach(ToolStripMenuItem item in Operators_MainForm.GetRootMenuItems())
			{
				menuStrip.Items.Add(item);
			}

		}
        private void OnClose(object o, EventArgs e)
		{
			SaveConfig();
            if (WorkspaceIsOpen())
			{
				CloseWorkspace();
			}
			Controls.Clear();
		}
		private Timer memoryMonitorTicker;
		private void ToolStrip_File_ImportAssetClicked(object sender, EventArgs e)
		{
		}
		private void ToolStrip_File_OpenAssetClicked(object sender, EventArgs e)
		{
		}
        private void Footer_DiscordImageClicked(object sender, EventArgs e)
		{
#if !DEBUG //I dont want to click it :P
			Process.Start("https://discord.gg/kfrCNUTaSc");
#endif
		}

        public DockPanel MainDockPanel { get { return dockpanel; } }

        private OperatorRegistrantToolstrip Operators_MainForm;
        public Operator Operator_File { get; private set; }
        public Operator Operator_Tools { get; private set; }
        private void InitToolstrip()
		{
			Operators_MainForm = new OperatorRegistrantToolstrip();

			Operator_File = new Operator("File");
            Operators_MainForm.AddOperator(Operator_File);

			Operator opNewWorkspace = new Operator("New Workspace");
			opNewWorkspace.OperatorActivated += (sender, e) =>
			{
				CreateWorkspaceWizard cww = new CreateWorkspaceWizard();
				if (cww.ShowDialog() == DialogResult.OK)
				{
					CreateWorkspace(cww.WorkspaceLocation, cww.WorkspaceName, cww.WorkspaceUnpackDefault);
				}
			};
			opNewWorkspace.Parent = Operator_File;

            Operator opOpenWorkspace = new Operator("Open Workspace");
			opOpenWorkspace.OperatorActivated += (sender, e) =>
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = string.Format("Foundry Project (*{0})|*{0}", ProjectFileExt);

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					OpenWorkspace(ofd.FileName);
				}
			};
            opOpenWorkspace.Parent = Operator_File;

            Operator opImportArchive = new Operator("Import Archive");
			opImportArchive.OperatorActivated += (sender, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Ensemble Resource Archive (*.era)|*.era";
                ofd.Multiselect = true;
                ofd.InitialDirectory = string.Format("{0}\\",
                            Path.GetDirectoryName(OpenedConfig.GetParamData(Config.Param.GameExe).Value)
                            );

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    UnpackErasAsync(ofd.FileNames, OpenedWorkspaceDir);
                }
            };
            opImportArchive.Parent = Operator_File;


            Operator_Tools = new Operator("Tools");
            Operators_MainForm.AddOperator(Operator_Tools);

			Operator opSettings = new Operator("Settings");
            opSettings.OperatorActivated += (sender, e) =>
            {
                ConfigPrompt prompt = new ConfigPrompt(OpenedConfig, true);
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    OpenedConfig = prompt.LocalConfig;
                }
            };
            opSettings.Parent = Operator_Tools;
        }

		public static string AppdataDir
		{
			get
			{
				string ret = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Foundry\\";
				if (!Directory.Exists(ret))
				{
					Directory.CreateDirectory(ret);
				}
                return ret;
            }
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region config
		public string ConfigFile { get { return AppdataDir + "config.cfg"; } }
		public class Config
		{
			public Config()
			{
				datas = new Dictionary<Param, ParamData>()
				{
					{ Param.GameExe, new ParamData("Game executable path", ParamType.File, null, "xgameFinal.exe") }
				};
			}

			public enum Param
			{
				GameExe
			}
			public enum ParamType
			{
				Directory,
				File,
				String,
				Number
			}
			public class ParamData
            {
                public ParamData(string displayName, ParamType type, string defaultValue = null, object validValue = null)
                {
                    DisplayName = displayName;
                    Value = defaultValue;
					Type = type;
					ValidValue = validValue;
                }
                public string Value { get; set; }
                public string DisplayName { get; private set; }
				public ParamType Type { get; private set; }
                public object ValidValue { get; private set; }
            }
			public bool AllParamsValid()
			{
				foreach (Param p in Enum.GetValues<Param>())
				{
					if (!ParamValid(p)) return false;
				}
				return true;
			}
			public bool ParamValid(Param param)
			{
				if (datas.ContainsKey(param))
				{
					ParamData data = datas[param];

					switch (data.Type)
					{
						case ParamType.Directory:
							if (Directory.Exists(data.Value)) return true;
							return false;

						case ParamType.File:
							if (File.Exists(data.Value))
							{
								if (data.ValidValue != null)
								{
									string validFile = (string)data.ValidValue;
									if (Path.GetFileName(data.Value) == validFile) return true;
									else return false;
								}
							}
							return false;
					}
				}
				return false;
			}
            public ParamData GetParamData(Param param)
			{
				return datas[param];
			}


			private Dictionary<Param, ParamData> datas;
		}
		public Config OpenedConfig { get; private set; }
		private void InitConfig()
		{
			OpenedConfig = new Config();

			if (!File.Exists(ConfigFile))
			{
				File.Create(ConfigFile);
			}
			string[] cfg = File.ReadAllLines(ConfigFile);

			foreach (string line in cfg)
			{
				string[] keyval = line.Split('=');
				if (keyval.Length != 2) continue;

				string key = keyval[0].Trim();
				string val = keyval[1].Trim();

				Config.Param p;
				bool valid = Enum.TryParse(key, out p);
				if (valid)
				{
					OpenedConfig.GetParamData(p).Value = val;
				}
			}

			//Ensure valid config data by constantly opening the prompt until everything is set.
			if (!OpenedConfig.AllParamsValid())
			{
				ConfigPrompt prompt = new ConfigPrompt(OpenedConfig, false);
				if (prompt.ShowDialog() == DialogResult.OK)
				{
					OpenedConfig = prompt.LocalConfig;
					SaveConfig();
				}
			}
		}
		private void SaveConfig()
		{
			if (!File.Exists(ConfigFile))
			{
				File.Create(ConfigFile);
			}

			string data = "";

			foreach (Config.Param p in Enum.GetValues<Config.Param>())
			{
				string val = OpenedConfig.GetParamData(p).Value;
                if (val == null) continue;

				data += string.Format("{0} = {1}\n", p.ToString(), val);
			}

			File.WriteAllText(ConfigFile, data);
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
			if (type == LogEntryType.DebugInfo || type == LogEntryType.DebugError) return;
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
			if (type == LogEntryType.DebugError)
			{
				MessageBox.Show(mainMessage, "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

        }
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region modules
		private List<BaseModule> Modules { get; set; }
		private Dictionary<string, List<Type>> ValidImportTypes { get; set; }
		private void InitModules()
		{
			if (Modules != null) return;

			Modules = new List<BaseModule>();
			ValidImportTypes = new Dictionary<string, List<Type>>();

			string modulesDir = Directory.GetCurrentDirectory() + "\\modules\\";

			if (!Directory.Exists(modulesDir))
				Directory.CreateDirectory(modulesDir);

            AssemblyLoadContext context = new AssemblyLoadContext(null);
            foreach (string file in Directory.EnumerateFiles(modulesDir))
			{
				if (file.EndsWith(".dll"))
				{
					AssemblyDependencyResolver resolver = new AssemblyDependencyResolver(file);
					string assembly = resolver.ResolveAssemblyToPath(new AssemblyName(Path.GetFileNameWithoutExtension(file)));
					context.LoadFromAssemblyPath(assembly);
                }
			}

			Type baseModuleType = typeof(BaseModule);
			Type baseEditorPageType = typeof(BaseEditorPage);
			var assemblies =
				AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(p => baseModuleType.IsAssignableFrom(p) && p.IsClass).ToArray();

			foreach (Type t in assemblies)
			{
				if (t == baseModuleType) continue; //dont make an instance of the base, silly. can this even happen?

				BaseModule module = (BaseModule)Activator.CreateInstance(t);
				if (!baseEditorPageType.IsAssignableFrom(module.PageType)) continue; //bad page type.

				module.Init(this);
				Modules.Add(module);
			}

			foreach(BaseModule module in Modules)
			{
				module.PostInit();
			}
		}

		public bool GetModuleByType<T>(out T module) where T : BaseModule
		{
			var valid = Modules.Where(e => e.GetType() == typeof(T));

            if (valid.Any())
			{
				module = (T)valid.First();
				return true;
			}

			module = null;
			return false;
		}
		#endregion


        //////////////////////////////////////////////////////////////////////////////////////
        #region background task
        public void StartBlockingBackgroundTask(Action<object> action, object argument, string displayText)
        {
			using (BackgroundWorker worker = new BackgroundWorker())
			{
				BlockingProgressBar progressBar = new BlockingProgressBar();

                worker.DoWork += (sender, e) =>
				{
					action(argument);
				};
				worker.RunWorkerCompleted += (sender, e) =>
				{
					progressBar.Close();
                };

				worker.RunWorkerAsync();
                progressBar.TaskDisplayText = displayText;
                progressBar.ShowDialog();
			}
        }
        #endregion


        //////////////////////////////////////////////////////////////////////////////////////
        #region workspace
        public const string ProjectFileExt = ".fworkspace";
		public static string[] DefaultEraFiles
		{
			get { return new string[] { "root.era", "root_update.era", "scenarioshared.era" }; }
		}
		public static string[] AllEraFiles =
		{
			"baron_1_swe.era",
			"beaconhill_2.era",
			"beasleys_plateau.era",
			"blood_gulch.era",
			"campaignTutorial.era",
			"campaignTutorialAdvanced.era",
			"chasms.era",
			"dlc01.era",
			"dlc02.era",
			"exile.era",
			"fort_deen.era",
			"frozen_valley.era",
			"glacial_ravine_3.era",
			"inGameUI.era",
			"labyrinth.era",
			"loadingUI.era",
			"locale.era",
			"miniloader.era",
			"PHXscn01.era",
			"PHXscn02.era",
			"PHXscn03.era",
			"PHXscn04.era",
			"PHXscn05.era",
			"PHXscn06.era",
			"PHXscn07.era",
			"PHXscn08.era",
			"PHXscn09.era",
			"PHXscn10.era",
			"PHXscn11.era",
			"PHXscn12.era",
			"PHXscn13.era",
			"PHXscn14.era",
			"PHXscn15.era",
			"pregameUI.era",
			"redriver_1.era",
			"release.era",
			"repository.era",
			"root.era",
			"root_update.era",
			"scenarioshared.era",
			"shader.era",
			"terminal_moraine.era",
			"the_docks.era",
			"tundra.era",
		};



		public string OpenedWorkspaceFile { get; private set; }
		public string OpenedWorkspaceDir
		{
			get
			{
				if (OpenedWorkspaceFile == null) return null;
				string ret = Path.GetDirectoryName(OpenedWorkspaceFile) + "\\";
				ret = ret.Replace("\\", "/");
                return ret;
			}
		}
		public string OpenedWorkspaceName
		{
			get
			{
				if (OpenedWorkspaceFile == null) return null;
				return Path.GetFileNameWithoutExtension(OpenedWorkspaceFile);
			}
		}
        public struct WorkspaceFileData
		{

		}
		public WorkspaceFileData OpenedWorkspaceFileData { get; private set; }
        public DiskEntryNode OpenedWorkspaceRoot { get; private set; }

        public bool WorkspaceIsOpen()
		{
			return OpenedWorkspaceFile != null;
		}
		public enum WorkspaceOpenResult
		{
			Cancelled,
			Opened,
			Invalid,
		}
		public WorkspaceOpenResult OpenWorkspace(string file)
		{
			if (WorkspaceIsOpen())
			{
				if (CloseWorkspace() == WorkspaceCloseResult.Cancelled)
				{
					return WorkspaceOpenResult.Cancelled;
				}
			}
			
			if (File.Exists(file) && Path.GetExtension(file) == ProjectFileExt)
			{
				OpenedWorkspaceFile = file;
				foreach (BaseModule m in Modules)
				{
					m.WorkspaceOpened();
				}
				return WorkspaceOpenResult.Opened;
			}
			else
			{
				return WorkspaceOpenResult.Invalid;
			}
		}
		public enum WorkspaceCloseResult
		{
			Cancelled,
			Closed,
		}
		public WorkspaceCloseResult CloseWorkspace()
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

            foreach (BaseModule m in Modules)
            {
                m.WorkspaceClosed();
            }

            OpenedWorkspaceFile = null;
            return WorkspaceCloseResult.Closed;
		}
		public enum WorkspaceCreateResult
		{
			Cancelled,
			Created,
			DirDoesntExist,
			DirNotEmpty,
			UnpackError,
		}
		public WorkspaceCreateResult CreateWorkspace(string dir, string name, bool unpackDefaultEras)
		{
			dir = dir.Replace("\\", "/");
			if (!dir.EndsWith("/")) dir += "/";

			if (!Directory.Exists(dir))
			{
				return WorkspaceCreateResult.DirDoesntExist;
            }

			if (Directory.EnumerateFileSystemEntries(dir).Any())
			{
				return WorkspaceCreateResult.DirNotEmpty;
			}

            string projectFile = dir + name + ProjectFileExt;

            if (unpackDefaultEras)
            {
				string[] fullEraPaths = DefaultEraFiles;
				for(int i = 0; i < fullEraPaths.Length; i++)
				{
					fullEraPaths[i] = string.Format("{0}\\{1}",
						Path.GetDirectoryName(OpenedConfig.GetParamData(Config.Param.GameExe).Value),
						DefaultEraFiles[i]
						);

					if (!File.Exists(fullEraPaths[i]))
					{
						AppendLog(LogEntryType.Error, "Could not find one or more default archive files. Check your game install path in settings.", true);
						return WorkspaceCreateResult.UnpackError;
					}
                }
				UnpackErasAsync(fullEraPaths, dir);
            }

            WorkspaceFileData wfd = new WorkspaceFileData();

            YAXLib.Options.SerializerOptions options = new YAXLib.Options.SerializerOptions();
            options.ExceptionBehavior = YAXLib.Enums.YAXExceptionTypes.Ignore;
            YAXSerializer<WorkspaceFileData> serializer = new YAXSerializer<WorkspaceFileData>(options);
            string data = serializer.Serialize(wfd);
            File.WriteAllText(projectFile, data);

            OpenedWorkspaceFile = projectFile;
            OpenedWorkspaceFileData = wfd;
            return WorkspaceCreateResult.Created;
		}

		public void UnpackErasAsync(string[] eras, string outdir)
		{
			Tuple<string[], string> args = new Tuple<string[], string>(eras, outdir);

			StartBlockingBackgroundTask(
			(o) =>
			{
				Tuple<string[], string> taskargs = (Tuple<string[], string>)o;


                List <Task> tasks = new List<Task>();
				foreach (string era in taskargs.Item1)
				{
					Task t = Task.Run(() =>
					{
						if (!Directory.Exists(taskargs.Item2))
						{
							return;
						}

						Util.ERA.ExpandERA(era, outdir);
					});
					tasks.Add(t);
				}

				Task.WaitAll(tasks.ToArray());
			},
            args,
			"Unpacking archives...");
		}



        /// <summary>
        /// Nodes representing a file or folder on disk.
        /// </summary>
        public class DiskEntryNode
		{
			public DiskEntryNode(string name, string path, bool isFolder)
			{
				IsFolder = isFolder;
				Name = name;
				Path = path;
				Children = new List<DiskEntryNode>();
			}
			public bool IsFolder { get; set; }
			public string Name { get; set; }
			public string Path { get; set; }
			public List<DiskEntryNode> Children { get; set; }
		}
		private void UpdateDirectory_ScanDirRecursive(DiskEntryNode node)
		{
			if (File.GetAttributes(node.Path).HasFlag(FileAttributes.Directory))
			{
				//get child folders first
				foreach (string dir in Directory.GetDirectories(node.Path))
				{
					DiskEntryNode child = new DiskEntryNode(Path.GetFileName(dir), dir, true);
					node.Children.Add(child);
					UpdateDirectory_ScanDirRecursive(child);
				}

				//get child files second
				foreach (string file in Directory.GetFiles(node.Path))
				{
					DiskEntryNode child = new DiskEntryNode(Path.GetFileName(file), file, false);
					node.Children.Add(child);
				}
			}
		}
		/// <summary>
		/// Scans the project directory for present files, and updates the explorers with this information.
		/// </summary>
		public void UpdateDirectory()
		{
			DiskEntryNode root = new DiskEntryNode(OpenedWorkspaceName, OpenedWorkspaceDir, true);
			UpdateDirectory_ScanDirRecursive(root);
			OpenedWorkspaceRoot = root;
		}
        //editors that are currently open.
        private List<BaseEditorPage> openEditors = new List<BaseEditorPage>();
		#endregion

	}
}
