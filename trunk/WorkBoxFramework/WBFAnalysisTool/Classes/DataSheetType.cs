using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;


namespace WBFAnalysisTool
{
    public abstract class DataSheetType : IComparable
    {
        public int RowIndex = -1; // A value to indicate that is hasn't been set yet
        public int ListIndex { get { return RowIndex - 2; } }
        public int KeyListIndex = 0;
        public Object DataSheet = null;

        public bool LoadedOK = false;

        public Dictionary<String, ExcelColumn> Columns = null;

        public DataSheetType()
        {
        }

        public virtual void ConfigureDataSheet<T>(DataSheet<T> datasheet) where T : DataSheetType, new()
        {
            // Add the columns for DataSheets of this type - by creating an 'row' with rowIndex = 1
        }

        public virtual void LoadFromRow<T>(DataSheet<T> datasheet, int rowIndex) where T : DataSheetType, new()
        {
        }

        public virtual void SaveToRow<T>(DataSheet<T> datasheet, int rowIndex) where T : DataSheetType, new()
        {
        }


        /// <summary>
        /// This method is for returning the cell name of the indicated cell to be used in creating formula strings
        /// </summary>
        /// <param name="title"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public String Cell(String title, int rowIndex)
        {
            if (Columns == null) return "<<Columns not setup yet>>";
            if (!Columns.ContainsKey(title)) return "<<Columns didn't contain title: " + title + ">>";
            return Columns[title].ToString() + rowIndex.ToString();
        }

        public virtual String Key()
        {
            return "";
        }


        public virtual int CompareTo(object obj)
        {
            if (obj is DataSheetType)
            {

                DataSheetType dataSheetType = (DataSheetType)obj;

                return Key().CompareTo(dataSheetType.Key());
            }
            else
                throw new ArgumentException("Object was not a DataSheetType");
        }
    }
}
