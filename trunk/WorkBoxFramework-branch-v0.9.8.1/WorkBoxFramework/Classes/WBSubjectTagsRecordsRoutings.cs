using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkBoxFramework
{
    public class WBSubjectTagsRecordsRoutings : IEnumerable<WBSubjectTagRecordsRoutings>
    {
        List<WBSubjectTagRecordsRoutings> Routings = new List<WBSubjectTagRecordsRoutings>();

        public WBSubjectTagsRecordsRoutings(WBTaxonomy subjectTags, String values)
        {
            String[] routingsStrings = values.Split(';');

            foreach (String routingString in routingsStrings)
            {
                WBSubjectTagRecordsRoutings routing = new WBSubjectTagRecordsRoutings(subjectTags, routingString);

                if (routing.IsOK)
                {
                    Routings.Add(routing);
                }
            }
        }

        public override string ToString()
        {
            List<String> values = new List<String>();
            foreach (WBSubjectTagRecordsRoutings routing in Routings)
            {
                values.Add(routing.ToString());
            }
            return String.Join(";", values.ToArray());
        }

        public void RemoveAtIndex(int index)
        {
            Routings.RemoveAt(index);
        }

        public int Count
        {
            get { return Routings.Count; }
        }

        public WBSubjectTagRecordsRoutings this[int index]
        {
            get
            {
                if (index < 0 || index >= Routings.Count) return null;

                return Routings[index];
            }

            set
            {
                Routings[index] = value;
            }
        }

        public void Add(WBSubjectTagRecordsRoutings newRouting)
        {
            Routings.Add(newRouting);
        }

        public List<String> PublicLibrariesToRouteTo(WBTermCollection<WBSubjectTag> subjectTagsToCheck)
        {
            List<String> publicLibraries = new List<String>();

            foreach (WBSubjectTag subjectTag in subjectTagsToCheck)
            {
                foreach (WBSubjectTagRecordsRoutings routing in Routings)
                {
                    if (!String.IsNullOrEmpty(routing.PublicDocumentsLibrary) 
                        && subjectTag.Term.GetIsDescendantOf(routing.SubjectTag.Term)
                        && !publicLibraries.Contains(routing.PublicDocumentsLibrary))
                    {
                        publicLibraries.Add(routing.PublicDocumentsLibrary);
                    }
                }
            }

            return publicLibraries;
        }

        public List<String> ExtranetLibrariesToRouteTo(WBTermCollection<WBSubjectTag> subjectTagsToCheck)
        {
            List<String> extranetLibraries = new List<String>();

            foreach (WBSubjectTag subjectTag in subjectTagsToCheck)
            {
                foreach (WBSubjectTagRecordsRoutings routing in Routings)
                {
                    if (!String.IsNullOrEmpty(routing.ExtranetDocumentsLibrary) 
                        && subjectTag.Term.GetIsDescendantOf(routing.SubjectTag.Term)
                        && !extranetLibraries.Contains(routing.ExtranetDocumentsLibrary))
                    {
                        extranetLibraries.Add(routing.ExtranetDocumentsLibrary);
                    }
                }
            }

            return extranetLibraries;
        }


        public List<String> AllPublicLibraries()
        {
            List<String> publicLibraries = new List<String>();
            foreach (WBSubjectTagRecordsRoutings routing in Routings)
            {
                if (!String.IsNullOrEmpty(routing.PublicDocumentsLibrary)
                    && !publicLibraries.Contains(routing.PublicDocumentsLibrary))
                {
                    publicLibraries.Add(routing.PublicDocumentsLibrary);
                }
            }

            return publicLibraries;
        }

        public List<String> AllExtranetLibraries()
        {
            List<String> extranetLibraries = new List<String>();

            foreach (WBSubjectTagRecordsRoutings routing in Routings)
            {
                if (!String.IsNullOrEmpty(routing.ExtranetDocumentsLibrary)
                    && !extranetLibraries.Contains(routing.ExtranetDocumentsLibrary))
                {
                    extranetLibraries.Add(routing.ExtranetDocumentsLibrary);
                }
            }

            return extranetLibraries;
        }


        IEnumerator<WBSubjectTagRecordsRoutings> IEnumerable<WBSubjectTagRecordsRoutings>.GetEnumerator()
        {
            return Routings.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Routings.GetEnumerator();
        }
    }
}
