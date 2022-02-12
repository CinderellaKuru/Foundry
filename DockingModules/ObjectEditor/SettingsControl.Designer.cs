namespace SMHEditor.DockingModules.ObjectEditor
{
    partial class SettingsControl
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
            this.object_name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.object_derrived = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.visual = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.tactics = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.objectClass = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this.kryptonNumericUpDown1 = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.object_derrived)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.visual)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tactics)).BeginInit();
            this.SuspendLayout();
            // 
            // object_name
            // 
            this.object_name.Location = new System.Drawing.Point(3, 3);
            this.object_name.Name = "object_name";
            this.object_name.Size = new System.Drawing.Size(494, 27);
            this.object_name.TabIndex = 0;
            this.object_name.Text = "object_name";
            // 
            // object_derrived
            // 
            this.object_derrived.DropDownWidth = 494;
            this.object_derrived.Location = new System.Drawing.Point(503, 3);
            this.object_derrived.Name = "object_derrived";
            this.object_derrived.Size = new System.Drawing.Size(494, 25);
            this.object_derrived.TabIndex = 1;
            this.object_derrived.Text = "Base Object";
            // 
            // visual
            // 
            this.visual.DropDownWidth = 494;
            this.visual.Location = new System.Drawing.Point(3, 36);
            this.visual.Name = "visual";
            this.visual.Size = new System.Drawing.Size(494, 25);
            this.visual.TabIndex = 2;
            this.visual.Text = "Tactics";
            // 
            // tactics
            // 
            this.tactics.DropDownWidth = 400;
            this.tactics.Location = new System.Drawing.Point(3, 67);
            this.tactics.Name = "tactics";
            this.tactics.Size = new System.Drawing.Size(494, 25);
            this.tactics.TabIndex = 3;
            this.tactics.Text = "Visual";
            // 
            // objectClass
            // 
            this.objectClass.Location = new System.Drawing.Point(503, 34);
            this.objectClass.Name = "objectClass";
            this.objectClass.Size = new System.Drawing.Size(494, 25);
            this.objectClass.TabIndex = 4;
            // 
            // kryptonNumericUpDown1
            // 
            this.kryptonNumericUpDown1.Location = new System.Drawing.Point(83, 97);
            this.kryptonNumericUpDown1.Name = "kryptonNumericUpDown1";
            this.kryptonNumericUpDown1.Size = new System.Drawing.Size(96, 26);
            this.kryptonNumericUpDown1.TabIndex = 5;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(3, 98);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(74, 24);
            this.kryptonLabel1.TabIndex = 6;
            this.kryptonLabel1.Values.Text = "Hitpoints";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(330, 99);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(167, 24);
            this.comboBox1.TabIndex = 7;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(202, 99);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(122, 24);
            this.kryptonLabel2.TabIndex = 8;
            this.kryptonLabel2.Values.Text = "Movement Type";
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.kryptonNumericUpDown1);
            this.Controls.Add(this.objectClass);
            this.Controls.Add(this.tactics);
            this.Controls.Add(this.visual);
            this.Controls.Add(this.object_derrived);
            this.Controls.Add(this.object_name);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(1000, 836);
            ((System.ComponentModel.ISupportInitialize)(this.object_derrived)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.visual)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tactics)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonTextBox object_name;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox object_derrived;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox visual;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox tactics;
        private ComponentFactory.Krypton.Toolkit.KryptonListBox objectClass;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDown1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
    }
}
