using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using YAXLib.Enums;
using YAXLib.Attributes;
using Aga.Controls.Tree;
using System.ComponentModel;

namespace hwFoundry.Project
{
    #region General
    /// <summary>
    /// The container used to de/serialize a Foundry project from/to XML.
    /// </summary>
    public class SerializableModProject
    {
        // Members
        [YAXSerializeAs("FolderData")]
        [YAXDictionary(EachPairName = "Folder", KeyName = "Name", ValueName = "Data", SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Element)]
        public Dictionary <string, FolderData> Hierarchy { get; set; }

        // Constructor
        public SerializableModProject() { Hierarchy = new(); }


        // Internal Types
        public class FolderData
        {
            // Members
            [YAXSerializeAs("Folded")]
            public bool IsFolded { get; set; }

            // Constructor
            public FolderData() { IsFolded = true; }
        }
    }
    #endregion

    #region Explorer

    /// <summary>
    /// The nodes in the Project Explorer
    /// </summary>
    public class EntryNode : Node
    {
        private string _fullPath = string.Empty;
        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        private string _subName = string.Empty;
        public string SubName
        {
            get { return _subName; }
            set { _subName = value; }
        }
    }

    /// <summary>
    /// Files that are part of the Foundry project.
    /// Attaches itself to the nodes of the Project Explorer.
    /// </summary>
    public class ContentFile
    {
        // Members
        private string _pathOnDisk;
        [Category("File"), Description("Location of the file on disk.")]
        public string PathOnDisk
        {
            get { return _pathOnDisk; }
            set { _pathOnDisk = value; }
        }

        private bool _includeInExport;
        [Category("File"), Description("Include this file in the project export.")]
        public bool IncludeInExport
        {
            get { return _includeInExport; }
            set { _includeInExport = value; }
        }

        public bool IsDirty = false;


        // Constructor
        public ContentFile(string filename) { _pathOnDisk = filename; }


        // Virtual Methods
        protected virtual void Save() { }
        protected virtual void Open(string subName) { }
        protected virtual void Import(string fileName) { }


        // Other Methods
        public virtual EntryNode GetRootNode()
        {
            return new()
            {
                Text = Path.GetFileName(_pathOnDisk),
                Image = Properties.Resources.page_white
            };
        }

        #region Events
        internal void MarkEdited() => IsDirty = true;
        internal void OpenFile(string subName) => Open(subName);
        internal void SaveFile()
        {
            if (IsDirty) { Save(); }
            IsDirty = false;
        }
        internal void ImportFile(string fileName) => Import(fileName);
        #endregion
    }

    #endregion
}
