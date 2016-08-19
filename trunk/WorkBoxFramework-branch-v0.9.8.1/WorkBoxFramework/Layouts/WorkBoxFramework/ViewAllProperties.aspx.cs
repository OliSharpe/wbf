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
using System.Collections;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class ViewAllProperties : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string html = "<table cellspacing=\"10\">\n";

            SPWeb web = SPContext.Current.Web;

            Hashtable properties = web.AllProperties;

            foreach (DictionaryEntry entry in properties)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td></tr>\n",
                    entry.Key.WBxToString(),
                    entry.Value.WBxToString());
            }

            html += "</table>\n";

            GeneratedHTML.Text = html;
        }
    }
}
