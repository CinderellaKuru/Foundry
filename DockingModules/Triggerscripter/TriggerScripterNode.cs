using SMHEditor.DockingModules.PropertyEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class TriggerScripterSocket
    {
        public static int socketSize = 10;
        public Rectangle rect;
        public TriggerScripterNode node;
        public TriggerScripterSocket(string name, string type, Color socketColor, TriggerScripterNode n, Rectangle r, bool showType)
        {
            color = socketColor;
            text = name;
            valueType = type;
            node = n;
            rect = r;
            this.showType = showType;
        }

        protected bool showType = true;
        public string text;
        public string valueType = "null";
        public Color color;
        public bool PointIsIn(int x, int y)
        {
            return (x >= node.x + rect.X && x < node.x + rect.X + rect.Width && y >= node.y + rect.Y && y < node.y + rect.Y + rect.Height);
        }
        public virtual void Draw(PaintEventArgs e)
        {
            Rectangle nodePlusOffs = new Rectangle(node.x + rect.X, node.y + rect.Y, rect.Width, rect.Height);
            e.Graphics.FillRectangle(new SolidBrush(color), nodePlusOffs);
            e.Graphics.DrawRectangle(new Pen(Color.DarkGray), nodePlusOffs);
        }
    }
    public class TriggerScripterSocketInput : TriggerScripterSocket
    {
        public List<TriggerScripterSocketOutput> connectedSockets = new List<TriggerScripterSocketOutput>();
        public TriggerScripterSocketInput(string name, string type, Color socketCoolor, TriggerScripterNode n, Rectangle r, bool showType)
            : base(name, type, socketCoolor, n, r, showType)
        {

        }
        public void FinalizeConnection(TriggerScripterSocketOutput s)
        {
            connectedSockets.Add(s);
        }

        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 14.5f, FontStyle.Regular);

            if (showType)
            {
                node.DrawStringOnNode(e.Graphics, f, text, Color.White, rect.X + 15, rect.Y - 20);
                node.DrawStringOnNode(e.Graphics, f, "[" + valueType + "]", Color.White, rect.X + 15, rect.Y + 4);
            }
            else
                node.DrawStringOnNode(e.Graphics, f, text, Color.White, rect.X + 15, rect.Y - 9);
        }
    }
    public class TriggerScripterSocketOutput : TriggerScripterSocket
    {
        public List<TriggerScripterSocketInput> connectedSockets = new List<TriggerScripterSocketInput>();
        public TriggerScripterSocketOutput(string name, string type, Color socketCoolor, TriggerScripterNode n, Rectangle r, bool showType)
            : base(name, type, socketCoolor, n, r, showType)
        {

        }
        
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 14.5f, FontStyle.Regular);
            if(showType)
            {
                node.DrawStringOnNode(e.Graphics, f, text, Color.White, rect.X, rect.Y - 20, StringFormatFlags.DirectionRightToLeft);
                node.DrawStringOnNode(e.Graphics, f, "[" + valueType + "]", Color.White, rect.X, rect.Y + 4, StringFormatFlags.DirectionRightToLeft);
            }
            else
                node.DrawStringOnNode(e.Graphics, f, text, Color.White, rect.X, rect.Y - 9, StringFormatFlags.DirectionRightToLeft);
        }
        public void DrawConnections(PaintEventArgs e)
        {
            foreach (TriggerScripterSocket s in connectedSockets)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 7.0f),
                    node.x + rect.X + (socketSize / 2),
                    node.y + rect.Y + (socketSize / 2),
                    s.node.x + s.rect.X + (socketSize / 2),
                    s.node.y + s.rect.Y + (socketSize / 2));

                e.Graphics.DrawLine(new Pen(s.color, 5.0f),
                    node.x + rect.X + (socketSize / 2),
                    node.y + rect.Y + (socketSize / 2),
                    s.node.x + s.rect.X + (socketSize / 2),
                    s.node.y + s.rect.Y + (socketSize / 2));
            }
        }
        public void Connect(TriggerScripterSocketInput s)
        {
            if (valueType == s.valueType)
            {
                connectedSockets.Add(s);
                s.FinalizeConnection(this);
            }
        }
    }

    public class TriggerScripterNode
    {
        public TriggerscripterControl owner;
        public int width = 350, height, bottomPadding = 0;
        public int x, y;
        public int layer = 0;
        public Color headerColor = Color.DarkOrange;
        public string nodeTitle = "node";
        public string typeTitle = "type";
        public string handleAs = "null";

        public object data;

        public int selectedX = 0, selectedY = 0;
        public bool selected = false;

        public Dictionary<Rectangle, TriggerScripterSocket> sockets = new Dictionary<Rectangle, TriggerScripterSocket>();
        int inSockets = 0, outSockets = 0;

        public TriggerScripterNode(TriggerscripterControl control, int px, int py)
        {
            owner = control;
            x = px;
            y = py;
        }

        public void SetPos(int px, int py)
        {
            x = px;
            y = py;
        }
        public void Move(int mx, int my)
        {
            x += mx;
            y += my;
        }
        
        public void AddSocket(bool isInput, string text, string type, Color color, bool showType = true)
        {
            if(isInput)
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2,
                    inSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 40,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketInput(text, type, color, this, r, showType));
                inSockets++;
            }
            else
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2 + width,
                    outSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 40,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketOutput(text, type, color, this, r, showType));
                outSockets++;
            }

            int greater = inSockets > outSockets ? inSockets : outSockets;
            height = 90 + (socketSpacing * greater);
        }
        public TriggerScripterSocket GetSocket(string name)
        {
            foreach(TriggerScripterSocket s in sockets.Values)
            {
                if (s.text == name)
                {
                    return s;
                }
            }
            {
                return null;
            }
        }
        public bool PointIsIn(int mx, int my, out int offsX, out int offsY)
        {
            if (mx >= x && mx <= x + width && my >= y && my <= y + height)
            {
                offsX = mx - x;
                offsY = my - y;
                return true;
            }
            else
            {
                offsX = 0;
                offsY = 0;
                return false;
            }
        }
        public bool PointIsInHeader(int mx, int my, out int offsX, out int offsY)
        {
            if (mx >= x && mx <= x + width && my >= y && my <= y + headerHeight)
            {
                offsX = mx - x;
                offsY = my - y;
                return true;
            }
            else
            {
                offsX = 0;
                offsY = 0;
                return false;
            }
        }

        public virtual void Selected()
        {

        }
        public virtual void Deselected()
        {

        }

        static int socketSpacing = 65;
        static int headerHeight = 70;
        static Brush backBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 40));
        public string TrunicateString(Graphics g, Font f, string s, int width)
        {
            string ts = s;
            Matrix m = g.Transform.Clone();
            g.Transform = new Matrix();
            SizeF size = g.MeasureString(s, f);
            g.Transform = m;

            bool trunicated = false;
            while (size.Width >= width)
            {
                ts = ts.Substring(0, ts.Length - 1);
                size = g.MeasureString(ts, f);
                trunicated = true;
            }
            if (trunicated) ts += "...";
            return ts;
        }
        public void DrawStringOnNode(Graphics g, Font f, string s, Color c, int xOffs, int yOffs)
        {
            DrawStringOnNode(g, f, s, c, xOffs, yOffs, (StringFormatFlags)0);
        }
        public void DrawStringOnNode(Graphics g, Font f, string s, Color c, int xOffs, int yOffs, StringFormatFlags flags)
        {
            g.DrawString(s, f, new SolidBrush(c), xOffs + x, yOffs + y, new StringFormat(flags));
        }
        public virtual void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(backBrush, x, y, width, height + bottomPadding);
            e.Graphics.FillRectangle(new SolidBrush(headerColor), x, y, width, headerHeight);

            Font fb = new Font("Arial", 18, FontStyle.Bold);
            Font fr = new Font("Arial", 18, FontStyle.Regular);
            DrawStringOnNode(e.Graphics, fb, TrunicateString(e.Graphics, fb, nodeTitle, width - 60), Color.White, 3, 3);
            DrawStringOnNode(e.Graphics, fr, TrunicateString(e.Graphics, fr, typeTitle, width - 60), Color.White, 3, 29);

            Color borderColor = selected ? Color.White : Color.Black;
            e.Graphics.DrawRectangle(new Pen(borderColor), x, y, width, height + bottomPadding);

            foreach (var socket in sockets)
            {
                socket.Value.Draw(e);
            }
        }
    }

    public class TriggerScripterNode_Trigger : TriggerScripterNode
    {
        public PropertyItem_String nameProperty;
        public PropertyItem_Bool conditionalType;
        public PropertyItem_Bool active;
        
        public TriggerScripterNode_Trigger(TriggerscripterControl control, int px, int py) : base(control, px, py)
        {
            typeTitle = "Trigger";
            handleAs = "Trigger";
            AddSocket(true, "Caller", "TRG", TriggerScripterPage.trgColor, false);
            AddSocket(true, "Conditions", "CND", TriggerScripterPage.cndColor, false);
            AddSocket(false, "Call On True", "EFF", TriggerScripterPage.effColor, false);
            AddSocket(false, "Call On False", "EFF", TriggerScripterPage.effColor, false);

            nameProperty = new PropertyItem_String("Name");
            nameProperty.tb.TextChanged += OnNameChange;

            conditionalType = new PropertyItem_Bool("Condition Type", "And", "Or");

            active = new PropertyItem_Bool("Active On Start", "False", "True");
        }

        public override void Selected()
        {
            MainWindow.propertyEditor.control.Clear();
            MainWindow.propertyEditor.control.AddProperty(nameProperty);
            MainWindow.propertyEditor.control.AddProperty(active);
            MainWindow.propertyEditor.control.AddProperty(conditionalType);
        }
        public override void Deselected()
        {
            MainWindow.propertyEditor.control.Clear();
        }

        public void OnNameChange(object o, EventArgs e)
        {
            nodeTitle = nameProperty.tb.Text;
        }

        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);

            if(active.state == true)
                e.Graphics.FillEllipse(new SolidBrush(Color.Green), x + width - 30, y + 10, 20, 20);
            else
                e.Graphics.FillEllipse(new SolidBrush(Color.Red), x + width - 30, y + 10, 20, 20);

            e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 1), x + width - 30, y + 10, 20, 20);
        }
    }
    public class TriggerScripterNode_Variable: TriggerScripterNode
    {
        public PropertyItem_String nameProperty;
        public PropertyItem_String valueProperty;
        public TriggerScripterNode_Variable(TriggerscripterControl c, int x, int y) : base(c,x,y)
        {
            handleAs = "Variable";
            nameProperty = new PropertyItem_String("Name");
            nameProperty.tb.TextChanged += OnNameChange;

            valueProperty = new PropertyItem_String("Value");
        }

        public override void Deselected()
        {
            MainWindow.propertyEditor.control.Clear();
        }
        public override void Selected()
        {
            MainWindow.propertyEditor.control.Clear();
            MainWindow.propertyEditor.control.AddProperty(nameProperty);
            MainWindow.propertyEditor.control.AddProperty(valueProperty);
        }

        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), x + 10, y + height - 10, width - 20, 50);
            Font f = new Font("Arial", 16, FontStyle.Regular);
            DrawStringOnNode(e.Graphics, f, TrunicateString(e.Graphics, f, valueProperty.tb.Text, width - 40), Color.White, 15, height);
        }
        public void OnNameChange(object o, EventArgs e)
        {
            nodeTitle = nameProperty.tb.Text;
        }
    }
}
