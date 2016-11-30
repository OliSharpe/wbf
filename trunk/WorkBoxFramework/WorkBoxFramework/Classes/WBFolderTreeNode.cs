using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBFolderTreeNode : TreeNode
    {
        public SPFolder Folder;

        public WBFolderTreeNode(SPFolder folder)
            : base()
        {
            Folder = folder;
            //ParentNode = parent;

            this.Text = Folder.Name;
            this.Value = Folder.Name;
            this.ImageUrl = "/_layouts/Images/FOLDER.GIF";
        }

        public WBFolderTreeNode(SPFolder folder, bool setNodeDetails)
            : base()
        {
            Folder = folder;
            //ParentNode = parent;

            if (setNodeDetails)
            {
                this.Text = Folder.Name;
                this.Value = Folder.Name;
                this.ImageUrl = "/_layouts/Images/FOLDER.GIF";
            }
        }

    }
}
