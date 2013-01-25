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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WorkBoxFramework
{
    public class WBIconItemTemplateField : ITemplate
    {

        public WBColumn LinkURLDataColumn { get; set; }
        public WBColumn IconImageURLDataColumn { get; set; }
        public String StaticIconImageURL { get; set; }

        public WBIconItemTemplateField(WBColumn iconImageURLDataColumn, WBColumn linkURLDataColumn)
        {
            LinkURLDataColumn = linkURLDataColumn;
            IconImageURLDataColumn = iconImageURLDataColumn;
            StaticIconImageURL = null;
        }

        public WBIconItemTemplateField(String staticIconImageURL, WBColumn linkURLDataColumn)
        {
            LinkURLDataColumn = linkURLDataColumn;
            StaticIconImageURL = staticIconImageURL;
            IconImageURLDataColumn = null;
        }


        public void InstantiateIn(System.Web.UI.Control container)
        {
            HyperLink link = new HyperLink();
            
            link.DataBinding += new EventHandler(link_DataBinding);

            container.Controls.Add(link);
        }

        void link_DataBinding(object sender, EventArgs e)
        {
            HyperLink link = (HyperLink)sender;

            GridViewRow row = (GridViewRow)link.NamingContainer;

            link.NavigateUrl = DataBinder.Eval(row.DataItem, LinkURLDataColumn.InternalName).WBxToString();

            if (IconImageURLDataColumn == null)
            {
                link.ImageUrl = StaticIconImageURL;
            }
            else
            {
                link.ImageUrl = DataBinder.Eval(row.DataItem, IconImageURLDataColumn.InternalName).WBxToString();
            }
        }

    }
}
