using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using SMHEditor.DockingModules.ProjectExplorer;
using Newtonsoft.Json;
using Ionic.Zip;
using SMHEditor.Project.FileTypes;

namespace SMHEditor.ZipDir
{
    public interface IZipVirtualFile
    {
        string Serialize();
    }

    //public class VirtualFileManager

	

  //  public class ZipVirtualDirectoryManager
  //  {
		//public class ZipVirtualDirectory
  //      {
  //          public ZipVirtualDirectory(string name, ZipVirtualDirectoryManager owner)
  //          {
  //              manager = owner;
  //              dirName = name;
  //              dirFolder = name.ToLower() + "/";
  //              dirManifest = dirFolder + ".dir";

		//		if(owner.fileStore.ContainsEntry(dirManifest))
  //              {
  //                  ZipEntry e = owner.fileStore.SelectEntries(dirManifest).First();

  //                  int len = (int)e.OpenReader().Length;
  //                  byte[] data = new byte[len];
  //                  e.OpenReader().Read(data, 0, len);
  //                  nodes = JsonConvert.DeserializeObject<Dictionary<string, Node>>(Encoding.UTF8.GetString(data));
  //              }
		//		else
  //              {
  //                  Node n = new Node();
  //                  n.name = name;
  //                  nodes.Add("/dir:", n);
  //              }
  //          }

  //          public class Node
  //          {
  //              public enum NodeType
  //              {
  //                  ROOT_DATA,
  //                  ROOT_ART,
  //                  FOLDER,
  //                  OBJECT,
  //              }
  //              public NodeType type;
  //              public string name;
  //              public string fullName;
  //              public string diskName = null;
  //              public IZipVirtualFile file;
  //              public List<string> children = new List<string>();

		//		//constructs visual tree from this node.
		//		public KryptonTreeNode BuildVisualNode(Dictionary<string, Node> nodes)
  //              {
  //                  KryptonTreeNode n = new KryptonTreeNode();
  //                  n.Text = name;
  //                  n.Name = fullName;
  //                  n.Tag = this;

		//			switch(type)
  //                  {
  //                      case NodeType.ROOT_DATA:
  //                          n.ImageIndex = ProjectExplorerControl.ImageIndex["dir_data"];
  //                          n.SelectedImageIndex = ProjectExplorerControl.ImageIndex["dir_data"];
  //                          break;

  //                      case NodeType.FOLDER:
  //                          n.ImageIndex = ProjectExplorerControl.ImageIndex["folder"];
  //                          n.SelectedImageIndex = ProjectExplorerControl.ImageIndex["folder"];
  //                          break;

  //                      case NodeType.OBJECT:
  //                          n.ImageIndex = ProjectExplorerControl.ImageIndex["object"];
  //                          n.SelectedImageIndex = ProjectExplorerControl.ImageIndex["object"];
  //                          break;
  //                  }

  //                  foreach (string c in children)
  //                  {
  //                      n.Nodes.Add(nodes[c].BuildVisualNode(nodes));
  //                  }
  //                  return n;
  //              }
  //          }
  //          private Dictionary<string, Node> nodes = new Dictionary<string, Node>();
  //          private Node GetOrCreateNode(string dir, string name)
  //          {
  //              if (nodes.Keys.Contains(dir))
  //              {
  //                  return nodes[dir];
  //              }
  //              else
  //              {
  //                  Node n = new Node();
  //                  n.name = name;
  //                  n.fullName = dir;

  //                  if (name.Contains(".obj"))
  //                  {
  //                      n.type = Node.NodeType.OBJECT;
  //                      n.file = new ObjectFile();
  //                  }
  //                  //else if
  //                  //else if
  //                  else n.type = Node.NodeType.FOLDER;


		//			if(n.type != Node.NodeType.FOLDER)
  //                  {
  //                      Guid guid = Guid.NewGuid();
  //                      n.diskName = guid.ToString();
  //                      manager.fileStore.AddEntry(dirFolder + guid.ToString(), "");
  //                  }

  //                  nodes.Add(dir, n);
  //                  return n;
  //              }
  //          }
  //          private Node BuildNode(string[] path, int cur)
  //          {
  //              string concat = "";
  //              for (int i = 0; i <= cur; i++)
  //              {
  //                  concat += "/" + path[i];
  //              }

  //              Node n = GetOrCreateNode(concat, path[cur]);
  //              if (cur < path.Count() - 1)
  //              {
  //                  Node c = BuildNode(path, cur + 1);
  //                  if (!n.children.Contains(c.fullName))
  //                      n.children.Add(c.fullName);
  //              }

  //              return n;
  //          }

  //          public void AddFile(string dir)
  //          {
  //              string[] split = dir.Split('/');
  //              split = split.Where(s => s != "").ToArray();

  //              BuildNode(split, 0);
  //          }
		//	public void UpdataFile(string dir, string data)
  //          {
  //              string s = dirFolder + dir.Substring(dir.IndexOf("dir:", 0));
  //              manager.fileStore.UpdateEntry(s, data);
  //          }

  //          public void Save()
  //          {
  //              manager.fileStore.UpdateEntry(dirManifest, JsonConvert.SerializeObject(nodes));
  //              foreach(Node n in nodes.Values)
  //              {
  //                  if(n.file != null)
  //                  {
  //                      manager.fileStore.UpdateDirectory(dirFolder + n.diskName, n.file.Serialize());
  //                  }
  //              }
  //          }

		//	//Visual view
		//	public KryptonTreeNode GetCurrentTreeView()
  //          {
  //              return nodes["/dir:"].BuildVisualNode(nodes);
  //          }

  //          private ZipVirtualDirectoryManager manager;
  //          private string dirManifest;
  //          private string dirFolder;
  //          private string dirName;
  //      };

  //      string typeID;
  //      public ZipVirtualDirectoryManager(string _typeID)
  //      {
  //          typeID = _typeID;
			
  //          fileStore = new ZipFile();
  //          fileStore.AddEntry("." + typeID, new byte[] { });

  //          fileStore.AddDirectoryByName(DATADIR_NAME);
  //          dataDir = new ZipVirtualDirectory("Data", this);
  //      }

  //      string loadedDir;
  //      public void Open(string dir)
  //      {
  //          string name = Path.GetFileNameWithoutExtension(dir);

  //          if (File.Exists(dir))
  //          {
  //              if (!ZipFile.IsZipFile(dir)) throw new Exception("Selected file was not a zip file.");
  //              fileStore = ZipFile.Read(dir);
                
  //              //if it doesnt have an id file, its not correct.
  //              if (!fileStore.ContainsEntry("." + typeID)) throw new Exception("Selected zip did not have valid ID file.");
  //              else infoFile = fileStore.SelectEntries("." + typeID).First();

		//		//doesnt have data dir? make it.
  //              if (!fileStore.ContainsEntry(DATADIR_NAME))
  //              {
  //                  fileStore.AddDirectoryByName(DATADIR_NAME);
  //              }
  //              dataDir = new ZipVirtualDirectory("Data", this);

  //              loadedDir = dir;
  //          }
  //          else throw new Exception("No file found.");
  //      }
  //      public void Save(string dir)
  //      {
  //          dataDir.Save();
  //          fileStore.Save(dir);
  //      }
		        
  //      ZipFile fileStore;
  //      ZipEntry infoFile;

  //      //directories - when adding new ones be sure to append Open() and the Ctor.
  //      const string DATADIR_NAME = "data/";
  //      public ZipVirtualDirectory dataDir;
  //  }
}
