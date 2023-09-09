using foundry;
using YAXLib;

namespace foundry.scenario
{
	public class ScenarioModule : BaseModule
	{
        public override string ImportExt { get { return ".xtd"; } }
        public override string SaveExt { get { return ".fmap"; } }
        public override Type PageType { get { return typeof(ScenarioEditorPage); } }

		protected override void OnInit()
		{
			YAXSerializer ser = new YAXSerializer(typeof(ScenarioClass));
		}
		protected override void OnWorkspaceOpened()
		{
			string scnDir = Instance.OpenedWorkspaceDir + "scenario/";
            foreach (string dir in Directory.EnumerateDirectories(scnDir, "*", SearchOption.AllDirectories))
			{
				foreach (string scenarioFile in Directory.GetFiles(dir).Where(f => Path.GetExtension(f) == ".scn"))
				{
                    ScenarioEntry entry = new ScenarioEntry(Path.GetFileNameWithoutExtension(scenarioFile), Path.GetRelativePath(scnDir, dir));
                    ScenarioEntries.Add(entry.Name, entry);
                }
			}
		}
		protected override void OnWorkspaceClosed()
		{
            ScenarioEntries.Clear();
        }

		private class ScenarioEntry
		{
			public ScenarioEntry(string name, string dir)
			{
				Name = name;
				Dir = dir;
			}
			public string ScnFile{ get { return Dir + Name + ".scn"; } }
			public string Sc2File{ get { return Dir + Name + ".sc2"; } }
			public string Sc3File{ get { return Dir + Name + ".sc3"; } }
            public string Name { get; set; }
			public string Dir { get; set; }
		}
		private Dictionary<string, ScenarioEntry> ScenarioEntries = new Dictionary<string, ScenarioEntry>();

		public void OpenScenario(string name)
		{

		}
	}
}