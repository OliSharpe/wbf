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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Microsoft.SharePoint;
using Microsoft.Office.RecordsManagement.RecordsRepository;

namespace WorkBoxFramework
{
    public class WBDocument : WBItem
    {

        #region Constructors
        public WBDocument(SPListItem item) : base(item)
        {            
            RecordsLibrary = null;
            WorkBox = null;
        }

        public WBDocument(WBRecordsLibrary library, SPListItem item)
            : base(item)
        {
            RecordsLibrary = library;
            WorkBox = null;
            DebugName = "<WBDocument>";
        }

        public WBDocument(WBRecordsLibrary library, SPListItemVersion item)
            : base(item)
        {
            RecordsLibrary = library;
            WorkBox = null;
            DebugName = "<WBDocument>";
        }

        public WBDocument(WorkBox workBox, SPListItem item)
            : base(item)
        {
            RecordsLibrary = null;
            WorkBox = workBox;
            DebugName = "<WBDocument>";
        }

        public WBDocument(WorkBox workBox, SPListItemVersion item)
            : base(item)
        {
            RecordsLibrary = null;
            WorkBox = workBox;
            DebugName = "<WBDocument>";
        }


        public WBDocument() : base()
        {
            RecordsLibrary = null;
            WorkBox = null;
            DebugName = "<WBDocument>";
        }

        public WBDocument(WBRecordsLibrary library)
            : base()
        {
            RecordsLibrary = library;
            WorkBox = null;
            DebugName = "<WBDocument>";
        }

        public WBDocument(WorkBox workBox)
            : base()
        {
            RecordsLibrary = null;
            WorkBox = workBox;
            DebugName = "<WBDocument>";
        }

        #endregion

        public String DebugName;

        public SPSite Site
        {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.Site;
                if (WorkBox != null) return WorkBox.Site;
                return null;
            }
        }

