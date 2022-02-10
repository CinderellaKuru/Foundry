using ComponentFactory.Krypton.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.DockingModules
{
    public class EditorPage : KryptonPage
    {
        string _fileName;
        public EditorPage(string fileName)
        {
            _fileName = fileName;

            if (!MainWindow.project.openPages.Keys.Contains(_fileName))
                MainWindow.project.openPages.Add(_fileName, this);

            GotFocus += new EventHandler(OnFocus);
            LostFocus += new EventHandler(OnUnfocus);
            Disposed += new EventHandler(OnClose);
        }
        private void OnClose(object o, EventArgs e)
        {
            if(MainWindow.project.openPages.Keys.Contains(_fileName))
                MainWindow.project.openPages.Remove(_fileName);
        }
        private void OnFocus(object o, EventArgs e)
        {
            MainWindow.project.activePage = this;
        }
        private void OnUnfocus(object o, EventArgs e)
        {
            MainWindow.project.activePage = null;
        }

        public void Save()
        {
            MainWindow.project.SaveFile(_fileName);
            OnSave();
        }
        public virtual void OnSave() { }
    }
}
