namespace SMHEditor.DockingModules.ObjectEditor.Veterancy
{
    partial class VeterancyControl
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
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.damagetaken = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.weaponrange = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.workrate = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.accuracy = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.damage = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.xp = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.level = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.velocity = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(897, 0);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(2);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(23, 24);
            this.deleteButton.TabIndex = 21;
            this.deleteButton.Values.Text = "X";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.velocity);
            this.kryptonPanel.Controls.Add(this.kryptonLabel8);
            this.kryptonPanel.Controls.Add(this.damagetaken);
            this.kryptonPanel.Controls.Add(this.kryptonLabel7);
            this.kryptonPanel.Controls.Add(this.weaponrange);
            this.kryptonPanel.Controls.Add(this.kryptonLabel6);
            this.kryptonPanel.Controls.Add(this.workrate);
            this.kryptonPanel.Controls.Add(this.kryptonLabel4);
            this.kryptonPanel.Controls.Add(this.accuracy);
            this.kryptonPanel.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel.Controls.Add(this.damage);
            this.kryptonPanel.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel.Controls.Add(this.xp);
            this.kryptonPanel.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel.Controls.Add(this.level);
            this.kryptonPanel.Controls.Add(this.kryptonLabel5);
            this.kryptonPanel.Controls.Add(this.deleteButton);
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelCustom1;
            this.kryptonPanel.Size = new System.Drawing.Size(922, 54);
            this.kryptonPanel.TabIndex = 1;
            // 
            // damagetaken
            // 
            this.damagetaken.DecimalPlaces = 2;
            this.damagetaken.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.damagetaken.Location = new System.Drawing.Point(853, 28);
            this.damagetaken.Margin = new System.Windows.Forms.Padding(2);
            this.damagetaken.Name = "damagetaken";
            this.damagetaken.Size = new System.Drawing.Size(52, 22);
            this.damagetaken.TabIndex = 35;
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(773, 30);
            this.kryptonLabel7.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(88, 20);
            this.kryptonLabel7.TabIndex = 34;
            this.kryptonLabel7.Values.Text = "DamageTaken";
            // 
            // weaponrange
            // 
            this.weaponrange.DecimalPlaces = 2;
            this.weaponrange.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.weaponrange.Location = new System.Drawing.Point(705, 28);
            this.weaponrange.Margin = new System.Windows.Forms.Padding(2);
            this.weaponrange.Name = "weaponrange";
            this.weaponrange.Size = new System.Drawing.Size(52, 22);
            this.weaponrange.TabIndex = 33;
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(622, 32);
            this.kryptonLabel6.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(91, 20);
            this.kryptonLabel6.TabIndex = 32;
            this.kryptonLabel6.Values.Text = "WeaponRange";
            // 
            // workrate
            // 
            this.workrate.DecimalPlaces = 2;
            this.workrate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.workrate.Location = new System.Drawing.Point(556, 28);
            this.workrate.Margin = new System.Windows.Forms.Padding(2);
            this.workrate.Name = "workrate";
            this.workrate.Size = new System.Drawing.Size(52, 22);
            this.workrate.TabIndex = 31;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(498, 32);
            this.kryptonLabel4.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(64, 20);
            this.kryptonLabel4.TabIndex = 30;
            this.kryptonLabel4.Values.Text = "WorkRate";
            // 
            // accuracy
            // 
            this.accuracy.DecimalPlaces = 2;
            this.accuracy.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.accuracy.Location = new System.Drawing.Point(433, 28);
            this.accuracy.Margin = new System.Windows.Forms.Padding(2);
            this.accuracy.Name = "accuracy";
            this.accuracy.Size = new System.Drawing.Size(52, 22);
            this.accuracy.TabIndex = 29;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(379, 32);
            this.kryptonLabel3.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(59, 20);
            this.kryptonLabel3.TabIndex = 28;
            this.kryptonLabel3.Values.Text = "Accuracy";
            // 
            // damage
            // 
            this.damage.DecimalPlaces = 2;
            this.damage.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.damage.Location = new System.Drawing.Point(182, 28);
            this.damage.Margin = new System.Windows.Forms.Padding(2);
            this.damage.Name = "damage";
            this.damage.Size = new System.Drawing.Size(52, 22);
            this.damage.TabIndex = 27;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(133, 32);
            this.kryptonLabel2.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(56, 20);
            this.kryptonLabel2.TabIndex = 26;
            this.kryptonLabel2.Values.Text = "Damage";
            // 
            // xp
            // 
            this.xp.Location = new System.Drawing.Point(66, 28);
            this.xp.Margin = new System.Windows.Forms.Padding(2);
            this.xp.Name = "xp";
            this.xp.Size = new System.Drawing.Size(52, 22);
            this.xp.TabIndex = 25;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(2, 32);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(69, 20);
            this.kryptonLabel1.TabIndex = 24;
            this.kryptonLabel1.Values.Text = "Experience";
            // 
            // level
            // 
            this.level.Location = new System.Drawing.Point(44, 3);
            this.level.Margin = new System.Windows.Forms.Padding(2);
            this.level.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.level.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.level.Name = "level";
            this.level.Size = new System.Drawing.Size(52, 22);
            this.level.TabIndex = 23;
            this.level.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(2, 5);
            this.kryptonLabel5.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(38, 20);
            this.kryptonLabel5.TabIndex = 22;
            this.kryptonLabel5.Values.Text = "Level";
            // 
            // velocity
            // 
            this.velocity.DecimalPlaces = 2;
            this.velocity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.velocity.Location = new System.Drawing.Point(313, 28);
            this.velocity.Margin = new System.Windows.Forms.Padding(2);
            this.velocity.Name = "velocity";
            this.velocity.Size = new System.Drawing.Size(52, 22);
            this.velocity.TabIndex = 37;
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(259, 32);
            this.kryptonLabel8.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(54, 20);
            this.kryptonLabel8.TabIndex = 36;
            this.kryptonLabel8.Values.Text = "Velocity";
            // 
            // VeterancyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel);
            this.Name = "VeterancyControl";
            this.Size = new System.Drawing.Size(922, 55);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            this.kryptonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton deleteButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown level;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown xp;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown damagetaken;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown weaponrange;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown workrate;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown accuracy;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown damage;
        public ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown velocity;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
    }
}
