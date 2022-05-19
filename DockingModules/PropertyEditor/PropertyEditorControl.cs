﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace SMHEditor.DockingModules.PropertyEditor
{
    public class PropertyItem
    {
        protected int xPadding = 5;
        public int y;
        public int height = 26;
        public string name = "name";
        
        public PropertyItem()
        {
        }

        public virtual void Register(PropertyEditorControl c) { c.Invalidate(); }
        public virtual void Unregister(PropertyEditorControl c) { c.Invalidate(); }

        public virtual void Draw(PaintEventArgs e, int controlWidth)
        {

        }
    }
    public class PropertyItemBase_TwoColumn : PropertyItem 
    {
        public override void Draw(PaintEventArgs e, int controlWidth)
        {
            base.Draw(e, controlWidth);
            e.Graphics.DrawRectangle(new Pen(Color.DarkGray), new Rectangle(
                xPadding,
                y,
                (controlWidth - xPadding * 2) / 2,
                height));

            e.Graphics.DrawRectangle(new Pen(Color.DarkGray), new Rectangle(
               xPadding + (controlWidth - xPadding * 2) / 2,
               y,
               (controlWidth - xPadding * 2) / 2,
               height));

            e.Graphics.DrawString(name, Control.DefaultFont, new SolidBrush(Color.DarkGray), xPadding, y + 6);
        }
    }
    public class PropertyItem_String : PropertyItemBase_TwoColumn
    {
        public KryptonTextBox tb = new KryptonTextBox();

        public PropertyItem_String() : base()
        {
        }

        public override void Register(PropertyEditorControl c)
        {
            c.Controls.Add(tb);
            base.Register(c);
        }
        public override void Unregister(PropertyEditorControl c)
        {
            c.Controls.Remove(tb);
            base.Unregister(c);
        }

        public override void Draw(PaintEventArgs e, int controlWidth)
        {
            base.Draw(e, controlWidth);
            tb.Location = new Point(((controlWidth - xPadding * 2) / 2) + xPadding + 1, y + 2);
            tb.Width = ((controlWidth - xPadding * 2) / 2) - 1;
            tb.Height = height - 1;
        }
    }
    public class PropertyItem_Bool : PropertyItemBase_TwoColumn
    {
        public string trueText = "True", falseText = "False";
        public bool state = false;
        public Button button;

        public PropertyItem_Bool(string falseText, string trueText) : base()
        {
            button = new Button();
            button.AutoSize = false;
            button.Click += OnPressed;
            this.falseText = falseText;
            this.trueText = trueText;
            button.Text = falseText;
            button.Font = Control.DefaultFont;
        }

        public override void Register(PropertyEditorControl c)
        {
            c.Controls.Add(button);
            base.Register(c);
        }
        public override void Unregister(PropertyEditorControl c)
        {
            c.Controls.Remove(button);
            base.Unregister(c);
        }

        public void OnPressed(object o, EventArgs e)
        {
            state = !state;

            if (state) button.Text = trueText;
            else button.Text = falseText;
        }

        public override void Draw(PaintEventArgs e, int controlWidth)
        {
            base.Draw(e, controlWidth);
            button.Location = new Point(((controlWidth - xPadding * 2) / 2) + xPadding, y);
            button.Width = ((controlWidth - xPadding * 2) / 2) + 1;
            button.Height = height + 1;
        }
    }

    public partial class PropertyEditorControl : UserControl
    {
        private List<PropertyItem> properties = new List<PropertyItem>();
        public PropertyEditorControl()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            currentY = 0;
            foreach (PropertyItem i in properties)
            {
                i.Unregister(this);
            }
            properties.Clear();
        }
        public void AddProperty(PropertyItem i)
        {
            i.y = currentY;
            properties.Add(i);
            i.Register(this);
            currentY += i.height;
        }
        public void RemoveProperty(PropertyItem i)
        {
            properties.Remove(i);
            i.Unregister(this);
            currentY -= i.height;
        }

        int currentY = 0;
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Program.window.darkmode.GetBackColor1(
                ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelClient,
                ComponentFactory.Krypton.Toolkit.PaletteState.Normal));
            base.OnPaint(e);


            foreach (PropertyItem i in properties)
            {
                i.Draw(e, Width);
            }
        }
    }
}
