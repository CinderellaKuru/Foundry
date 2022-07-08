using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;

namespace SMHEditor
{
    partial class Foundry
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Foundry));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.label = new System.Windows.Forms.ToolStripStatusLabel();
            this.versionReadout = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileTSMI = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectTMI = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectTMI = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveTMI = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsTMI = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewWindowTMI = new System.Windows.Forms.ToolStripMenuItem();
            this.workspace = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.statusBar.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label,
            this.versionReadout});
            this.statusBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusBar.Location = new System.Drawing.Point(0, 659);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusBar.Size = new System.Drawing.Size(1264, 22);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusBar";
            // 
            // label
            // 
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(105, 17);
            this.label.Text = "No project loaded.";
            // 
            // versionReadout
            // 
            this.versionReadout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.versionReadout.ForeColor = System.Drawing.Color.White;
            this.versionReadout.Name = "versionReadout";
            this.versionReadout.Size = new System.Drawing.Size(45, 17);
            this.versionReadout.Text = "version";
            // 
            // menuStrip
            // 
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileTSMI,
            this.windowToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileTSMI
            // 
            this.fileTSMI.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectTMI,
            this.openProjectTMI,
            this.toolStripSeparator1,
            this.saveTMI,
            this.saveAsTMI});
            this.fileTSMI.Name = "fileTSMI";
            this.fileTSMI.Size = new System.Drawing.Size(37, 20);
            this.fileTSMI.Text = "File";
            // 
            // newProjectTMI
            // 
            this.newProjectTMI.Name = "newProjectTMI";
            this.newProjectTMI.Size = new System.Drawing.Size(123, 22);
            this.newProjectTMI.Text = "New";
            // 
            // openProjectTMI
            // 
            this.openProjectTMI.Name = "openProjectTMI";
            this.openProjectTMI.Size = new System.Drawing.Size(123, 22);
            this.openProjectTMI.Text = "Open...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // saveTMI
            // 
            this.saveTMI.Name = "saveTMI";
            this.saveTMI.Size = new System.Drawing.Size(123, 22);
            this.saveTMI.Text = "Save";
            // 
            // saveAsTMI
            // 
            this.saveAsTMI.Name = "saveAsTMI";
            this.saveAsTMI.Size = new System.Drawing.Size(123, 22);
            this.saveAsTMI.Text = "Save As...";
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewWindowTMI});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // addNewWindowTMI
            // 
            this.addNewWindowTMI.Name = "addNewWindowTMI";
            this.addNewWindowTMI.Size = new System.Drawing.Size(170, 22);
            this.addNewWindowTMI.Text = "Add New Window";
            // 
            // workspace
            // 
            this.workspace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workspace.Location = new System.Drawing.Point(0, 27);
            this.workspace.Name = "workspace";
            this.workspace.Size = new System.Drawing.Size(1264, 629);
            this.workspace.TabIndex = 3;
            this.workspace.Theme = new VS2015LightTheme();
            // 
            // Foundry
            // 
            this.AllowFormChrome = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.workspace);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Foundry";
            this.Text = "Foundry";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusBar;
        public System.Windows.Forms.ToolStripStatusLabel label;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileTSMI;
        private System.Windows.Forms.ToolStripMenuItem openProjectTMI;
        private System.Windows.Forms.ToolStripMenuItem saveTMI;
        private System.Windows.Forms.ToolStripMenuItem saveAsTMI;
        private System.Windows.Forms.ToolStripMenuItem newProjectTMI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel versionReadout;
        public ComponentFactory.Krypton.Docking.KryptonDockableWorkspace dockableWorkspace;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewWindowTMI;
        private DockPanel workspace;
    }
}