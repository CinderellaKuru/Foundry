using SMHEditor.DockingModules.ObjectEditor.Flags;
using SMHEditor.DockingModules.ObjectEditor.Veterancy;
using SMHEditor.Project.FileTypes;

namespace SMHEditor.DockingModules.ObjectEditor
{
    internal class ObjectEditorPage : EditorPage
    {
        private readonly ObjectFile openedFile;
        private readonly ObjectEditorControl oec;

        public ObjectEditorPage(ObjectFile o, string fileName, string name) : base(fileName)
        {
            oec = new ObjectEditorControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };

            oec.hardpoints.Controls.Add(new HardpointsControl(o));
            oec.veterancy.Controls.Add(new VeterancysControl(o));
            oec.settings.Controls.Add(new SettingsControl(o));
            oec.flags.Controls.Add(new FlagsControl(o));

            Name = fileName;
            Text = name;
            TextTitle = "Object Editor";

            Controls.Add(oec);

            openedFile = o;
        }
    }
}
