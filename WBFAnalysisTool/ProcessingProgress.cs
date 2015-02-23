using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WBFAnalysisTool
{
    public partial class ProcessingProgress : Form
    {
        public ProcessingProgress()
        {
            InitializeComponent();

            this.Shown += new EventHandler(ProcessingProgress_Shown);
        }


        private void ProcessingProgress_Shown(Object sender, EventArgs e)
        {
            OKButton.Enabled = false;

            ProgressInformation.Text = "The process has started";

            int failures = Globals.ThisAddIn.ProcessWBFData(ProgressInformation);

            ProgressInformation.Text = "The process has finished. (Failures: " + failures + ")";

            OKButton.Enabled = true;
        }

    }
}
