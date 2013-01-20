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
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class routing : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string localID = Request.QueryString["LocalID"];
            if (localID == null || localID == "") localID = Request.QueryString["IASClientID"];


            if (localID == null || localID == "") 
            {
                ErrorMessage.Text = "Could not find the LocalID on the request parameters.";
                return;
            }

            bool justFind = false;
            string justFindString = Request.QueryString["JustFind"];
            if (justFindString != null && justFindString != "")
            {
                WBUtils.logMessage("Testing JustFind = " + justFindString);

                justFind = true.ToString().ToLower().Equals(justFindString.ToLower());

                WBUtils.logMessage("Testing justFind = " + justFind);
                WBUtils.logMessage("true.ToString() = " + true.ToString());

            }
            else
            {
                WBUtils.logMessage("No JustFind query parameter");
            }

            using (WBCollection collection = new WBCollection(SPContext.Current))
            {

                WorkBox workBox = collection.FindByLocalID(localID);

                if (workBox == null) WBUtils.logMessage(" workBox was NULL");

                if (workBox == null && justFind)
                {
                    string html = "<h3>There is no work box with ID: " + localID + "</h3>\n";

                    html += "<a href=\"routing.aspx?LocalID=" + localID + "&justFind=false\">Click here to create a new work box with this ID</a>\n";

                    DoesNotExistYet.Text = html;
                }
                else
                {
                    using (SPLongOperation longOperation = new SPLongOperation(this.Page))
                    {

                        if (workBox == null)
                        {
                            longOperation.LeadingHTML = "No documents found: creating new work box.";
                            longOperation.TrailingHTML = "This service user doesn't yet have a work box so it is being created.";
                        }
                        else
                        {
                            longOperation.LeadingHTML = "Found service user's work box.";
                            longOperation.TrailingHTML = "Please wait while the work box is opened.";
                        }

                        longOperation.Begin();

                        collection.Site.AllowUnsafeUpdates = true;
                        collection.Web.AllowUnsafeUpdates = true;
                        if (workBox == null)
                        {
                            workBox = collection.RequestNewWorkBox("", localID);
                        }

                        if (workBox.HasBeenCreated) workBox.Web.AllowUnsafeUpdates = true;

                        if (!workBox.HasBeenOpened) workBox.Open();

                        string workBoxUrl = workBox.Url;

                        workBox.Web.AllowUnsafeUpdates = false;
                        collection.Web.AllowUnsafeUpdates = false;
                        collection.Site.AllowUnsafeUpdates = false;

                        workBox.Dispose();

                        longOperation.End(workBoxUrl, SPRedirectFlags.Static, Context, "");
                    }
                }

            }

        }
    }
}
