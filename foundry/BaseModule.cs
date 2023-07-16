using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
	public abstract class BaseModule
	{
        /// <summary>
        /// Reference of the running FoundryInstance that this module is loaded into.
        /// </summary>
        public FoundryInstance Instance { get; private set; }
        /// <summary>
        /// The extension to use when saving an intermediate project file.
        /// If it is not overridden this module will not have saving/loading functionality.
        /// </summary>
        public virtual string SaveExt { get { return null; } }
        /// <summary>
        /// The extension to use when importing a game asset file into the project.
        /// If it is not overridden this module will not have importing functionality.
        /// </summary>
        public virtual string ImportExt { get { return null; } }
        /// <summary>
        /// The type of page to instantiate.
        /// Must be a type that inherits from BaseEditorPage, otherwise the module will not be loaded.
        /// </summary>
        public abstract Type PageType { get; }

        public void Init(FoundryInstance i)
        {
            Instance = i;
            OnInit();
        }
        public void Import(string importedFile, string destinationPath)
        {
            OnImport(importedFile, destinationPath);
        }

        /// <summary>
        /// Occurs when the module is loaded.
        /// Go nuts.
        /// </summary>
        protected virtual void OnInit()
        { 
        
        }

        protected virtual void OnImport(string importedFile, string destinationPath)
        {

        }
	}
}
