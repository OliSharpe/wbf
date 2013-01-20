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
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Publishing;

namespace WorkBoxFramework.Layouts.WorkBoxFramework
{
    public partial class MigrationAdmin : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WBFarm farm = WBFarm.Local;


            if (!IsPostBack)
            {
                MigrationType.DataSource = WBFarm.GetMigrationTypes();
                MigrationType.DataBind();
                MigrationType.WBxSafeSetSelectedValue(farm.MigrationType);

                MigrationSourceSystem.Text = farm.MigrationSourceSystem;

                MigrationControlListUrl.Text = farm.MigrationControlListUrl;
                MigrationControlListView.Text = farm.MigrationControlListView;

                MigrationMappingListUrl.Text = farm.MigrationMappingListUrl;
                MigrationMappingListView.Text = farm.MigrationMappingListView;

                MigrationSubjectsListUrl.Text = farm.MigrationSubjectsListUrl;
                MigrationSubjectsListView.Text = farm.MigrationSubjectsListView;

                ItemsPerCycle.Text = farm.MigrationItemsPerCycle;

                UserName.Text = farm.MigrationUserName;
                UserPassword.Text = "";

            }
        }


        protected void CancelButton_OnClick(object sender, EventArgs e)
        {
            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }

        protected void UpdateButton_OnClick(object sender, EventArgs e)
        {
            WBFarm farm = WBFarm.Local;

            farm.MigrationType = MigrationType.SelectedValue;

            farm.MigrationSourceSystem = MigrationSourceSystem.Text;

            farm.MigrationControlListUrl = MigrationControlListUrl.Text;
            farm.MigrationControlListView = MigrationControlListView.Text;

            farm.MigrationMappingListUrl = MigrationMappingListUrl.Text;
            farm.MigrationMappingListView = MigrationMappingListView.Text;

            farm.MigrationSubjectsListUrl = MigrationSubjectsListUrl.Text;
            farm.MigrationSubjectsListView = MigrationSubjectsListView.Text;

            farm.MigrationItemsPerCycle = ItemsPerCycle.Text;

            farm.MigrationUserName = UserName.Text;
            farm.MigrationPassword = UserPassword.Text;

            farm.Update();

            SPUtility.Redirect("/applications.aspx", SPRedirectFlags.Static, Context);
        }


    }
}
