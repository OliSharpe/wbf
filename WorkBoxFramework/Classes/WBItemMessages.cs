using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkBoxFramework
{
    public class WBItemMessages : Dictionary<WBColumn,String>
    {               
        
        public new String this[WBColumn column]
        {
            get
            {
                if (base.ContainsKey(column)) return base[column];
                return "";
            }

            set
            {
                string trimmed = value.WBxTrim();
                if (trimmed == "")
                {
                    if (base.ContainsKey(column)) base.Remove(column);
                }
                else
                {
                    base[column] = trimmed; 
                }
            }               
        }
    }
}
