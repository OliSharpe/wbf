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
