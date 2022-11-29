namespace hwFoundry.GUI
{
    partial class TriggerScripter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (Program.mainWindow.modProject.activeFile.IsDirty)
            {
                var result = MessageBox.Show("Current file has not been saved. Would you like to save the file before closing?", "Closing File Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                    Program.mainWindow.modProject.SaveActiveFile();
                
                else if (result == DialogResult.Cancel)
                    return;
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            IsDead = true;
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.nodeEditor = new ST.Library.UI.NodeEditor.STNodeEditor();
            this.nodeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uLockLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uLockConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // nodeEditor
            // 
            this.nodeEditor.AllowDrop = true;
            this.nodeEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.nodeEditor.Curvature = 0.3F;
            this.nodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeEditor.Location = new System.Drawing.Point(0, 0);
            this.nodeEditor.LocationBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeEditor.MarkBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeEditor.MarkForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeEditor.MinimumSize = new System.Drawing.Size(100, 100);
            this.nodeEditor.Name = "nodeEditor";
            this.nodeEditor.Size = new System.Drawing.Size(800, 450);
            this.nodeEditor.TabIndex = 0;
            this.nodeEditor.Text = "NodeEditor";
            // 
            // nodeContextMenuStrip
            // 
            this.nodeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.uLockLocationToolStripMenuItem,
            this.uLockConnectionToolStripMenuItem});
            this.nodeContextMenuStrip.Name = "nodeContextMenuStrip";
            this.nodeContextMenuStrip.Size = new System.Drawing.Size(181, 92);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // uLockLocationToolStripMenuItem
            // 
            this.uLockLocationToolStripMenuItem.Name = "uLockLocationToolStripMenuItem";
            this.uLockLocationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.uLockLocationToolStripMenuItem.Text = "U/Lock &Location";
            this.uLockLocationToolStripMenuItem.Click += new System.EventHandler(this.uLockLocationToolStripMenuItem_Click);
            // 
            // uLockConnectionToolStripMenuItem
            // 
            this.uLockConnectionToolStripMenuItem.Name = "uLockConnectionToolStripMenuItem";
            this.uLockConnectionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.uLockConnectionToolStripMenuItem.Text = "U/Lock &Connection";
            this.uLockConnectionToolStripMenuItem.Click += new System.EventHandler(this.uLockConnectionToolStripMenuItem_Click);
            // 
            // TriggerScripter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.nodeEditor);
            this.Name = "TriggerScripter";
            this.Text = "Trigger Scripter";
            this.nodeContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public ST.Library.UI.NodeEditor.STNodeEditor nodeEditor;
        private ContextMenuStrip nodeContextMenuStrip;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStripMenuItem uLockLocationToolStripMenuItem;
        private ToolStripMenuItem uLockConnectionToolStripMenuItem;
    }
}