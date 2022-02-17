﻿using SMHEditor.Project.FileTypes;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.ObjectEditor.Veterancy
{
    public partial class VeterancysControl : UserControl
    {
        private readonly ObjectFile obj;
        public VeterancysControl(ObjectFile o)
        {
            InitializeComponent();
            add.MouseClick += new MouseEventHandler(Add);
            Dock = DockStyle.Fill;
            obj = o;
        }

        private void Add(object o, EventArgs e)
        {
            Project.FileTypes.Veterancy vt = new Project.FileTypes.Veterancy
            {
                Level = $"{obj.Veterancy.Count + 1}"
            };
            obj.Veterancy.Add(vt);
            VeterancyControl vc = new VeterancyControl(obj, vt, this)
            {
                Tag = obj.Veterancy.Count
            };
            flowLayoutPanel.Controls.Add(vc);

            if (obj.Veterancy.Count > 4)
            {
                add.Enabled = false;
                add.Visible = false;
            }
        }

        private void automatic_Click(object sender, EventArgs e)
        {
            if (obj.Veterancy.Count > 0)
            {
                if (MessageBox.Show("This will delete your current levels and add basic ones, are you sure you want to proceed?", "Automatic", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                obj.Veterancy.Clear();
                flowLayoutPanel.Controls.Clear();
            }

            for (int i = 1; i <= 5; i++)
            {
                Project.FileTypes.Veterancy vt = new Project.FileTypes.Veterancy
                {
                    Level = $"{i}",
                    XP = $"{i * 2}",
                    Damage = $"{0.9 + 0.1 * i}",
                    Velocity = $"{1}",
                    Accuracy = $"{0.9 + 0.1 * i}",
                    WorkRate = $"{0.75 + 0.25 * i}",
                    WeaponRange = $"{1}",
                    DamageTaken = $"{1.05 - 0.05 * i}",
                };

                obj.Veterancy.Add(vt);
                VeterancyControl vc = new VeterancyControl(obj, vt, this);
                vc.level.Value = (decimal)float.Parse(vt.Level);
                vc.xp.Value = (decimal)float.Parse(vt.XP);
                vc.damage.Value = (decimal)float.Parse(vt.Damage);
                vc.velocity.Value = (decimal)float.Parse(vt.Velocity);
                vc.accuracy.Value = (decimal)float.Parse(vt.Accuracy);
                vc.workrate.Value = (decimal)float.Parse(vt.WorkRate);
                vc.weaponrange.Value = (decimal)float.Parse(vt.WeaponRange);
                vc.damagetaken.Value = (decimal)float.Parse(vt.DamageTaken);
                vc.Tag = obj.Veterancy.Count;
                flowLayoutPanel.Controls.Add(vc);

                add.Enabled = false;
                add.Visible = false;
            }

        }

        private void remove_Click(object sender, EventArgs e)
        {
            ControlCollection controls = flowLayoutPanel.Controls;
            Control latestVeterancy = controls.Cast<Control>().FirstOrDefault(control => string.Equals(control.Tag, obj.Veterancy.Count));
            if (latestVeterancy.Tag.ToString() == "1") { return; }
            flowLayoutPanel.Controls.Remove(latestVeterancy);
            obj.Veterancy.Remove(obj.Veterancy.Last());

            if (obj.Veterancy.Count < 5)
            {
                add.Enabled = true;
                add.Visible = true;
            }
        }
    }
}
