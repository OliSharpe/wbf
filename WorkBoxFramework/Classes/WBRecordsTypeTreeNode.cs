using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBRecordsTypeTreeNode : WBFolderTreeNode 
    {
        public WBRecordsType RecordsType;
        public WBTerm FunctionalArea;

        public WBRecordsTypeTreeNode(WBTerm functionalArea, WBRecordsType recordsType, SPFolder folder)
            : base(folder)
        {
            FunctionalArea = functionalArea;
            RecordsType = recordsType;
        }

        public TreeNode AsTreeNode()
        {
            TreeNode node = new TreeNode();
            node.Text = RecordsType.Name;
            node.Value = RecordsType.Name;
            node.ImageUrl = "/_layouts/Images/FOLDER.GIF";

            return node;
        }


    }
}
