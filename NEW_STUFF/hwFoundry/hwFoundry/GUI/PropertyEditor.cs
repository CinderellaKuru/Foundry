using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace hwFoundry.GUI
{
    public partial class PropertyEditor : DockContent
    {
        public PropertyEditor()
        {
            InitializeComponent();
            propertyGrid.HiddenProperties = new string[]
            {
                "AutoSize", "BackColor", "Bottom", "ControlsCount",
                "ContextMenuStrip", "ForeColor", "Guid", "Height",
                "InputOptionsCount", "IsActive", "IsSelected", "ItemHeight",
                "Left", "LetGetOptions", "Location", "LockOption", "Mark",
                "MarkColor", "MarkLines", "MarkRectangle", "OutputOptionsCount",
                "Owner", "Rectangle", "Right", "Size", "Tag", "Title", "TitleColor",
                "TitleHeight", "TitleRectangle", "Top", "Width", "LockLocation",
                "Length"
            };
        }

        public void SetSelectedObject(object o)
            => propertyGrid.SelectedObject = o ?? string.Empty;
    }
}
