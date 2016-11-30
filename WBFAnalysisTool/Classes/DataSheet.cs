using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using System.Reflection;
using System.Windows.Forms;

namespace WBFAnalysisTool
{
    public class DataSheet<T> : IEnumerable<T> where T : DataSheetType, new()
    {
        private List<T> _list = new List<T>();
        private Dictionary<String, List<T>> _dictionary = new Dictionary<String, List<T>>();
        private Excel.Workbook _workbook = null;

        public Excel.Worksheet Worksheet { get; private set; }
        public T TitleRow { get; private set; }

        public int NextFreeColumnIndex = 1;
        public Dictionary<String, ExcelColumn> Columns { get; private set; }

        public bool IsReadOnly = false;
        public bool IsWriteOnly = false;
        
        public DataSheet(Excel.Workbook workbook, String sheetName, bool writeOnly)
        {
            _workbook = workbook;
            IsWriteOnly = writeOnly;

            if (!_workbook.xSheetExists(sheetName))
            {
                Worksheet = (Excel.Worksheet)_workbook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                Worksheet.Name = sheetName;
            }
            else
            {
                Worksheet = (Excel.Worksheet)_workbook.Worksheets[sheetName];
            }

            TitleRow = new T();
            Columns = new Dictionary<String, ExcelColumn>();

            if (IsWriteOnly)
            {
                ClearContents();
                FormatSheet();
            }
            else LoadFromSheet();
        }

        public int Count { get { return _list.Count; } }

        public ExcelColumn AddColumn(ExcelColumn column)
        {
            Columns.Add(column.Title, column);
            if (column.Index >= NextFreeColumnIndex) NextFreeColumnIndex = column.Index + 1;
            return column;
        }

        public ExcelColumn AddDateColumn()
        {
            return AddDateColumn("Date");
        }

        public ExcelColumn AddDateColumn(String title)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.Date, title, 12));
        }

        public ExcelColumn AddTextColumn(String title)
        {
            return AddTextColumn(title, 20);
        }

        public ExcelColumn AddTextColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.Text, title, width));
        }

        public ExcelColumn AddGBPColumn(String title)
        {
            return AddGBPColumn(title, 12);
        }

        public ExcelColumn AddGBPColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.GBP, title, width));
        }

        public ExcelColumn AddGBPFormulaColumn(String title)
        {
            return AddGBPFormulaColumn(title, 12);
        }

        public ExcelColumn AddGBPFormulaColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.GBPFormula, title, width));
        }

        public ExcelColumn AddPercentageColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.Percentage, title, width));
        }

        public ExcelColumn AddPercentageFormulaColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.PercentageFormula, title, width));
        }

        public ExcelColumn AddIntegerColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.Integer, title, width));
        }

        public ExcelColumn AddLongColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.Long, title, width));
        }

        public ExcelColumn AddFileSizeMBColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.FileSizeMB, title, width));
        }

        public ExcelColumn AddFileSizeGBColumn(String title, int width)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.FileSizeGB, title, width));
        }

        public ExcelColumn AddBooleanColumn(String title)
        {
            return AddColumn(new ExcelColumn(Worksheet, NextFreeColumnIndex, ExcelColumn.DataType.Boolean, title, 10));
        }


        /* Not sure if I'm going to use this double indexer - so avoiding it for the moment.
        public Object this[String title, int rowIndex]
        {
            get
            {
                if (Columns.ContainsKey(title))
                {
                    ExcelColumn column = Columns[title];
                    return column.Get(rowIndex);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (Columns.ContainsKey(title))
                {
                    ExcelColumn column = Columns[title];
                    column.Set(rowIndex, value);
                }
            }
        }
        */

        public void LoadFromSheet()
        {
            for (int rowIndex = 2; rowIndex < Worksheet.Rows.Count; rowIndex++)
            {
                T dataEntry = new T();
                if (Columns.Count == 0) dataEntry.ConfigureDataSheet(this);

                // Don't try to load a blank row (as indicated by having no date value in column A:
                if (Worksheet.xCellHasNoValue(rowIndex, "A"))
                {
                    // MessageBox.Show("Found no cell value in column A for rowIndex = " + rowIndex);
                    break;
                }
                try
                {
                    dataEntry.LoadFromRow(this, rowIndex);
                    dataEntry.LoadedOK = true;
                }
                catch (Exception e)
                {
                    dataEntry.LoadedOK = false;
                }

                dataEntry.DataSheet = (Object)this;
                dataEntry.RowIndex = rowIndex;

                Add(dataEntry);
            }
        }

        public void FormatSheet()
        {
            if (IsReadOnly) return;

            Worksheet.xFormatTitleRow();

            // If there are no columns set then this DataSheet hasn't been formatted yet so:
            if (Columns.Count == 0)
            {
                TitleRow.ConfigureDataSheet(this);
            }

            foreach (ExcelColumn column in Columns.Values)
            {
                column.Format();
            }
        }

        public void SaveToSheet()
        {
            if (IsReadOnly) return;

            FormatSheet();

            int rowIndex = 2;
            foreach (T dataEntry in _list)
            {
                dataEntry.SaveToRow(this, rowIndex);
                dataEntry.RowIndex = rowIndex;
                rowIndex++;
            }
        }

        public bool ContainsKey(String key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(T dataEntry)
        {
            dataEntry.RowIndex = _list.Count + 2;
            _list.Add(dataEntry);

            List<T> entriesWithKey = null;
            if (_dictionary.ContainsKey(dataEntry.Key()))
            {
                entriesWithKey = _dictionary[dataEntry.Key()];
            }
            else
            {
                entriesWithKey = new List<T>();
                _dictionary[dataEntry.Key()] = entriesWithKey;
            }

            dataEntry.KeyListIndex = entriesWithKey.Count;
            entriesWithKey.Add(dataEntry);
        }

        public void AddIfNew(T dataEntry)
        {
            if (!_dictionary.ContainsKey(dataEntry.Key())) Add(dataEntry);
        }

        public T this[String key]
        {
            get
            {
                return Get(key, 0);
            }
        }

        public T Get(String key, int keyListIndex)
        {
            if (String.IsNullOrEmpty(key)) return null;

            if (!_dictionary.ContainsKey(key)) return null;
            List<T> keyList = _dictionary[key];

            if (keyList == null) return null;
            if (keyList.Count <= keyListIndex) return null;
            return keyList[keyListIndex];
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T dataEntry in _list)
            {
                yield return dataEntry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Sort()
        {
            _list.Sort();

            // Now update the RowIndex value of each of the data entries:
            int rowIndex = 2;
            foreach (T dataEntry in _list)
            {
                dataEntry.RowIndex = rowIndex;
                rowIndex++;
            }

        }

        public void ClearContents()
        {
            Worksheet.Cells.ClearContents();
            _list.Clear();
            _dictionary.Clear();
            Columns.Clear();
        }
    }
}
