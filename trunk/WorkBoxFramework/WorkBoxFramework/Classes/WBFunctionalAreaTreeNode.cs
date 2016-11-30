using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBFunctionalAreaTreeNode : WBFolderTreeNode
    {
        public WBTerm FunctionalArea;

        public WBFunctionalAreaTreeNode(WBTerm functionalArea, SPFolder folder)
            : base(folder, false)
        {
            FunctionalArea = functionalArea;

            this.Text = functionalArea.Name;
            this.Value = functionalArea.Name;
            this.ImageUrl = "/_layouts/Images/FOLDER.GIF";
        }
    }
}
