using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Foundry.Project.Modules.TriggerscriptEditor.TriggerscriptEditorPage;

namespace Foundry.Project.Modules.TriggerscriptEditor
{
    //////////////////////////////////////////////////////////////////////////////////////
    public class TriggerscripterSocket
    {
        public static int SocketSize = 20;
		public TriggerscripterNode OwnerNode { get; private set; }
		public List<TriggerscripterSocket> ConnectedSockets { get; private set; }
		public Rectangle BoundingRect { get; private set; }
        public bool MultiConnection { get; private set; }
        public bool ShowType { get; protected set; }
        public string Text { get; private set; }
        public Color Color { get; private set; }
        public string ValueType { get; private set; }

        public TriggerscripterSocket(string name, string type, Color socketColor, TriggerscripterNode n, Rectangle bounds, bool showType, bool multiConnection)
        {
            OwnerNode = n;
			ConnectedSockets = new List<TriggerscripterSocket>();
            BoundingRect = bounds;
            MultiConnection = multiConnection;
            ShowType = showType;
            Text = name;
            Color = socketColor;
            ValueType = type;
		}
		/// <summary>
		/// Draws the socket. Intended to be called by its owner node.
		/// Can be overriden by different types of sockets for specialized drawing.
		/// </summary>
        public virtual void Draw(PaintEventArgs e)
        {
            Rectangle nodePlusOffs = new Rectangle(OwnerNode.PosX + BoundingRect.X, OwnerNode.PosY + BoundingRect.Y, BoundingRect.Width, BoundingRect.Height);
            e.Graphics.FillRectangle(new SolidBrush(Color), nodePlusOffs);
			if (OwnerNode.Owner.DrawDetail())
			{
				e.Graphics.DrawRectangle(new Pen(Color.Black, 1.0f), nodePlusOffs);
			}
        }
        public bool PointIsIn(int x, int y)
        {
            return x >= OwnerNode.PosX + BoundingRect.X - BoundingRect.Width && 
				   x < OwnerNode.PosX + BoundingRect.X + (BoundingRect.Width*2) && 
                   y >= OwnerNode.PosY + BoundingRect.Y - BoundingRect.Height && 
                   y < OwnerNode.PosY + BoundingRect.Y + (BoundingRect.Height*2);
        }
    }
    public class TriggerscripterSocket_Input : TriggerscripterSocket
    {
        public TriggerscripterSocket_Input(string name, string type, Color socketCoolor, TriggerscripterNode n, Rectangle bounds, bool showType, bool multiConnection)
            : base(name, type, socketCoolor, n, bounds, showType, multiConnection)
        {

        }
		/// <summary>
		/// Called by a connecting TriggerscripterSocket_Output to add this connection to this socket's list.
		/// </summary>
		public void FinalizeConnection(TriggerscripterSocket_Output s)
        {
            ConnectedSockets.Add(s);
        }
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 14.5f, FontStyle.Regular);

