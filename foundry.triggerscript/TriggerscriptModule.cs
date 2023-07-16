using Foundry;
using SharpDX;

namespace hwfoundry.scenario
{
    public class TriggerscriptModule : BaseModule
    {
        public override string ImportExt { get { return ".triggerscript"; } }
        public override string SaveExt { get { return ".ftriggerscript"; } }
        public override Type PageType { get { return typeof(TriggerscriptEditorPage); } }

        protected override void OnInit()
        {
        }
    }
}