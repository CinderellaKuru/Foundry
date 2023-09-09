using KSoft;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static foundry.FoundryInstance;

namespace foundry
{
	public abstract class BaseEditorPage : BaseToolPage
	{
		private string cachedFileName = null;
		private bool loaded = false;
		private bool edited = false;
        public BaseEditorPage()
		{
			mouseState = new MouseState();
			downKeys = new List<Keys>();

			ControlAdded += new ControlEventHandler(Internal_ControlAdded);
			Resize += new EventHandler(Internal_Resize);

			MouseMove += new MouseEventHandler(Internal_MouseMoved);
			MouseWheel += new MouseEventHandler(Internal_MouseWheelMoved);
			MouseDown += new MouseEventHandler(Internal_MouseButtonDown);
			MouseUp += new MouseEventHandler(Internal_MouseButtonUp);

			KeyDown += new KeyEventHandler(Internal_KeyDown);
			KeyUp += new KeyEventHandler(Internal_KeyUp);

			OnPageInit += (sender, e) =>
			{
				SetRenderInterval(16);
				renderTimer.Start();
			};
        }

		#region internal events
		private void Internal_ControlAdded(object o, ControlEventArgs e)
		{
			e.Control.ControlAdded += Internal_ControlAdded;

            e.Control.MouseMove += Internal_MouseMoved;
			e.Control.MouseWheel += Internal_MouseWheelMoved;
			e.Control.MouseDown += Internal_MouseButtonDown;
			e.Control.MouseUp += Internal_MouseButtonUp;

			e.Control.KeyDown += Internal_KeyDown;
			e.Control.KeyUp += Internal_KeyUp;
		}
		private void Internal_Resize(object o, EventArgs e)
		{
			if (!Disposing)
			{
                OnPageResize?.Invoke(this, null);
			}
		}

		//mouse
		protected struct MouseState
		{
			public bool leftDown, rightDown, middleDown;
			public bool leftDownLast, rightDownLast, middleDownLast;
            public int X, Y;
			public int deltaX, deltaY, deltaScroll;
		}
		private MouseState mouseState;
		private void Internal_MouseMoved(object o, MouseEventArgs e)
		{
			mouseState.deltaX = e.X - mouseState.X;
			mouseState.deltaY = e.Y - mouseState.Y;
			mouseState.X = e.X;
			mouseState.Y = e.Y;

			Internal_Tick();

			mouseState.deltaX = 0;
			mouseState.deltaY = 0;
		}
		private void Internal_MouseWheelMoved(object o, MouseEventArgs e)
		{
			mouseState.deltaScroll = e.Delta;

			Internal_Tick();

			mouseState.deltaScroll = 0;
		}
		private void Internal_MouseButtonDown(object o, MouseEventArgs e)
		{
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

            mouseState.leftDownLast = mouseState.leftDown;
            mouseState.rightDownLast = mouseState.rightDown;
            mouseState.middleDownLast = mouseState.middleDown;
        }
		private void Internal_MouseButtonUp(object o, MouseEventArgs e)
		{
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

            mouseState.leftDownLast = mouseState.leftDown;
            mouseState.rightDownLast = mouseState.rightDown;
            mouseState.middleDownLast = mouseState.middleDown;
        }

		protected MouseState GetMouseState() { return mouseState; }

		//keyboard
		private List<Keys> downKeys;
		private List<Keys> downKeysLast;
        private void Internal_KeyDown(object o, KeyEventArgs e)
		{
			if (!downKeys.Contains(e.KeyCode))
			{
				downKeys.Add(e.KeyCode);
			}

			Internal_Tick();

			downKeysLast = downKeys;
		}
		private void Internal_KeyUp(object o, KeyEventArgs e)
		{
			if (downKeys.Contains(e.KeyCode))
			{
				downKeys.RemoveAll(x => x == e.KeyCode);
			}

			Internal_Tick();

			downKeysLast = downKeys;
        }
        protected bool GetKeyIsDown(Keys k)
		{
			return downKeys.Contains(k);
		}
		protected bool GetKeyWasDown(Keys k)
		{
            return downKeysLast.Contains(k);
        }

        //tick
        public void SetRenderInterval(long milliseconds) { renderIntervalMilliseconds = milliseconds; }
        private Stopwatch renderTimer = new Stopwatch();
		private long renderIntervalMilliseconds;


