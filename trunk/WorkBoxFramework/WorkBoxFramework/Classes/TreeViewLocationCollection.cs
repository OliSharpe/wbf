#region Copyright and License

// Copyright (c) Islington Council 2010-2016
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
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace WorkBoxFramework
{
    public class TreeViewLocationCollection : List<TreeViewLocation>, IHierarchicalEnumerable
    {
        public TreeViewLocationCollection()
            : base()
        {
        }

        public TreeViewLocationCollection(WBRecordsManager manager, string mode, string minimumProtectiveZone, WBTerm functionalArea)
            : base()
        {
            Add(new TreeViewLocation(null, manager, mode, minimumProtectiveZone, functionalArea));
        }

        public TreeViewLocationCollection(WBRecordsManager manager, string mode, string minimumProtectiveZone, WBSubjectTag subjectTag)
            : base()
        {
            Add(new TreeViewLocation(null, manager, mode, minimumProtectiveZone, subjectTag));
        }

        public TreeViewLocationCollection(WBRecordsManager manager, string mode, string minimumProtectiveZone, SPFolder folder)
            : base()
        {
            Add(new TreeViewLocation(null, manager, mode, minimumProtectiveZone, folder));
        }


        #region IHierarchicalEnumerable Members

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }

        #endregion

    }
}
