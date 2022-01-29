using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMHEditor.Project.FileTypes;
using SMHEditor.ZipDir;

namespace SMHEditor.Project
{    
    public class Project
    {
        public ZipDirectoryManager vfs;


        /// Files
        List<ObjectFile> objects;

        /// IO
        public static Project OpenProject(string path, bool overWrite)
        {
            Project p = new Project();
            p.vfs = new ZipDirectoryManager("Project");

            p.vfs.Open("C:/users/jaken/desktop/ok.test");
            p.vfs.Save("C:/users/jaken/desktop/ok.test");

            return p;
        }
        public void SaveProject(string path)
        {

        }
        public void CloseProject()
        {

        }
        ///
    }
    
}
