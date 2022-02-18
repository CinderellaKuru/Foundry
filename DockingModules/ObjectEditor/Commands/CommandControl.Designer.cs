namespace SMHEditor.DockingModules.ObjectEditor.Commands
{
    partial class CommandControl
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
            this.deleteButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.position = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.autoclose = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.type = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.placementBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.type)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.placementBox)).BeginInit();
            this.SuspendLayout();
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(167, 4);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(2);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(23, 24);
            this.deleteButton.TabIndex = 21;
            this.deleteButton.Values.Text = "X";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // position
            // 
            this.position.Location = new System.Drawing.Point(55, 58);
            this.position.Margin = new System.Windows.Forms.Padding(2);
            this.position.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.position.Name = "position";
            this.position.Size = new System.Drawing.Size(52, 22);
            this.position.TabIndex = 3;
            this.position.ValueChanged += new System.EventHandler(this.position_ValueChanged);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(2, 59);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(54, 20);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "Position";
            // 
            // autoclose
            // 
            this.autoclose.Location = new System.Drawing.Point(9, 83);
            this.autoclose.Margin = new System.Windows.Forms.Padding(2);
            this.autoclose.Name = "autoclose";
            this.autoclose.Size = new System.Drawing.Size(79, 20);
            this.autoclose.TabIndex = 1;
            this.autoclose.Values.Text = "AutoClose";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(2, 5);
            this.name.Margin = new System.Windows.Forms.Padding(2);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(161, 23);
            this.name.TabIndex = 0;
            this.name.Text = "Name";
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.placementBox);
            this.kryptonPanel.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel.Controls.Add(this.type);
            this.kryptonPanel.Controls.Add(this.deleteButton);
            this.kryptonPanel.Controls.Add(this.position);
            this.kryptonPanel.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel.Controls.Add(this.autoclose);
            this.kryptonPanel.Controls.Add(this.name);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelCustom1;
            this.kryptonPanel.Size = new System.Drawing.Size(207, 131);
            this.kryptonPanel.TabIndex = 1;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(2, 33);
            this.kryptonLabel2.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(37, 20);
            this.kryptonLabel2.TabIndex = 24;
            this.kryptonLabel2.Values.Text = "Type";
            // 
            // type
            // 
            this.type.DropDownWidth = 146;
            this.type.Items.AddRange(new object[] {
            ""});
            this.type.Location = new System.Drawing.Point(44, 33);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(146, 21);
            this.type.TabIndex = 23;
            // 
            // placementBox
            // 
            this.placementBox.Image = global::SMHEditor.Properties.Resources.Placement0;
            this.placementBox.Location = new System.Drawing.Point(126, 59);
            this.placementBox.Name = "placementBox";
            this.placementBox.Size = new System.Drawing.Size(64, 64);
            this.placementBox.TabIndex = 3;
            this.placementBox.TabStop = false;
            // 
            // CommandControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel);
            this.Name = "CommandControl";
            this.Size = new System.Drawing.Size(207, 131);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            this.kryptonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.type)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.placementBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton deleteButton;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown position;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox autoclose;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox name;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox type;
        private System.Windows.Forms.PictureBox placementBox;
    }
}
