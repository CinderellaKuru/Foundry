using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static Foundry.Project.FoundryInstance;

namespace Foundry.Project.Modules.Base
{
	public abstract class BaseEditorPage : DockContent
	{
		private FoundryInstance instance;
		private string cachedFileName = null;
		private bool loaded = false;
		private bool edited = false;
		public BaseEditorPage(FoundryInstance i)
		{
			instance = i;
			SetRenderInterval(16);

			mouseState = new MouseState();
			downKeys = new List<Keys>();

			ControlAdded += new ControlEventHandler(InternalOnly_ControlAdded);
			FormClosing += new FormClosingEventHandler(Internal_Closed);
			Resize += new EventHandler(Internal_Resize);

			MouseMove += new MouseEventHandler(Internal_MouseMoved);
			MouseWheel += new MouseEventHandler(Internal_MouseWheelMoved);
			MouseDown += new MouseEventHandler(Internal_MouseButtonDown);
			MouseUp += new MouseEventHandler(Internal_MouseButtonUp);

			KeyDown += new KeyEventHandler(Internal_KeyDown);
			KeyUp += new KeyEventHandler(Internal_KeyUp);

			renderTimer.Start();
		}
		protected FoundryInstance Instance()
		{
			return instance;
		}
		protected abstract string GetSaveExtension();
		protected abstract string GetImportExtension();


		#region internal events
		private void InternalOnly_ControlAdded(object o, ControlEventArgs e)
		{
			e.Control.MouseMove += new MouseEventHandler(Internal_MouseMoved);
			e.Control.MouseWheel += new MouseEventHandler(Internal_MouseWheelMoved);
			e.Control.MouseDown += new MouseEventHandler(Internal_MouseButtonDown);
			e.Control.MouseUp += new MouseEventHandler(Internal_MouseButtonUp);

			e.Control.KeyDown += new KeyEventHandler(Internal_KeyDown);
			e.Control.KeyUp += new KeyEventHandler(Internal_KeyUp);
		}
		private void Internal_Closed(object o, FormClosingEventArgs e)
		{
			Controls.Clear();
			OnClose();
		}
		private void Internal_Resize(object o, EventArgs e)
		{
			if (!Disposing)
			{
				OnResize();
			}
		}

		//mouse
		protected struct MouseState
		{
			public bool leftDown, rightDown, middleDown;
			public int X, Y;
			public int deltaX, deltaY, deltaScroll;
		}
		private MouseState mouseState;
		private void Internal_MouseMoved(object o, MouseEventArgs e)
		{
			mouseState.deltaScroll = 0;
			mouseState.deltaX = mouseState.X - e.X;
			mouseState.deltaY = mouseState.Y - e.Y;
			mouseState.X = e.X;
			mouseState.Y = e.Y;
			Internal_Tick();
		}
		private void Internal_MouseWheelMoved(object o, MouseEventArgs e)
		{
			mouseState.deltaScroll = e.Delta;
			Internal_Tick();
		}
		private void Internal_MouseButtonDown(object o, MouseEventArgs e)
		{
			mouseState.deltaScroll = 0;
			if (e.Button == MouseButtons.Left)
			{
				mouseState.leftDown = true;
			}
			if (e.Button == MouseButtons.Right)
			{
				mouseState.rightDown = true;
			}
			if (e.Button == MouseButtons.Middle)
			{
				mouseState.middleDown = true;
			}
			Internal_Tick();
		}
		private void Internal_MouseButtonUp(object o, MouseEventArgs e)
		{
			mouseState.deltaScroll = 0;
			if (e.Button == MouseButtons.Left)
			{
				mouseState.leftDown = false;
			}
			if (e.Button == MouseButtons.Right)
			{
				mouseState.rightDown = false;
			}
			if (e.Button == MouseButtons.Middle)
			{
				mouseState.middleDown = false;
			}
			Internal_Tick();
		}
		protected MouseState GetMouseState()
		{
			return mouseState;
		}

		//keyboard
		private List<Keys> downKeys;
		private void Internal_KeyDown(object o, KeyEventArgs e)
		{
			if (!downKeys.Contains(e.KeyCode))
			{
				downKeys.Add(e.KeyCode);
			}
			Internal_Tick();
		}
		private void Internal_KeyUp(object o, KeyEventArgs e)
		{
			if (downKeys.Contains(e.KeyCode))
			{
				downKeys.RemoveAll(x => x == e.KeyCode);
			}
			Internal_Tick();
		}
		protected bool GetKeyIsDown(Keys k)
		{
			return downKeys.Contains(k);
		}

		//tick
		public void SetRenderInterval(int milliseconds) { renderIntervalMilliseconds = milliseconds; }
		private int renderIntervalMilliseconds;
		private Stopwatch renderTimer = new Stopwatch();
		/// <summary>
		/// Calls OnTick, and when enough time has passed, OnDraw().
		/// </summary>
		private void Internal_Tick()
		{
			try
			{
				OnTick();
			}
			catch (Exception e)
			{
				Instance().AppendLog(LogEntryType.Error, "Internal_Tick(): OnTick() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}", GetType().Name, cachedFileName, e.Message));
			}

