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
using System.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class TreeViewTerm : IHierarchyData
    {
        private Term _term;
        private string _guidString;
        private TreeViewTerm _parent;
        private TreeViewTermCollection _children;
        private string _name;

        /// <summary>
        /// Hide the default public constructor
        /// </summary>
        private TreeViewTerm()
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="term">The underlying Term object that is being wrapped</param>
        public TreeViewTerm(TreeViewTerm parent, Term term)
        {
            _parent = parent;
            _term = term;
            _name = term.GetDefaultLabel(1033);
            _guidString = term.Id.ToString();
        }


        public TreeViewTerm(Term rootTerm)
        {
            _parent = null;
            _term = rootTerm;
            _name = rootTerm.GetDefaultLabel(1033);
            _guidString = rootTerm.Id.ToString();
        }



        public TreeViewTerm(TermSet termSet)
        {
            _parent = null;
            _term = null;
            _name = termSet.Name;
            _guidString = "";

            _children = new TreeViewTermCollection();

            foreach (Term term in termSet.Terms)
            {
                if (term.IsAvailableForTagging)
                {
                    _children.Add(new TreeViewTerm(this, term));
                }
            }
        }


        #region IHierarchyData Members

        public IHierarchicalEnumerable GetChildren()
        {
            if (_children == null)
            {
                _children = new TreeViewTermCollection();

                TermCollection terms = _term.Terms;

                foreach (Term childTerm in terms)
                {
                    if (childTerm.IsAvailableForTagging)
                    {
                        _children.Add(new TreeViewTerm(this, childTerm));
                    }
                }
            }

            return _children;
        }

        public IHierarchyData GetParent()
        {
            return _parent;
        }

        public bool HasChildren
        {
            get
            {
                TreeViewTermCollection children = GetChildren() as TreeViewTermCollection;
                return children.Count > 0;
            }
        }

        public object Item
        {
            get { return this; }
        }

        public string Path
        {
            get {

                string parentsPath = ""; 
                if (_parent != null) parentsPath = _parent.Path;
                

                return parentsPath + "/" + _guidString; 
            
            }
        }

        public string Type
        {
            get { return this.GetType().ToString(); }
        }

        #endregion

        public override string ToString()
        {
            return _name;
        }

    }
}
