using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.Triggerscripter
{
    class TriggerScripterConnection
    {
        public TriggerScripterConnection(TriggerScripterSocket i, TriggerScripterSocket o) { inSocket = i; outSocket = o; }
        public TriggerScripterSocket inSocket, outSocket;
    }

    class TriggerScripterSocket
    {
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
            return (x >= rect.X && x < rect.X + rect.Width && y >= rect.Y && y < rect.Y + rect.Height);
        }
        public void Draw(PaintEventArgs e)
        {
            Rectangle nodePlusOffs = new Rectangle(node.x + rect.X, node.y + rect.Y, rect.Width, rect.Height);
            e.Graphics.FillRectangle(new SolidBrush(color), nodePlusOffs);
            e.Graphics.DrawRectangle(new Pen(Color.DarkGray), nodePlusOffs);
            e.Graphics.DrawString(text, new Font("Arial", 7.5f, FontStyle.Regular), new SolidBrush(Color.White), rect.X + 12, rect.Y - 3);
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
        public Dictionary<Rectangle, TriggerScripterSocket> inputSockets = new Dictionary<Rectangle, TriggerScripterSocket>();
        public Dictionary<Rectangle, TriggerScripterSocket> outputSockets = new Dictionary<Rectangle, TriggerScripterSocket>();

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
                    -socketSize / 2,
                    inputSockets.Count * socketSpacing - (socketSize / 2) + headerHeight + 25,
                    socketSize,
                    socketSize);
                inputSockets.Add(r, new TriggerScripterSocket(text, color, this, r));
            }

            int greater = inputSockets.Count > outputSockets.Count ? inputSockets.Count : outputSockets.Count;
            height = 50 + (socketSpacing * greater);
        }

        static int socketSize = 10;
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

            foreach(var v in inputSockets)
            {
                v.Value.Draw(e);
            }
        }
    }
}
