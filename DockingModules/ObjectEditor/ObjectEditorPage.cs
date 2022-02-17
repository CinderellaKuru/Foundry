using SMHEditor.DockingModules.ObjectEditor.Commands;
using SMHEditor.DockingModules.ObjectEditor.Flags;
using SMHEditor.DockingModules.ObjectEditor.Object_Types;
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
            oec.objectTypes.Controls.Add(new ObjectTypesControl(o));
            oec.commands.Controls.Add(new CommandsControl(o));

            Name = fileName;
            Text = name;
            TextTitle = "Object Editor";

            Controls.Add(oec);

            openedFile = o;
        }
    }
}
