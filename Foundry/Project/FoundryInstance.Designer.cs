using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Foundry.Project
{
    partial class FoundryInstance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoundryInstance));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.logStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.versionReadout = new System.Windows.Forms.ToolStripStatusLabel();
            this.discordLink = new System.Windows.Forms.ToolStripStatusLabel();
            this.memoryReadout = new System.Windows.Forms.ToolStripStatusLabel();
            this.workspace = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logStatus,
            this.versionReadout,
            this.discordLink,
            this.memoryReadout});
            this.statusBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusBar.Location = new System.Drawing.Point(0, 656);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusBar.Size = new System.Drawing.Size(1264, 25);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusBar";
            // 
            // logStatus
            // 
            this.logStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.logStatus.Name = "logStatus";
            this.logStatus.Size = new System.Drawing.Size(42, 20);
            this.logStatus.Spring = true;
            this.logStatus.Text = "Ready.";
            // 
            // versionReadout
            // 
            this.versionReadout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.versionReadout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.versionReadout.ForeColor = System.Drawing.Color.Black;
            this.versionReadout.Name = "versionReadout";
            this.versionReadout.Size = new System.Drawing.Size(45, 20);
            this.versionReadout.Text = "version";
            // 
            // discordLink
            // 
            this.discordLink.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.discordLink.AutoToolTip = true;
            this.discordLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.discordLink.Image = global::Foundry.Properties.Resources.discord;
            this.discordLink.Name = "discordLink";
            this.discordLink.Size = new System.Drawing.Size(20, 20);
            this.discordLink.ToolTipText = "Join our discord!";
            this.discordLink.Click += new System.EventHandler(this.Footer_DiscordImageLink_Click);
            // 
            // memoryReadout
            // 
            this.memoryReadout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.memoryReadout.Name = "memoryReadout";
            this.memoryReadout.Size = new System.Drawing.Size(31, 20);
            this.memoryReadout.Text = "0mb";
            // 
            // workspace
            // 
            this.workspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspace.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.workspace.Location = new System.Drawing.Point(0, 24);
            this.workspace.Name = "workspace";
            this.workspace.ShowAutoHideContentOnHover = false;
            this.workspace.Size = new System.Drawing.Size(1264, 632);
            this.workspace.TabIndex = 3;
            // 
            // menuStrip
            // 
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjectToolStripMenuItem,
            this.importToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.ToolStrip_File_OpenProjectClicked);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importToolStripMenuItem.Text = "Import...";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.ToolStrip_File_ImportClicked);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // FoundryInstance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.workspace);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FoundryInstance";
            this.Text = "Foundry";
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel logStatus;
        private System.Windows.Forms.ToolStripStatusLabel versionReadout;
        private DockPanel workspace;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private ToolStripStatusLabel discordLink;
        private ToolStripStatusLabel memoryReadout;
        private ToolStripMenuItem importToolStripMenuItem;
    }
}