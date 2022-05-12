using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using OpenTK;
using OpenTK.Input;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public partial class TriggerscripterControl : UserControl
    {
        Timer t = new Timer();
        List<TriggerScripterNode> nodes = new List<TriggerScripterNode>();

        public TriggerscripterControl()
        {
            InitializeComponent();
            Tick(null, null);
            Paint += new PaintEventHandler(DrawControl);
            DoubleBuffered = true;
            t.Interval = 1;
            t.Tick += Tick;
            t.Start();

            TriggerScripterNode n = new TriggerScripterNode(0, 0);
            n.inputSockets.Add(new TriggerScripterSocket("socket", Color.Black));
            n.inputSockets.Add(new TriggerScripterSocket("socket", Color.White));
            nodes.Add(n);
        }
        void Tick(object o, EventArgs e)
        {
            MouseState mouse = Mouse.GetCursorState();

            UpdateMatrices();

            Point min = PointToScreen(new Point(0, 0));
            Point max = PointToScreen(new Point(Width, Height));
            if (mouse.X > min.X && mouse.X < max.X)
            {
                if(mouse.Y > min.Y && mouse.Y < max.Y)
                {
                    PollMouse(mouse);
                }
            }

            lastX = mouse.X;
            lastY = mouse.Y;
            lastM = mouse.Scroll.Y;
            Invalidate();
        }


        public int majorGridSpace = 75;
        Pen gridPen = new Pen(Color.DimGray);
        void DrawControl(object o, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Transform = transform;
            g.Clear(Program.window.darkmode.GetBackColor1(
                ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient,
                ComponentFactory.Krypton.Toolkit.PaletteState.Normal));

            PointF[] points = new PointF[]
            {
                new PointF(e.ClipRectangle.Left, e.ClipRectangle.Top),
                new PointF(e.ClipRectangle.Right, e.ClipRectangle.Bottom)
            };

            transformInv.TransformPoints(points);

            float left = points[0].X;
            float right = points[1].X;
            float top = points[0].Y;
            float bottom = points[1].Y;

            int xOffs = (int)Math.Round(left / (float)majorGridSpace) * majorGridSpace;
            int yOffs = (int)Math.Round(top / (float)majorGridSpace) * majorGridSpace;

            for(int x = xOffs; x < right; x += majorGridSpace)
            {
                g.DrawLine(gridPen, x, top, x, bottom);
            }
            for (int y = yOffs; y < bottom; y += majorGridSpace)
            {
                g.DrawLine(gridPen, left, y, right, y);
            }

            foreach(TriggerScripterNode n in nodes)
            {
                n.Draw(e);
            }
        }

        int x = 0, y = 0;
        float zoom = 1.0f;
        float zoomMax = 2.5f, zoomMin = .25f;
        private Matrix transform = new Matrix();
        private Matrix transformInv = new Matrix();
        void UpdateMatrices()
        {
            transform.Reset();
            transform.Translate(x, y);
            transform.Scale(zoom, zoom);

            transformInv.Reset();
            transformInv.Scale(1.0f / zoom, 1.0f / zoom);
            transformInv.Translate(-x, -y);
        }

        
        float lastX = 0, lastY = 0, lastM = 0;
        void PollMouse(MouseState m)
        {
            if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
            {
                Point[] p = new Point[] { PointToClient(new Point(m.X, m.Y)) };
                transformInv.TransformPoints(p);


            }

            if (m.MiddleButton == OpenTK.Input.ButtonState.Pressed)
            {
                x += m.X - (int)lastX;
                y += m.Y - (int)lastY;
            }
            
            zoom += (m.Scroll.Y - lastM) / 20;
            zoom = zoom < zoomMin ? zoomMin : zoom;
            zoom = zoom > zoomMax ? zoomMax : zoom;
        }
    }
}
