using ComponentFactory.Krypton.Navigator;
using SMHEditor.Project.FileTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Platform.Windows;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Drawing;
using OpenTK;

namespace SMHEditor.DockingModules.MapEditor
{
    public class ViewportScene
    {
        public class SceneObject
        {
            public SceneObject()
            {
            }

            public Transform transform = new Transform();
            public Dictionary<string, int> vertexBuffers = new Dictionary<string, int>();
            public int indexBuffer;

            public virtual void Draw(int shader) { }
        }

        public ViewportPage host;
        public Camera camera;
        protected Dictionary<int, List<SceneObject>> objects = new Dictionary<int, List<SceneObject>>();
        public bool cameraPosLocked;
        public ViewportScene(bool cameraPosLock)
        {
            cameraPosLocked = cameraPosLock;
            camera = new Camera();
        }
        public virtual void DrawScene()
        {
            camera.SetScreenDims(host.viewport.glControl.Width, host.viewport.glControl.Height);
            camera.UpdateViewMatrix();
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, camera.mDataBuff);
            foreach(var l in objects)
            {
                GL.UseProgram(l.Key);
                foreach(var o in l.Value)
                {
                    camera.SetModelMatrix(o.transform.GetModelMatrix());
                    camera.UpdateCameraBuffer();
                    o.Draw(l.Key);
                }
            }
        }
    }


    public class ViewportPage : KryptonPage
    {
        public ViewportControl viewport;
        private ViewportScene activeScene;
        private Timer renderInterval;
        
        public ViewportPage()
        {
            viewport = new ViewportControl();
            viewport.Dock = System.Windows.Forms.DockStyle.Fill;
            viewport.glControl.Paint += new System.Windows.Forms.PaintEventHandler(OnFrame);
            viewport.glControl.HandleDestroyed += new EventHandler(Dispose);
            viewport.glControl.Resize += new EventHandler(OnResize);
            Controls.Add(viewport);

            renderInterval = new Timer();
            renderInterval.Interval = 1; //1000 ticks/s
            renderInterval.Tick += new EventHandler(OnFrame);
            renderInterval.Start();

            viewport.glControl.MakeCurrent();
        }

        Point lastMouse; float lastScroll;
        //RenderTick causes the OnFrame event to be fired.
        private void OnFrame(object o, EventArgs e)
        {
            GL.ClearColor(.11f, .11f, .115f, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            if (activeScene != null)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // INPUT
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var keyboard = OpenTK.Input.Keyboard.GetState();
                var mouse = OpenTK.Input.Mouse.GetCursorState();
                var mouseLoc = new Point(mouse.X, mouse.Y);
                if (viewport.glControl.Focused && Util.DrawingPointIsInsideControl(mouseLoc, viewport.glControl))
                {

                    float deltaX = lastMouse.X - mouseLoc.X;
                    float deltaY = lastMouse.Y - mouseLoc.Y;
                    // pan camera
                    if (!activeScene.cameraPosLocked)
                    {
                        if (keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft) && mouse.IsButtonDown(OpenTK.Input.MouseButton.Middle))
                        {
                            activeScene.camera.MoveRelativeToScreen(deltaX, -deltaY);
                        }
                    }
                    // rotate camera
                    if (!keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft) && mouse.IsButtonDown(OpenTK.Input.MouseButton.Middle))
                    {
                        activeScene.camera.Rotate(deltaX / 100, -deltaY / 100);
                    }
                    // zoom
                    activeScene.camera.AddRadius(lastScroll - mouse.Scroll.Y);

                    lastMouse = new Point(OpenTK.Input.Mouse.GetCursorState().X, OpenTK.Input.Mouse.GetCursorState().Y);
                    lastScroll = mouse.Scroll.Y;
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                activeScene.DrawScene();
            }
            viewport.glControl.SwapBuffers();

        }
        private void Dispose(object o, EventArgs e)
        {
            renderInterval.Stop();
        }
        public void SetScene(ViewportScene scene)
        {
            activeScene = scene;
            activeScene.host = this;
            activeScene.camera.UpdateProjMatrix();
            activeScene.camera.UpdateCameraBuffer();
        }
        public void OnResize(object o, EventArgs e)
        {
            viewport.glControl.MakeCurrent();
            GL.Viewport(0, 0, viewport.glControl.Width, viewport.glControl.Height);
            if (activeScene != null)
            {
                activeScene.camera.SetScreenDims(viewport.glControl.Width, viewport.glControl.Height);
                activeScene.camera.UpdateProjMatrix();
                activeScene.camera.UpdateCameraBuffer();
            }
        }
    }
}
