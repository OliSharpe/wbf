using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace WBFAnalysisTool
{
    public static class Extensions
    {

        public static bool xSheetExists(this Excel.Workbook workbook, String sheetName)
        {
            foreach (Excel.Worksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name.Equals(sheetName)) return true;
            }

            return false;
        }

        public static bool xCellHasValue(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            return ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2 != null;
        }

        public static bool xCellHasNoValue(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            return !xCellHasValue(worksheet, rowIndex, columnLetter);
        }

        public static bool xCellIsEmpty(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            return String.IsNullOrEmpty(worksheet.xGetCellAsString(rowIndex, columnLetter).Trim());
        }

        public static double xGetCellAsDouble(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            String cellValue = xGetCellAsString(worksheet, rowIndex, columnLetter);

            if (String.IsNullOrEmpty(cellValue)) return 0;

            return double.Parse(cellValue);
        }

        public static void xSetCellAsDouble(this Excel.Worksheet worksheet, int rowIndex, String columnLetter, double value)
        {
            ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2 = value.ToString();
        }


        public static int xGetCellAsInt(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            String cellValue = xGetCellAsString(worksheet, rowIndex, columnLetter);

            if (String.IsNullOrEmpty(cellValue)) return 0;

            return int.Parse(cellValue);
        }

        public static void xSetCellAsInt(this Excel.Worksheet worksheet, int rowIndex, String columnLetter, int value)
        {
            ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2 = value.ToString();
        }

        public static long xGetCellAsLong(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            String cellValue = xGetCellAsString(worksheet, rowIndex, columnLetter);

            if (String.IsNullOrEmpty(cellValue)) return 0;

            return long.Parse(cellValue);
        }

        public static void xSetCellAsLong(this Excel.Worksheet worksheet, int rowIndex, String columnLetter, long value)
        {
            ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2 = value.ToString();
        }


        public static void xSetCellAsFormula(this Excel.Worksheet worksheet, int rowIndex, String columnLetter, String formula)
        {
            ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2 = formula;
        }



        public static DateTime xGetCellAsDateTime(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            return DateTime.FromOADate(double.Parse(((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2.ToString()));
        }

        public static void xSetCellAsDateTime(this Excel.Worksheet worksheet, int rowIndex, String columnLetter, DateTime dateTime)
        {
            ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2 = dateTime.ToOADate().ToString().Replace(",", ".");
        }



        public static String xGetCellAsString(this Excel.Worksheet worksheet, int rowIndex, String columnLetter)
        {
            Object value = ((Excel.Range)worksheet.Cells[rowIndex, columnLetter]).Value2;

            if (value == null) return "";

            return value.ToString().Trim();
        }

        public static void xSetCellAsString(this Excel.Worksheet worksheet, int rowIndex, String columnLetter, String value)
        {
            worksheet.Cells[rowIndex, columnLetter] = value;
        }

        public static void xFormatTitleRow(this Excel.Worksheet worksheet)
        {
            Excel.Range row = worksheet.get_Range("A1");
            row.EntireRow.Font.Bold = true;
            row.EntireRow.Font.Size = 12;
            row.EntireRow.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
        }

        public static void xTitleColumn(this Excel.Worksheet worksheet, String columnLetter, String title)
        {
            worksheet.Cells[1, columnLetter] = title;
        }


        public static void xFormatColumnAsDate(this Excel.Worksheet worksheet, String columnLetter)
        {
            worksheet.xFormatColumnAsDate(columnLetter, "Date");
        }

        public static void xFormatColumnAsDate(this Excel.Worksheet worksheet, String columnLetter, String title)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "DD/MM/YYYY";
            column.EntireColumn.ColumnWidth = 12;
            column.EntireColumn.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            worksheet.xTitleColumn(columnLetter, title);
        }

        public static void xFormatColumnAsText(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }

        public static void xFormatColumnAsGBP(this Excel.Worksheet worksheet, String columnLetter, String title)
        {
            worksheet.xFormatColumnAsGBP(columnLetter, title, 12);
        }

        public static void xFormatColumnAsGBP(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "£#,##0.00;[Red]-£#,##0.00";
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }

        public static void xFormatColumnAsInteger(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "#,##0";
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }

        public static void xFormatColumnAsLong(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "#,##0";
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }

        public static void xFormatColumnAsFileSizeMB(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "#,##0.0,,\" MB\"";
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }

        public static void xFormatColumnAsFileSizeGB(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "#,##0.0,,,\" GB\"";
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }


        public static void xFormatColumnAsPercentage(this Excel.Worksheet worksheet, String columnLetter, String title, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.NumberFormat = "0%";
            column.EntireColumn.ColumnWidth = width;

            worksheet.xTitleColumn(columnLetter, title);
        }



        public static void xFormatColumnWidth(this Excel.Worksheet worksheet, String columnLetter, int width)
        {
            Excel.Range column = worksheet.get_Range(columnLetter + "1", columnLetter + "1");
            column.EntireColumn.ColumnWidth = width;
        }

        public static String xAddItem(this String currentValues, String newValue)
        {
            if (String.IsNullOrEmpty(newValue)) return currentValues;
            newValue = newValue.Trim();

            if (String.IsNullOrEmpty(currentValues)) return newValue;

            List<String> items = new List<String>(currentValues.Split(';'));
            items.Add(newValue);
            return String.Join(";", items.ToArray());
        }

        public static String xAddItemIfNew(this String currentValues, String newValue)
        {
            if (String.IsNullOrEmpty(newValue)) return currentValues;
            newValue = newValue.Trim();

            if (String.IsNullOrEmpty(currentValues)) return newValue;

            List<String> items = new List<String>(currentValues.Split(';'));
            foreach (String item in items)
            {
                if (item.Equals(newValue)) return currentValues;
            }
            items.Add(newValue);
            return String.Join(";", items.ToArray());
        }

        // Thanks to: http://stackoverflow.com/questions/837155/fastest-function-to-generate-excel-column-letters-in-c-sharp
        public static String xExcelColumnName(this int column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            return columnString;
        }

        public static int xExcelColumnIndex(this String column)
        {
            int retVal = 0;
            string col = column.ToUpper();
            for (int iChar = col.Length - 1; iChar >= 0; iChar--)
            {
                char colPiece = col[iChar];
                int colNum = colPiece - 64;
                retVal = retVal + colNum * (int)Math.Pow(26, col.Length - (iChar + 1));
            }
            return retVal;
        }

    }
}
