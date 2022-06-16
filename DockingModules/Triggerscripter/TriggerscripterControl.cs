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
using Newtonsoft.Json;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Docking;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Workspace;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class Input
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }
    public class Output
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
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


    public partial class TriggerscripterControl : UserControl
    {
        Timer t = new Timer();
        public List<TriggerscripterNode> nodes = new List<TriggerscripterNode>();
        bool copyPastePressed;

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
            if (!Focused)
            {
                Invalidate();
                return;
            }

            MouseState mouse = Mouse.GetCursorState();
            KeyboardState keyboard = Keyboard.GetState();

            UpdateMatrices();

            PollMouse(mouse);

            lastX = mouse.X;
            lastY = mouse.Y;
            lastM = mouse.Scroll.Y;
            Invalidate();

            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.Delete))
            {
                foreach (TriggerscripterNode n in nodes.ToArray())
                {
                    if (n.selected)
                    {
                        //for each socket in this node's sockets
                        foreach (TriggerscripterSocket s in n.sockets.Values)
                        {
                            //if it is an input
                            if (s is TriggerscripterSocket_Input)
                            {
                                //foreach output connected to this input
                                foreach (TriggerscripterSocket cs in ((TriggerscripterSocket_Input)s).connectedSockets.ToArray())
                                {
                                    //disconnect this input from the output
                                    ((TriggerscripterSocket_Output)cs).Disconnect((TriggerscripterSocket_Input)s);
                                }
                            }

                            //if it is an output
                            else
                            {
                                //foreach input connected to this output
                                foreach (TriggerscripterSocket cs in s.connectedSockets.ToArray())
                                {
                                    ((TriggerscripterSocket_Output)s).Disconnect((TriggerscripterSocket_Input)cs);
                                }
                            }

                            nodes.Remove(n);
                        }
                    }
                }
            }
            if (keyboard.IsKeyDown(Key.ControlLeft) && keyboard.IsKeyDown(Key.C))
            {
                if (!copyPastePressed)
                {
                    CopyGraph();
                }
                copyPastePressed = true;
            }
            else if (keyboard.IsKeyDown(Key.ControlLeft) && keyboard.IsKeyDown(Key.V))
            {
                if (!copyPastePressed)
                {
                    PasteGraph();
                }
                copyPastePressed = true;
            }
            else
                copyPastePressed = false;


#if DEBUG
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.L))
            {
                //TriggerscripterCompiler.Compile(nodes, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.triggerscript");
                //SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
                LoadFromFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.K))
                SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.J))
            {
                TriggerscripterCompiler c = new TriggerscripterCompiler();
                c.Compile(nodes, varID, "D:\\StumpyHWDEMod\\SMEditorTests\\data\\triggerscripts\\test.triggerscript");
            }