			if (renderTimer.ElapsedMilliseconds > renderIntervalMilliseconds)
			{
				try
				{
					OnDraw();
				}
				catch (Exception e)
				{
					Instance().AppendLog(LogEntryType.Error, "Internal_Tick(): OnDraw() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}", GetType().Name, cachedFileName, e.Message));
				}
				renderTimer.Restart();
			}
		}
		#endregion


		#region external interactions
		public bool TrySetEdited()
		{
			if (loaded)
			{
				if (!edited)
				{
					try
					{
						OnEdit();
						edited = true;
						return true;
					}
					catch (Exception e)
					{
						Instance().AppendLog(LogEntryType.Error, "TrySetEdited(): OnEdit() encountered an error. See console for details.", true,
						string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}", GetType().Name, cachedFileName, e.Message));
						return false;
					}
				}
			}
			return false;
		}
		public bool TryOpen(string file)
		{
			if (File.Exists(file) && Path.GetExtension(file) == GetSaveExtension() && !loaded)
			{
				try
				{
					if (OnLoadFile(file))
					{
						edited = false;
						loaded = true;
						cachedFileName = file;
						return true;
					}
				}
				catch (Exception e)
				{
					Instance().AppendLog(LogEntryType.Error, "TryOpen(): OnLoadFile() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Tried file: {1}\n--Exception information: {2}", GetType().Name, file, e.Message));
					return false;
				}
			}
			return false;
		}
		public bool TryImport(string file)
		{
			if (!loaded && File.Exists(file) && Path.GetExtension(file) == GetImportExtension())
			{
				try
				{
					if (OnImportFile(file))
					{
						edited = true;
						loaded = true;
						return true;
					}
				}
				catch (Exception e)
				{
					Instance().AppendLog(LogEntryType.Error, "TryImport(): OnImportFile() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Tried file: {1}\n--Exception information: {2}", GetType().Name, file, e.Message));
					return false;
				}
			}
			return false;
		}
		public bool TryShow(DockPanel workspace, DockState state)
		{
			if (loaded)
			{
				try
				{
					Show(workspace, state);
					return true;
				}
				catch (Exception e)
				{
					Instance().AppendLog(LogEntryType.Error, "TryShow(): Show() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}", GetType().Name, cachedFileName, e.Message));
					return false;
				}
			}
			return false;
		}
		public bool TryClose(bool force = false)
		{
			if (loaded)
			{
				if (edited && !force)
				{
					//edited and do not force -- display prompt.
					if (MessageBox.Show("Are you sure you want to close this page? Any unsaved progress will be lost.", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						goto CLOSE_ROUTINE;
					}
					return false;
				}
				else
				{
					//either not edited, or is forced -- no prompt.
					goto CLOSE_ROUTINE;
				}
			}
			return false;

			CLOSE_ROUTINE:
			try
			{
				OnClose();
			}
			catch (Exception e)
			{
				Instance().AppendLog(LogEntryType.Error, "TryClose(): OnClose() encountered an error. See console for details.", true,
				string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}", GetType().Name, cachedFileName, e.Message));
				return false;
			}
			Close();
			edited = false;
			loaded = false;
			cachedFileName = null;
			return true;
		}
		public bool TrySave()
		{
			if (loaded && edited)
			{
				if (cachedFileName == null)
				{
					OpenFileDialog ofd = new OpenFileDialog();
					ofd.Filter = "Foundry Content (*" + GetSaveExtension() + ")|*" + GetSaveExtension();
					if (ofd.ShowDialog() == DialogResult.OK)
					{
						cachedFileName = ofd.FileName;
						Instance().ScanProjectDirectoryAndUpdate();
					}
				}
				if (Path.GetExtension(cachedFileName) == GetSaveExtension())
				{
					try
					{
						if (OnSaveFile(cachedFileName))
						{
							edited = false;
							return true;
						}
					}
					catch (Exception e)
					{
						Instance().AppendLog(LogEntryType.Error, "TrySave(): OnSaveFile() encountered an error. See console for details.", true,
						string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}", GetType().Name, cachedFileName, e.Message));
						return false;
					}
				}
				return false;
			}
			return false;

		}
		public bool TrySaveAs(string file)
		{
			if (loaded)
			{
				try
				{
					if (OnSaveFile(file))
					{
						edited = false;
						return true;
					}
				}
				catch (Exception e)
				{
					Instance().AppendLog(LogEntryType.Error, "TrySaveAs(): OnSaveFile() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Tried file: {2}\n--Exception information: {3}", GetType().Name, cachedFileName, file, e.Message));
					return false;
				}
			}
			return false;

		}
		public bool IsEdited()
		{
			return edited;
		}
		#endregion


		#region virtual functions
		/// <summary>
		/// Occurs when the editor is set to edited.
		/// </summary>
		protected virtual void OnEdit() { }
		/// <summary>
		/// Occurs when the editor is loaded from a saved file.
		/// Can assume the file is valid to be loaded into this editor.
		/// </summary>
		/// <returns>If file was successfully loaded into the editor.</returns>
		protected virtual bool OnLoadFile(string file) { return true; }
		/// <summary>
		/// Occurs when the editor is loaded from an imported file.
		/// Can assume the file is valid to be imported to this editor.
		/// </summary>
		/// <returns>If the file was successfully imported into the editor.</returns>
		protected virtual bool OnImportFile(string file) { return true; }
		/// <summary>
		/// Occurs when the editor is saved.
		/// Allowed to overwrite existing files.
		/// </summary>
		/// <returns>If the data was successfully saved to the file.</returns>
		protected virtual bool OnSaveFile(string file) { return true; }
		/// <summary>
		/// Occurs when the editor is resized.
		/// </summary>
		protected virtual void OnResize() { }
		/// <summary>
		/// Occurs when the editor's GUI is shown.
		/// </summary>
		protected virtual void OnShow() { }
		/// <summary>
		/// Occurs when the editor is closed.
		/// </summary>
		protected virtual void OnClose() { }
		/// <summary>
		/// Occurs whenever there is any input given to the editor.
		/// </summary>
		protected virtual void OnTick() { }
		/// <summary>
		/// Occurs on the next OnTick if more time than the render interval has passed.
		/// </summary>
		protected virtual void OnDraw() { }
		#endregion
	}
}
