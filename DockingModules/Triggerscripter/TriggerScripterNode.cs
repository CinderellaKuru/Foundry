using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.Triggerscripter
{
    class TriggerScripterConectionComparer : IEqualityComparer<TriggerScripterConnection>
    {
        public bool Equals(TriggerScripterConnection x, TriggerScripterConnection y)
        {
            if (x.a == y.a &&
                x.b == y.b) return true;

            if (x.a == y.b &&
                x.b == y.a) return true;

            return false;
        }

        public int GetHashCode(TriggerScripterConnection obj)
        {
            throw new NotImplementedException();
        }
    }
    class TriggerScripterConnection
    {
        public TriggerScripterConnection(TriggerScripterSocket i, TriggerScripterSocket o) { a = i; b = o; }
        public TriggerScripterSocket a, b;


    }

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
            Font f = new Font("Arial", 14.5f * node.owner.zoom, FontStyle.Regular);

            Point[] p = new Point[] {
                new Point(node.x + rect.X + 15, node.y + rect.Y - 14),
                new Point(node.x + rect.X + 15, node.y + rect.Y + 5),
                new Point(node.x + rect.X + 15, node.y + rect.Y - 7)
            };
            e.Graphics.Transform.TransformPoints(p);
            if (valueType != "")
            {
                TextRenderer.DrawText(e.Graphics, text, f, p[0], Color.White);
                TextRenderer.DrawText(e.Graphics, "[" + valueType + "]", f, p[1], Color.White);
            }
            else
                TextRenderer.DrawText(e.Graphics, "[" + valueType + "]", f, p[2], Color.White);
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
            Font fs = new Font("Arial", 14.5f * node.owner.zoom, FontStyle.Regular);
            Point[] p = new Point[] {
                new Point(node.x + rect.X - TextRenderer.MeasureText(e.Graphics, text, f).Width, node.y + rect.Y - 14),
                new Point(node.x + rect.X - TextRenderer.MeasureText(e.Graphics, "[" + valueType + "]", f).Width, node.y + rect.Y + 5),
                new Point(node.x + rect.X - TextRenderer.MeasureText(e.Graphics, "[" + valueType + "]", f).Width, node.y + rect.Y - 7)
            };
            e.Graphics.Transform.TransformPoints(p);
            if (valueType != "")
            {
                TextRenderer.DrawText(e.Graphics, text, fs, p[0], Color.White);
                TextRenderer.DrawText(e.Graphics, "[" + valueType + "]", fs, p[1], Color.White);
            }
            else
                TextRenderer.DrawText(e.Graphics, "[" + valueType + "]", fs, p[2], Color.White);
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
                    inSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 25,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketInput(text, type, color, this, r));
                inSockets++;
            }
            else
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2 + width,
                    outSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 25,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketOutput(text, type, color, this, r));
                outSockets++;
            }

            int greater = inSockets > outSockets ? inSockets : outSockets;
            height = 50 + (socketSpacing * greater);
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

        static int socketSpacing = 60;
        static int headerHeight = 60;
        static Brush backBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 40));
        public void Draw(PaintEventArgs e)
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
}