			if (OwnerNode.Owner.DrawDetail())
			{
				if (ShowType)
				{
					OwnerNode.DrawStringOnNode(e.Graphics, f, Text, Color.White,
						BoundingRect.X + 27,
						BoundingRect.Y - 14);
					OwnerNode.DrawStringOnNode(e.Graphics, f, "[" + ValueType + "]", Color.White,
						BoundingRect.X + 27,
						BoundingRect.Y + 8);
				}
				else
				{
					OwnerNode.DrawStringOnNode(e.Graphics, f, Text, Color.White,
						BoundingRect.X + 27,
						BoundingRect.Y - 1);
				}
			}
        }
    }
    public class TriggerscripterSocket_Output : TriggerscripterSocket
    {
        public TriggerscripterSocket_Output(string name, string type, Color socketColor, TriggerscripterNode n, Rectangle bounds, bool showType, bool multiConnection)
            : base(name, type, socketColor, n, bounds, showType, multiConnection)
        {

        }
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
			if (OwnerNode.Owner.DrawDetail())
			{
				Font f = new Font("Arial", 14.5f, FontStyle.Regular);
				if (ShowType)
				{
					OwnerNode.DrawStringOnNode(e.Graphics, f, Text, Color.White,
						BoundingRect.X - 7,
						BoundingRect.Y - 12,
						StringFormatFlags.DirectionRightToLeft);
					OwnerNode.DrawStringOnNode(e.Graphics, f, "[" + ValueType + "]", Color.White,
						BoundingRect.X - 7,
						BoundingRect.Y + 10,
						StringFormatFlags.DirectionRightToLeft);
				}
				else
				{
					OwnerNode.DrawStringOnNode(e.Graphics, f, Text, Color.White,
						BoundingRect.X - 8,
						BoundingRect.Y,
						StringFormatFlags.DirectionRightToLeft);
				}
			}
        }
		/// <summary>
		/// Draw all of this socket's connections. Intended to be called from a control paint event.
		/// </summary>
        public void DrawConnections(PaintEventArgs e)
        {
            foreach (TriggerscripterSocket s in ConnectedSockets)
            {
                int x1 = OwnerNode.PosX + BoundingRect.X + (SocketSize / 2);
                int y1 = OwnerNode.PosY + BoundingRect.Y + (SocketSize / 2);
                int x2 = s.OwnerNode.PosX + s.BoundingRect.X + (SocketSize / 2);
                int y2 = s.OwnerNode.PosY + s.BoundingRect.Y + (SocketSize / 2);

                int halfWidth = (x2 - x1) / 2;

                Pen p = new Pen(new SolidBrush(Color), 5.0f);
                e.Graphics.DrawLine(p, x1, y1, x1 + halfWidth, y1);
                e.Graphics.DrawLine(p, x1 + halfWidth, y1, x1 + halfWidth, y2);
                e.Graphics.DrawLine(p, x1 + halfWidth, y2, x2, y2);
            }
        }
        public void Connect(TriggerscripterSocket_Input s)
        {
            if (!MultiConnection && ConnectedSockets.Count > 0)
				//this socket is not multi-connect enabled, so if there are more than 0 connected sockets ignore this connect attempt.
                return;
			if (!s.MultiConnection && s.ConnectedSockets.Count > 0)
				//the connected socket is not multi-connect enabled, so if there is more than 0 sockets connected to it, ignore this connect attempt.
                return;


            if (ValueType == s.ValueType)
            {
				//if the value type matches, connect these sockets. 
                ConnectedSockets.Add(s);
                s.FinalizeConnection(this);
            }
        }
        public void Disconnect(TriggerscripterSocket_Input s)
        {
            if (ConnectedSockets.Contains(s))
            {
				//if the connection exists, remove connection from both nodes' sockets.
                ConnectedSockets.Remove(s);
                s.ConnectedSockets.Remove(this);
            }
        }
    }


    //////////////////////////////////////////////////////////////////////////////////////
    public class TriggerscripterNode
    {
        public TriggerscriptEditorPage Owner { get; private set; }
        public object Data { get; private set; }
		public int Width { get; private set; } = 350;
		public int Height { get; private set; } = 0;
		public int BottomPadding { get; private set; } = 0;
        public int PosX { get; private set; }
		public int PosY { get; private set; }
        public Color HeaderColor { get; private set; } = Color.Black;
        public string Name { get; private set; } = "node";
        public string Type { get; private set; } = "null";
        public string HandleAs { get; private set; } = "null";
        public int Id { get; private set; } = -1;


		//TODO make these properties.
        public int selectedX = 0, selectedY = 0;
        public bool selected = false;
		//these too
        public Dictionary<string, TriggerscripterSocket> sockets = new Dictionary<string, TriggerscripterSocket>();
        int inSockets = 0, outSockets = 0;

        public TriggerscripterNode(TriggerscriptEditorPage control, object data, int px, int py, int id, string name, string type, string handleAs, Color headerColor, int bottomPadding = 0)
        {
            Owner = control;
			Data = data;
            PosX = px;
            PosY = py;
			Name = name;
			Id = id;
			HeaderColor = headerColor;
			Type = type;
			HandleAs = handleAs;
			BottomPadding = bottomPadding;
        }

        public void SetPos(int px, int py)
        {
            PosX = px;
            PosY = py;
        }
        public void Move(int mx, int my)
        {
            PosX += mx;
            PosY += my;
        }
        
        public void AddSocket(bool isInput, string text, string type, Color Color, bool showType = true, bool multiConnection = true)
        {
            if(isInput)
            {
                Rectangle r = new Rectangle(
                    -TriggerscripterSocket.SocketSize / 2,
                    inSockets * socketSpacing - (TriggerscripterSocket.SocketSize / 2) + headerHeight + 40,
                    TriggerscripterSocket.SocketSize,
                    TriggerscripterSocket.SocketSize);
                sockets.Add(text, new TriggerscripterSocket_Input(text, type, Color, this, r, showType, multiConnection));
                inSockets++;
            }
            else
            {
                Rectangle r = new Rectangle(
                    -TriggerscripterSocket.SocketSize / 2 + Width,
                    outSockets * socketSpacing - (TriggerscripterSocket.SocketSize / 2) + headerHeight + 40,
                    TriggerscripterSocket.SocketSize,
                    TriggerscripterSocket.SocketSize);
                try
                {
                    sockets.Add(text, new TriggerscripterSocket_Output(text, type, Color, this, r, showType, multiConnection));
                }
                catch { }
                outSockets++;
            }

            int greater = inSockets > outSockets ? inSockets : outSockets;
            Height = 90 + (socketSpacing * greater);
        }
        public TriggerscripterSocket GetSocket(string name)
        {
            foreach(TriggerscripterSocket s in sockets.Values)
            {
                if (s.Text == name)
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
            if (mx >= PosX && mx <= PosX + Width && my >= PosY && my <= PosY + Height)
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
            if (mx >= PosX && mx <= PosX + Width && my >= PosY && my <= PosY + headerHeight)
            {
                offsX = mx - PosX;
                offsY = my - PosY;
                return true;
            }
            else
            {
                offsX = mx - PosX;
                offsY = my - PosY;
                return false;
            }
        }
        public bool IntersectsRect(int rx, int ry, int rwidth, int rheight)
        {
            Rectangle r1 = new Rectangle(PosX, PosY, Width, Height);
            Rectangle r2 = new Rectangle(rx, ry, rwidth, rheight);

            return r1.IntersectsWith(r2);
        }
        public void GetPointOffset(int mx, int my, out int ox, out int oy)
        {
            ox = mx - PosX;
            oy = my - PosY;
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
			g.DrawString(s, f, new SolidBrush(c), xOffs + PosX, yOffs + PosY, new StringFormat(flags));
		}
        public virtual void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(backBrush, PosX, PosY, Width, Height + BottomPadding);
            e.Graphics.FillRectangle(new SolidBrush(HeaderColor), PosX, PosY, Width, headerHeight);

			if (Owner.DrawDetail())
			{
            Font fb = new Font("Arial", 18, FontStyle.Bold);
            Font fr = new Font("Arial", 18, FontStyle.Regular);
            DrawStringOnNode(e.Graphics, fb, TrunicateString(e.Graphics, fb, Name, Width - 60), Color.White, 3, 3);
            DrawStringOnNode(e.Graphics, fr, TrunicateString(e.Graphics, fr, Type, Width - 60), Color.White, 3, 29);

				Color borderColor = selected ? Color.White : Color.Black;
				e.Graphics.DrawRectangle(new Pen(borderColor, 1.0f), PosX, PosY, Width, Height + BottomPadding);
			}

            foreach (var socket in sockets)
            {
                socket.Value.Draw(e);
            }
        }
    }
	// trigger ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TriggerscripterNode_Trigger : TriggerscripterNode
    {
        public bool ConditionalOr { get; set; }
		public bool Active { get; set; }
		public bool Conditional { get; set; }

		public TriggerscripterNode_Trigger(TriggerscriptEditorPage control, SerializedTrigger t, int px, int py, int id)
			: base(control, t, px, py, id, t.name, "Trigger", "Trigger", trgColor)
        {
            AddSocket(true, "Caller", "TRG", TriggerscriptEditorPage.trgColor, false, true);
            AddSocket(true, "Conditions", "CND", TriggerscriptEditorPage.cndColor, false, true);
            AddSocket(false, "Call On True", "EFF", TriggerscriptEditorPage.effColor, false, false);
            AddSocket(false, "Call On False", "EFF", TriggerscriptEditorPage.effColor, false, false);
        }
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);

			if (Owner.DrawDetail())
			{
				e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2.0f), PosX + Width - 30, PosY + 10, 20, 20);
				if (Active)
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.Green), PosX + Width - 30, PosY + 10, 20, 20);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.Red), PosX + Width - 30, PosY + 10, 20, 20);
				}

				e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2.0f), PosX + Width - 60, PosY + 10, 20, 20);
				if (!Conditional)
				{
					e.Graphics.FillEllipse(backBrush, PosX + Width - 60, PosY + 10, 20, 20);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(205, 170, 0)), PosX + Width - 60, PosY + 10, 20, 20);
				}
			}
        }
    }
	// variable //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class TriggerscripterNode_Variable : TriggerscripterNode
    {
        public string Value { get; set; }

		public TriggerscripterNode_Variable(TriggerscriptEditorPage control, SerializedVariable v, int px, int py, int id)
			: base(control, v, px, py, id, v.name, v.type, "Variable", requiredVarColor, 50)
		{
			AddSocket(true, "Set", v.type, requiredVarColor, false);
			AddSocket(false, "Use", v.type, requiredVarColor, false);
			Value = v.value;
		}
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
			if (Owner.DrawDetail())
			{
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), PosX + 10, PosY + Height - 10, Width - 20, 50);
				Font f = new Font("Arial", 16, FontStyle.Regular);
				DrawStringOnNode(e.Graphics, f, TrunicateString(e.Graphics, f, Value, Width - 40), Color.White, 15, Height);
			}
        }
    }
	// effect ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class TriggerscripterNode_Effect : TriggerscripterNode
	{
		public TriggerscripterNode_Effect(TriggerscriptEditorPage control, SerializedEffect e, int px, int py, int id)
			: base(control, e, px, py, id, e.name, "Condition", "Condition", effColor)
		{
			AddSocket(true, "Caller", "EFF", effColor, false, false);
			AddSocket(false, "Call", "EFF", effColor, false, false);

			if (!e.name.Contains("Trigger"))
			{
				foreach (Input i in e.inputs)
				{
					Color color = i.optional ? optionalVarColor : requiredVarColor;
					AddSocket(true, i.name, i.valueType, color, true, false);
				}
				foreach (Output ou in e.outputs)
				{
					Color color = ou.optional ? optionalVarColor : requiredVarColor;
					AddSocket(false, ou.name, ou.valueType, color, true, false);
				}
			}
			else
			{
				AddSocket(false, "Trigger", "TRG", trgColor, false, false);
			}
		}

		public override void Draw(PaintEventArgs e)
		{
			base.Draw(e);
		}
	}
	// condition /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class TriggerscripterNode_Condition : TriggerscripterNode
    {
        public bool Inverted { get; set; }

		public TriggerscripterNode_Condition(TriggerscriptEditorPage control, SerializedCondition c, int px, int py, int id)
			: base(control, c, px, py, id, c.name, "Condition", "Condition", cndColor)
		{
			AddSocket(false, "Result", "CND", cndColor, false, false);
			foreach (Input i in c.inputs)
			{
				Color color = i.optional ? optionalVarColor : requiredVarColor;
				AddSocket(true, i.name, i.valueType, color, true, false);
			}
			foreach (Output ou in c.outputs)
			{
				Color color = ou.optional ? optionalVarColor : requiredVarColor;
				AddSocket(false, ou.name, ou.valueType, color, true, false);
			}
		}
        public override void Draw(PaintEventArgs e)
        {
            base.Draw(e);
            Font f = new Font("Arial", 27, FontStyle.Regular);

			if (Owner.DrawDetail())
			{
				e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 2.0f), PosX + Width - 30, PosY + 10, 20, 20);
				if (!Inverted)
				{
					e.Graphics.FillEllipse(backBrush, PosX + Width - 30, PosY + 10, 20, 20);
				}
				else
				{
					e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(205, 170, 0)), PosX + Width - 30, PosY + 10, 20, 20);
				}
			}
        }
    }
}
