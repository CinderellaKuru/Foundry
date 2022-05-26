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
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class Input
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
        public int value;
    }
    public class Output
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
        public int value;
    }
    public class Effect
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public List<string> sources = new List<string>();
        public int dbid;
        public int version;
    }
    public class Condition
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public int dbid;
        public int version;
    }
    public class Variable
    {
        public int id;
        public string name;
        public string value;
        public string type;
    }
    public class Trigger
    {
        public string name;
        public bool cndIsOr;
        public bool active;
        public int id;
    }

    public partial class TriggerscripterControl : UserControl
    {
        Timer t = new Timer();
        public List<TriggerscripterNode> nodes = new List<TriggerscripterNode>();

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

            if(OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.K))
            {
                TriggerscripterCompiler.Compile(nodes, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.triggerscript");
                SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
            }
        }

        public void AddNode(TriggerscripterNode n)
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
            foreach (TriggerscripterNode n in nodes)
            {
                n.Draw(e);
            }
            //draw sockets
            foreach (TriggerscripterNode n in nodes)
            {
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    s.Draw(e);
                }
            }
            //draw connections
            foreach (TriggerscripterNode n in nodes)
            {
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    if (s is TriggerscripterSocket_Output)
                    {
                        ((TriggerscripterSocket_Output)s).DrawConnections(e);
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


        TriggerscripterSocket selectedSocket = null;
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
                        foreach (TriggerscripterNode n in nodes)
                        {
                            foreach (TriggerscripterSocket s in n.sockets.Values)
                            {
                                if (s.PointIsIn(p[0].X, p[0].Y))
                                {
                                    if (selectedSocket.node != s.node && (
                                         (selectedSocket is TriggerscripterSocket_Input && s is TriggerscripterSocket_Output) ||
                                         (selectedSocket is TriggerscripterSocket_Output && s is TriggerscripterSocket_Input))
                                        )
                                    {
                                        TriggerscripterSocket_Output outSocket;
                                        if (selectedSocket is TriggerscripterSocket_Output)
                                        {
                                            outSocket = selectedSocket as TriggerscripterSocket_Output;
                                            outSocket.Connect(s as TriggerscripterSocket_Input);
                                        }
                                        if (s is TriggerscripterSocket_Output)
                                        {
                                            outSocket = s as TriggerscripterSocket_Output;
                                            outSocket.Connect(selectedSocket as TriggerscripterSocket_Input);
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
            foreach (TriggerscripterNode n in nodes)
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
            foreach (TriggerscripterNode n in nodes)
            {
                if (n.selected)
                {
                    n.SetPos(mx - n.selectedX, my - n.selectedY);
                }
            }
        }


        public static Color requiredVarColor = Color.ForestGreen;
        public static Color optionalVarColor = Color.Green;
        public static Color cndColor = Color.Maroon;
        public static Color trgColor = Color.Blue;
        public static Color effColor = Color.DeepPink;

        
        Point[] mCap = new Point[] { new Point() };
        public void CaptureMousePos(object o, EventArgs e)
        {
            mCap[0] = PointToClient(new Point(
                OpenTK.Input.Mouse.GetCursorState().X,
                OpenTK.Input.Mouse.GetCursorState().Y));

            transformInv.TransformPoints(mCap);
            Console.WriteLine(mCap[0]);
        }

        int trgID = 0; int varID = 0;
        int cndID = 0; int effID = 0;
        public void CreateTriggerNode(object o, EventArgs e)
        {
            TriggerscripterNode_Trigger n = new TriggerscripterNode_Trigger(this, mCap[0].X, mCap[0].Y);

            n.data = ((MenuItem)o).Tag;
            n.nodeTitle = "NewTrigger" + trgID.ToString();
            n.nameProperty.tb.Text = n.nodeTitle;
            n.id = trgID;

            AddNode(n);
            trgID++;
        }
        public void CreateVarNode(object o, EventArgs e)
        {
            string var = (o as MenuItem).Tag as string;
            TriggerscripterNode_Variable n = new TriggerscripterNode_Variable(this, mCap[0].X, mCap[0].Y);

            n.data = ((MenuItem)o).Tag;
            n.id = varID;

            n.nodeTitle = "New" + var + varID.ToString();
            n.nameProperty.tb.Text = n.nodeTitle;

            n.typeTitle = var;
            n.AddSocket(true, "Set", var, requiredVarColor, false);
            n.AddSocket(false, "Use", var, requiredVarColor, false);

            n.bottomPadding = 50;

            AddNode(n);
            varID++;
        }
        public void CreateEffectNode(object o, EventArgs e)
        {
            Effect eff = (o as MenuItem).Tag as Effect;
            TriggerscripterNode n = new TriggerscripterNode(this, mCap[0].X, mCap[0].Y);

            n.data = ((MenuItem)o).Tag;
            n.id = effID;
            n.nodeTitle = eff.name;
            n.typeTitle = "Effect";
            n.handleAs = "Effect";
            n.AddSocket(true, "Caller", "EFF", effColor, false);
            n.AddSocket(false, "Call", "EFF", effColor, false);

            foreach (Input i in eff.inputs)
            {
                Color color = i.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(true, i.name, i.valueType, color);
            }
            foreach (Output ou in eff.outputs)
            {
                Color color = ou.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(false, ou.name, ou.valueType, color);
            }

            AddNode(n);
            effID++;
        }
        public void CreateConditionNode(object o, EventArgs e)
        {
            Condition cnd = (o as MenuItem).Tag as Condition;
            TriggerscripterNode n = new TriggerscripterNode(this, mCap[0].X, mCap[0].Y);
            n.id = cndID;
            n.data = ((MenuItem)o).Tag;
            n.nodeTitle = cnd.name;
            n.typeTitle = "Condition";
            n.handleAs = "Condition";

            n.AddSocket(false, "Result", "CND", cndColor, false);

            foreach (Input i in cnd.inputs)
            {
                Color color = i.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(true, i.name, i.valueType, color);
            }
            foreach (Output ou in cnd.outputs)
            {
                Color color = ou.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(false, ou.name, ou.valueType, color);
            }

            AddNode(n);
            cndID++;
        }

        public class SavableNode
        {
            public int x, y;
            public object nodeObj;
        }
        public List<SavableNode> GetSerializedNodes()
        {
            List<SavableNode> serializedNodes = new List<SavableNode>();
            foreach (TriggerscripterNode n in nodes)
            {
                SavableNode sn = new SavableNode();
                sn.nodeObj = n.data;
                if (n is TriggerscripterNode_Trigger)
                {
                    Trigger t = new Trigger();
                    t.active = ((TriggerscripterNode_Trigger)n).active.state;
                    t.cndIsOr = ((TriggerscripterNode_Trigger)n).conditionalType.state;
                    t.id = ((TriggerscripterNode_Trigger)n).id;
                    t.name = ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text;
                    sn.nodeObj = t;
                }
                if (n is TriggerscripterNode_Variable)
                {
                    Variable v = new Variable();
                    v.id = n.id;
                    v.value = ((TriggerscripterNode_Variable)n).valueProperty.tb.Text;
                    v.name = ((TriggerscripterNode_Variable)n).nameProperty.tb.Text;
                    v.type = n.typeTitle;
                }
                if (n.handleAs == "Effect")
                {
                    Effect e = (Effect)n.data;
                    foreach(Input i in e.inputs)
                    {
                        if(n.sockets[i.name].connectedSockets.Count >= 1)
                        {
                            i.value = n.sockets[i.name].connectedSockets[0].node.id;
                        }
                    }
                    foreach (Output o in e.outputs)
                    {
                        if (n.sockets[o.name].connectedSockets.Count >= 1)
                        {
                            o.value = n.sockets[o.name].connectedSockets[0].node.id;
                        }
                    }
                }

                sn.x = n.x;
                sn.y = n.y;
                serializedNodes.Add(sn);
            }

            return serializedNodes;
        }

        void SaveToFile(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(GetSerializedNodes(), Newtonsoft.Json.Formatting.Indented));
        }
    }
}
