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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkBoxFramework.ShowTimeToAutoClose
{
    [ToolboxItemAttribute(false)]
    public class ShowTimeToAutoClose : WebPart
    {
        protected override void CreateChildControls()
        {
            Label timeRemaining = new Label();
            this.Controls.Add(timeRemaining);

                   
            timeRemaining.CssClass = "wbf-time-to-auto-close";

            try
            {
                WorkBox workBox = WorkBox.GetIfWorkBox(SPContext.Current);

                if (workBox != null)
                {

                    DateTime endTime = workBox.calculateAutoCloseDate();

                    if (endTime.Year == WBRecordsType.YEAR_REPRESENTING_A_PERMANENT_DATE)
                    {
                        timeRemaining.Text = "This work box will not auto close.";
                    }
                    else
                    {
                        if (endTime < DateTime.Now)
                        {
                            timeRemaining.Text = "No time remaining before auto close.";
                        }
                        else
                        {
                            TimeSpan timeSpan = endTime - DateTime.Now;

                            timeRemaining.Text = String.Format("{0} Days {1} Hours {2} Minutes",
                                timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
                        }
                    }


                    workBox.Dispose();
                }
                else
                {
                    timeRemaining.Text = "You can only use this web part on a work box.";
                }
            }
            catch (Exception e)
            {
                WBLogging.Generic.Unexpected("Error: " + e.Message);
                timeRemaining.Text = "An error occurred";
            }
        }
    }
}
