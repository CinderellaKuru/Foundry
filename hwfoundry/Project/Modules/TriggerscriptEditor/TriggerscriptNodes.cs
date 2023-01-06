using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry.Project.Modules.TriggerscriptEditor
{
    //////////////////////////////////////////////////////////////////////////////////////
    public class TriggerscripterSocket
    {
        public static int socketSize = 20;
        public Rectangle rect;
        public TriggerscripterNode node;
        public Type limitType;
        public List<TriggerscripterSocket> connectedSockets = new List<TriggerscripterSocket>();
        public TriggerscripterSocket(
            string name, string type, Color socketColor, TriggerscripterNode n, Rectangle r, 
            bool showType, bool multiConnection, Type limitType)
        {
            color = socketColor;
            text = name;
            valueType = type;
            node = n;
            rect = r;
            this.multiConnection = multiConnection;
            this.showType = showType;
            this.limitType = limitType;
        }

        public bool multiConnection = true;
        protected bool showType = true;
        public string text;
        public string valueType = "null";
        public Color color;
        public bool PointIsIn(int x, int y)
        {
            return (
                x >= node.x + rect.X-rect.Width && 
                x < node.x + rect.X + (rect.Width*2) && 
                y >= node.y + rect.Y - rect.Height && 
                y < node.y + rect.Y + (rect.Height*2));
        }
        public virtual void Draw(PaintEventArgs e)
        {
            Rectangle nodePlusOffs = new Rectangle(node.x + rect.X, node.y + rect.Y, rect.Width, rect.Height);
            e.Graphics.FillRectangle(new SolidBrush(color), nodePlusOffs);
			if (node.owner.DrawDetail())
			{
				e.Graphics.DrawRectangle(new Pen(Color.Black, 1.0f), nodePlusOffs);
			}
        }
    }
    public class TriggerscripterSocket_Input : TriggerscripterSocket
    {
        public TriggerscripterSocket_Input(string name, string type, Color socketCoolor, TriggerscripterNode n, Rectangle r, bool showType, bool multiConnection, Type limitType)
            : base(name, type, socketCoolor, n, r, showType, multiConnection, limitType)
        {

        }
        public void FinalizeConnection(TriggerscripterSocket_Output s)
        {
            connectedSockets.Add(s);
        }
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 14.5f, FontStyle.Regular);

			if (node.owner.DrawDetail())
			{
				if (showType)
				{
					node.DrawStringOnNode(e.Graphics, f, text, Color.White,
						rect.X + 27,
						rect.Y - 14);
					node.DrawStringOnNode(e.Graphics, f, "[" + valueType + "]", Color.White,
						rect.X + 27,
						rect.Y + 8);
				}
				else
				{
					node.DrawStringOnNode(e.Graphics, f, text, Color.White,
						rect.X + 27,
						rect.Y - 1);
				}
			}
        }
    }
    public class TriggerscripterSocket_Output : TriggerscripterSocket
    {
        public TriggerscripterSocket_Output(string name, string type, Color socketColor, TriggerscripterNode n, Rectangle r, bool showType, bool multiConnection, Type limitType)
            : base(name, type, socketColor, n, r, showType, multiConnection, limitType)
        {

        }
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
			if (node.owner.DrawDetail())
			{
				Font f = new Font("Arial", 14.5f, FontStyle.Regular);
				if (showType)
				{
					node.DrawStringOnNode(e.Graphics, f, text, Color.White,
						rect.X - 7,
						rect.Y - 12,
						StringFormatFlags.DirectionRightToLeft);
					node.DrawStringOnNode(e.Graphics, f, "[" + valueType + "]", Color.White,
						rect.X - 7,
						rect.Y + 10,
						StringFormatFlags.DirectionRightToLeft);
				}
				else
				{
					node.DrawStringOnNode(e.Graphics, f, text, Color.White,
						rect.X - 8,
						rect.Y,
						StringFormatFlags.DirectionRightToLeft);
				}
			}
        }
        public void DrawConnections(PaintEventArgs e)
        {
            foreach (TriggerscripterSocket s in connectedSockets)
            {
                int x1 = node.x + rect.X + (socketSize / 2);
                int y1 = node.y + rect.Y + (socketSize / 2);
                int x2 = s.node.x + s.rect.X + (socketSize / 2);
                int y2 = s.node.y + s.rect.Y + (socketSize / 2);

                int halfWidth = (x2 - x1) / 2;

                Pen p = new Pen(new SolidBrush(color), 5.0f);
                e.Graphics.DrawLine(p, x1, y1, x1 + halfWidth, y1);
                e.Graphics.DrawLine(p, x1 + halfWidth, y1, x1 + halfWidth, y2);
                e.Graphics.DrawLine(p, x1 + halfWidth, y2, x2, y2);
            }
        }
        public void Connect(TriggerscripterSocket_Input s)
        {
            if (!multiConnection && connectedSockets.Count > 0)
                return;
            if (!s.multiConnection && s.connectedSockets.Count > 0)
                return;

            if(limitType !=null)
                if (!limitType.Equals(s.node.GetType()))
                    return;
            if(s.limitType != null)
                if (!s.limitType.Equals(node.GetType()))
                    return;

            if (valueType == s.valueType)
            {
                connectedSockets.Add(s);
                s.FinalizeConnection(this);
            }
        }
        public void Disconnect(TriggerscripterSocket_Input s)
        {
            if (connectedSockets.Contains(s))
            {
                connectedSockets.Remove(s);
                s.connectedSockets.Remove(this);
            }
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////
    public class TriggerscripterNode
    {
        public TriggerscriptEditorPage owner;
        public int width = 350, height, bottomPadding = 0;
        public int x, y;
        public int layer = 0;
        public Color headerColor = Color.DarkOrange;
        public string nodeTitle = "node";
        public string typeTitle = "type";
        public string handleAs = "null";
        public int id = -1;

        public object data;

        public int selectedX = 0, selectedY = 0;
        public bool selected = false;

        public Dictionary<string, TriggerscripterSocket> sockets = new Dictionary<string, TriggerscripterSocket>();
        int inSockets = 0, outSockets = 0;

        public TriggerscripterNode(TriggerscriptEditorPage control, int px, int py)
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
        
        public void AddSocket(bool isInput, string text, string type, Color color, bool showType = true, bool multiConnection = true, Type limitType = null)
        {
            if(isInput)
            {
                Rectangle r = new Rectangle(
                    -TriggerscripterSocket.socketSize / 2,
                    inSockets * socketSpacing - (TriggerscripterSocket.socketSize / 2) + headerHeight + 40,
                    TriggerscripterSocket.socketSize,
                    TriggerscripterSocket.socketSize);
                sockets.Add(text, new TriggerscripterSocket_Input(text, type, color, this, r, showType, multiConnection, limitType));
                inSockets++;
            }
            else
            {
                Rectangle r = new Rectangle(
                    -TriggerscripterSocket.socketSize / 2 + width,
                    outSockets * socketSpacing - (TriggerscripterSocket.socketSize / 2) + headerHeight + 40,
                    TriggerscripterSocket.socketSize,
                    TriggerscripterSocket.socketSize);
                try
                {
                    sockets.Add(text, new TriggerscripterSocket_Output(text, type, color, this, r, showType, multiConnection, limitType));
                }
                catch { }
                outSockets++;
            }

            int greater = inSockets > outSockets ? inSockets : outSockets;
            height = 90 + (socketSpacing * greater);
        }
        public TriggerscripterSocket GetSocket(string name)
        {
            foreach(TriggerscripterSocket s in sockets.Values)
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
        public bool PointIsIn(int mx, int my)
        {
            if (mx >= x && mx <= x + width && my >= y && my <= y + height)
            {
                return true;
            }
            else
            {
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
                offsX = mx - x;
                offsY = my - y;
                return false;
            }
        }
        public bool IntersectsRect(int rx, int ry, int rwidth, int rheight)
        {
            Rectangle r1 = new Rectangle(x, y, width, height);
            Rectangle r2 = new Rectangle(rx, ry, rwidth, rheight);

            return r1.IntersectsWith(r2);
        }
        public void GetPointOffset(int mx, int my, out int ox, out int oy)
        {
            ox = mx - x;
            oy = my - y;
        }

        static int socketSpacing = 65;
        static int headerHeight = 70;
        public static Brush backBrush = new SolidBrush(Color.FromArgb(255, 40, 40, 40));
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

			if (owner.DrawDetail())
			{
            Font fb = new Font("Arial", 18, FontStyle.Bold);
            Font fr = new Font("Arial", 18, FontStyle.Regular);
            DrawStringOnNode(e.Graphics, fb, TrunicateString(e.Graphics, fb, nodeTitle, width - 60), Color.White, 3, 3);
            DrawStringOnNode(e.Graphics, fr, TrunicateString(e.Graphics, fr, typeTitle, width - 60), Color.White, 3, 29);

				Color borderColor = selected ? Color.White : Color.Black;
				e.Graphics.DrawRectangle(new Pen(borderColor, 1.0f), x, y, width, height + bottomPadding);
			}

            foreach (var socket in sockets)
            {
                socket.Value.Draw(e);
            }
        }
    }
    public class TriggerscripterNode_Trigger : TriggerscripterNode
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private bool conditionalOr;
        public bool ConditionalOr
        {
            get { return conditionalOr; }
            set { conditionalOr = value; }
        }
        private bool active;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }
        private bool conditional;
        public bool Conditional
        {
            get { return conditional; }
            set { conditional = value; }
        }
        
        public TriggerscripterNode_Trigger(TriggerscriptEditorPage control, int px, int py) : base(control, px, py)
        {
            typeTitle = "Trigger";
            handleAs = "Trigger";
            AddSocket(true, "Caller", "TRG", TriggerscriptEditorPage.trgColor, false, true);
            AddSocket(true, "Conditions", "CND", TriggerscriptEditorPage.cndColor, false, true);
            AddSocket(false, "Call On True", "EFF", TriggerscriptEditorPage.effColor, false, false);
            AddSocket(false, "Call On False", "EFF", TriggerscriptEditorPage.effColor, false, false);
        }
        
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);

			if (owner.DrawDetail())
			{
				e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2.0f), x + width - 30, y + 10, 20, 20);
				if (active)
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.Green), x + width - 30, y + 10, 20, 20);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.Red), x + width - 30, y + 10, 20, 20);
				}

				e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2.0f), x + width - 60, y + 10, 20, 20);
				if (!conditional)
				{
					e.Graphics.FillEllipse(backBrush, x + width - 60, y + 10, 20, 20);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(205, 170, 0)), x + width - 60, y + 10, 20, 20);
				}
			}
        }
    }
    public class TriggerscripterNode_Variable : TriggerscripterNode
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string valueString;
        public string Value
        {
            get { return valueString; }
            set { valueString = value; }
        }

        public TriggerscripterNode_Variable(TriggerscriptEditorPage c, int x, int y) : base(c,x,y)
        {
            handleAs = "Variable";
        }
        
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
			if (owner.DrawDetail())
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), x + 10, y + height - 10, width - 20, 50);
				Font f = new Font("Arial", 16, FontStyle.Regular);
				DrawStringOnNode(e.Graphics, f, TrunicateString(e.Graphics, f, valueString, width - 40), Color.White, 15, height);
			}
        }
    }
    public class TriggerscripterNode_Condition : TriggerscripterNode
    {
        private bool inverted;
        public bool Inverted
        {
            get { return inverted; }
            set { inverted = value; }
        }

        public TriggerscripterNode_Condition(TriggerscriptEditorPage c, int x, int y) : base(c, x, y)
        {
            handleAs = "Condition";
            typeTitle = "Condition";
        }
        
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 27, FontStyle.Regular);

			if (owner.DrawDetail())
			{
				e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2.0f), x + width - 30, y + 10, 20, 20);
				if (!inverted)
				{
					e.Graphics.FillEllipse(backBrush, x + width - 30, y + 10, 20, 20);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(205, 170, 0)), x + width - 30, y + 10, 20, 20);
				}
			}
        }
    }
}
