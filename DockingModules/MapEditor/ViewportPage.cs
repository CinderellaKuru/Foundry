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
        }

        public ViewportPage host;
        public Camera camera;
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
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 1, camera.colorBuff);
        }
    }


    public class ViewportPage : KryptonPage
    {
        public ViewportControl viewport;
        private ViewportScene activeScene;
        private Timer renderInterval;
        int pickFBOColor;
        int pickFBODepth;
        int pickFBO;

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
            OnResize(null, null);


            pickFBO = GL.GenFramebuffer();
            pickFBOColor = GL.GenTexture();
            pickFBODepth = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, pickFBOColor);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, viewport.glControl.Width, viewport.glControl.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, (IntPtr)0);

            GL.BindTexture(TextureTarget.Texture2D, pickFBODepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, viewport.glControl.Width, viewport.glControl.Height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, (IntPtr)0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, pickFBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, pickFBOColor, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, pickFBODepth, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

        }

        Point lastMouse; float lastScroll;
        //RenderTick causes the OnFrame event to be fired.
        private void OnFrame(object o, EventArgs e)
        {
            SetRenderTarget(RenderTarget.PICK);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            viewport.glControl.MakeCurrent();
            SetRenderTarget(RenderTarget.DEFAULT);
            GL.ClearColor(.11f, .11f, .115f, 1);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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
                    float multiplier = 10f;
                    if (keyboard.IsKeyDown(OpenTK.Input.Key.AltLeft)) multiplier = 30f;
                    float deltaX = lastMouse.X - mouseLoc.X;
                    float deltaY = lastMouse.Y - mouseLoc.Y;
                    // pan camera
                    if (!activeScene.cameraPosLocked)
                    {
                        if (keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft) && mouse.IsButtonDown(OpenTK.Input.MouseButton.Middle))
                        {
                            activeScene.camera.MoveRelativeToScreen(deltaX * multiplier, -deltaY * multiplier);
                        }
                    }
                    // rotate camera
                    if (!keyboard.IsKeyDown(OpenTK.Input.Key.ShiftLeft) && mouse.IsButtonDown(OpenTK.Input.MouseButton.Middle))
                    {
                        activeScene.camera.Rotate(deltaX / 100, -deltaY / 100);
                    }
                    // zoom
                    activeScene.camera.AddRadius((lastScroll - mouse.Scroll.Y) * multiplier);
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

            GL.BindTexture(TextureTarget.Texture2D, pickFBOColor);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, viewport.glControl.Width, viewport.glControl.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, (IntPtr)0);

            GL.BindTexture(TextureTarget.Texture2D, pickFBODepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, viewport.glControl.Width, viewport.glControl.Height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, (IntPtr)0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (activeScene != null)
            {
                activeScene.camera.SetScreenDims(viewport.glControl.Width, viewport.glControl.Height);
                activeScene.camera.UpdateProjMatrix();
                activeScene.camera.UpdateCameraBuffer();
            }
        }

        public enum RenderTarget
        {
            DEFAULT,
            PICK
        }
        public void SetRenderTarget(RenderTarget t)
        {
            switch(t)
            {
                case RenderTarget.DEFAULT:
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    break;
                case RenderTarget.PICK:
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, pickFBO);
                    break;
            }
        }

        public uint Pick(int x, int y)
        {
            SetRenderTarget(RenderTarget.PICK);
            uint pixel = 0;
            GL.ReadPixels(x, y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref pixel);
            SetRenderTarget(RenderTarget.DEFAULT);
            return pixel;
        }

        public void SetDepthTest(bool dt)
        {
            if (dt) GL.Enable(EnableCap.DepthTest);
            else GL.Disable(EnableCap.DepthTest);
        }
    }
}
