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
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;


namespace WorkBoxFramework
{

    public class WorkBoxMetaDataItemChangeEventReceiver : SPItemEventReceiver
    {
       
        public override void ItemAdded(SPItemEventProperties properties)       
        {
            WBLogging.Generic.HighLevel("WorkBoxMetaDataItemChangeEventReceiver.ItemAdded()");
            WBCollection collection = new WBCollection(properties.ListItem);

            using (WorkBox workBox = new WorkBox(collection, properties.ListItem))
            {
                processChangeRequest(workBox);
            }
           
            base.ItemAdded(properties);
       }

       /// <summary>
       /// An item was updated.
       /// </summary>
       public override void ItemUpdated(SPItemEventProperties properties)
       {
           WBLogging.Generic.Verbose("WorkBoxMetaDataItemChangeEventReceiver.ItemUpdated()");
           WBCollection collection = new WBCollection(properties.ListItem);
           using (WorkBox workBox = new WorkBox(collection, properties.ListItem))
           {
               processChangeRequest(workBox);
           }


           base.ItemUpdated(properties);
       }

       private void processChangeRequest(WorkBox workBox)
       {
           WBLogging.Generic.Verbose("WorkBoxMetaDataItemChangeEventReceiver.processChangeRequest()");

           String currentStatus = workBox.Status;
           String requestedChange = workBox.StatusChangeRequest;

           WBLogging.Generic.Verbose("WorkBoxMetaDataItemChangeEventReceiver.processChangeRequest(): Current status = " + currentStatus + " and requested change = " + requestedChange);

//           this.EventFiringEnabled = false;

           if (requestedChange.Equals(WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__CREATE))
           {
               workBox.Create();
           }
           else if (requestedChange.Equals(WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__OPEN))
           {
               workBox.Open();
           }
           else if (requestedChange.Equals(WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__CLOSE))
           {
               workBox.Close();
           }
           else if (requestedChange.Equals(WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__ARCHIVE))
           {
               workBox.Archive();
           }
           else if (requestedChange.Equals(WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__DELETE))
           {
               workBox.Delete();
           }
           else if (requestedChange.Equals(WorkBox.REQUEST_WORK_BOX_STATUS_CHANGE__REAPPLY_PERMISSIONS))
           {
               workBox.ReapplyPermissions();
               workBox.ClearStatusChangeRequest();
           }
           else
           {
               WBLogging.Generic.Verbose("WorkBoxMetaDataItemChangeEventReceiver.processChangeRequest(): No request for change so doing nothing");
           }

//           this.EventFiringEnabled = true;
       }
    }

}
