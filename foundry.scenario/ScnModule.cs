using Foundry;

namespace hwfoundry.scenario
{
	public class ScenarioModule : BaseModule
	{
        public override string ImportExt { get { return ".xtd"; } }
        public override string SaveExt { get { return ".fmap"; } }
        public override Type PageType { get { return typeof(ScenarioEditorPage); } }

		protected override void OnInit()
		{

		}
	}
}