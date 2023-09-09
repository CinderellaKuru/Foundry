using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace foundry
{
    public class OperatorRegistrantToolstrip : OperatorRegistrant
    {
        private void AddMenuItemChildren(ToolStripMenuItem item, Operator op)
        {
            foreach (Operator childOp in op.Children)
            {
                ToolStripMenuItem childItem = new ToolStripMenuItem(childOp.Name, null, 
                    (sender, e) => 
                    { 
                        childOp.Activate(); 
                    });
                item.DropDownItems.Add(childItem);
                AddMenuItemChildren(childItem, childOp);
            }
        }

        public List<ToolStripMenuItem> GetRootMenuItems()
        {
            List<ToolStripMenuItem> ret = new List<ToolStripMenuItem>();
            foreach (Operator op in Operators)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(op.Name, null,
                    (sender, e) =>
                    {
                        op.Activate();
                    });
                AddMenuItemChildren(item, op);
                ret.Add(item);
            }
            return ret;
        }
    }
}
