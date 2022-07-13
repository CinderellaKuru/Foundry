namespace SMHEditor.DockingModules.ObjectEditor
{
    partial class UIControl
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
            this.name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.description = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
            this.iconFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.icon = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.size = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.subtitle = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(11, 12);
            this.name.Margin = new System.Windows.Forms.Padding(2);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(175, 23);
            this.name.TabIndex = 1;
            this.name.Text = "Name";
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(11, 40);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(360, 128);
            this.description.TabIndex = 2;
            this.description.Text = "Description";
            // 
            // icon
            // 
            this.icon.Location = new System.Drawing.Point(189, 12);
            this.icon.Margin = new System.Windows.Forms.Padding(2);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(182, 23);
            this.icon.TabIndex = 3;
            this.icon.Text = "Icon";
            // 
            // kryptonTextBox1
            // 
            this.kryptonTextBox1.Location = new System.Drawing.Point(11, 173);
            this.kryptonTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonTextBox1.Name = "kryptonTextBox1";
            this.kryptonTextBox1.Size = new System.Drawing.Size(161, 23);
            this.kryptonTextBox1.TabIndex = 4;
            this.kryptonTextBox1.Text = "Minimap Icon";
            // 
            // size
            // 
            this.size.Location = new System.Drawing.Point(207, 174);
            this.size.Margin = new System.Windows.Forms.Padding(2);
            this.size.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(52, 22);
            this.size.TabIndex = 6;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(176, 176);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(32, 20);
            this.kryptonLabel1.TabIndex = 5;
            this.kryptonLabel1.Values.Text = "Size";
            // 
            // subtitle
            // 
            this.subtitle.Location = new System.Drawing.Point(11, 200);
            this.subtitle.Margin = new System.Windows.Forms.Padding(2);
            this.subtitle.Name = "subtitle";
            this.subtitle.Size = new System.Drawing.Size(360, 23);
            this.subtitle.TabIndex = 7;
            this.subtitle.Text = "Subtitle";
            // 
            // UIControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Controls.Add(this.subtitle);
            this.Controls.Add(this.size);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.kryptonTextBox1);
            this.Controls.Add(this.icon);
            this.Controls.Add(this.description);
            this.Controls.Add(this.name);
            this.Name = "UIControl";
            this.Size = new System.Drawing.Size(750, 679);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonTextBox name;
        private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox description;
        private System.Windows.Forms.OpenFileDialog iconFileDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox icon;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox kryptonTextBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown size;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox subtitle;
    }
}
