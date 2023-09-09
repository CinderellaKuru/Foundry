using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static foundry.FoundryInstance;

namespace foundry
{
    public partial class BaseToolPage : DockContent
    {
		public FoundryInstance Instance { get; private set; }
        public BaseToolPage()
        {
            FormClosing += new FormClosingEventHandler(Internal_Closed);
        }
        private void Internal_Closed(object o, FormClosingEventArgs e)
        {
            Controls.Clear();
            OnPageClose?.Invoke(this, null);
        }

        public void Init(FoundryInstance i)
        {
            Instance = i;
            OnPageInit?.Invoke(this, new InitArgs() { Instance = i });
        }

        public bool TryShow(FoundryInstance i, DockState state)
        {
            return TryShow(i.MainDockPanel, state);
        }
        public bool TryShow(BaseToolPage page, DockState state)
        {
            return TryShow(page.DockPanel, state);
        }
        private bool TryShow(DockPanel panel, DockState state)
        {
            if (Instance == null) return false;

            try
            {
                Show(panel, state);
                return true;
            }
            catch (Exception e)
            {
                Instance.AppendLog(LogEntryType.Error, "TryShow(): Show() encountered an error. See console for details.", true,
                    string.Format("--Error info:\n--Editor type: {0}\n--Exception information: {1}'\n'--Stacktrace:{2}", GetType().Name, e.Message, e.StackTrace));
                return false;
            }
        }
        public bool TryClose(bool force = false)
        {
            if (Instance == null) return false;

            Close();
            try
            {
                OnPageClose?.Invoke(this, null);
            }
            catch (Exception e)
            {
                Instance.AppendLog(LogEntryType.Error, "TryClose(): OnClose() encountered an error. See console for details.", true,
                    string.Format("--Error info:\n--Editor type: {0}\n--Exception information: {1}'\n'--Stacktrace:{2}", GetType().Name, e.Message, e.StackTrace));
                return false;
            }
            return true;
        }

        public class InitArgs
        {
            public FoundryInstance Instance { get; set; }
        }
        public event EventHandler<InitArgs> OnPageInit;
        public event EventHandler OnPageShow;
        public event EventHandler OnPageClose;
    }
}
