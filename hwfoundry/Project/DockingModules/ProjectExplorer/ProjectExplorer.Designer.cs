namespace SMHEditor.DockingModules.ProjectExplorer
{
    partial class ProjectExplorer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeViewAdv = new Aga.Controls.Tree.TreeViewAdv();
            this.SuspendLayout();
            // 
            // treeViewAdv
            // 
            this.treeViewAdv.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdv.DefaultToolTipProvider = null;
            this.treeViewAdv.DisplayDraggingNodes = true;
            this.treeViewAdv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAdv.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv.LoadOnDemand = true;
            this.treeViewAdv.Location = new System.Drawing.Point(0, 0);
            this.treeViewAdv.Model = null;
            this.treeViewAdv.Name = "treeViewAdv";
            this.treeViewAdv.SelectedNode = null;
            this.treeViewAdv.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
            this.treeViewAdv.Size = new System.Drawing.Size(155, 134);
            this.treeViewAdv.TabIndex = 1;
            this.treeViewAdv.Text = "treeView";
            // 
            // ProjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(155, 134);
            this.Controls.Add(this.treeViewAdv);
            this.Name = "ProjectExplorer";
            this.ResumeLayout(false);

        }

        #endregion
        
        private Aga.Controls.Tree.TreeViewAdv treeViewAdv;
    }
}
