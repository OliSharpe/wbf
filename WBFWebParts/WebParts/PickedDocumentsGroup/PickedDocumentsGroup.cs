﻿#region Copyright and License

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

namespace WBFWebParts.PickedDocumentsGroup
{
    [ToolboxItemAttribute(false)]
    public class PickedDocumentsGroup : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/WBFWebParts/PickedDocumentsGroup/PickedDocumentsGroupUserControl.ascx";

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [System.ComponentModel.Category("Configuration")]
        public String DocumentsGroupDescription { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [System.ComponentModel.Category("Configuration")]
        public String DocumentsGroupSubjectTags { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [System.ComponentModel.Category("Configuration")]
        public String DocumentsGroupCoverage { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [System.ComponentModel.Category("Configuration")]
        public String PickedDocumentsDetails { get; set; }


        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
