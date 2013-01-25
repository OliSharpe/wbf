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
    public class WBFormatStringTemplateField : ITemplate
    {
        List<WBColumn> Columns { get; set; }
        String FormatString { get; set; }

        public WBFormatStringTemplateField(String formatString, List<WBColumn> columns)
        {
            FormatString = formatString;
            Columns = columns;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            Literal literal = new Literal();

            literal.DataBinding += new EventHandler(literal_DataBinding);

            container.Controls.Add(literal);
        }

        void literal_DataBinding(object sender, EventArgs e)
        {
            Literal literal = (Literal)sender;

            GridViewRow row = (GridViewRow)literal.NamingContainer;

            List<String> values = new List<String>();

            foreach (WBColumn column in Columns)
            {
                values.Add(DataBinder.Eval(row.DataItem, column.InternalName).WBxToString());
            }

            literal.Text = String.Format(FormatString, values.ToArray());
        }
    }
}
