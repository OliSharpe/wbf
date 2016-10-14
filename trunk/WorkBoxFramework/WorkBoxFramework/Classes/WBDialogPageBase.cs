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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;


namespace WorkBoxFramework
{
    public abstract class WBDialogPageBase : LayoutsPageBase
    {
        protected const int INVALID_RESULT = -1;
        protected const int CANCEL_RESULT = 0;
        protected const int OK_RESULT = 1;
        protected const int OK_REFRESH = 2;
        protected const int OK_REDIRECT = 3;

        protected bool validValues = true;
        protected string errorMessage = "";
        protected bool pageRenderingRequired = true;


        protected void addErrorMessage(string message) 
        {
            errorMessage += message;
            WBUtils.logMessage("Error on a Work Box dialog page: " + message);
        }

        protected bool checkForErrors()
        {
            if (!validValues)
            {
                errorMessage += "You must enter valid data before proceeding. ";
            }

            if (errorMessage.Length > 0)
            {
                pageRenderingRequired = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Close methods
        protected void CloseDialogAndRefresh()
        {
            returnFromDialog(OK_REFRESH, "");
        }

        [Obsolete("returnFromDialogOKAndRefresh is deprecated, please use CloseDialogAndRefresh instead.", true)]
        protected void returnFromDialogOKAndRefresh()
        {
            CloseDialogAndRefresh();
        }

        protected void CloseDialogAndRefresh(String refreshQueryString)
        {
            returnFromDialog(OK_REFRESH, refreshQueryString);
        }

        [Obsolete("returnFromDialogOKAndRefresh is deprecated, please use CloseDialogAndRefresh instead.", true)]
        protected void returnFromDialogOKAndRefresh(String refreshQueryString)
        {
            CloseDialogAndRefresh(refreshQueryString);
        }

        protected void CloseDialogAndRedirect(String redirectURL)
        {
            returnFromDialog(OK_REDIRECT, redirectURL);
        }

        [Obsolete("returnFromDialogOKAndRedirect is deprecated, please use CloseDialogAndRedirect instead.", true)]
        protected void returnFromDialogOKAndRedirect(String redirectURL)
        {
            CloseDialogAndRedirect(redirectURL);
        }

        protected void CloseDialogWithOK()
        {
            returnFromDialogOK("");
        }

        protected void CloseDialogWithOK(String message)
        {
            returnFromDialogOK(message);
        }

        protected void CloseDialogWithCancel()
        {
            returnFromDialogCancel("");
        }

        protected void CloseDialogWithCancel(String message)
        {
            returnFromDialogCancel(message);
        }

        protected void CloseDialogWithError(String message)
        {
            returnFromDialogError(message);
        }
        #endregion

        #region return value methods
        protected void returnFromDialogOK(string returnValue)
        {
            returnFromDialog(OK_RESULT, returnValue);
        }

        protected void returnFromDialogError(string returnValue)
        {
            returnFromDialog(INVALID_RESULT, returnValue);
        }

        protected void returnFromDialogCancel(string returnValue)
        {
            returnFromDialog(CANCEL_RESULT, returnValue);
        }

        protected void returnFromDialog(int resultValue, string returnValue)
        {
            WBUtils.logMessage("Dialog returning with values: result = " + resultValue + " return = " + returnValue);

//            if (Page.Request.QueryString["IsDlg"] != null)
  //              SPUtility.Redirect("/", SPRedirectFlags.UseSource, Context, "");


            Page.Response.Clear();
            Page.Response.Write(String.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose({0}, {1});</script>", new object[] { resultValue, String.IsNullOrEmpty(returnValue) ? "null" : String.Format("\"{0}\"", returnValue) }));
            Page.Response.End(); 
        }

        protected void ReturnJSONFromDialogOK(string returnValue)
        {
            int resultValue = OK_RESULT;

            WBLogging.Debug("Dialog returning with values: result = " + resultValue + " return = " + returnValue);

            //            if (Page.Request.QueryString["IsDlg"] != null)
            //              SPUtility.Redirect("/", SPRedirectFlags.UseSource, Context, "");


            Page.Response.Clear();
            Page.Response.Write(String.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">\n var returnVal = {1}; \n window.frameElement.commonModalDialogClose({0}, returnVal);</script>", new object[] { resultValue, String.IsNullOrEmpty(returnValue) ? "null" : String.Format("'{0}'", returnValue) }));
            Page.Response.End();
        }

        #endregion

        #region Go To methods
        protected void GoToGenericOKPage(String pageTitle, String pageText)
        {
            pageTitle = Uri.EscapeDataString(pageTitle);
            pageText = Uri.EscapeDataString(pageText);

            string redirectUrl = "WorkBoxFramework/GenericOKPage.aspx";
            string queryString = "pageTitle=" + pageTitle + "&pageText=" + pageText;

            SPUtility.Redirect(redirectUrl, SPRedirectFlags.RelativeToLayoutsPage, Context, queryString);
        }
        #endregion

    }
}
