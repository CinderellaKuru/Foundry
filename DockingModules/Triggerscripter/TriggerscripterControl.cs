﻿using System;
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
using Newtonsoft.Json;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Workspace;

namespace SMHEditor.DockingModules.Triggerscripter
{
    class Input
    {
        public string name;
        public string valueType;
        public bool optional;
        public string sigId;
    }
    class Output
    {
        public string name;
        public string valueType;
        public bool optional;
        public string sigId;
    }
    class Effect
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public List<string> sources = new List<string>();
        public string dbid;
        public string version;
    }
    class Condition
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public string dbid;
        public string version;
    }

    public partial class TriggerscripterControl : UserControl
    {
        Timer t = new Timer();
        List<TriggerScripterNode> nodes = new List<TriggerScripterNode>();

        public TriggerscripterControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Tick(null, null);

            Paint += new PaintEventHandler(DrawControl);

            t.Interval = 1;
            t.Tick += Tick;
            t.Start();
        }
        void Tick(object o, EventArgs e)
        {
            MouseState mouse = Mouse.GetCursorState();

            UpdateMatrices();

            PollMouse(mouse);

            lastX = mouse.X;
            lastY = mouse.Y;
            lastM = mouse.Scroll.Y;
            Invalidate();
        }

        public void AddNode(TriggerScripterNode n)
        {
            nodes.Add(n);
        }


        Pen gridPen = new Pen(Color.DimGray);
        public int majorGridSpace = 150;
        public float zoom = .25f;
        float x = 0, y = 0;
        float zoomMax = 1.5f, zoomMin = .025f;
        public Matrix transform = new Matrix();
        public Matrix transformInv = new Matrix();
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
                foreach (TriggerScripterSocket s in n.sockets.Values)
                {
                    s.Draw(e);
                }
            }
            //draw connections
            foreach (TriggerScripterNode n in nodes)
            {
                foreach (TriggerScripterSocket s in n.sockets.Values)
                {
                    if (s is TriggerScripterSocketOutput)
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
                e.Graphics.DrawLine(new Pen(selectedSocket.color, 5.0f), new Point(
                    selectedSocket.node.x + selectedSocket.rect.X + selectedSocket.rect.Width / 2,
                    selectedSocket.node.y + selectedSocket.rect.Y + selectedSocket.rect.Height / 2),
                    pos[0]);
                e.Graphics.DrawLine(new Pen(selectedSocket.color, 2.0f), new Point(
                    selectedSocket.node.x + selectedSocket.rect.X + selectedSocket.rect.Width / 2,
                    selectedSocket.node.y + selectedSocket.rect.Y + selectedSocket.rect.Height / 2),
                    pos[0]);
            }
        }
        void UpdateMatrices()
        {
            transform.Reset();
            transform.Translate(Width / 2, Height / 2);
            transform.Scale(zoom, zoom);
            transform.Translate(x, y);

            transformInv.Reset();
            transformInv.Translate(-x, -y);
            transformInv.Scale(1 / zoom, 1 / zoom);
            transformInv.Translate(-Width / 2, -Height / 2);
        }


        TriggerScripterSocket selectedSocket = null;
        bool lastClicked = false, suspendInput = false;
        float lastX = 0, lastY = 0, lastM = 0;
        void PollMouse(MouseState m)
        {
            Point min = PointToScreen(new Point(0, 0));
            Point max = PointToScreen(new Point(Width, Height));
            if (m.X >= min.X && m.X <= max.X &&
                m.Y >= min.Y && m.Y <= max.Y &&
                !suspendInput)
            {
                //translated mouse pos
                Point[] p = new Point[] { PointToClient(new Point(m.X, m.Y)) };
                transformInv.TransformPoints(p);

                Console.WriteLine(lastClicked);

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

                    if (selectedSocket != null)
                    {
                        foreach (TriggerScripterNode n in nodes)
                        {
                            foreach (TriggerScripterSocket s in n.sockets.Values)
                            {
                                if (s.PointIsIn(p[0].X, p[0].Y))
                                {
                                    if (selectedSocket.node != s.node && (
                                         (selectedSocket is TriggerScripterSocketInput && s is TriggerScripterSocketOutput) ||
                                         (selectedSocket is TriggerScripterSocketOutput && s is TriggerScripterSocketInput))
                                        )
                                    {
                                        TriggerScripterSocketOutput outSocket;
                                        if (selectedSocket is TriggerScripterSocketOutput)
                                        {
                                            outSocket = selectedSocket as TriggerScripterSocketOutput;
                                            outSocket.Connect(s as TriggerScripterSocketInput);
                                        }
                                        if (s is TriggerScripterSocketOutput)
                                        {
                                            outSocket = s as TriggerScripterSocketOutput;
                                            outSocket.Connect(selectedSocket as TriggerScripterSocketInput);
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
                    x += (m.X - (int)lastX) * 1 / zoom;
                    y += (m.Y - (int)lastY) * 1 / zoom;
                }

                zoom += (m.Scroll.Y - lastM) / 100;
                zoom = zoom < zoomMin ? zoomMin : zoom;
                zoom = zoom > zoomMax ? zoomMax : zoom;
            }
            else
            {
                if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
                {
                    suspendInput = true;
                }
                else
                    suspendInput = false;
            }
        }
        void OnClick(int x, int y)
        {
            foreach (TriggerScripterNode n in nodes)
            {
                int ox, oy;
                if (n.PointIsInHeader(x, y, out ox, out oy))
                {
                    n.selectedX = ox;
                    n.selectedY = oy;
                    selectedSocket = null;
                    n.selected = true;
                    n.Selected();
                }
                else
                {
                    n.selected = false;
                    n.Deselected();
                }

                foreach (var s in n.sockets)
                {
                    if (s.Value.PointIsIn(x, y))
                    {
                        selectedSocket = s.Value;
                    }
                }
            }
        }
        void OnMouseHeld(int mx, int my)
        {
            foreach (TriggerScripterNode n in nodes)
            {
                if (n.selected)
                {
                    n.SetPos(mx - n.selectedX, my - n.selectedY);
                }
            }
        }
    }
}
