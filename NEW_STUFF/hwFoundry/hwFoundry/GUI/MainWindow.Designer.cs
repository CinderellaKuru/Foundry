namespace hwFoundry.GUI
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compileProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.label_projectStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_middle = new System.Windows.Forms.ToolStripStatusLabel();
            this.label_version = new System.Windows.Forms.ToolStripStatusLabel();
            this.workspace = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1498, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.compileProjectToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.newProjectToolStripMenuItem.Text = "New project...";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.CreateNewProject_Clicked);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openToolStripMenuItem.Text = "Open project...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenProject_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.saveToolStripMenuItem.Text = "Save project";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveProject_Clicked);
            // 
            // saveProjectAsToolStripMenuItem
            // 
            this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            this.saveProjectAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.saveProjectAsToolStripMenuItem.Text = "Save project as...";
            this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.SaveProjectAs_Clicked);
            // 
            // compileProjectToolStripMenuItem
            // 
            this.compileProjectToolStripMenuItem.Name = "compileProjectToolStripMenuItem";
            this.compileProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.B)));
            this.compileProjectToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.compileProjectToolStripMenuItem.Text = "Compile...";
            this.compileProjectToolStripMenuItem.Click += new System.EventHandler(this.CompileProject_Clicked);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertyEditorToolStripMenuItem,
            this.projectExplorerToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.windowToolStripMenuItem.Text = "Views";
            // 
            // propertyEditorToolStripMenuItem
            // 
            this.propertyEditorToolStripMenuItem.Name = "propertyEditorToolStripMenuItem";
            this.propertyEditorToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.propertyEditorToolStripMenuItem.Text = "Property Editor";
            this.propertyEditorToolStripMenuItem.Click += new System.EventHandler(this.OpenPropertyEditor_Click);
            // 
            // projectExplorerToolStripMenuItem
            // 
            this.projectExplorerToolStripMenuItem.Name = "projectExplorerToolStripMenuItem";
            this.projectExplorerToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.projectExplorerToolStripMenuItem.Text = "Project Explorer";
            this.projectExplorerToolStripMenuItem.Click += new System.EventHandler(this.OpenProjectExplorer_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label_projectStatus,
            this.label_middle,
            this.label_version});
            this.statusStrip1.Location = new System.Drawing.Point(0, 720);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1498, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label_projectStatus
            // 
            this.label_projectStatus.Name = "label_projectStatus";
            this.label_projectStatus.Size = new System.Drawing.Size(105, 17);
            this.label_projectStatus.Text = "No project loaded.";
            // 
            // label_middle
            // 
            this.label_middle.Name = "label_middle";
            this.label_middle.Size = new System.Drawing.Size(1303, 17);
            this.label_middle.Spring = true;
            // 
            // label_version
            // 
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(75, 17);
            this.label_version.Text = "Version: 0.0.0";
            // 
            // workspace
            // 
            this.workspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspace.Location = new System.Drawing.Point(0, 24);
            this.workspace.Name = "workspace";
            this.workspace.Size = new System.Drawing.Size(1498, 696);
            this.workspace.TabIndex = 2;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1498, 742);
            this.Controls.Add(this.workspace);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Foundry";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem windowToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel label_projectStatus;
        private ToolStripStatusLabel label_middle;
        private ToolStripStatusLabel label_version;
        private WeifenLuo.WinFormsUI.Docking.DockPanel workspace;
        private ToolStripMenuItem propertyEditorToolStripMenuItem;
        private ToolStripMenuItem projectExplorerToolStripMenuItem;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private ToolStripMenuItem compileProjectToolStripMenuItem;
    }
}