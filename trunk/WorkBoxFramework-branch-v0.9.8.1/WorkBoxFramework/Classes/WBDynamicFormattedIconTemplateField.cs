﻿#region Copyright and License

// Copyright (c) Islington Council 2010-2015
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
    class WBDynamicFormattedIconTemplateField : ITemplate
    {
        public WBColumn LinkURLDataColumn { get; set; }
        List<WBColumn> Columns { get; set; }
        String FormatString { get; set; }
        bool UseLowerCaseIconURL { get; set; }

        public WBDynamicFormattedIconTemplateField(String formatString, List<WBColumn> columns, WBColumn linkURLDataColumn)
        {
            LinkURLDataColumn = linkURLDataColumn;
            FormatString = formatString;
            Columns = columns;
            UseLowerCaseIconURL = true;
        }

        public WBDynamicFormattedIconTemplateField(String formatString, List<WBColumn> columns, bool useLowerCaseIconURL, WBColumn linkURLDataColumn)
        {
            LinkURLDataColumn = linkURLDataColumn;
            FormatString = formatString;
            Columns = columns;
            UseLowerCaseIconURL = useLowerCaseIconURL;
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

            List<String> values = new List<String>();

            foreach (WBColumn column in Columns)
            {
                values.Add(DataBinder.Eval(row.DataItem, column.InternalName).WBxToString());
            }

            String formattedString = String.Format(FormatString, values.ToArray());
            if (UseLowerCaseIconURL) formattedString = formattedString.ToLower();

            link.ImageUrl = formattedString;
        }

    }
}

 