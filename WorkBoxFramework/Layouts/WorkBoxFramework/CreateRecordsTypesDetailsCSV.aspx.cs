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
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class CreateRecordsTypesDetailsCSV : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string csv = "";

            WBTaxonomy recordsTypes = WBTaxonomy.GetRecordsTypes(SPContext.Current.Site);

            foreach (Term classTerm in recordsTypes.TermSet.Terms)
            {
                WBRecordsType recordsClass = new WBRecordsType(recordsTypes, classTerm);

                csv += AddRecordsType(recordsClass);

            }

            CSVOutput.Text = csv;

        }

        private String AddRecordsType(WBRecordsType recordsType)
        {
            List<String> properties = recordsType.GetAllPropertyValues();
            List<String> csvValues = new List<String>();

            foreach (String property in properties) 
            {
                csvValues.Add("\"" + property + "\"");
            }

            string csvText = "";

            csvText += String.Join(",", csvValues.ToArray()) + "\n";

            foreach (Term child in recordsType.Term.Terms)
            {
                csvText += AddRecordsType(new WBRecordsType(recordsType.Taxonomy, child));
            }

            return csvText;
        }
    }
}
