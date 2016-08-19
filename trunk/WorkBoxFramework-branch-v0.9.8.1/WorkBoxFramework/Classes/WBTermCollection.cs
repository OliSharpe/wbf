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
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class WBTermCollection<T> : IEnumerable<T> where T : WBTerm, new()
    {
        #region Constructors


        public WBTermCollection(WBTaxonomy taxonomy, String UIControlValue)
        {
            _taxonomy = taxonomy;
            _UIControlValue = UIControlValue;
            _list = null;
        }

        public WBTermCollection(WBTaxonomy taxonomy, List<T> collection)
        {
            _taxonomy = taxonomy;
            _list = collection;
            _UIControlValue = null; 
        }

        public WBTermCollection(WBTermCollection<T> collectionToCopy)
        {
            _taxonomy = collectionToCopy.Taxonomy;
            _UIControlValue = null;

            _list = new List<T>(collectionToCopy.List);
        }

        public WBTermCollection(WBTaxonomy taxonomy, T firstTerm)
        {
            _taxonomy = taxonomy;
            _list = new List<T>();
            _list.Add(firstTerm);
            _UIControlValue = null;
        }

        
        #endregion


        #region Properties

        private WBTaxonomy _taxonomy = null;
        public WBTaxonomy Taxonomy { get { return _taxonomy; } }

        private string _UIControlValue = null;
        public String UIControlValue
        {
            get
            {
                if (String.IsNullOrEmpty(_UIControlValue))
                {

                    if (_list == null || _list.Count == 0)
                    {
                        _UIControlValue = "";
                    }
                    else
                    {
                        List<String> parts = new List<String>();
                        foreach (T wbTerm in _list)
                        {
                            parts.Add(wbTerm.UIControlValue);
                        }
                        _UIControlValue = string.Join(";", parts.ToArray());
                    }
                }
                return _UIControlValue;
            }
        }

        public T this[int index]
        {
            get
            {
                return List[index];
            }
        }

        public int Count { get { return List.Count; } }



        private List<T> _list = null;
        private List<T> List
        {
            get
            {
                if (_list == null) makeList();
                return _list;
            }
        }

        private void makeList()
        {
            _list = new List<T>();
            if (_UIControlValue == null || _UIControlValue == "") return;

            string[] values = _UIControlValue.Split(';');
            foreach (string value in values)
            {
                T term = new T();
                term.Initialise(_taxonomy, value);
                _list.Add(term);
            }
        }

        #endregion

        #region Public Methods

        public void Add(WBTermCollection<T> collection)
        {
            foreach (T term in collection)
            {
                Add(term);
            }
        }

        public void Add(T term)
        {
            if (!List.Contains(term))
            {
                List.Add(term);
                _UIControlValue = null;
            }
        }

        public void Remove(T term)
        {
            if (List.Contains(term))
            {
                List.Remove(term);
                _UIControlValue = null;
            }
        }

        public void Remove(WBTermCollection<T> collection)
        {
            foreach (T term in collection)
            {
                Remove(term);
            }
        }

        public override String ToString()
        {
            return UIControlValue;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(WBTerm term)
        {
            string termGUIDString = term.Id.WBxToString();

            if (termGUIDString == "") return false;

            return UIControlValue.Contains(termGUIDString);
        }

        public String Names()
        {
            List<String> names = new List<String>();
            foreach (T term in List)
            {
                names.Add(term.Name);
            }

            return String.Join("; ", names.ToArray());
        }

        #endregion
    }
}
