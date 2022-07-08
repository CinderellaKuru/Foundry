namespace SMHEditor.DockingModules.ObjectEditor.Flags
{
    partial class FlagControl
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
            this.flag = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.deleteButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flag)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.deleteButton);
            this.kryptonPanel.Controls.Add(this.kryptonLabel1);
            this.kryptonPanel.Controls.Add(this.flag);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelCustom1;
            this.kryptonPanel.Size = new System.Drawing.Size(309, 31);
            this.kryptonPanel.TabIndex = 2;
            // 
            // flag
            // 
            this.flag.DropDownWidth = 243;
            this.flag.Items.AddRange(new object[] {
            "AirMovement",
            "AppearsBelowDecals",
            "AttackWhileCloaked",
            "AutoCloak",
            "AutoExplorationGroup",
            "Beam",
            "BlockLOS",
            "BlockMovement",
            "Build",
            "CanSetAsRallyPoint",
            "Capturable",
            "CheckLOSAgainstBase",
            "ChildForDamageTakenScalar",
            "CommandableByAnyPlayer",
            "DamagedDeathReplacement",
            "DamageGarrisoned",
            "Destructible",
            "DieAtZeroResources",
            "DieLast",
            "DoNotFilterOrient",
            "DontAttackWhileMoving",
            "DontAutoAttackMe",
            "DontRotateObstruction",
            "Doppled",
            "ExpireOnTimer",
            "ExplodeOnTimer",
            "ExternalShield",
            "FadeOnDeath",
            "ForceAnimRate",
            "ForceCreateObstruction",
            "ForceToGaiaPlayer",
            "ForceUpdateContainedUnits",
            "GrayMapDoppled",
            "HasHPBar",
            "HasPivotingEngines",
            "HasTrackMask",
            "HideOnImpact",
            "HighArc",
            "IgnoreSquadAI",
            "IKTransitionToIdle",
            "Immoveable",
            "Invulnerable",
            "IsAffectedByGravity",
            "IsFlameEffect",
            "IsSticky",
            "KBAware",
            "KBCreatesBase",
            "KillChildObjectsOnDeath",
            "KillGarrisoned",
            "KillOnDetach",
            "LinearCostEscalation",
            "LockdownMenu",
            "ManualBuild",
            "MoveWhileCloaked",
            "MustOwnToSelect",
            "Neutral",
            "NoActionOverrideMove",
            "NoCorpse",
            "NoGrayMapDoppledInCampaign",
            "NonCollidable",
            "Noncollideable",
            "NonCollideable",
            "NonRotatable",
            "NonSolid",
            "NoRandomMoveAnimStart",
            "NoRender",
            "NoStickyCam",
            "NoTieToGround",
            "NotSelectableWhenChildObject",
            "Obscurable",
            "ObstructsAir",
            "OneSquadContainment",
            "OrientUnitWithGround",
            "PassiveGarrisoned",
            "PermanentSocket",
            "PhysicsDetonateOnDeath",
            "PlayerOwnsObstruction",
            "ProjectileObstructable",
            "ProjectileTumbles",
            "RegularAttacksMeleeOnly",
            "RenderBelowDecals",
            "Repairable",
            "RocketOnDeath",
            "ScaleBuildAnimRate",
            "SecondaryBuildingQueue",
            "SelectedRect",
            "SelectionDontConformToTerrain",
            "SelfDamage",
            "SelfParkingLot",
            "ShatterDeathReplacement",
            "ShowRange",
            "ShowRescuedCount",
            "SingleSocketBuilding",
            "SoundBehindFOW",
            "StartAtMaxAmmo",
            "SyncAnimRateToPhysics",
            "TargetsFootOfUnit",
            "Teleporter",
            "Tracking",
            "TriggersBattleMusicWhenAttacked",
            "TurnInPlace",
            "UnGarrisonToGaia",
            "UnlimitedResources",
            "Update",
            "UseAutoParkingLot",
            "UseBuildRotation",
            "UseRelaxedSpeedGroup",
            "VisibleForOwnerOnly",
            "VisibleToAll"});
            this.flag.Location = new System.Drawing.Point(33, 3);
            this.flag.Name = "flag";
            this.flag.Size = new System.Drawing.Size(243, 21);
            this.flag.Sorted = true;
            this.flag.TabIndex = 3;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(2, 2);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(2);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(33, 20);
            this.kryptonLabel1.TabIndex = 7;
            this.kryptonLabel1.Values.Text = "Flag";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(281, 3);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(2);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(23, 24);
            this.deleteButton.TabIndex = 22;
            this.deleteButton.Values.Text = "X";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // FlagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel);
            this.Name = "FlagControl";
            this.Size = new System.Drawing.Size(309, 31);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            this.kryptonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flag)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox flag;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton deleteButton;
    }
}
