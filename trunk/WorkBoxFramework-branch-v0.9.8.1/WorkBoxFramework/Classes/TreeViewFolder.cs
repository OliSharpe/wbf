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
using Microsoft.SharePoint;

namespace WorkBoxFramework
{
    public class TreeViewFolder : IHierarchyData
    {
        private TreeViewFolder _parent;
        private TreeViewFolderCollection _children;
        private SPFolder _folder;
        private String _name;

        private TreeViewFolder()
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="term">The underlying SPFolder object that is being wrapped</param>
        public TreeViewFolder(TreeViewFolder parent, SPFolder folder)
        {
            _parent = parent;
            _folder = folder;
        }

        public TreeViewFolder(SPFolder folder)
        {
            _parent = null;
            _folder = folder;
        }

        public TreeViewFolder(SPFolder folder, String name)
        {
            _parent = null;
            _folder = folder;
            _name = name;
        }



        #region IHierarchyData Members

        public IHierarchicalEnumerable GetChildren()
        {
            if (_children == null)
            {
                _children = new TreeViewFolderCollection();

                foreach (SPFolder child in _folder.SubFolders)
                {
                    // For the moment we're assuming that we should hide the Forms folder in root of doc lib:                   
                    if (_parent == null && child.Name == "Forms") continue;

                    _children.Add(new TreeViewFolder(this, child));
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
                TreeViewFolderCollection children = GetChildren() as TreeViewFolderCollection;
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
                

                return parentsPath + "/" + this.ToString(); 
            
            }
        }

        public string Type
        {
            get { return this.GetType().ToString(); }
        }

        #endregion

        public override string ToString()
        {
            if (_name != null) return _name;
            return _folder.Name;
        }

    }
}
