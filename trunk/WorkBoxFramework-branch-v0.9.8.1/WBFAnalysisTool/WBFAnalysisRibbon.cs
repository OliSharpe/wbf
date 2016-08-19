using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace WBFAnalysisTool
{
    public partial class WBFAnalysisRibbon
    {
        private void WBFAnalysisRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void ProcessDataButton_Click(object sender, RibbonControlEventArgs e)
        {
            ProcessingProgress processingProgress = new ProcessingProgress();
            processingProgress.Show();
        }
    }
}
