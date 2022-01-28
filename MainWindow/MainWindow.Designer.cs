namespace SMHEditor
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
            this.components = new System.ComponentModel.Container();
            this.dockingManager = new ComponentFactory.Krypton.Docking.KryptonDockingManager();
            this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.dockableWorkspace = new ComponentFactory.Krypton.Docking.KryptonDockableWorkspace();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dockableWorkspace)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.dockableWorkspace);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Size = new System.Drawing.Size(800, 450);
            this.kryptonPanel.TabIndex = 0;
            // 
            // dockableWorkspace
            // 
            this.dockableWorkspace.AutoHiddenHost = false;
            this.dockableWorkspace.CompactFlags = ((ComponentFactory.Krypton.Workspace.CompactFlags)(((ComponentFactory.Krypton.Workspace.CompactFlags.RemoveEmptyCells | ComponentFactory.Krypton.Workspace.CompactFlags.RemoveEmptySequences) 
            | ComponentFactory.Krypton.Workspace.CompactFlags.PromoteLeafs)));
            this.dockableWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockableWorkspace.Location = new System.Drawing.Point(0, 0);
            this.dockableWorkspace.Name = "dockableWorkspace";
            // 
            // 
            // 
            this.dockableWorkspace.Root.UniqueName = "6F66E8854D3D484E3E920DE6F433C814";
            this.dockableWorkspace.Root.WorkspaceControl = this.dockableWorkspace;
            this.dockableWorkspace.ShowMaximizeButton = false;
            this.dockableWorkspace.Size = new System.Drawing.Size(800, 450);
            this.dockableWorkspace.TabIndex = 0;
            this.dockableWorkspace.TabStop = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.kryptonPanel);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dockableWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private ComponentFactory.Krypton.Docking.KryptonDockingManager dockingManager;
        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Docking.KryptonDockableWorkspace dockableWorkspace;
    }
}