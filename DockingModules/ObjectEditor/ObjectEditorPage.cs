using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.Project.FileTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.DockingModules.ObjectEditor
{
    class ObjectEditorPage : EditorPage
    {
        private ObjectFile openedFile;
        ObjectEditorControl oec;

        public ObjectEditorPage(ObjectFile o, string fileName, string name) : base(fileName)
        {
            oec = new ObjectEditorControl();
            oec.Dock = System.Windows.Forms.DockStyle.Fill;
            
            Name = fileName;
            Text = name;
            TextTitle = "Object Editor";

            Controls.Add(oec);

            openedFile = o;
            oec.propertyGrid.SelectedObject = openedFile;
        }
    }
}
