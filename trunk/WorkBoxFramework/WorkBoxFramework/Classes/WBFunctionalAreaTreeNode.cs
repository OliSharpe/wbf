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
            : base(folder)
        {
            FunctionalArea = functionalArea;
        }

        public TreeNode AsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Text = FunctionalArea.Name;
            node.Value = FunctionalArea.Name;
            node.ImageUrl = "/_layouts/Images/FOLDER.GIF";

            return node;
        }

    }
}