        /// <summary>
        /// Calls OnTick, and when enough time has passed, OnDraw().
        /// </summary>
        private void Internal_Tick()
		{
			//Main Tick
            try { OnPageTick?.Invoke(this, null); }
			catch (Exception e) {
				Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnTick() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}'\n'--Stacktrace:{3}", GetType().Name, cachedFileName, e.Message, e.StackTrace));
			}

			//Dragging
			if (mouseState.leftDown && !mouseState.leftDownLast)
			{
				try { OnPageClickL?.Invoke(this, null); }
				catch (Exception e) {
                    Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDragStart() encountered an error. See console for details.", true,
                        string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}'\n'--Stacktrace:{3}", GetType().Name, cachedFileName, e.Message, e.StackTrace));
                }
			}
			if (mouseState.leftDown && mouseState.leftDownLast)
			{
				try { OnPageDragL?.Invoke(this, null); }
				catch (Exception e) {
                    Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDragging() encountered an error. See console for details.", true,
                        string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}'\n'--Stacktrace:{3}", GetType().Name, cachedFileName, e.Message, e.StackTrace));
                }
            }
            if (!mouseState.leftDown && mouseState.leftDownLast)
            {
                try { OnPageReleaseL?.Invoke(this, null); }
                catch (Exception e)   {
                    Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDragStop() encountered an error. See console for details.", true,
                        string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}'\n'--Stacktrace:{3}", GetType().Name, cachedFileName, e.Message, e.StackTrace));
                }
            }

            //Draw
            if (renderTimer.ElapsedMilliseconds > renderIntervalMilliseconds)
			{
                try { OnPageDraw?.Invoke(this, null); }
				catch (Exception e) {
					Instance.AppendLog(LogEntryType.Error, "Internal_Tick(): OnDraw() encountered an error. See console for details.", true,
					string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}'\n'--Stacktrace:{3}", GetType().Name, cachedFileName, e.Message, e.StackTrace));
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
						OnPageEdit?.Invoke(this, null);
						edited = true;
						return true;
					}
					catch (Exception e)
					{
						Instance.AppendLog(LogEntryType.Error, "TrySetEdited(): OnEdit() encountered an error. See console for details.", true,
							string.Format("--Error info:\n--Editor type: {0}\n--Loaded file: {1}\n--Exception information: {2}'\n'--Stacktrace:{3}", GetType().Name, cachedFileName, e.Message, e.StackTrace)
						);
						return false;
					}
				}
			}
			return false;
		}
		public bool IsEdited()
		{
			return edited;
		}

		public void Redraw()
		{
			OnPageDraw?.Invoke(this, null);
		}
		#endregion

		public event EventHandler OnPageSave;
		public event EventHandler OnPageSaveAs;
		public event EventHandler OnPageEdit;

		public event EventHandler OnPageTick;
		public event EventHandler OnPageDraw;
		public event EventHandler OnPageResize;

		public event EventHandler OnPageClickL;
		public event EventHandler OnPageDragL;
		public event EventHandler OnPageReleaseL;

        //#region virtual functions
        ///// <summary>
        ///// Occurs when the editor is set to edited.
        ///// </summary>
        //protected virtual void OnEdit() { }
        ///// <summary>
        ///// Occurs when the editor is loaded from a saved file.
        ///// Can assume the file is valid to be loaded into this editor.
        ///// </summary>
        ///// <returns>If file was successfully loaded into the editor.</returns>
        //protected virtual bool OnLoadFile(string file) { return true; }
        ///// <summary>
        ///// Occurs when the editor is loaded from an imported file.
        ///// Can assume the file is valid to be imported to this editor.
        ///// </summary>
        ///// <returns>If the file was successfully imported into the editor.</returns>
        //protected virtual bool OnImportFile(string file) { return true; }
        ///// <summary>
        ///// Occurs when the editor is saved.
        ///// Allowed to overwrite existing files.
        ///// </summary>
        ///// <returns>If the data was successfully saved to the file.</returns>
        //protected virtual bool OnSaveFile(string file) { return true; }
        ///// <summary>
        ///// Occurs when the editor is resized.
        ///// </summary>
        //protected virtual void OnResize() { }
        ///// <summary>
        ///// Occurs whenever there is any input given to the editor.
        ///// </summary>
        //protected virtual void OnTick() { }
        //      protected virtual void OnClickL() { }
        //protected virtual void OnDragL() { }
        //protected virtual void OnReleaseL() { }
        ///// <summary>
        ///// Occurs on the next OnTick if more time than the render interval has passed.
        ///// </summary>
        //protected virtual void OnDraw() { }
        //#endregion
    }
}
