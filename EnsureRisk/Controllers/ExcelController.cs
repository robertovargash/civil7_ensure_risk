using DataMapping.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EnsureBusinesss;
using EnsureRisk.Classess;
using EnsureRisk.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk
{
    public class ExcelController
    {
        #region ImportExcel
        /// <summary>
        /// Convert an excel Sheet in a datatable
        /// </summary>
        /// <param name="filpath">Path of the excel file</param>
        /// <returns>The Excel converted to Datatable</returns>
        public static DataTable ExcelToDataTable(string filpath)
        {
            DataTable dt = new DataTable();
            List<int> nonEmptyColumns = new List<int>();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(filpath, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();
                
                for (int i = 0; i < rows.ElementAt(0).Count(); i++)
                {
                    string cellValue = GetCellValue(spreadSheetDocument, rows.ElementAt(0).Descendants<Cell>().ElementAt(i));
                    if (string.IsNullOrWhiteSpace(cellValue))
                    {
                        nonEmptyColumns.Add(i);
                    }
                    if (dt.Columns.Contains(cellValue))
                    {
                        int number = 1;
                        while (dt.Columns.Contains(cellValue + number))
                        {
                            number++;
                        }
                        dt.Columns.Add(cellValue + number);
                    }
                    else
                    {
                        dt.Columns.Add(cellValue);
                    }
                }

                foreach (Row row in rows) //this will also include your header row...
                {
                    DataRow tempRow = dt.NewRow();

                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        Cell cell = row.Descendants<Cell>().ElementAt(i);
                        int index = CellReferenceToIndex(cell);
                        tempRow[index] = GetCellValue(spreadSheetDocument, cell);
                    }

                    dt.Rows.Add(tempRow);
                }
            }
            dt.Rows.RemoveAt(0); //...so i'm taking it out here.
            for (int i = nonEmptyColumns.Count - 1; i >= 0; i--)
            {
                dt.Columns.RemoveAt(nonEmptyColumns[i]);
            }
            return dt;
        }

        public static int CellReferenceToIndex(Cell cell)
        {
            int index = -1;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index + 1) * 26 + value;
                }
                else
                    return index;
            }
            return index;
        }

        /// <summary>
        /// Return the value of a Cell in the Spreadsheet Document
        /// </summary>
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerXml;
            }
            else
            {
                return "";
            }

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Return to a List, all the excel header
        /// </summary>
        public static List<HeaderExcelContent> ReadExcelColumns(DataTable dtExcel)
        {
            List<HeaderExcelContent> listaHeader = new List<HeaderExcelContent>();
            int i = 0;
            foreach (DataColumn item in dtExcel.Columns)
            {
                listaHeader.Add(new HeaderExcelContent(item.ColumnName, i));
                i++;
            }
            return listaHeader;
        }       

        /// <summary>
        /// return as header Format the columns of the Table created from the Excel file
        /// </summary>
        public static WindowHeaderClasification SelectExcelColumns(DataTable dtExcel)
        {
            List<HeaderExcelContent> listaHeader = ReadExcelColumns(dtExcel);
            ServiceClasifications.WebServiceClasificator wsClasification = new ServiceClasifications.WebServiceClasificator();
            DataSet dsClasification = wsClasification.GetAllClasifications().Copy();
            wsClasification.Dispose();
            WindowHeaderClasification whc = new WindowHeaderClasification
            {
                MyDataset = dsClasification.Copy(),
                MyList = listaHeader
            };
            return whc;
        }
        #endregion
    }
}