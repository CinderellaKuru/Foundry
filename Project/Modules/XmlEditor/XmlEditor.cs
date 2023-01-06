using Foundry.Project.Modules.Base;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foundry.Project.Modules.XmlEditor
{
    public class XmlEditorPage : BaseSceneEditorPage
    {
        Scintilla editor;
        public XmlEditorPage(FoundryInstance i) : base(i)
        {
            editor = new Scintilla();
            editor.Location = new Point(0, 0);
            editor.Size = new Size(1, 1);
            editor.Dock = DockStyle.Fill;
            editor.MultipleSelection = true;
            editor.MultiPaste = MultiPaste.Each;
            editor.EndAtLastLine = false;
            editor.EolMode = Eol.Lf;
            editor.CaretForeColor = Color.White;

            editor.StyleResetDefault();
            editor.Styles[Style.Default].Font = "Consolas";
            editor.Styles[Style.Default].Size = 10;
            editor.Styles[Style.Default].BackColor = Color.FromArgb(41, 49, 52);
            editor.StyleClearAll();

            editor.Styles[Style.Xml.Default].ForeColor = Color.White;
            editor.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(103, 140, 177);
            editor.Styles[Style.Xml.Comment].ForeColor = Color.DarkGray;
            editor.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(179, 182, 137);
            editor.Styles[Style.Xml.DoubleString].ForeColor = Color.FromArgb(225, 226, 207);
            editor.Styles[Style.Xml.Other].ForeColor = Color.FromArgb(225, 226, 207);
            editor.Lexer = Lexer.Xml;

            Controls.Add(editor);
        }
    }
}
