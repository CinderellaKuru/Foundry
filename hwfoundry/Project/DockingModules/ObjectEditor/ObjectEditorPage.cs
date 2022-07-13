using ComponentFactory.Krypton.Navigator;
using SMHEditor.DockingModules.ObjectEditor.Commands;
using SMHEditor.DockingModules.ObjectEditor.Flags;
using SMHEditor.DockingModules.ObjectEditor.Object_Childs;
using SMHEditor.DockingModules.ObjectEditor.Object_Types;
using SMHEditor.DockingModules.ObjectEditor.Veterancy;
using SMHEditor.Project.FileTypes;

namespace SMHEditor.DockingModules.ObjectEditor
{
    class ObjectEditorPage : KryptonPage
    {
        private readonly ObjectFile openedFile;
        private readonly ObjectEditorControl oec;

        public ObjectEditorPage(ObjectFile o, string fileName, string name)
        {
            oec = new ObjectEditorControl
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };

            oec.hardpoints.Controls.Add(new HardpointsControl(o));
            oec.veterancy.Controls.Add(new VeterancysControl(o));
            oec.general.Controls.Add(new GeneralControl(o));
            oec.ui.Controls.Add(new UIControl(o));
            oec.flags.Controls.Add(new FlagsControl(o));
            oec.objectTypes.Controls.Add(new ObjectTypesControl(o));
            oec.objectChilds.Controls.Add(new ObjectChildsControl(o));
            oec.commands.Controls.Add(new CommandsControl(o));
            
            Name = fileName;
            Text = name;
            TextTitle = "Object Editor";

            Controls.Add(oec);

            openedFile = o;
        }
    }
}
