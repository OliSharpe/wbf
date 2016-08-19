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
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class WBDocument : WBItem
    {

        #region Constructors
        public WBDocument(SPListItem item) : base(item)
        {
        }

        public WBDocument() : base()
        {
        }
        #endregion


        public WBRecordsType RecordsType
        {
            get 
            {
                Object value = this[WBColumn.RecordsType];
                if (value == null) return null;
                return new WBRecordsType(null, value.ToString());
            }
            set { this[WBColumn.RecordsType] = value; }
        }

        public WBTermCollection<WBTerm> FunctionalArea
        {
            get 
            {
                Object value = this[WBColumn.FunctionalArea];
                if (value == null) return null;
                return new WBTermCollection<WBTerm>(null, value.ToString());
            }
            set { this[WBColumn.FunctionalArea] = value; }
        }

        public WBTermCollection<WBTerm> SubjectTags
        {
            get
            {
                Object value = this[WBColumn.SubjectTags];
                if (value == null) return null;
                return new WBTermCollection<WBTerm>(null, value.ToString());
            }
            set { this[WBColumn.SubjectTags] = value; }
        }

        public WBTerm SeriesTag
        {
            get
            {
                Object value = this[WBColumn.SeriesTag];
                if (value == null) return null;
                return new WBTerm(null, value.ToString());
            }
            set { this[WBColumn.SeriesTag] = value; }
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
            get { return this[WBColumn.ReferenceID] as String; }
            set { this[WBColumn.ReferenceID] = value; }
        }

        public String OriginalFilename
        {
            get { return this[WBColumn.OriginalFilename] as String; }
            set { this[WBColumn.OriginalFilename] = value; }
        }

        public String Name
        {
            get { return this[WBColumn.Name] as String; }
            set { this[WBColumn.Name] = value; }
        }

        public String Title
        {
            get { return this[WBColumn.Title] as String; }
            set { this[WBColumn.Title] = value; }
        }

        public String Filename
        {
            get { return this[WBColumn.Name] as String; }
            set { this[WBColumn.Name] = value; }
        }

        public String ProtectiveZone
        {
            get { return this[WBColumn.ProtectiveZone] as String; }
            set { this[WBColumn.ProtectiveZone] = value; }
        }


        public WBTeam OwningTeam
        {
            get 
            {
                Object value = this[WBColumn.OwningTeam];
                if (value == null) return null;
                return new WBTeam(null, value.ToString());
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

            involvedTeams.Add(OwningTeam);

            // It's a little inefficient but we'll just call the Proerty set:
            this.InvolvedTeams = involvedTeams;
        }

        public WBTermCollection<WBTeam> InvolvedTeams
        {
            get {
                Object value = this[WBColumn.InvolvedTeams];
                if (value == null) return new WBTermCollection<WBTeam>(null, ""); ;
                return new WBTermCollection<WBTeam>(null, value.ToString());
            }
            set
            {
                value.Add(OwningTeam);
                this[WBColumn.InvolvedTeams] = value;
            }
        }

        public Stream BinaryStream { get; set; }

    }
}
