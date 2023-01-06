using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Foundry.Project.Modules.Base
{
    public abstract class BaseSceneEditorPage : DockContent
    {
        private FoundryInstance instance;
        bool loaded = false;
        bool edited = false;
        public BaseSceneEditorPage(FoundryInstance i)
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
            OnTick();
            if (renderTimer.ElapsedMilliseconds > renderIntervalMilliseconds)
            {
                OnDraw();
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
                    edited = true;
                    OnEdit();
                    return true;
                }
            }
            return false;
        }
        public bool TryOpen(string file, DockPanel location, DockState state)
        {
            if (!loaded)
            {
                if (File.Exists(file))
                {
                    edited = false;

                    if (OnLoadFile(file))
                    {
                        Show(location, state);
                        OnTick();
                        OnDraw();
                        return true;
                    }
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
                    if (MessageBox.Show("Are you sure you want to close this page? Any unsaved progress will be lost.", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        edited = false;
                        Close();
                        return true;
                    }
                    return false;
                }
                else
                {
                    Close();
                    return true;
                }
            }
            return false;
        }
        public bool TrySave(string file)
        {
            if (loaded)
            {
                if (edited)
                {
                    if (OnSaveFile(file))
                    {
                        edited = false;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool TrySaveAs(string file)
        {
            if (loaded)
            {
                if (OnSaveFile(file))
                {
                    edited = false;
                    return true;
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
		/// Occurs when the editor is loaded from a file.
		/// </summary>
		/// <param name="file">The file to load. Can assume the file is valid for this editor.</param>
		/// <returns>If file was successfully loaded into the editor.</returns>
        protected virtual bool OnLoadFile(string file) { return true; }
		/// <summary>
		/// Occurs when the editor is saved.
		/// </summary>
		/// <param name="file">The file to serialize the editor data to. Can overwrite existing files.</param>
		/// <returns>If the data was successfully saved to the file.</returns>
        protected virtual bool OnSaveFile(string file) { return true; }
		/// <summary>
		/// Occurs when the editor is resized.
		/// </summary>
		protected virtual void OnResize() { }
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
