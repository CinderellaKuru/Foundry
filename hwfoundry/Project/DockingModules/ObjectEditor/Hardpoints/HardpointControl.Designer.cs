namespace SMHEditor.DockingModules.ObjectEditor
{
    partial class HardpointControl
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
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.name = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.autocenter = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.yawrate = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.pitchrate = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.infiniterate = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.yawmaxangle = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pitchminangle = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pitchmaxangle = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.relative = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.singleboneik = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.useastolerance = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.combined = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.yawattachment = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.pitchattachment = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.StartYawSound = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.StopYawSound = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.deleteButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yawattachment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchattachment)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.deleteButton);
            this.kryptonPanel.Controls.Add(this.StopYawSound);
            this.kryptonPanel.Controls.Add(this.StartYawSound);
            this.kryptonPanel.Controls.Add(this.pitchattachment);
            this.kryptonPanel.Controls.Add(this.yawattachment);
            this.kryptonPanel.Controls.Add(this.combined);
            this.kryptonPanel.Controls.Add(this.useastolerance);
            this.kryptonPanel.Controls.Add(this.singleboneik);
            this.kryptonPanel.Controls.Add(this.relative);
            this.kryptonPanel.Controls.Add(this.pitchmaxangle);
            this.kryptonPanel.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel.Controls.Add(this.pitchminangle);
            this.kryptonPanel.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel.Controls.Add(this.yawmaxangle);
            this.kryptonPanel.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel.Controls.Add(this.infiniterate);
            this.kryptonPanel.Controls.Add(this.pitchrate);
            this.kryptonPanel.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel.Controls.Add(this.yawrate);
            this.kryptonPanel.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel.Controls.Add(this.autocenter);
            this.kryptonPanel.Controls.Add(this.name);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelCustom1;
            this.kryptonPanel.Size = new System.Drawing.Size(1084, 95);
            this.kryptonPanel.TabIndex = 0;
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(3, 6);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(215, 27);
            this.name.TabIndex = 0;
            this.name.Text = "name";
            // 
            // autocenter
            // 
            this.autocenter.Location = new System.Drawing.Point(232, 6);
            this.autocenter.Name = "autocenter";
            this.autocenter.Size = new System.Drawing.Size(100, 24);
            this.autocenter.TabIndex = 1;
            this.autocenter.Values.Text = "Autocenter";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(232, 36);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(69, 24);
            this.kryptonLabel1.TabIndex = 2;
            this.kryptonLabel1.Values.Text = "YawRate";
            // 
            // yawrate
            // 
            this.yawrate.Location = new System.Drawing.Point(313, 34);
            this.yawrate.Name = "yawrate";
            this.yawrate.Size = new System.Drawing.Size(70, 26);
            this.yawrate.TabIndex = 3;
            // 
            // pitchrate
            // 
            this.pitchrate.Location = new System.Drawing.Point(313, 64);
            this.pitchrate.Name = "pitchrate";
            this.pitchrate.Size = new System.Drawing.Size(70, 26);
            this.pitchrate.TabIndex = 5;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(232, 66);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(75, 24);
            this.kryptonLabel2.TabIndex = 4;
            this.kryptonLabel2.Values.Text = "PitchRate";
            // 
            // infiniterate
            // 
            this.infiniterate.Location = new System.Drawing.Point(346, 6);
            this.infiniterate.Name = "infiniterate";
            this.infiniterate.Size = new System.Drawing.Size(211, 24);
            this.infiniterate.TabIndex = 6;
            this.infiniterate.Values.Text = "InfiniteRateWhenHasTarget";
            // 
            // yawmaxangle
            // 
            this.yawmaxangle.Location = new System.Drawing.Point(519, 34);
            this.yawmaxangle.Name = "yawmaxangle";
            this.yawmaxangle.Size = new System.Drawing.Size(70, 26);
            this.yawmaxangle.TabIndex = 8;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(401, 36);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(108, 24);
            this.kryptonLabel3.TabIndex = 7;
            this.kryptonLabel3.Values.Text = "YawMaxAngle";
            // 
            // pitchminangle
            // 
            this.pitchminangle.Location = new System.Drawing.Point(519, 62);
            this.pitchminangle.Name = "pitchminangle";
            this.pitchminangle.Size = new System.Drawing.Size(70, 26);
            this.pitchminangle.TabIndex = 10;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(401, 64);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(111, 24);
            this.kryptonLabel4.TabIndex = 9;
            this.kryptonLabel4.Values.Text = "PitchMinAngle";
            // 
            // pitchmaxangle
            // 
            this.pitchmaxangle.Location = new System.Drawing.Point(731, 60);
            this.pitchmaxangle.Name = "pitchmaxangle";
            this.pitchmaxangle.Size = new System.Drawing.Size(70, 26);
            this.pitchmaxangle.TabIndex = 12;
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(610, 62);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(113, 24);
            this.kryptonLabel5.TabIndex = 11;
            this.kryptonLabel5.Values.Text = "PitchMaxAngle";
            // 
            // relative
            // 
            this.relative.Location = new System.Drawing.Point(573, 6);
            this.relative.Name = "relative";
            this.relative.Size = new System.Drawing.Size(78, 24);
            this.relative.TabIndex = 13;
            this.relative.Values.Text = "Relative";
            // 
            // singleboneik
            // 
            this.singleboneik.Location = new System.Drawing.Point(667, 6);
            this.singleboneik.Name = "singleboneik";
            this.singleboneik.Size = new System.Drawing.Size(114, 24);
            this.singleboneik.TabIndex = 14;
            this.singleboneik.Values.Text = "SingleBoneIK";
            // 
            // useastolerance
            // 
            this.useastolerance.Location = new System.Drawing.Point(793, 6);
            this.useastolerance.Name = "useastolerance";
            this.useastolerance.Size = new System.Drawing.Size(132, 24);
            this.useastolerance.TabIndex = 15;
            this.useastolerance.Values.Text = "UseAsTolerance";
            // 
            // combined
            // 
            this.combined.Location = new System.Drawing.Point(938, 6);
            this.combined.Name = "combined";
            this.combined.Size = new System.Drawing.Size(95, 24);
            this.combined.TabIndex = 16;
            this.combined.Values.Text = "Combined";
            // 
            // yawattachment
            // 
            this.yawattachment.DropDownWidth = 215;
            this.yawattachment.Location = new System.Drawing.Point(3, 36);
            this.yawattachment.Name = "yawattachment";
            this.yawattachment.Size = new System.Drawing.Size(215, 25);
            this.yawattachment.TabIndex = 17;
            this.yawattachment.Text = "YawAttachment";
            // 
            // pitchattachment
            // 
            this.pitchattachment.DropDownWidth = 215;
            this.pitchattachment.Location = new System.Drawing.Point(3, 64);
            this.pitchattachment.Name = "pitchattachment";
            this.pitchattachment.Size = new System.Drawing.Size(215, 25);
            this.pitchattachment.TabIndex = 18;
            this.pitchattachment.Text = "PitchAttachment";
            // 
            // StartYawSound
            // 
            this.StartYawSound.Location = new System.Drawing.Point(851, 34);
            this.StartYawSound.Name = "StartYawSound";
            this.StartYawSound.Size = new System.Drawing.Size(182, 27);
            this.StartYawSound.TabIndex = 19;
            this.StartYawSound.Text = "StartYawSound";
            // 
            // StopYawSound
            // 
            this.StopYawSound.Location = new System.Drawing.Point(851, 62);
            this.StopYawSound.Name = "StopYawSound";
            this.StopYawSound.Size = new System.Drawing.Size(182, 27);
            this.StopYawSound.TabIndex = 20;
            this.StopYawSound.Text = "StopYawSound";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(1052, 1);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(31, 30);
            this.deleteButton.TabIndex = 21;
            this.deleteButton.Values.Text = "X";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // HardpointControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel);
            this.Name = "HardpointControl";
            this.Size = new System.Drawing.Size(1084, 95);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            this.kryptonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yawattachment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchattachment)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox name;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown pitchmaxangle;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown pitchminangle;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown yawmaxangle;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox infiniterate;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown pitchrate;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown yawrate;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox autocenter;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox StopYawSound;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox StartYawSound;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox pitchattachment;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox yawattachment;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox combined;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox useastolerance;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox singleboneik;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox relative;
        private ComponentFactory.Krypton.Toolkit.KryptonButton deleteButton;
    }
}
