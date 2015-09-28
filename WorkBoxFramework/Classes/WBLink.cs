#region Copyright and License

// Copyright (c) Islington Council 2010-2015
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

namespace WorkBoxFramework
{
    public class WBLink
    {
        public bool IsOK = true;

        public String Title;
        public String URL;
        public String UniqueID;
        public String SPWebGUID;

        public bool UsingTicksWhenVisited = false;
        public String TicksWhenVisitedString;

        public WBLink(WorkBox workbox, bool visitingNow)
        {
            Title = workbox.Title;
            URL = workbox.Url;
            UniqueID = workbox.UniqueID;
            SPWebGUID = workbox.Web.ID.ToString();

            if (visitingNow) TicksWhenVisitedString = DateTime.Now.Ticks.ToString();
            UsingTicksWhenVisited = visitingNow;
        }

        public WBLink(String values)
        {
            if (String.IsNullOrEmpty(values))
            {
                IsOK = false;
                WBLogging.Debug("WBLink being created with blank or null values string:" + values);
                return;
            }

            String[] valueArray = values.Split('|');
            if (valueArray.Length == 5)
            {
                UsingTicksWhenVisited = true;
            }
            else if (valueArray.Length == 4)
            {
                UsingTicksWhenVisited = false;
            }
            else
            {
                IsOK = false;
                WBLogging.Debug("WBLink being created with values string with the wrong number of values: " + values);
                return;
            }

            Title = WBUtils.PutBackDelimiterCharacters(valueArray[0]);
            URL = WBUtils.PutBackDelimiterCharacters(valueArray[1]);
            UniqueID = WBUtils.PutBackDelimiterCharacters(valueArray[2]);
            SPWebGUID = valueArray[3];

            if (UsingTicksWhenVisited)
            {
                TicksWhenVisitedString = valueArray[4];
            }
        }

        public override string ToString()
        {
            List<String> values = new List<String>();
            values.Add(WBUtils.ReplaceDelimiterCharacters(this.Title));
            values.Add(WBUtils.ReplaceDelimiterCharacters(this.URL));
            values.Add(WBUtils.ReplaceDelimiterCharacters(this.UniqueID));
            values.Add(this.SPWebGUID);
            if (UsingTicksWhenVisited)
            {
                values.Add(TicksWhenVisitedString);
            }
            return String.Join("|", values.ToArray());
        }

    }
}
