#region Copyright and License

// Copyright (c) Islington Council 2010-2013
// Author: Oli Sharpe  (oli@gometa.co.uk)
//
// This file is part of the Work Box Framework.
//
// The Work Box Framework is free software: you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public License as  
// published by the Free Software Foundation, either version 2.1 of the 
// License, or (at your option) any later version.
//
// The Work Box Framework (WBF) is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with the WBF.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.ListWorkBoxesWebPart
{
    [ToolboxItemAttribute(false)]
    public class ListWorkBoxesWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/WorkBoxFramework/ListWorkBoxesWebPart/ListWorkBoxesWebPartUserControl.ascx";

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Work Box Collection URL")]
        [WebDescription("Enter the URL for the work box collection you want to list")]
        [System.ComponentModel.Category("Configuration")]
        public string WorkBoxCollectionURL { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Group by Work Box Template?")]
        [WebDescription("Should the list of work boxes be grouped by template?")]
        [System.ComponentModel.Category("Configuration")]
        public bool GroupByWorkBoxTemplate { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show Empty Work Box Templates?")]
        [WebDescription("Should the web part list as 'none' when a Work Box Templates grouping is empty?")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowEmptyWorkBoxTemplates { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.User)]
        [WebDisplayName("Show Closed Work Boxes?")]
        [WebDescription("Should the list of work boxes include closed work boxes?")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowClosedWorkBoxes { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.User)]
        [WebDisplayName("Show Deleted Work Boxes?")]
        [WebDescription("Should the list of work boxes include deleted work boxes?")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowDeletedWorkBoxes { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.User)]
        [WebDisplayName("Show Create New Link?")]
        [WebDescription("Should the link for creating new work boxes be visible?")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowCreateNewLink { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Create New Link Text")]
        [WebDescription("Enter the text for the create new link.")]
        [System.ComponentModel.Category("Configuration")]
        public string CreateNewLinkText { get; set; }

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
