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

        PaintEventHandler nodeDraw;
        PaintEventHandler socketDraw;
        PaintEventHandler connectionDraw;


        public TriggerscripterControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Tick(null, null);

            Paint += new PaintEventHandler(DrawControl);

            t.Interval = 1;
            t.Tick += Tick;
            t.Start();

            TriggerScripterNode n = new TriggerScripterNode(0, 0);
            n.AddSocket(true, "node1", Color.Black);
            n.AddSocket(true, "node2", Color.Black);
            n.AddSocket(false, "node3", Color.Black);
            nodes.Add(n);

            TriggerScripterNode n2 = new TriggerScripterNode(300, 0);
            n2.AddSocket(true, "node1", Color.Black);
            n2.AddSocket(true, "node2", Color.Black);
            n2.AddSocket(false, "node3", Color.Black);
            nodes.Add(n2);

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


        Pen gridPen = new Pen(Color.DimGray);
        public int majorGridSpace = 150;
        int x = 0, y = 0;
        float zoom = 1.0f;
        float zoomMax = 1.5f, zoomMin = .1f;
        private Matrix transform = new Matrix();
        private Matrix transformInv = new Matrix();

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

            for (int x = xOffs; x < right; x += majorGridSpace)
            {
                g.DrawLine(gridPen, x, top, x, bottom);
            }
            for (int y = yOffs; y < bottom; y += majorGridSpace)
            {
                g.DrawLine(gridPen, left, y, right, y);
            }

            //draw nodes
            foreach (TriggerScripterNode n in nodes)
            {
                n.Draw(e);
            }
            //draw sockets
            foreach (TriggerScripterNode n in nodes)
            {
                foreach(TriggerScripterSocket s in n.sockets.Values)
                {
                    s.Draw(e);
                }
            }
            //draw connections
            foreach (TriggerScripterNode n in nodes)
            {
                foreach(TriggerScripterSocket s in n.sockets.Values)
                {
                    if(s is TriggerScripterSocketOutput)
                    {
                        ((TriggerScripterSocketOutput)s).DrawConnections(e);
                    }
                }
            }
            //draw temp connection
            Point[] pos = new Point[] { PointToClient(new Point((int)lastX, (int)lastY)) };
            transformInv.TransformPoints(pos);
            if (selectedSocket != null)
            {
                e.Graphics.DrawLine(TriggerScripterSocketOutput.connectionPen, new Point(
                    selectedSocket.node.x + selectedSocket.rect.X + selectedSocket.rect.Width / 2,
                    selectedSocket.node.y + selectedSocket.rect.Y + selectedSocket.rect.Height / 2),
                    pos[0]);
            }
        }
        void UpdateMatrices()
        {
            transform.Reset();
            transform.Translate(x, y);
            transform.Scale(zoom, zoom);

            transformInv.Reset();
            transformInv.Scale(1.0f / zoom, 1.0f / zoom);
            transformInv.Translate(-x, -y);
        }

        
        TriggerScripterSocket selectedSocket = null;
        bool lastClicked = false;
        float lastX = 0, lastY = 0, lastM = 0;
        void PollMouse(MouseState m)
        {
            //translated mouse pos
            Point[] p = new Point[] { PointToClient(new Point(m.X, m.Y)) };
            transformInv.TransformPoints(p);

            
            if (!lastClicked)
            {
                if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
                {
                    OnClick(p[0].X, p[0].Y);
                    lastClicked = true;
                }
                else lastClicked = false;
            }
            if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
            {
                OnMouseHeld(p[0].X, p[0].Y);
            }
            else
            {
                lastClicked = false;

                if(selectedSocket != null)
                {
                    foreach(TriggerScripterNode n in nodes)
                    {
                        foreach(TriggerScripterSocket s in n.sockets.Values)
                        {
                            if (s.MouseIsIn(p[0].X, p[0].Y))
                            {
                                if (selectedSocket.node != s.node && (
                                     (selectedSocket is TriggerScripterSocketInput  && s is TriggerScripterSocketOutput) ||
                                     (selectedSocket is TriggerScripterSocketOutput && s is TriggerScripterSocketInput))
                                    )
                                {
                                    TriggerScripterSocketOutput outSocket;
                                    if (selectedSocket is TriggerScripterSocketOutput)
                                    {
                                        outSocket = selectedSocket as TriggerScripterSocketOutput;
                                        outSocket.Connect(s);
                                    }
                                    if (s is TriggerScripterSocketOutput)
                                    {
                                        outSocket = s as TriggerScripterSocketOutput;
                                        outSocket.Connect(selectedSocket);
                                    }

                                }
                            }
                        }
                    }
                }
                selectedSocket = null;
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
        void OnClick(int x, int y)
        {
            foreach (TriggerScripterNode n in nodes)
            {
                int ox, oy;
                if (n.MouseIsInHeader(x, y, out ox, out oy))
                {
                    n.selectedX = ox;
                    n.selectedY = oy;
                    selectedSocket = null;
                    n.selected = true;
                }
                else
                {
                    n.selected = false;
                }

                foreach (var s in n.sockets)
                {
                    if(s.Value.MouseIsIn(x, y))
                    {
                        selectedSocket = s.Value;
                    }
                }
            }
        }
        void OnMouseHeld(int mx, int my)
        {
            foreach(TriggerScripterNode n in nodes)
            {
                if(n.selected)
                {
                    n.SetPos(mx - n.selectedX, my - n.selectedY);
                }
            }
        }
    }
}
