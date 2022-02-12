namespace SMHEditor.DockingModules.ObjectEditor
{
    partial class ObjectEditorControl
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
            this.kryptonNavigator = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.settings = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.hardpoints = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.ui = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.flags = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.objectTypes = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.sounds = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.commands = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.veterancy = new ComponentFactory.Krypton.Navigator.KryptonPage();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator)).BeginInit();
            this.kryptonNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hardpoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flags)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectTypes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sounds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.commands)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.veterancy)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonNavigator
            // 
            this.kryptonNavigator.Bar.BarOrientation = ComponentFactory.Krypton.Toolkit.VisualOrientation.Right;
            this.kryptonNavigator.Bar.ItemOrientation = ComponentFactory.Krypton.Toolkit.ButtonOrientation.FixedTop;
            this.kryptonNavigator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonNavigator.Location = new System.Drawing.Point(0, 0);
            this.kryptonNavigator.Name = "kryptonNavigator1";
            this.kryptonNavigator.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.settings,
            this.hardpoints,
            this.ui,
            this.veterancy,
            this.sounds,
            this.commands,
            this.objectTypes,
            this.flags});
            this.kryptonNavigator.SelectedIndex = 7;
            this.kryptonNavigator.Size = new System.Drawing.Size(478, 493);
            this.kryptonNavigator.TabIndex = 0;
            this.kryptonNavigator.Text = "kryptonNavigator1";
            // 
            // settings
            // 
            this.settings.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.settings.Flags = 65534;
            this.settings.LastVisibleSet = true;
            this.settings.MinimumSize = new System.Drawing.Size(50, 50);
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(374, 491);
            this.settings.Text = "Settings";
            this.settings.ToolTipTitle = "Page ToolTip";
            this.settings.UniqueName = "645F1B3D322345949AB78CB67882184A";
            // 
            // hardpoints
            // 
            this.hardpoints.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.hardpoints.Flags = 65534;
            this.hardpoints.LastVisibleSet = true;
            this.hardpoints.MinimumSize = new System.Drawing.Size(50, 50);
            this.hardpoints.Name = "hardpoints";
            this.hardpoints.Size = new System.Drawing.Size(374, 491);
            this.hardpoints.Text = "Hardpoints";
            this.hardpoints.ToolTipTitle = "Page ToolTip";
            this.hardpoints.UniqueName = "453D6ADB71244DDFC99DF4F54E67D173";
            // 
            // ui
            // 
            this.ui.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.ui.Flags = 65534;
            this.ui.LastVisibleSet = true;
            this.ui.MinimumSize = new System.Drawing.Size(50, 50);
            this.ui.Name = "ui";
            this.ui.Size = new System.Drawing.Size(374, 491);
            this.ui.Text = "UI";
            this.ui.ToolTipTitle = "Page ToolTip";
            this.ui.UniqueName = "91AC824B9B8944EB39A95A8B14C4436F";
            // 
            // flags
            // 
            this.flags.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.flags.Flags = 65534;
            this.flags.LastVisibleSet = true;
            this.flags.MinimumSize = new System.Drawing.Size(50, 50);
            this.flags.Name = "flags";
            this.flags.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.flags.Size = new System.Drawing.Size(374, 491);
            this.flags.Text = "Flags";
            this.flags.ToolTipTitle = "Page ToolTip";
            this.flags.UniqueName = "37148650FC0845113EB3B7CCBCC2B841";
            // 
            // objectTypes
            // 
            this.objectTypes.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.objectTypes.Flags = 65534;
            this.objectTypes.LastVisibleSet = true;
            this.objectTypes.MinimumSize = new System.Drawing.Size(50, 50);
            this.objectTypes.Name = "objectTypes";
            this.objectTypes.Size = new System.Drawing.Size(369, 491);
            this.objectTypes.Text = "Object Types";
            this.objectTypes.ToolTipTitle = "Page ToolTip";
            this.objectTypes.UniqueName = "66FE122DE634463C7AAAF3A78E8E0542";
            // 
            // sounds
            // 
            this.sounds.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.sounds.Flags = 65534;
            this.sounds.LastVisibleSet = true;
            this.sounds.MinimumSize = new System.Drawing.Size(50, 50);
            this.sounds.Name = "sounds";
            this.sounds.Size = new System.Drawing.Size(374, 491);
            this.sounds.Text = "Sounds";
            this.sounds.ToolTipTitle = "Page ToolTip";
            this.sounds.UniqueName = "415443E6F4A44ABA6584CB1E4BCB3642";
            // 
            // commands
            // 
            this.commands.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.commands.Flags = 65534;
            this.commands.LastVisibleSet = true;
            this.commands.MinimumSize = new System.Drawing.Size(50, 50);
            this.commands.Name = "commands";
            this.commands.Size = new System.Drawing.Size(374, 491);
            this.commands.Text = "Commands";
            this.commands.ToolTipTitle = "Page ToolTip";
            this.commands.UniqueName = "01592CFD36E94ADA67856F8B3649C4CF";
            // 
            // veterancy
            // 
            this.veterancy.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.veterancy.Flags = 65534;
            this.veterancy.LastVisibleSet = true;
            this.veterancy.MinimumSize = new System.Drawing.Size(50, 50);
            this.veterancy.Name = "veterancy";
            this.veterancy.Size = new System.Drawing.Size(374, 491);
            this.veterancy.Text = "Veterancy";
            this.veterancy.ToolTipTitle = "Page ToolTip";
            this.veterancy.UniqueName = "77AA741AE993424EC6B556050E5CF569";
            // 
            // ObjectEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonNavigator);
            this.Name = "ObjectEditorControl";
            this.Size = new System.Drawing.Size(478, 493);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator)).EndInit();
            this.kryptonNavigator.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.settings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hardpoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ui)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flags)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectTypes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sounds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.commands)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.veterancy)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator;
        public ComponentFactory.Krypton.Navigator.KryptonPage settings;
        public ComponentFactory.Krypton.Navigator.KryptonPage hardpoints;
        public ComponentFactory.Krypton.Navigator.KryptonPage ui;
        public ComponentFactory.Krypton.Navigator.KryptonPage veterancy;
        public ComponentFactory.Krypton.Navigator.KryptonPage sounds;
        public ComponentFactory.Krypton.Navigator.KryptonPage commands;
        public ComponentFactory.Krypton.Navigator.KryptonPage objectTypes;
        public ComponentFactory.Krypton.Navigator.KryptonPage flags;
    }
}