#endif

        }
        public void AddNode(TriggerscripterNode n)
        {
            nodes.Add(n);
        }


        Pen gridPen = new Pen(Color.FromArgb(255, 70, 70, 70));
        public int majorGridSpace = 100;
        public float zoom = .25f;
        float x = 0, y = 0;
        float zoomMax = 1.5f, zoomMin = .025f;
        public Matrix transform = new Matrix();
        public Matrix transformInv = new Matrix();
        void DrawControl(object o, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.High;
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
            if (mouseState == CurrentMouseState.DraggingSocket)
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
            //draw nodes
            foreach (TriggerscripterNode n in nodes)
            {
                n.Draw(e);
                //draw sockets
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    s.Draw(e);
                }
            }
            //draw marquee
            if(mouseState == CurrentMouseState.DraggingMarquee)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 10, 10, 10)),
                    Math.Min(marqueeX1, marqueeX2), 
                    Math.Min(marqueeY1, marqueeY2), 
                    Math.Abs(marqueeX2 - marqueeX1), 
                    Math.Abs(marqueeY2 - marqueeY1));
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
        enum CurrentMouseState
        {
            None,
            NodesSelected,
            DraggingMarquee,
            DraggingSocket
        }
        
        int marqueeX1, marqueeY1;
        int marqueeX2, marqueeY2;

        CurrentMouseState mouseState;
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
                if(lastClicked && m.LeftButton == OpenTK.Input.ButtonState.Released)
                {
                    OnMouseReleased(p[0].X, p[0].Y);
                }
                if (m.LeftButton == OpenTK.Input.ButtonState.Released)
                {
                    lastClicked = false;
                }

                //zoom
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
        void OnClick(int mx, int my)
        {
            MainWindow.propertyEditor.control.Clear();
            MainWindow.propertyEditor.control.FinishLayout();

            marqueeX1 = mx;
            marqueeY1 = my;
            marqueeX2 = mx;
            marqueeY2 = my;

            var nodesReversed = nodes.ToList();
            nodesReversed.Reverse();

            //dragging socket
            foreach (TriggerscripterNode n in nodesReversed)
            {
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    if (s.PointIsIn(mx, my))
                    {
                        mouseState = CurrentMouseState.DraggingSocket;
                        if (!s.multiConnection)
                        {
                            if (s.connectedSockets.Count > 0)
                            {
                                selectedSocket = s.connectedSockets[0];
                                if (s is TriggerscripterSocket_Input)
                                {
                                    ((TriggerscripterSocket_Output)s.connectedSockets[0]).Disconnect((TriggerscripterSocket_Input)s);
                                }
                                else
                                {
                                    ((TriggerscripterSocket_Output)s).Disconnect((TriggerscripterSocket_Input)s.connectedSockets[0]);
                                }
                                return;
                            }
                        }

                        foreach (TriggerscripterNode n2 in nodesReversed)
                            n2.selected = false;
                        selectedSocket = s;
                        return;
                    }
                }
            }

            //none
            if (mouseState == CurrentMouseState.None)
            {
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    if (n.PointIsIn(mx, my))
                    {
                        int ox, oy;
                        n.GetPointOffset(mx, my, out ox, out oy);
                        n.selectedX = ox;
                        n.selectedY = oy;
                        nodes.Remove(n);
                        nodes.Add(n);

                        foreach (TriggerscripterNode n2 in nodesReversed)
                            n2.selected = false;
                        n.selected = true;

                        mouseState = CurrentMouseState.NodesSelected;

                        MainWindow.propertyEditor.control.Clear();
                        n.Selected();
                        MainWindow.propertyEditor.control.FinishLayout();

                        return;
                    }
                }
            }

            if (mouseState == CurrentMouseState.NodesSelected)
            {
                bool somethingUnderMouse = false;
                bool selectedUnderMouse = false;
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    int ox, oy;
                    n.GetPointOffset(mx, my, out ox, out oy);
                    n.selectedX = ox;
                    n.selectedY = oy;

                    if(n.selected && n.PointIsIn(mx, my))
                        selectedUnderMouse = true;

                    if (!n.selected && n.PointIsIn(mx, my) && !selectedUnderMouse)
                    {
                        foreach (TriggerscripterNode n2 in nodesReversed)
                        {
                            n2.selected = false;
                        }
                        n.selected = true;
                        nodes.Remove(n);
                        nodes.Add(n);
                    }
                    if (n.PointIsIn(mx, my))
                        somethingUnderMouse = true;
                }
                if (!somethingUnderMouse)
                {
                    foreach (TriggerscripterNode n in nodes)
                        n.selected = false;
                    mouseState = CurrentMouseState.None;
                }
            }

            if (mouseState == CurrentMouseState.None)
            {
                foreach (TriggerscripterNode n in nodes)
                    n.selected = false;

                mouseState = CurrentMouseState.DraggingMarquee;
                marqueeX1 = mx;
                marqueeY1 = my;
            }

        }
        void OnMouseHeld(int mx, int my)
        {
            var nodesReversed = nodes.ToList();
            nodesReversed.Reverse();

            if (mouseState == CurrentMouseState.NodesSelected)
            {
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    if(n.selected)
                    {
                        n.SetPos(mx - n.selectedX, my - n.selectedY);
                    }
                }
            }
            if (mouseState == CurrentMouseState.DraggingMarquee)
            {
                MainWindow.propertyEditor.control.Clear();
                marqueeX2 = mx;
                marqueeY2 = my;
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    if (n.IsInRect(
                    Math.Min(marqueeX1, marqueeX2),
                    Math.Min(marqueeY1, marqueeY2),
                    Math.Abs(marqueeX2 - marqueeX1),
                    Math.Abs(marqueeY2 - marqueeY1)))
                    {
                        n.selected = true;
                        nodes.Remove(n);
                        nodes.Add(n);
                    }
                }
            }
        }
        void OnMouseReleased(int mx, int my)
        {
            if (mouseState == CurrentMouseState.DraggingMarquee)
            {
                foreach (TriggerscripterNode n in nodes)
                {
                    if (n.IsInRect(
                    Math.Min(marqueeX1, marqueeX2),
                    Math.Min(marqueeY1, marqueeY2),
                    Math.Abs(marqueeX2 - marqueeX1),
                    Math.Abs(marqueeY2 - marqueeY1)))
                    {
                        mouseState = CurrentMouseState.NodesSelected;
                    }
                }
                if (mouseState != CurrentMouseState.NodesSelected)
                    mouseState = CurrentMouseState.None;
            }
            if (mouseState == CurrentMouseState.DraggingSocket)
            {
                foreach(TriggerscripterNode n in nodes)
                {
                    foreach(TriggerscripterSocket s in n.sockets.Values)
                    {
                        if (s == selectedSocket) break;
                        if (s.PointIsIn(mx,my))
                        {
                            if(selectedSocket is TriggerscripterSocket_Output)
                            {
                                if (s is TriggerscripterSocket_Input)
                                {
                                    ((TriggerscripterSocket_Output)selectedSocket).Connect(s as TriggerscripterSocket_Input);
                                    goto DraggingSocketFinish;
                                }
                            }
                            else
                            {
                                if (s is TriggerscripterSocket_Output)
                                {
                                    ((TriggerscripterSocket_Output)s).Connect(selectedSocket as TriggerscripterSocket_Input);
                                    goto DraggingSocketFinish;
                                }
                            }
                        }
                    }
                }
DraggingSocketFinish:
                mouseState = CurrentMouseState.None;
                selectedSocket = null;
            }
        }


        public static Color requiredVarColor = Color.FromArgb(255, 64, 130, 64);
        public static Color optionalVarColor = Color.FromArgb(255, 122, 130, 64);
        public static Color cndColor = Color.FromArgb(255, 130, 64, 64);
        public static Color trgColor = Color.FromArgb(255, 64, 117, 130);
        public static Color effColor = Color.FromArgb(255, 130, 64, 106);


        Point[] nodeAddLocation = new Point[] { new Point() };
        public void CaptureMousePos(object o, EventArgs e)
        {
            nodeAddLocation[0] = PointToClient(new Point(
                OpenTK.Input.Mouse.GetCursorState().X,
                OpenTK.Input.Mouse.GetCursorState().Y));

            transformInv.TransformPoints(nodeAddLocation);
            Console.WriteLine(nodeAddLocation[0]);
        }

        public int trgID = 0, varID = 0;
        public int cndID = 0, effID = 0;

        public void CreateNewTriggerPressed(object o, EventArgs e)
        {
            SerializedTrigger t = new SerializedTrigger();
            t.name = "NewTrigger" + trgID;
            CreateTriggerNode(t, trgID, nodeAddLocation[0].X, nodeAddLocation[0].Y);
            trgID++;
        }
        public TriggerscripterNode CreateTriggerNode(SerializedTrigger t, int id, int x, int y)
        {
            TriggerscripterNode_Trigger n = new TriggerscripterNode_Trigger(this, x, y);
            n.headerColor = trgColor;

            n.id = id;
            n.data = t;

            if (t.active)
                n.activeProperty.button.PerformClick();
            if (t.cndIsOr)
                n.conditionalTypeProperty.button.PerformClick();
            n.nameProperty.tb.Text = t.name;


            n.nameProperty.tb.Text = n.nodeTitle;
            n.id = id;
            AddNode(n);
            return n;
        }

        public void CreateNewVarPressed(object o, EventArgs e)
        {
            SerializedVariable var = new SerializedVariable();
            var.type = (string)((MenuItem)o).Tag;
            var.name = "New" + var.type + varID.ToString();
            var.value = "";
            CreateVarNode(var, varID, nodeAddLocation[0].X, nodeAddLocation[0].Y);
            varID++;
        }
        public TriggerscripterNode CreateVarNode(SerializedVariable v, int id, int x, int y)
        {
            TriggerscripterNode_Variable n = new TriggerscripterNode_Variable(this, x, y);
            n.headerColor = requiredVarColor;

            n.data = v;
            n.id = id;

            n.nodeTitle = v.name;
            n.nameProperty.tb.Text = v.name;
            n.valueProperty.tb.Text = v.value;

            n.typeTitle = v.type;
            n.AddSocket(true, "Set", v.type, requiredVarColor, false);
            n.AddSocket(false, "Use", v.type, requiredVarColor, false);

            n.bottomPadding = 50;

            AddNode(n);
            return n;
        }

        public void CreateNewEffectPressed(object o, EventArgs e)
        {
            CreateEffectNode((Effect)((MenuItem)o).Tag, effID++, nodeAddLocation[0].X, nodeAddLocation[0].Y);
        }
        public TriggerscripterNode CreateEffectNode(Effect e, int id, int x, int y)
        {
            TriggerscripterNode n = new TriggerscripterNode(this, x, y);
            n.headerColor = effColor;

            n.data = e;
            n.id = id;

            n.nodeTitle = e.name;
            n.typeTitle = "Effect";
            n.handleAs = "Effect";
            n.AddSocket(true, "Caller", "EFF", effColor, false, false);
            n.AddSocket(false, "Call", "EFF", effColor, false, false);

            if (!e.name.Contains("Trigger"))
            {
                foreach (Input i in e.inputs)
                {
                    Color color = i.optional ? optionalVarColor : requiredVarColor;
                    n.AddSocket(true, i.name, i.valueType, color, true, false);
                }
                foreach (Output ou in e.outputs)
                {
                    Color color = ou.optional ? optionalVarColor : requiredVarColor;
                    n.AddSocket(false, ou.name, ou.valueType, color, true, false);
                }
            }
            else
            {
                n.AddSocket(false, "Trigger", "TRG", trgColor, false, false);
            }

            AddNode(n);

            return n;
        }

        public void CreateNewConditionPressed(object o, EventArgs e)
        {
            CreateConditionNode((Condition)((MenuItem)o).Tag, cndID++, nodeAddLocation[0].X, nodeAddLocation[0].Y);
        }
        public TriggerscripterNode CreateConditionNode(Condition c, int id, int x, int y)
        {
            TriggerscripterNode_Condition n = new TriggerscripterNode_Condition(this, x, y);
            n.headerColor = cndColor;

            n.data = c;
            n.id = id;

            n.id = cndID;
            n.nodeTitle = c.name;

            n.AddSocket(false, "Result", "CND", cndColor, false, false);

            foreach (Input i in c.inputs)
            {
                Color color = i.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(true, i.name, i.valueType, color, true, false);
            }
            foreach (Output ou in c.outputs)
            {
                Color color = ou.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(false, ou.name, ou.valueType, color, true, false);
            }

            AddNode(n);

            return n;
        }

        public class SerializedVariable
        {
            public string name;
            public string value;
            public string type;
        }
        public class SerializedTrigger
        {
            public string name;
            public bool cndIsOr;
            public bool active;
        }
        public class SerializedNodeLink
        {
            public string sourceType;
            public string sourceSocketName;
            public int sourceId;

            public int targetId;
            public string targetType;
            public string targetSocketName;
        }
        public class SerializableNode
        {
            public int id;
            public int x, y;
            public string handleAs;
            public bool selected;
            public SerializedTrigger trigger;
            public SerializedVariable variable;
            public Effect effect;
            public Condition condition;
        }
        public class SerializedTriggerscripter
        {
            public int lastTrg, lastVar, lastEff, lastCnd;
            public List<SerializableNode> nodes = new List<SerializableNode>();
            public List<SerializedNodeLink> links = new List<SerializedNodeLink>();
        }
        public SerializedTriggerscripter GetSerializedGraph()
        {
            SerializedTriggerscripter sts = new SerializedTriggerscripter();
            foreach (TriggerscripterNode n in nodes)
            {
                SerializableNode sn = new SerializableNode();
                sn.handleAs = n.handleAs;
                sn.selected = n.selected;
                if (n.handleAs == "Trigger")
                {
                    SerializedTrigger t = new SerializedTrigger();
                    t.active = ((TriggerscripterNode_Trigger)n).activeProperty.state;
                    t.cndIsOr = ((TriggerscripterNode_Trigger)n).conditionalTypeProperty.state;
                    t.name = ((TriggerscripterNode_Trigger)n).nameProperty.tb.Text;
                    sn.trigger = t;
                }
                if (n.handleAs == "Variable")
                {
                    SerializedVariable v = new SerializedVariable();
                    v.value = ((TriggerscripterNode_Variable)n).valueProperty.tb.Text;
                    v.name = ((TriggerscripterNode_Variable)n).nameProperty.tb.Text;
                    v.type = n.typeTitle;
                    sn.variable = v;
                }
                if (n.handleAs == "Effect")
                {
                    Effect e = (Effect)n.data;
                    sn.effect = e;
                }
                if (n.handleAs == "Condition")
                {
                    Condition c = (Condition)n.data;
                    sn.condition = c;
                }

                foreach (TriggerscripterSocket os in n.sockets.Values)
                {
                    if (os is TriggerscripterSocket_Output)
                    {
                        foreach (TriggerscripterSocket s in os.connectedSockets)
                        {
                            SerializedNodeLink link = new SerializedNodeLink();
                            link.sourceId = n.id;
                            link.sourceSocketName = os.text;

                            if (n is TriggerscripterNode_Variable)
                                link.sourceType = "Variable";
                            else
                                link.sourceType = n.typeTitle;

                            link.targetId = s.node.id;
                            link.targetSocketName = s.text;
                            if (s.node is TriggerscripterNode_Variable)
                                link.targetType = "Variable";
                            else
                                link.targetType = s.node.typeTitle;

                            sts.links.Add(link);
                        }


                    }
                }

                sn.x = n.x;
                sn.y = n.y;
                sn.id = n.id;

                sts.lastTrg = trgID;
                sts.lastVar = varID;
                sts.lastEff = effID;
                sts.lastCnd = cndID;
                sts.nodes.Add(sn);
            }

            return sts;
        }

        public void SaveToFile(string path)
        {
            JsonSerializerSettings s = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            File.WriteAllText(path, JsonConvert.SerializeObject(GetSerializedGraph(), s));
        }
        public void LoadFromFile(string path)
        {
            nodes.Clear();
            Dictionary<int, TriggerscripterNode> triggers = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> variables = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> effects = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> conditions = new Dictionary<int, TriggerscripterNode>();
            SerializedTriggerscripter sts = JsonConvert.DeserializeObject<SerializedTriggerscripter>(File.ReadAllText(path));
            foreach (SerializableNode n in sts.nodes)
            {
                try
                {
                    if (n.handleAs == "Trigger")
                    {
                        triggers.Add(n.id, CreateTriggerNode(n.trigger, n.id, n.x, n.y));
                    }
                    if (n.handleAs == "Variable")
                    {
                        variables.Add(n.id, CreateVarNode(n.variable, n.id, n.x, n.y));
                    }
                    if (n.handleAs == "Effect")
                    {
                        effects.Add(n.id, CreateEffectNode(n.effect, n.id, n.x, n.y));
                    }
                    if (n.handleAs == "Condition")
                    {
                        conditions.Add(n.id, CreateConditionNode(n.condition, n.id, n.x, n.y));
                    }
                }
                catch(Exception e)
                {
                    Program.window.label.Text = "Paste error. See log.";
                    Console.WriteLine(e.Message);
                }
            }
            foreach (SerializedNodeLink l in sts.links)
            {
                if(l.sourceType == "Trigger")
                {
                    switch(l.targetType)
                    {
                        case "Effect":
                            ((TriggerscripterSocket_Output)triggers[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                            break;
                        default:
                            break;
                    }
                }
                if(l.sourceType == "Effect")
                {
                    switch (l.targetType)
                    {
                        case "Effect":
                            ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                            break;
                        case "Condition":
                            ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)conditions[l.targetId].sockets[l.targetSocketName]);
                            break;
                        case "Variable":
                            ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)variables[l.targetId].sockets[l.targetSocketName]);
                            break;
                        case "Trigger":
                            ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)triggers[l.targetId].sockets[l.targetSocketName]);
                            break;
                        default:
                            break;
                    }
                }
                if (l.sourceType == "Condition")
                {
                    switch (l.targetType)
                    {
                        case "Effect":
                            ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                            break;
                        case "Trigger":
                            ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)triggers[l.targetId].sockets[l.targetSocketName]);
                            break;
                        case "Variable":
                            ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)variables[l.targetId].sockets[l.targetSocketName]);
                            break;
                        default:
                            break;
                    }
                }
                if (l.sourceType == "Variable")
                {
                    switch (l.targetType)
                    {
                        case "Effect":
                            ((TriggerscripterSocket_Output)variables[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                            break;
                        case "Condition":
                            ((TriggerscripterSocket_Output)variables[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)conditions[l.targetId].sockets[l.targetSocketName]);
                            break;
                        default:
                            break;
                    }
                }
            }

            trgID = sts.lastTrg;
            varID = sts.lastVar;
            effID = sts.lastEff;
            cndID = sts.lastCnd;
        }

        int pasteOffset = 50;
        SerializedTriggerscripter copyBuffer = new SerializedTriggerscripter();
        public void CopyGraph()
        {
            copyBuffer = GetSerializedGraph();
        }
        public void PasteGraph()
        {
            foreach (TriggerscripterNode n in nodes)
                n.selected = false;

            Dictionary<int, TriggerscripterNode> trgMap = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> effMap = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> varMap = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> cndMap = new Dictionary<int, TriggerscripterNode>();
            foreach (SerializableNode sn in copyBuffer.nodes)
            {
                if(sn.selected)
                {
                    if(sn.handleAs == "Trigger")
                    {
                        TriggerscripterNode n = CreateTriggerNode(sn.trigger, trgID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        trgMap.Add(sn.id, n);
                        trgID++;
                    }
                    if(sn.handleAs == "Effect")
                    {
                        TriggerscripterNode n = CreateEffectNode(sn.effect, effID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        effMap.Add(sn.id, n);
                        effID++;
                    }
                    if (sn.handleAs == "Condition")
                    {
                        TriggerscripterNode n = CreateConditionNode(sn.condition, cndID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        cndMap.Add(sn.id, n);
                        cndID++;
                    }
                    if (sn.handleAs == "Variable")
                    {
                        TriggerscripterNode n = CreateVarNode(sn.variable, varID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        varMap.Add(sn.id, n);
                        varID++;
                    }
                }
            }
            foreach(SerializedNodeLink sl in copyBuffer.links)
            {
                if(sl.sourceType == "Trigger")
                {
                    if(sl.targetType == "Effect")
                    {
                        ((TriggerscripterSocket_Output)trgMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                }

                if(sl.sourceType == "Effect")
                {
                    if(sl.targetType == "Variable")
                    {
                        ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)varMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Effect")
                    {
                        ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Condition")
                    {
                        ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)cndMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Trigger")
                    {
                        ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)trgMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                }

                if(sl.sourceType == "Condition")
                {
                    if (sl.targetType == "Variable")
                    {
                        ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)varMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Effect")
                    {
                        ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Condition")
                    {
                        ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)cndMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Trigger")
                    {
                        ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)trgMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                }

                if(sl.sourceType == "Variable")
                {
                    if (sl.targetType == "Effect")
                    {
                        ((TriggerscripterSocket_Output)varMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                    if (sl.targetType == "Condition")
                    {
                        ((TriggerscripterSocket_Output)varMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                            (TriggerscripterSocket_Input)cndMap[sl.targetId].sockets[sl.targetSocketName]);
                    }
                }
            }

            mouseState = CurrentMouseState.NodesSelected;
        }

        public void ImportScript(string file)
        {
            XDocument doc = XDocument.Load(file);
            XElement vars = doc.Element("TriggerSystem").Element("TriggerVars");
            XElement triggers = doc.Element("TriggerSystem").Element("Trigger");

        }
    }
}
