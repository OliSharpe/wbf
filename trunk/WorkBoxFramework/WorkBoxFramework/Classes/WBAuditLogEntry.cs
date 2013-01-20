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
// The Work Box Framework is distributed in the hope that it will be 
// useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;


namespace WorkBoxFramework
{

    public class WBAuditLogEntry
    {
        private const int DATE_TIME = 0;
        private const int USER_LOGIN_NAME = 1;
        private const int TITLE = 2;
        private const int COMMENT = 3;

        private const int NUM_PARTS = 4;

        private String[] _logEntryParts;


        public WBAuditLogEntry(String unsplitString)
        {
            _logEntryParts = unsplitString.Split('|');

            if (_logEntryParts == null || _logEntryParts.Length != NUM_PARTS)
            {
                _logEntryParts = new String[NUM_PARTS];

                unsplitString = unsplitString.Replace(";", "<<SEMI-COLON>>");
                unsplitString = unsplitString.Replace("|", "<<PIPE_CHARACTER>>");

                _logEntryParts[DATE_TIME] = DateTime.Now.WBxToString();
                _logEntryParts[USER_LOGIN_NAME] = "<<NONE>>";
                _logEntryParts[TITLE] = "<<FORMAT ERROR>>";
                _logEntryParts[COMMENT] = unsplitString;
            }
        }

        public WBAuditLogEntry(SPUser user, String title, String comment)
        {
            _logEntryParts = new String[NUM_PARTS];

            _logEntryParts[DATE_TIME] = DateTime.Now.WBxToString();

            if (user == null)
            {
                _logEntryParts[USER_LOGIN_NAME] = "<<NULL>>";    
            }
            else
            {
                _logEntryParts[USER_LOGIN_NAME] = user.LoginName;
            }


            if (title == null || title == "")
            {
                _logEntryParts[TITLE] = "<<NO TITLE GIVEN>>";
            }
            else
            {
                title = title.Replace(';', ' ');
                title = title.Replace('|', ' ');
                _logEntryParts[TITLE] = title.WBxTrim();
            }

            // This WBxTrim method extension will take care of comment being null as well:
            comment = comment.WBxTrim();
            comment = comment.Replace(';', ' ');
            comment = comment.Replace('|', ' ');
            _logEntryParts[COMMENT] = comment;
        }

        public override String ToString()
        {
            return String.Join("|", _logEntryParts);
        }

        public String DateTimeAsString { get { return _logEntryParts[DATE_TIME]; } }

        public String UserLoginName { get { return _logEntryParts[USER_LOGIN_NAME]; } }

        public String Title { get { return _logEntryParts[TITLE]; } }

        public String Comment { get { return _logEntryParts[COMMENT]; } }


        public static List<WBAuditLogEntry> CreateListOfEntries(String columnValue)
        {
            string[] entries = columnValue.Split(';');

            List<WBAuditLogEntry> list = new List<WBAuditLogEntry>();
            foreach (string entry in entries)
            {
                list.Add(new WBAuditLogEntry(entry));
            }

            return list;
        }
    }
}
