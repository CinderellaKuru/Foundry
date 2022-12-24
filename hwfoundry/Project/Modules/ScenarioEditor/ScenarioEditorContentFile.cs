using Foundry.Project.Modules.Triggerscripter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using static Foundry.FoundryInstance;

namespace Foundry.Project.Modules.ScenarioEditor
{
    public class ScenarioEditorContentFile : ContentFile
    {
        ScenarioEditorPage page;

        public ScenarioEditorContentFile(FoundryInstance i, string fileName) : base(i, fileName)
        {
            if (!File.Exists(fileName)) return;
        }

        protected override DockContent DoOpenFile()
        {
            page = new ScenarioEditorPage(Instance());
            page.Text = Path.GetFileName(FileName());

            return page;
        }
        protected override void DoSaveFile()
        {

        }
    }
}
