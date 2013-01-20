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
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.ViewSubjectPages
{
    [ToolboxItemAttribute(false)]
    public class ViewSubjectPages : WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Root Subject Tag")]
        [WebDescription("What is the root subject tag from which to display?")]
        [System.ComponentModel.Category("Configuration")]
        public String RootSubjectTag { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Filter By Owning Team")]
        [WebDescription("Only show documents that belong to this owning team.")]
        [System.ComponentModel.Category("Configuration")]
        public String FilterByOwningTeam { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Only Show Live Records")]
        [WebDescription("Only show records that are marked as 'Live'")]
        [System.ComponentModel.Category("Configuration")]
        public bool OnlyLiveRecords { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Only Show Terms With Documents")]
        [WebDescription("Trim the list of terms to only show those with documents.")]
        [System.ComponentModel.Category("Configuration")]
        public bool OnlyTermsWithDocuments { get; set; }

        
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show A to Z")]
        [WebDescription("Display the A to Z letters and associated functionality.")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowAToZ { get; set; }


        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Hide Document on Root Page")]
        [WebDescription("Hide documents from showing on the root page.")]
        [System.ComponentModel.Category("Configuration")]
        public bool HideDocumentsOnRootPage { get; set; }


        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Show Records Types")]
        [WebDescription("Show records types as a second level browsing experience")]
        [System.ComponentModel.Category("Configuration")]
        public bool ShowRecordTypes { get; set; }


        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/WorkBoxFramework/ViewSubjectPages/ViewSubjectPagesUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
