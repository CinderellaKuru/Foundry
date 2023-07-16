using Foundry.Project.Modules.XmlEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Foundry.Project.Modules.Xml
{
	public class ObjectEditorPage : XmlEditorPage
	{
		public ObjectEditorPage(FoundryInstance i) : base(i)
		{
			
		}

		//cache it to avoid having the whole document loaded for each object.
		private static Dictionary<string, XDocument> loadedObjectXmls = new Dictionary<string, XDocument>();
		private XDocument docRef = null;

		protected override bool OnImportFile(string file)
		{
			if (!loadedObjectXmls.ContainsKey(file))
			{
				loadedObjectXmls.Add(file, XDocument.Load(file));
			}
			docRef = loadedObjectXmls[file];
			return true;
		}
		public bool OpenObject(string objectName)
		{
			if (docRef == null)
			{
				Instance().AppendLog(FoundryInstance.LogEntryType.DebugError, "You cannot call OpenObject() without first opening an objects.xml file.", false);
				return false;
			}
			try //get selected element.
			{
				XElement foundObject = null;
				var elements = docRef.Root.Elements();
				foreach (var e in elements.Where(x=>x.Name == "object"))
				{
					if (e.HasAttributes)
					{
						if (e.Attributes().Where(x => x.Name == "name").Count() > 0)
						{
							if (e.Attribute("name").Value == objectName)
							{
								foundObject = e;
							}
						}
					}
				}
				if (foundObject == null)
				{
					Instance().AppendLog(FoundryInstance.LogEntryType.Warning, String.Format("Object {0} was not found in the selected xml file.", objectName), true);
					return false;
				}
			}
			catch (Exception e)
			{
				Instance().AppendLog(FoundryInstance.LogEntryType.Warning, "There was an error parsing the loaded xml file.", true, e.Message);
				return false;
			}
			return true;
		}
	}
}
