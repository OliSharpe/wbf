using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBFolderTreeNode
    {
        public SPFolder Folder;

        public WBFolderTreeNode(SPFolder folder)
            : base()
        {
            Folder = folder;
        }

        public TreeNode AsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Text = Folder.Name;
            node.Value = Folder.Name;
            node.ImageUrl = "/_layouts/Images/FOLDER.GIF";

            return node;
        }

    }
}
