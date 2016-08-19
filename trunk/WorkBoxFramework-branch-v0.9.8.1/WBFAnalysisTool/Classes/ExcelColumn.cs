using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace WBFAnalysisTool
{
    /// <summary>
    /// This class is used to model an excel column
    /// </summary>
    public class ExcelColumn
    {
        public enum DataType
        {
            Date,
            Text,
            GBP,
            GBPFormula,
            Percentage,
            PercentageFormula,
            Integer,
            Long,
            FileSizeMB,
            FileSizeGB,
            Boolean
        }

        public Excel.Worksheet Worksheet { get; private set; }
        public int Index { get; private set; }
        public String Name { get; private set; }
        public DataType Type { get; private set; }
        public int Width { get; private set; }

        public String Title { get; set; }

        public ExcelColumn(Excel.Worksheet worksheet, int index, DataType type, String title, int width)
        {
            Worksheet = worksheet;
            Index = index;
            Name = index.xExcelColumnName();

            Type = type;
            Title = title;
            Width = width;
        }

        public void Format()
        {
            switch (Type)
            {
                case DataType.Date:
                    {
                        Worksheet.xFormatColumnAsDate(Name, Title);
                        break;
                    }
                case DataType.Text:
                    {
                        Worksheet.xFormatColumnAsText(Name, Title, Width);
                        break;
                    }
                case DataType.GBP:
                    {
                        Worksheet.xFormatColumnAsGBP(Name, Title, Width);
                        break;
                    }
                case DataType.GBPFormula:
                    {
                        Worksheet.xFormatColumnAsGBP(Name, Title, Width);
                        break;
                    }
                case DataType.Percentage:
                    {
                        Worksheet.xFormatColumnAsPercentage(Name, Title, Width);
                        break;
                    }
                case DataType.PercentageFormula:
                    {
                        Worksheet.xFormatColumnAsPercentage(Name, Title, Width);
                        break;
                    }
                case DataType.Integer:
                    {
                        Worksheet.xFormatColumnAsInteger(Name, Title, Width);
                        break;
                    }
                case DataType.Long:
                    {
                        Worksheet.xFormatColumnAsLong(Name, Title, Width);
                        break;
                    }
                case DataType.FileSizeMB:
                    {
                        Worksheet.xFormatColumnAsFileSizeMB(Name, Title, Width);
                        break;
                    }
                case DataType.FileSizeGB:
                    {
                        Worksheet.xFormatColumnAsFileSizeGB(Name, Title, Width);
                        break;
                    }
                case DataType.Boolean:
                    {
                        Worksheet.xFormatColumnAsText(Name, Title, Width);
                        break;
                    }
            }
        }

        public Object this[int rowIndex]
        {
            get { return Get(rowIndex); }
            set { Set(rowIndex, value); }
        }

        public bool HasValue(int rowIndex)
        {
            return Worksheet.xCellHasValue(rowIndex, Name);
        }

        public bool HasNoValue(int rowIndex)
        {
            return Worksheet.xCellHasNoValue(rowIndex, Name);
        }

        public void Set(int rowIndex, Object value)
        {
            if (value == null)
            {
                Worksheet.xSetCellAsString(rowIndex, Name, "");
                return;
            }

            switch (Type)
            {
                case DataType.Date:
                    {
                        Worksheet.xSetCellAsDateTime(rowIndex, Name, (DateTime)value);
                        break;
                    }
                case DataType.Text:
                    {
                        Worksheet.xSetCellAsString(rowIndex, Name, (String)value);
                        break;
                    }
                case DataType.GBP:
                    {
                        Worksheet.xSetCellAsDouble(rowIndex, Name, (double)value);
                        break;
                    }
                case DataType.GBPFormula:
                    {
                        Worksheet.xSetCellAsString(rowIndex, Name, (String)value);
                        break;
                    }
                case DataType.Percentage:
                    {
                        Worksheet.xSetCellAsDouble(rowIndex, Name, (double)value);
                        break;
                    }
                case DataType.PercentageFormula:
                    {
                        Worksheet.xSetCellAsString(rowIndex, Name, (String)value);
                        break;
                    }
                case DataType.Integer:
                    {
                        Worksheet.xSetCellAsInt(rowIndex, Name, (int)value);
                        break;
                    }
                case DataType.Long:
                    {
                        Worksheet.xSetCellAsLong(rowIndex, Name, (long)value);
                        break;
                    }
                case DataType.FileSizeMB:
                    {
                        Worksheet.xSetCellAsLong(rowIndex, Name, (long)value);
                        break;
                    }
                case DataType.FileSizeGB:
                    {
                        Worksheet.xSetCellAsLong(rowIndex, Name, (long)value);
                        break;
                    }
                case DataType.Boolean:
                    {
                        Worksheet.xSetCellAsString(rowIndex, Name, value.ToString());
                        break;
                    }
            }
        }

        public Object Get(int rowIndex)
        {
            switch (Type)
            {
                case DataType.Date:
                    {
                        return (Object)Worksheet.xGetCellAsDateTime(rowIndex, Name);
                    }
                case DataType.Text:
                    {
                        return (Object)Worksheet.xGetCellAsString(rowIndex, Name);
                    }
                case DataType.GBP:
                    {
                        return (Object)Worksheet.xGetCellAsDouble(rowIndex, Name);
                    }
                case DataType.GBPFormula:
                    {
                        return (Object)Worksheet.xGetCellAsString(rowIndex, Name);
                    }
                case DataType.Percentage:
                    {
                        return (Object)Worksheet.xGetCellAsDouble(rowIndex, Name);
                    }
                case DataType.PercentageFormula:
                    {
                        return (Object)Worksheet.xGetCellAsString(rowIndex, Name);
                    }
                case DataType.Integer:
                    {
                        return (Object)Worksheet.xGetCellAsInt(rowIndex, Name);
                    }
                case DataType.Long:
                    {
                        return (Object)Worksheet.xGetCellAsLong(rowIndex, Name);
                    }
                case DataType.FileSizeMB:
                    {
                        return (Object)Worksheet.xGetCellAsLong(rowIndex, Name);
                    }
                case DataType.FileSizeGB:
                    {
                        return (Object)Worksheet.xGetCellAsLong(rowIndex, Name);
                    }
                case DataType.Boolean:
                    {
                        return (Object)(true.ToString().Equals(Worksheet.xGetCellAsString(rowIndex, Name)));
                    }
            }

            throw new NotImplementedException("Not yet handling other types of ExcelColumns");
        }

        public override String ToString()
        {
            return Name;
        }

    }
}
