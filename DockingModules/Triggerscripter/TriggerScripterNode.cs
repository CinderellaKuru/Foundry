using SMHEditor.DockingModules.PropertyEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public TriggerScripterSocket(string name, string type, Color socketColor, TriggerScripterNode n, Rectangle r)
        {
            color = socketColor;
            text = name;
            valueType = type;
            node = n;
            rect = r;
        }

        public string valueType = "null";
        protected string text;
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
        List<TriggerScripterSocketOutput> connectedSockets = new List<TriggerScripterSocketOutput>();
        public TriggerScripterSocketInput(string name, string type, Color socketCoolor, TriggerScripterNode n, Rectangle r)
            : base(name, type, socketCoolor, n, r)
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
            if (valueType != "")
            {
                e.Graphics.DrawString(text, f, new SolidBrush(Color.White), new PointF(node.x + rect.X + 15, node.y + rect.Y - 20));
                e.Graphics.DrawString(valueType, f, new SolidBrush(Color.White), new PointF(node.x + rect.X + 15, node.y + rect.Y + 4));
            }
        }
    }
    public class TriggerScripterSocketOutput : TriggerScripterSocket
    {
        List<TriggerScripterSocketInput> connectedSockets = new List<TriggerScripterSocketInput>();
        public TriggerScripterSocketOutput(string name, string type, Color socketCoolor, TriggerScripterNode n, Rectangle r)
            : base(name, type, socketCoolor, n, r)
        {

        }
        
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 14.5f, FontStyle.Regular);
            if (valueType != "")
            {
                e.Graphics.DrawString(text, f, new SolidBrush(Color.White), new PointF(node.x + rect.X, node.y + rect.Y - 20), new StringFormat(StringFormatFlags.DirectionRightToLeft));
                e.Graphics.DrawString("[" + valueType + "]", f, new SolidBrush(Color.White), new PointF(node.x + rect.X, node.y + rect.Y + 4), new StringFormat(StringFormatFlags.DirectionRightToLeft));
            }
            else
                e.Graphics.DrawString(text, f, new SolidBrush(Color.White), new PointF(node.x + rect.X, node.y + rect.Y - 4), new StringFormat(StringFormatFlags.DirectionRightToLeft));
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
        public int width = 350, height;
        public int x, y;
        public int layer = 0;
        public Color headerColor = Color.DarkOrange;
        public string nodeTitle = "node";
        public string typeTitle = "type";
        public string handleAs = "null";
        public int id;

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
        
        public void AddSocket(bool isInput, string text, string type, Color color)
        {
            if(isInput)
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2,
                    inSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 40,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketInput(text, type, color, this, r));
                inSockets++;
            }
            else
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2 + width,
                    outSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 40,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketOutput(text, type, color, this, r));
                outSockets++;
            }

            int greater = inSockets > outSockets ? inSockets : outSockets;
            height = 90 + (socketSpacing * greater);
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
        public virtual void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(backBrush, x, y, width, height);
            e.Graphics.FillRectangle(new SolidBrush(headerColor), x, y, width, headerHeight);

            e.Graphics.DrawString(nodeTitle, new Font("Arial", 18, FontStyle.Bold), new SolidBrush(Color.White), x + 3, y + 3);
            e.Graphics.DrawString(typeTitle, new Font("Arial", 18, FontStyle.Regular), new SolidBrush(Color.White), x + 3, y + 29);

            Color borderColor = selected ? Color.White : Color.Black;
            e.Graphics.DrawRectangle(new Pen(borderColor), x, y, width, height);

            foreach (var s in sockets)
            {
                s.Value.Draw(e);
            }
        }
    }

    public class TriggerScripterNode_Trigger : TriggerScripterNode
    {
        public PropertyItem_String nameProperty;
        public PropertyItem_Bool conditionalType;
        public TriggerScripterNode_Trigger(TriggerscripterControl control, int px, int py) : base(control, px, py)
        {
            nameProperty = new PropertyItem_String();
            nameProperty.tb.TextChanged += OnNameChange;

            conditionalType = new PropertyItem_Bool("And", "Or");
        }

        public override void Selected()
        {
            MainWindow.propertyEditor.control.Clear();
            MainWindow.propertyEditor.control.AddProperty(nameProperty);
            MainWindow.propertyEditor.control.AddProperty(conditionalType);
        }
        public override void Deselected()
        {
            MainWindow.propertyEditor.control.RemoveProperty(nameProperty);
            MainWindow.propertyEditor.control.RemoveProperty(conditionalType);
        }

        public void OnNameChange(object o, EventArgs e)
        {
            nodeTitle = nameProperty.tb.Text;
        }
    }
}
