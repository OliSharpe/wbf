using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkBoxFramework
{
    public class WBSubjectTagRecordsRoutings
    {
        public bool IsOK = true;

        public WBSubjectTag SubjectTag;
        public String PublicDocumentsLibrary {get; set;}
        public String ExtranetDocumentsLibrary {get; set;}


        public WBSubjectTagRecordsRoutings(WBTaxonomy subjectTags, String subjectTagUIControlValue, String publicLibrary, String extranetLibrary)
        {
            SubjectTag = new WBSubjectTag(subjectTags, subjectTagUIControlValue);
            PublicDocumentsLibrary = publicLibrary;
            ExtranetDocumentsLibrary = extranetLibrary;
        }


        public WBSubjectTagRecordsRoutings(WBTaxonomy subjectTags, String values)
        {
            if (String.IsNullOrEmpty(values))
            {
                IsOK = false;
                WBLogging.Debug("WBSubjectTagRecordsRoutings being created with blank or null values string:" + values);
                return;
            }

            String[] valueArray = values.Split('|');
            if (valueArray.Length == 3)
            {
                SubjectTag = new WBSubjectTag(subjectTags, WBUtils.PutBackDelimiterCharacters(valueArray[0])); 
                PublicDocumentsLibrary = WBUtils.PutBackDelimiterCharacters(valueArray[1]);
                ExtranetDocumentsLibrary = WBUtils.PutBackDelimiterCharacters(valueArray[2]);
            }
            else
            {
                IsOK = false;
                WBLogging.Debug("WBSubjectTagRecordsRoutings being created with values string with the wrong number of values: " + values);
                return;
            }


        }

        public override string ToString()
        {
            List<String> values = new List<String>();
            values.Add(WBUtils.ReplaceDelimiterCharacters(this.SubjectTag.UIControlValue));
            values.Add(WBUtils.ReplaceDelimiterCharacters(this.PublicDocumentsLibrary));
            values.Add(WBUtils.ReplaceDelimiterCharacters(this.ExtranetDocumentsLibrary));
            return String.Join("|", values.ToArray());
        }

    }
}
