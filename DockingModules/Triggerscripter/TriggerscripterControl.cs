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

            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.L))
            {
                //TriggerscripterCompiler.Compile(nodes, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.triggerscript");
                //SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
                LoadFromFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.K))
                SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.tsp");
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(Key.C))
            {
                TriggerscripterCompiler c = new TriggerscripterCompiler();
                c.Compile(nodes, varID, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\test.triggerscript");
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
        TriggerscripterNode selectedNode = null;
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

                if (!lastClicked)
                {
                    if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
                    {
                        OnClick(p[0].X, p[0].Y);
                        Console.WriteLine("Q");
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
            selectedNode = null;
            foreach (TriggerscripterNode n in nodes)
            {
                int ox, oy;
                if (n.PointIsInHeader(x, y, out ox, out oy))
                {
                    n.selectedX = ox;
                    n.selectedY = oy;
                    selectedSocket = null;
                    n.selected = true;
                    selectedNode = n;
                }
                else
                {
                    n.selected = false;
                }

                foreach (var s in n.sockets)
                {
                    if (s.Value.PointIsIn(x, y))
                    {
                        selectedSocket = s.Value;
                    }
                }
            }
            MainWindow.propertyEditor.control.Clear();
            if (selectedNode != null)
            {
                selectedNode.Selected();
            }
            MainWindow.propertyEditor.control.FinishLayout();
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


        Point[] nodeAddLocation = new Point[] { new Point() };
        public void CaptureMousePos(object o, EventArgs e)
        {
            nodeAddLocation[0] = PointToClient(new Point(
                OpenTK.Input.Mouse.GetCursorState().X,
                OpenTK.Input.Mouse.GetCursorState().Y));

            transformInv.TransformPoints(nodeAddLocation);
            Console.WriteLine(nodeAddLocation[0]);
        }

        int trgID = 0; int varID = 0;
        int cndID = 0; int effID = 0;

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

            n.id = id;
            n.data = t;

            if (t.active)
                n.activeProperty.button.PerformClick();
            if (t.cndIsOr)
                n.conditionalTypeProperty.button.PerformClick();
            n.nameProperty.tb.Text = t.name;


            n.nameProperty.tb.Text = n.nodeTitle;
            n.id = trgID;
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

            n.data = e;
            n.id = id;

            n.nodeTitle = e.name;
            n.typeTitle = "Effect";
            n.handleAs = "Effect";
            n.AddSocket(true, "Caller", "EFF", effColor, false);
            n.AddSocket(false, "Call", "EFF", effColor, false);

            foreach (Input i in e.inputs)
            {
                Color color = i.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(true, i.name, i.valueType, color);
            }
            foreach (Output ou in e.outputs)
            {
                Color color = ou.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(false, ou.name, ou.valueType, color);
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
            TriggerscripterNode n = new TriggerscripterNode(this, x, y);

            n.data = c;
            n.id = id;

            n.id = cndID;
            n.nodeTitle = c.name;
            n.typeTitle = "Condition";
            n.handleAs = "Condition";

            n.AddSocket(false, "Result", "CND", cndColor, false);

            foreach (Input i in c.inputs)
            {
                Color color = i.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(true, i.name, i.valueType, color);
            }
            foreach (Output ou in c.outputs)
            {
                Color color = ou.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(false, ou.name, ou.valueType, color);
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

        void SaveToFile(string path)
        {
            JsonSerializerSettings s = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            File.WriteAllText(path, JsonConvert.SerializeObject(GetSerializedGraph(), s));
        }
        void LoadFromFile(string path)
        {
            nodes.Clear();
            Dictionary<int, TriggerscripterNode> triggers = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> variables = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> effects = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> conditions = new Dictionary<int, TriggerscripterNode>();
            SerializedTriggerscripter sts = JsonConvert.DeserializeObject<SerializedTriggerscripter>(File.ReadAllText(path));
            foreach (SerializableNode n in sts.nodes)
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
                        case "Variable":
                            ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)variables[l.targetId].sockets[l.targetSocketName]);
                            break;
                        default:
                            break;
                    }
                }
                if (l.sourceType == "Condition")
                {
                    switch (l.targetType)
                    {
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
    }
}