        public SPWeb Web
        {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.Web;
                if (WorkBox != null) return WorkBox.Web;
                return null;
            }
        }

        public WBRecordsLibrary RecordsLibrary { get; private set; }
        public WorkBox WorkBox { get; private set; }

        public WBTaxonomy RecordsTypesTaxonomy {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.RecordsTypesTaxonomy;
                if (WorkBox != null) return WorkBox.RecordsTypesTaxonomy;
                return null;
            }
        }

        public WBTaxonomy TeamsTaxonomy
        {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.TeamsTaxonomy;
                if (WorkBox != null) return WorkBox.TeamsTaxonomy;
                return null;
            }
        }
        public WBTaxonomy SeriesTagsTaxonomy
        {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.SeriesTagsTaxonomy;
                if (WorkBox != null) return WorkBox.SeriesTagsTaxonomy;
                return null;
            }
        }
        public WBTaxonomy SubjectTagsTaxonomy
        {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.SubjectTagsTaxonomy;
                if (WorkBox != null) return WorkBox.SubjectTagsTaxonomy;
                WBLogging.Debug("Returning null from WBDocument.SubjectTagsTaxonomy because RecordsLibrary = " + RecordsLibrary + " in doc: " + DebugName);
                return null;
            }
        }
        public WBTaxonomy FunctionalAreasTaxonomy
        {
            get
            {
                if (RecordsLibrary != null) return RecordsLibrary.FunctionalAreasTaxonomy;
                if (WorkBox != null) return WorkBox.FunctionalAreasTaxonomy;
                return null;
            }
        }


        public String AbsoluteURL
        {
            get
            {
                if (IsSPListItem || IsSPListItemVersion) return Item.Web.Url + "/" + Item.Url;
                return "";
            }
        }

        public String LibraryRelativePath
        {
            get
            {
                if ((IsSPListItem || IsSPListItemVersion) && RecordsLibrary != null)
                {
                    WBLogging.Debug("AbsoluteURL = " + AbsoluteURL);
                    WBLogging.Debug("RecordsLibrary.URL = " + RecordsLibrary.URL);

                    String path = AbsoluteURL.Replace(RecordsLibrary.URL, "");
                    if (path.Length > 0 && path[0] == '/') path = path.Remove(0, 1);

                    WBLogging.Debug("LibraryRelativePath = " + path);

                    return path;
                } else {
                    return "<n/a>";
                }
            }
        }

        public String LibraryLocation
        {
            get
            {
                String path = LibraryRelativePath;
                if (path == "<n/a>") return "<n/a>";

                int startOfFilename = path.LastIndexOf(Filename);
                // Plus we want to get rid of the last forward slash:
                if (startOfFilename > 0) startOfFilename = startOfFilename - 1;

                path = path.Remove(startOfFilename);

                return path;
            }
        }

        public WBRecordsType RecordsType
        {
            get 
            {
                Object value = this[WBColumn.RecordsType];
                if (value == null) return null;
                return new WBRecordsType(RecordsTypesTaxonomy, value.ToString());
            }
            set { this[WBColumn.RecordsType] = value; }
        }

        public WBTermCollection<WBTerm> FunctionalArea
        {
            get 
            {
                Object value = this[WBColumn.FunctionalArea];
                if (value == null) return null;
                return new WBTermCollection<WBTerm>(FunctionalAreasTaxonomy, value.ToString());
            }
            set { this[WBColumn.FunctionalArea] = value; }
        }

        public WBTermCollection<WBSubjectTag> SubjectTags
        {
            get
            {
                Object value = this[WBColumn.SubjectTags];
                if (value == null) return null;
                return new WBTermCollection<WBSubjectTag>(SubjectTagsTaxonomy, value.ToString());
            }
            set { this[WBColumn.SubjectTags] = value; }
        }

        public WBTerm SeriesTag
        {
            get
            {
                Object value = this[WBColumn.SeriesTag];
                if (value == null) return null;
                return new WBTerm(SeriesTagsTaxonomy, value.ToString());
            }
            set { this[WBColumn.SeriesTag] = value; }
        }

        public bool HasDateForFiling
        {
            get {
                //if (HasReferenceDate) return true;
                //if (HasDatePublished) return true;

                // In the absence of a metadata set value - we'll just use the current date - so we 'have' a usable date:
                return true;
            }
        }

        public DateTime DateForFiling
        {
            get
            {
                if (HasReferenceDate) return ReferenceDate;
                if (HasDatePublished) return DatePublished;

                // In the absence of any other appropriate date we'll just return today's date:
                return DateTime.Now;
            }
        }

        public bool HasDatePublished { get { return this.IsNotEmpty(WBColumn.DatePublished); } }
        public DateTime DatePublished
        {
            get
            {
                if (this.IsNullOrEmpty(WBColumn.DatePublished))
                {
                    WBLogging.Generic.Unexpected("Trying to read a 'DatePublished' value of a WBDocument that hasn't been set!!");
                    return DateTime.Now;
                }

                return (DateTime)this[WBColumn.DatePublished];
            }
            set
            {
                this[WBColumn.DatePublished] = value;
            }
        }

        public bool HasReviewDate { get { return this.IsNotEmpty(WBColumn.ReviewDate); } }
        public DateTime ReviewDate
        {
            get
            {
                if (this.IsNullOrEmpty(WBColumn.ReviewDate))
                {
                    WBLogging.Generic.Unexpected("Trying to read a 'ReviewDate' value of a WBDocument that hasn't been set!!");
                    return DateTime.Now;
                }

                return (DateTime)this[WBColumn.ReviewDate];
            }
            set
            {
                this[WBColumn.ReviewDate] = value;
            }
        }

        public bool HasReferenceDate { get { return this.IsNotEmpty(WBColumn.ReferenceDate); } }
        public DateTime ReferenceDate
        {
            get 
            {
                if (this.IsNullOrEmpty(WBColumn.ReferenceDate))
                {
                    WBLogging.Generic.Unexpected("Trying to read a 'ReferenceDate' value of a WBDocument that hasn't been set!!");
                    return DateTime.Now;
                }

                return (DateTime)this[WBColumn.ReferenceDate];
            }
            set 
            {
                this[WBColumn.ReferenceDate] = value;
            }
        }


        public bool HasModified { get { return this.IsNotEmpty(WBColumn.Modified); } }
        public DateTime Modified
        {
            get
            {
                if (this.IsNullOrEmpty(WBColumn.Modified))
                {
                    WBLogging.Generic.Unexpected("Trying to read a 'Modified' value of a WBDocument that hasn't been set!!");
                    return DateTime.Now;
                }

                return (DateTime)this[WBColumn.Modified];
            }
            set
            {
                this[WBColumn.Modified] = value;
            }
        }


        public bool HasDeclaredRecord { get { return this.IsNotEmpty(WBColumn.DeclaredRecord); } }
        public DateTime DeclaredRecord
        {
            get
            {
                if (this.IsNullOrEmpty(WBColumn.DeclaredRecord))
                {
                    WBLogging.Generic.Unexpected("Trying to read a 'DeclaredRecord' value of a WBDocument that hasn't been set!!");
                    return DateTime.Now;
                }

                return (DateTime)this[WBColumn.DeclaredRecord];
            }
            set
            {
                this[WBColumn.DeclaredRecord] = value;
            }
        }

        public bool HasScanDate { get { return this.IsNotEmpty(WBColumn.ScanDate); } }
        public DateTime ScanDate
        {
            get
            {
                if (this.IsNullOrEmpty(WBColumn.ScanDate))
                {
                    WBLogging.Generic.Unexpected("Trying to read a 'ScanDate' value of a WBDocument that hasn't been set!!");
                    return DateTime.Now;
                }

                return (DateTime)this[WBColumn.ScanDate];
            }
            set
            {
                this[WBColumn.ScanDate] = value;
            }
        }




        public String ReferenceID
        {
            get { return this[WBColumn.ReferenceID].WBxToString(); }
            set { this[WBColumn.ReferenceID] = value; }
        }

        public String OriginalFilename
        {
            get { return this[WBColumn.OriginalFilename].WBxToString(); }
            set { this[WBColumn.OriginalFilename] = value; }
        }

        public String Name
        {
            get { return this[WBColumn.Name].WBxToString(); }
            set { this[WBColumn.Name] = value; }
        }

        public String Title
        {
            get { return this[WBColumn.Title].WBxToString(); }
            set { this[WBColumn.Title] = value; }
        }

        public String Filename
        {
            get { return this[WBColumn.Name].WBxToString(); }
            set { this[WBColumn.Name] = value; }
        }

        public String FileType
        {
            get { return Path.GetExtension(Filename).WBxTrim().ToLower().Replace(".", ""); }   // Use of WBxTrim is mostly to change any null into a "" 
        }

        public String ProtectiveZone
        {
            get { return this[WBColumn.ProtectiveZone].WBxToString(); }
            set { this[WBColumn.ProtectiveZone] = value; }
        }

        public String LiveOrArchived
        {
            get { return this[WBColumn.LiveOrArchived].WBxToString(); }
            set { this[WBColumn.LiveOrArchived] = value; }
        }

        public String RecordID
        {
            get {
                Object gotValue = this[WBColumn.RecordID];
                // WBLogging.Debug("Got value back for RecordID as: " + gotValue);

                String asStringValue = gotValue.WBxToString();
                //WBLogging.Debug("Got value back for RecordID ToString as: " + asStringValue);

                return asStringValue; 
            }
            set { this[WBColumn.RecordID] = value; }
        }

        public String RecordSeriesID
        {
            get { 
                String recordSeriesID = this[WBColumn.RecordSeriesID].WBxToString();

                if (String.IsNullOrEmpty(recordSeriesID))
                {
                    recordSeriesID = RecordID;
                    SetupNewRecordSeriesColumns();
                }

                return recordSeriesID;
            }
            set { this[WBColumn.RecordSeriesID] = value; }
        }

        public bool CheckAndFixMetadataForRecord()
        {
            if (this.IsNullOrEmpty(WBColumn.RecordSeriesID) && !this.IsSPListItemVersion)
            {
                SetupNewRecordSeriesColumns();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void SetupNewRecordSeriesColumns()
        {
            // We can't change the underlying values if this is a list item version:
            if (this.IsSPListItemVersion) return;

            // OK so this record hasn't been set up with the latest metadata model so:
            ReplacesRecordID = null;
            RecordSeriesID = RecordID;
            RecordSeriesIssue = "1";

            if (LiveOrArchived == WBColumn.LIVE_OR_ARCHIVED__ARCHIVED)
            {
                this[WBColumn.RecordSeriesStatus] = WBColumn.RECORD_SERIES_STATUS__ARCHIVED;
            }
            else
            {
                this[WBColumn.RecordSeriesStatus] = WBColumn.RECORD_SERIES_STATUS__LATEST;
            }

            if (!HasDatePublished)
            {
                if (HasDeclaredRecord)
                {
                    DatePublished = DeclaredRecord;
                }
                else
                {
                    if (HasModified)
                    {
                        DatePublished = Modified;
                        this[WBColumn.ReviewDate] = DatePublished.AddYears(2);
                    }
                }
            }

            if (HasDatePublished && !HasReviewDate)
            {
                ReviewDate = DatePublished.AddYears(2);
            }

            if (this.IsNullOrEmpty(WBColumn.PublishedBy))
            {
                this[WBColumn.PublishedBy] = this[WBColumn.Author];
            }
        }

        
        public String ReplacesRecordID
        {
            get { return this[WBColumn.ReplacesRecordID].WBxToString(); }
            set { this[WBColumn.ReplacesRecordID] = value; }
        }

        public String RecordSeriesIssue
        {
            get { return this[WBColumn.RecordSeriesIssue].WBxToString(); }
            set { this[WBColumn.RecordSeriesIssue] = value; }
        }

        public WBTeam OwningTeam
        {
            get 
            {
                Object value = this[WBColumn.OwningTeam];
                if (value == null) return null;
                return new WBTeam(TeamsTaxonomy, value.ToString());
            }

            set 
            { 
                this[WBColumn.OwningTeam] = value;
                CheckOwningTeamIsAlsoInvolved();
            }
        }

        public void CheckOwningTeamIsAlsoInvolved()
        {
            WBTermCollection<WBTeam> involvedTeams = this.InvolvedTeams;

            if (!involvedTeams.Contains(OwningTeam))
            {
                involvedTeams.Add(OwningTeam);
            }

            // It's a little inefficient but we'll just call the Proerty set:
            this.InvolvedTeams = involvedTeams;
        }

        public WBTermCollection<WBTeam> InvolvedTeams
        {
            get {
                Object value = this[WBColumn.InvolvedTeams];
                if (value == null) return new WBTermCollection<WBTeam>(TeamsTaxonomy, ""); ;
                return new WBTermCollection<WBTeam>(TeamsTaxonomy, value.ToString());
            }
            set
            {
                value.Add(OwningTeam);
                this[WBColumn.InvolvedTeams] = value;
            }
        }

        public WBTermCollection<WBTeam> InvolvedTeamsWithoutOwningTeam
        {
            get
            {
                WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(InvolvedTeams);
                involvedTeams.Remove(OwningTeam);

                return involvedTeams;
            }
            set
            {
                // The set of this property adds the owning team already:
                InvolvedTeams = value;
            }
        }


        public String InvolvedTeamsWithoutOwningTeamAsUIControlValue
        {
            get
            {
                return InvolvedTeamsWithoutOwningTeam.UIControlValue;
            }
            set
            {
                WBTermCollection<WBTeam> involvedTeams = new WBTermCollection<WBTeam>(TeamsTaxonomy, value);
                // The set of this property adds the owning team already:
                InvolvedTeams = involvedTeams; 
            }
        }



        public Stream OpenBinaryStream()
        {
            if (!IsSPListItem) throw new Exception("You can only call WBDocument.OpenBinaryStream() on an SPListItem backed WBDocument");

            SPFile file = Item.File;
            if (file == null) throw new Exception("The SPFile of the SPListItem was null");

            SPDocumentLibrary library = file.DocumentLibrary;

            if (library.EnableVersioning)
            {
                SPListItemVersionCollection versionCollection = Item.Versions;
                SPListItemVersion version = versionCollection[0];

                file = version.ListItem.File;
            }

            return file.OpenBinaryStream();
        }

        public bool MaybeUpdateRecordColumns(String callingUserLogin, WBDocument documentToCopy, IEnumerable<WBColumn> columnsToCopy, String reasonForUpdate)
        {
            WBLogging.RecordsTypes.Verbose("In WBDocument.MaybeUpdateRecordColumns() for " + DebugName);

            bool updateRequired = false;
            foreach (WBColumn column in columnsToCopy)
            {
                if (Item.WBxGet(column) != documentToCopy[column])
                {
                    updateRequired = true;
                }
            }

            if (updateRequired)
            {
                WBLogging.RecordsTypes.Verbose("In WBDocument.MaybeUpdateRecordColumns() An update is required.");
                UpdateRecordColumns(callingUserLogin, documentToCopy, columnsToCopy, reasonForUpdate);
            }
            else
            {
                WBLogging.RecordsTypes.Verbose("In WBDocument.MaybeUpdateRecordColumns() There is no need for an update.");
            }

            return updateRequired;
        }


        public void UpdateRecordColumns(String callingUserLogin, WBDocument documentToCopy, IEnumerable<WBColumn> columnsToCopy, String reasonForUpdate)
        {
            // WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns() running as: " + Web.CurrentUser.Name);
            WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns() SPSite system account: " + Site.SystemAccount.Name);

            WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns() for " + DebugName);

            bool previousSetting = RecordsLibrary.Web.AllowUnsafeUpdates;
            RecordsLibrary.Web.AllowUnsafeUpdates = true;

            WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns() About to bypass locks on " + DebugName);
            Records.BypassLocks(this.Item, delegate(SPListItem item)
            {
                WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns() inside BypassLocks() for " + DebugName);
                if (item.File.CheckOutType != SPFile.SPCheckOutType.None)
                {
                    WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns() Somehow the record being updated (Record ID = " + this.RecordID + ") was checked out to: " + item.File.CheckedOutByUser.LoginName);
                    item.File.UndoCheckOut();
                }

                WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): Checkingout the record copy");
                item.File.CheckOut();
                WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): done check out for " + DebugName);

                foreach (WBColumn column in columnsToCopy)
                {
                    if (Item.WBxGet(column) != documentToCopy[column])
                    {
                        item.WBxSet(column, documentToCopy[column]);
                    }
                }
                WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): Finished setting all of the columns that needed to be set");
               
                SPUser callingUser = item.Web.WBxEnsureUserOrNull(callingUserLogin);

                if (callingUserLogin != null)
                {
                    WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): Updating with callingUserLogin = " + callingUserLogin + " and callingUser = " + callingUser.Name);
                    item.WBxSet(WBColumn.ModifiedBy, callingUserLogin);
                    item.WBxSet(WBColumn.Modified, DateTime.Now);
                }
                else
                {
                    WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): Updating withtout a calling user (callingUserLogin = " + callingUserLogin + ")");
                }

                item.Update();
                WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): Done the actual item.Update()");
                item.File.WBxCheckInAs(reasonForUpdate, callingUser);
                WBLogging.RecordsTypes.Verbose("In WBDocument.UpdateRecordColumns(): Done the check in with user from callingUserLogin = " + callingUserLogin);                
            });

            RecordsLibrary.Web.AllowUnsafeUpdates = previousSetting;
        }

    }
}
