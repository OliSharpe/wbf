namespace WBFAnalysisTool
{
    partial class WBFAnalysisRibbon : Microsoft.Office.Tools.Ribbon.OfficeRibbon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public WBFAnalysisRibbon()
        {
            InitializeComponent();
        }

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
            this.WBFAnalysisTab = new Microsoft.Office.Tools.Ribbon.RibbonTab();
            this.WBFAnalysisGroup = new Microsoft.Office.Tools.Ribbon.RibbonGroup();
            this.ProcessDataButton = new Microsoft.Office.Tools.Ribbon.RibbonButton();
            this.WBFAnalysisTab.SuspendLayout();
            this.WBFAnalysisGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // WBFAnalysisTab
            // 
            this.WBFAnalysisTab.Groups.Add(this.WBFAnalysisGroup);
            this.WBFAnalysisTab.Label = "WBF Analysis";
            this.WBFAnalysisTab.Name = "WBFAnalysisTab";
            // 
            // WBFAnalysisGroup
            // 
            this.WBFAnalysisGroup.Items.Add(this.ProcessDataButton);
            this.WBFAnalysisGroup.Label = "WBF Analysis";
            this.WBFAnalysisGroup.Name = "WBFAnalysisGroup";
            // 
            // ProcessDataButton
            // 
            this.ProcessDataButton.Label = "Process Data";
            this.ProcessDataButton.Name = "ProcessDataButton";
            this.ProcessDataButton.Click += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs>(this.ProcessDataButton_Click);
            // 
            // WBFAnalysisRibbon
            // 
            this.Name = "WBFAnalysisRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.WBFAnalysisTab);
            this.Load += new System.EventHandler<Microsoft.Office.Tools.Ribbon.RibbonUIEventArgs>(this.WBFAnalysisRibbon_Load);
            this.WBFAnalysisTab.ResumeLayout(false);
            this.WBFAnalysisTab.PerformLayout();
            this.WBFAnalysisGroup.ResumeLayout(false);
            this.WBFAnalysisGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab WBFAnalysisTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup WBFAnalysisGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ProcessDataButton;
    }

    partial class ThisRibbonCollection : Microsoft.Office.Tools.Ribbon.RibbonReadOnlyCollection
    {
        internal WBFAnalysisRibbon WBFAnalysisRibbon
        {
            get { return this.GetRibbon<WBFAnalysisRibbon>(); }
        }
    }
}
