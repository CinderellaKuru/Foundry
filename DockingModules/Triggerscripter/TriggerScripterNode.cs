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

    class TriggerScripterSocket
    {
        public static int socketSize = 10;
        public Rectangle rect;
        public TriggerScripterNode node;
        public TriggerScripterSocket(string name, Color socketColor, TriggerScripterNode n, Rectangle r)
        {
            color = socketColor;
            text = name;
            node = n;
            rect = r;
        }

        string text;
        Color color;
        public bool MouseIsIn(int x, int y)
        {
            return (x >= node.x + rect.X && x < node.x + rect.X + rect.Width && y >= node.y + rect.Y && y < node.y + rect.Y + rect.Height);
        }
        public virtual void Draw(PaintEventArgs e)
        {
            Rectangle nodePlusOffs = new Rectangle(node.x + rect.X, node.y + rect.Y, rect.Width, rect.Height);
            e.Graphics.FillRectangle(new SolidBrush(color), nodePlusOffs);
            e.Graphics.DrawRectangle(new Pen(Color.DarkGray), nodePlusOffs);
            e.Graphics.DrawString(text, new Font("Arial", 7.5f, FontStyle.Regular), new SolidBrush(Color.White), node.x + rect.X + 12, node.y + rect.Y - 3);
        }
    }
    class TriggerScripterSocketInput : TriggerScripterSocket
    {
        List<TriggerScripterSocket> connectedSockets = new List<TriggerScripterSocket>();
        public TriggerScripterSocketInput(string name, Color socketCoolor, TriggerScripterNode n, Rectangle r)
            : base(name, socketCoolor, n, r)
        {

        }
    }
    class TriggerScripterSocketOutput : TriggerScripterSocket
    {
        public TriggerScripterSocketOutput(string name, Color socketCoolor, TriggerScripterNode n, Rectangle r)
            : base(name, socketCoolor, n, r)
        {

        }
        List<TriggerScripterSocket> connectedSockets = new List<TriggerScripterSocket>();

        Pen connectionPen = new Pen(Color.DarkGray, 3.0f);
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            foreach(TriggerScripterSocket s in connectedSockets)
            {
                e.Graphics.DrawLine(connectionPen,
                    node.x + rect.X + (socketSize / 2),
                    node.y + rect.Y + (socketSize / 2), 
                    s.node.x + s.rect.X + (socketSize / 2), 
                    s.node.y + s.rect.Y + (socketSize / 2));
            }
        }
        public void Connect(TriggerScripterSocket s)
        {
            connectedSockets.Add(s);
        }
    }

    class TriggerScripterNode
    {
        public int width = 210, height;
        public int x, y;
        public int layer = 0;
        public bool selected = false;
        public Color headerColor = Color.DarkOrange;
        public string nodeTitle = "node";
        public string typeTitle = "type";

        public Dictionary<Rectangle, TriggerScripterSocket> sockets = new Dictionary<Rectangle, TriggerScripterSocket>();
        int inSockets = 0, outSockets = 0;

        public TriggerScripterNode(int px, int py)
        {
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
        
        public void AddSocket(bool isInput, string text, Color color)
        {
            if(isInput)
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2,
                    inSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 25,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketInput(text, color, this, r));
                inSockets++;
            }
            else
            {
                Rectangle r = new Rectangle(
                    -TriggerScripterSocket.socketSize / 2 + width,
                    outSockets * socketSpacing - (TriggerScripterSocket.socketSize / 2) + headerHeight + 25,
                    TriggerScripterSocket.socketSize,
                    TriggerScripterSocket.socketSize);
                sockets.Add(r, new TriggerScripterSocketOutput(text, color, this, r));
                outSockets++;
            }

            int greater = inSockets > outSockets ? inSockets : outSockets;
            height = 50 + (socketSpacing * greater);
        }

        static int socketSpacing = 35;
        static int headerHeight = 35;
        static Brush backBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 40));
        public void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(backBrush, x, y, width, height);
            e.Graphics.FillRectangle(new SolidBrush(headerColor), x, y, width, headerHeight);

            e.Graphics.DrawString(nodeTitle, new Font("Arial", 10.0f, FontStyle.Bold), new SolidBrush(Color.White), x + 3, y + 1);
            e.Graphics.DrawString(typeTitle, new Font("Arial", 10.0f, FontStyle.Regular), new SolidBrush(Color.White), x + 3, y + 16);

            Color borderColor = selected ? Color.White : Color.Black;
            e.Graphics.DrawRectangle(new Pen(borderColor), x, y, width, height);

            foreach (var s in sockets)
            {
                s.Value.Draw(e);
            }
        }
    }
}
