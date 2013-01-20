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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace WorkBoxFramework.ControlTemplates.WorkBoxFramework
{
    public partial class LinkToWorkBoxHome : UserControl
    {
        public String LinkToHome = "";
        public String TextForLink = "";
        public String CSSClass = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);

            if (workBox != null)
            {
                LinkToHome = workBox.Web.Url;

                if (TextForLink == "")
                    TextForLink = workBox.Web.Title;

                // This wont do anything because the web and site come from context, but still:
                workBox.Dispose();
            }
        }
    }
}
