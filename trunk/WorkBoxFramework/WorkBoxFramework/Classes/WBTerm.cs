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
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class WBTerm
    {
        #region Constructors

        public WBTerm()
        {
            _taxonomy = null;
            _term = null;
            _UIControlValue = null;
        }

        public WBTerm(WBTaxonomy taxonomy, Term term)
        {
            Initialise(taxonomy, term);
        }

        public WBTerm(WBTaxonomy taxonomy, Guid guid)
        {
            Initialise(taxonomy, taxonomy.TermSet.GetTerm(guid));
        }


        public WBTerm(WBTaxonomy taxonomy, String UIControlValue)
        {
            Initialise(taxonomy, UIControlValue);
        }

        #endregion


        #region Properties

        protected Term _term = null;
        public Term Term 
        { 
            get
            {
                if (_term == null) 
                {
                    _term = _taxonomy.TermSet.GetTerm(this.Id);
                }
                return _term; 
            } 
        }

        public bool TermNotResolvedYet { get { return _term == null; } } 

        protected WBTaxonomy _taxonomy = null;
        public WBTaxonomy Taxonomy { 
            get 
            { 
                return _taxonomy; 
            }
            set
            {
                if (_taxonomy == null)
                {
                    _taxonomy = value;
                }
            }            
        }

        public bool JustUIControlValue { get { return (_taxonomy == null && _term == null && !String.IsNullOrEmpty(_UIControlValue)); } }      

        protected Guid? _id = null;
        public Guid Id
        {
            get
            {
                if (!_id.HasValue)
                {
                    setNameAndId();
                }
                return _id.Value;
            }
        }

        public bool IsAvailableForTegging { get { return Term.IsAvailableForTagging; } }

        protected string _name = null;
        public String Name 
        { 
            get
            {
                if (_name == null || _name == "")
                {
                    setNameAndId();
                }
                return _name;
            }

            set
            {
                Term.Name = value;
            }

        }

        protected void setNameAndId()
        {
            if (_term == null) 
            {
                if (_UIControlValue != null && _UIControlValue != "")
                {
                    string [] parts = _UIControlValue.Split('|');
                    if (parts.Length != 2) throw new Exception("The UIControlValue for this term was badly formed: " + _UIControlValue);
                    _name = parts[0];
                    _id = new Guid(parts[1]);
                }
            }
            else
            {
                _name = _term.Name;
                _id = _term.Id;
            }
        }

        public String Description
        {
            get { return Term.GetDescription(WorkBox.LOCALE_ID_ENGLISH).WBxTrim(); }
            set { Term.SetDescription(value, WorkBox.LOCALE_ID_ENGLISH); }
        }

        protected string _UIControlValue;
        public String UIControlValue
        {
            get
            {
                if (_UIControlValue == null || _UIControlValue == "")
                {
                    if (_term == null)
                    {
                        _UIControlValue = string.Format("{0}|{1}", _name, _id.ToString());
                    }
                    else
                    {
                        _UIControlValue = _term.WBxUIControlValue();
                    }
                }
                return _UIControlValue;
            }
        }

        

        private string _fullPath = null;
        public String FullPath
        {
            get
            {
                if (_fullPath == null)
                {
                    _fullPath = WBUtils.NormalisePath(Term.WBxFullPath());
                }
                return _fullPath;
            }
        }

        #endregion

        #region Methods

       
        public void Initialise(WBTaxonomy taxonomy, String UIControlValue)
        {
            if (_taxonomy == null)
            {
                if (UIControlValue == null || UIControlValue == "") throw new Exception("You cannot create a WBTerm (or derivative) with a null or blank UIControlValue");

                _taxonomy = taxonomy;
                _UIControlValue = UIControlValue;

                _term = null;
            }
            else
            {
                WBUtils.shouldThrowError("You should never call this method on an initialised WBTerm or derived class");
            }
        }


        public void Initialise(WBTaxonomy taxonomy, String name, Guid id)
        {
            if (_taxonomy == null)
            {
                if (name == null || name == "") throw new Exception("You cannot create a WBTerm (or derivative) with a null or blank name");

                WBLogging.Generic.Verbose("Initialising a term with name | id : " + name + " | " + id);

                _taxonomy = taxonomy;
                _name = name;
                _id = id;

                _UIControlValue = null;
                _term = null;

                WBLogging.Generic.Verbose("Right now the UIControlValue comes back as: " + UIControlValue);

            }
            else
            {
                WBUtils.shouldThrowError("You should never call this method on an initialised WBTerm or derived class");
            }
        }


        public void Initialise(WBTaxonomy taxonomy)
        {
            if (_taxonomy == null && _term == null && !String.IsNullOrEmpty(_UIControlValue))
            {
                _taxonomy = taxonomy;
            }
            else
            {
                WBUtils.shouldThrowError("You can only initialise just the taxonomy if the term and taxonomy are null by the UIControlValue is not empty");
            }
        }

        public void Initialise(WBTaxonomy taxonomy, Term term)
        {
            if (_taxonomy == null)
            {
                if (term == null) throw new Exception("You cannot create a WBTerm (or derivative) with a null Term object");

                _taxonomy = taxonomy;
                _term = term;

                _UIControlValue = "";
            }
            else
            {
                WBUtils.shouldThrowError("You should never call this method on an initialised WBTerm or derived class");
            }
        }

        public virtual void Update()
        {
            WBLogging.Generic.Verbose("Calling commit all on the taxonomy");
            Taxonomy.CommitAll();
        }

        public String ObjectStatus()
        {
            string status = "Status of a WBTerm:\n";
            if (_term == null) 
            {
                status += "_term == null";
            } 
            else 
            {
                status += "_term = " + _term.Name + " with Id = " + _term.Id;
            }
            status += "\n";

            if (_id == null) 
            {
                status += "_id == null";
            } 
            else 
            {
                status += "_id = " + _id.ToString();
            }
            status += "\n";

            if (_UIControlValue == null)
            {
                status += "_UIControlValue == null";
            }
            else
            {
                status += "_UIControlValue = " + _UIControlValue;
            }
            status += "\n";

            if (_taxonomy == null)
            {
                status += "_taxonomy == null";
            }
            else
            {
                status += "_taxonomy = " + _taxonomy.TermSet.Name;
            }
            status += "\n";

            return status;
        }

        #endregion

        #region Implementation for equalities checks:

        public bool Equals(WBTerm other)
        {
            if (other == null)
                return false;

            if (this.Id == other.Id)
                return true;
            else
                return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            WBTerm term = obj as WBTerm;
            if (term == null)
                return false;
            else
                return Equals(term);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(WBTerm term1, WBTerm term2)
        {
            if ((object)term1 == null || ((object)term2) == null)
                return Object.Equals(term1, term2);

            return term1.Equals(term2);
        }

        public static bool operator !=(WBTerm term1, WBTerm term2)
        {
            if (term1 == null || term2 == null)
                return !Object.Equals(term1, term2);

            return !(term1.Equals(term2));
        }

        public override string ToString()
        {
            return this.UIControlValue;
        }

        #endregion

    }

}
