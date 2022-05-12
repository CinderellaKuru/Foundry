using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.Triggerscripter
{
    class TriggerScripterSocket
    {
        public TriggerScripterSocket(string name, Color socketColor)
        {
            color = socketColor;
            text = name;
        }

        string text;
        Color color;
        public void Draw(PaintEventArgs e, int x, int y)
        {
            e.Graphics.FillRectangle(new SolidBrush(color), new RectangleF(x - 5, y - 5, 10, 10));
            e.Graphics.DrawString(text, new Font("Arial", 9.0f, FontStyle.Regular), new SolidBrush(Color.White), x + 5, y - 8);
        }
    }

    class TriggerScripterNode
    {
        int width = 210, height = 20;

        public int x, y;
        public int layer = 0;
        public bool selected = false;
        public Color headerColor = Color.DarkOrange;
        public string nodeTitle = "node";
        public string typeTitle = "type";
        public List<TriggerScripterSocket> inputSockets = new List<TriggerScripterSocket>();
        public List<TriggerScripterSocket> outputSockets = new List<TriggerScripterSocket>();

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

        static int socketSpacing = 15;
        static int headerHeight = 35;
        static Brush backBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 40));
        public void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(headerColor), x, y, width, headerHeight);
            e.Graphics.FillRectangle(backBrush, x, y + headerHeight, width, height);

            e.Graphics.DrawString(nodeTitle, new Font("Arial", 10.0f, FontStyle.Bold), new SolidBrush(Color.White), x, y + 1);
            e.Graphics.DrawString(typeTitle, new Font("Arial", 10.0f, FontStyle.Regular), new SolidBrush(Color.White), x, y + 16);


            int yTracker = headerHeight + (socketSpacing * 2);
            height = 20;
            foreach(TriggerScripterSocket s in inputSockets)
            {
                s.Draw(e, 0, yTracker);
                yTracker += socketSpacing * 2;
                height += socketSpacing * 2;
            }
        }
    }
}
