using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hwFoundry.Modules.TriggerScripter.Nodes
{
    public abstract class BaseNode : STNode
    {
        /// <summary>
        /// Allows the ability to address a socket by name
        /// </summary>
        private Dictionary<string, STNodeOption> _Sockets = new();
        public Dictionary<string, STNodeOption> Sockets
        {
            get
            {
                if (_Sockets.Count > 0) return _Sockets;
                foreach (STNodeOption socket in InputOptions)  _Sockets.Add(socket.Text, socket);
                foreach (STNodeOption socket in OutputOptions) _Sockets.Add(socket.Text, socket);
                return _Sockets;
            }
        }

        public string ConnectedNodes
        {
            get
            {
                StringBuilder sb = new();
                foreach (STNodeOption socket in InputOptions)
                {
                    if (socket.ConnectionCount > 0)
                        foreach (STNodeOption connection in socket.GetConnectedSockets())
                            sb.Append((connection.Owner as BaseNode).nodeTitle);
                }
                return sb.ToString();
            }
        }

        // Defaults
        protected int NodeWidth     = 200;
        protected int SocketSpacing = 20;
        protected int HeaderHeight  = 30;

        protected Color HeaderColor = Color.DarkOrange;
        protected Color BackgroundColor = Color.FromArgb(255, 40, 40, 40);
        internal readonly Color TrgColor = Color.FromArgb(255, 64, 117, 130);
        internal readonly Color EffColor = Color.FromArgb(255, 130, 64, 106);
        internal readonly Color CndColor = Color.FromArgb(255, 130, 64, 64);
        internal readonly Color RequiredVarColor = Color.FromArgb(255, 64, 130, 64);
        internal readonly Color OptionalVarColor = Color.FromArgb(255, 122, 130, 64);

        public string nodeTitle = "node";
        public string typeTitle = "type";
        public string handleAs  = "null";

        // Properties
        public int id = -1;
        public object? data;

        public BaseNode(int x, int y, int id)
        {
            Location = new Point(x, y);
            this.id = id;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            // Header properties
            Title = " ";
            TitleHeight = HeaderHeight;
            TitleColor  = HeaderColor;

            // Body properties
            AutoSize   = false;
            ItemHeight = SocketSpacing;
            BackColor  = BackgroundColor;
        }

        protected override void OnDrawTitle(DrawingTools dt)
        {
            base.OnDrawTitle(dt);

            // Draw left aligned titles
            dt.Graphics.DrawString(nodeTitle, new Font(Font.FontFamily, Font.Size, FontStyle.Bold), dt.SolidBrush, TitleRectangle.X + 3, TitleRectangle.Y);
            dt.Graphics.DrawString(typeTitle, Font, dt.SolidBrush, TitleRectangle.X + 3, TitleRectangle.Y + Font.Height);
        }

        protected override void OnOwnerChanged()
        {
            base.OnOwnerChanged();

            if (Owner != null)
            {
                // Assign colors based on expected node types
                Owner.SetTypeColor(typeof(ConditionNode), CndColor);
                Owner.SetTypeColor(typeof(EffectNode),    EffColor);
                Owner.SetTypeColor(typeof(TriggerNode),   TrgColor);
                Owner.SetTypeColor(typeof(VariableNode),  RequiredVarColor);

                // Auto-size nodes based on need (intended to fire only once)
                using (Graphics g = this.Owner.CreateGraphics())
                {
                    Size defaultSize = base.GetDefaultNodeSize(g);
                    int newWidth = defaultSize.Width > NodeWidth ? defaultSize.Width : NodeWidth;
                    int newHeight = defaultSize.Height + (ItemHeight * ControlsCount);

                    if (Width != newWidth)
                        Width = newWidth;

                    if (Height != newHeight)
                        Height = newHeight;
                }
            }
        }

        protected override void OnDrawOptionDot(DrawingTools dt, STNodeOption op)
        {
            base.OnDrawOptionDot(dt, op);

            if (op.IsSingle)
            {
                dt.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Black), 1.2f), op.DotRectangle.X, op.DotRectangle.Y, op.DotRectangle.Width, op.DotRectangle.Height);
            }
            else
            {
                dt.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1.2f), op.DotRectangle.X, op.DotRectangle.Y, op.DotRectangle.Width, op.DotRectangle.Height);
            }
        }

        public bool SetAndInvalidate<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                Invalidate();
                return true;
            }
            return false;
        }
    }
}
