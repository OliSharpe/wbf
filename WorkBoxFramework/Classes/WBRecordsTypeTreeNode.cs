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
        //public TreeNode ParentNode;

        public WBRecordsTypeTreeNode(WBTerm functionalArea, WBRecordsType recordsType, SPFolder folder)
            : base(folder, false)
        {
            FunctionalArea = functionalArea;
            RecordsType = recordsType;
            //ParentNode = parent;

            this.Text = RecordsType.Name;
            this.Value = RecordsType.Name;
            this.ImageUrl = "/_layouts/Images/FOLDER.GIF";
        }

    }
}
