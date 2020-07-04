using DataMapping.Data;
using EnsureRisk.Export.Trader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualBasic.FileIO;
using DocumentFormat.OpenXml;

namespace EnsureRisk.Export
{
    public class ExportRiskTree : IDisposable
    {
        private const int BEGIN_AT_ROWINDEX = 2;
        private readonly RiskTreeDataSetTrader _riskTreeDataSetTrader;

        public DataTable DtToExport { get; set; }
        private DataRow drToExport;
        private readonly string _fileName;

        //private Excel.Application _excelApplication;
        //private Excel.Workbook _workbook;
        //private Excel.Worksheet _worksheet;

        private int _columnIndex;
        private int _rowIndex;
        private readonly int _rowsCount;

        #region Constantes
        private const string RiskID = "Risk ID";
        private const string RiskName = "Risk Name";
        private const string RiskComments = "Risk Comments";
        private const string RiskProbability = "Probability";
        private const string RiskStatus = "Risk Status";
        private const string RiskFather = "Father";
        private const string RiskWBSName = "WBS Name";

        private const string CMID = "CounterM ID";
        private const string CMName = "CM Name";
        private const string CMComments = "CM Comments";
        private const string CMRiskRed = "Risk Reduction";
        private const string CMStatus = "CM Status";
        #endregion


        private void ExportDataSet(DataSet ds, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook
                {
                    Sheets = new Sheets()
                };
                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };

                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<DataColumn> dataColumnsList = new List<DataColumn>();
                    foreach (DataColumn column in table.Columns)
                    {
                        Run run1 = new Run();
                        run1.Append(new Text(column.ColumnName));
                        //create runproperties and append a "Bold" to them
                        RunProperties run1Properties = new RunProperties();
                        run1Properties.Append(new Bold());
                        //set the first runs RunProperties to the RunProperties containing the bold
                        run1.RunProperties = run1Properties;

                        dataColumnsList.Add(new DataColumn(column.ColumnName, column.DataType));
                        Cell cell = new Cell
                        {
                            DataType = CellValues.InlineString
                        };
                        InlineString inlineString = new InlineString();
                        inlineString.Append(run1);
                        cell.Append(inlineString);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);
                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (var col in dataColumnsList)
                        {
                            Cell cell = new Cell
                            {
                                DataType = CellValues.String,
                                CellValue = new CellValue(dsrow[col.ColumnName].ToString()),
                                StyleIndex = Convert.ToUInt32(1)
                            };
                            if (col.DataType == typeof(decimal))
                            {
                                cell.DataType = CellValues.Number;
                            }
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }

                }
                AddStyleSheet(workbook);
            }
        }


        private WorkbookStylesPart AddStyleSheet(SpreadsheetDocument spreadsheet)
        {
            WorkbookStylesPart stylesheet = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            Stylesheet workbookstylesheet = new Stylesheet();

            // <Fonts>
            Font font0 = new Font();            // Default font
            Fonts fonts = new Fonts();          // <APPENDING Fonts>
            fonts.Append(font0);

            // <Fills>
            Fill fill0 = new Fill();            // Default fill
            Fills fills = new Fills();          // <APPENDING Fills>
            fills.Append(fill0);

            // <Borders>
            Border border0 = new Border();      // Defualt border
            Borders borders = new Borders();    // <APPENDING Borders>
            borders.Append(border0);

            // <CellFormats>
            CellFormat cellformat0 = new CellFormat()   // Default style : Mandatory
            {
                FontId = 0,
                FillId = 0,
                BorderId = 0
            };
            CellFormat cellformat1 = new CellFormat(new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Top  });          // Style with textwrap set

            // <APPENDING CellFormats>
            CellFormats cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);

            // Append FONTS, FILLS , BORDERS & CellFormats to stylesheet <Preserve the ORDER>
            workbookstylesheet.Append(fonts);
            workbookstylesheet.Append(fills);
            workbookstylesheet.Append(borders);
            workbookstylesheet.Append(cellformats);

            // Finalize
            stylesheet.Stylesheet = workbookstylesheet;
            stylesheet.Stylesheet.Save();

            return stylesheet;
        }

        public ExportRiskTree(RiskTreeDataSetTrader riskTreeDataSetTrader, string fileName)
        {
            _riskTreeDataSetTrader = riskTreeDataSetTrader ?? throw new ArgumentNullException(nameof(riskTreeDataSetTrader));
            _fileName = fileName;
            _rowsCount = _riskTreeDataSetTrader.RowsCount();
        }
        public void Export(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            InitializeExcel();
            SetHeader();
            //FreezeFirstRow();
            Fill(backgroundWorker, e);
            try
            {
                if (!e.Cancel)
                {
                    //SaveToExcel();
                    using (DataSet ds = new DataSet())
                    {
                        ds.Tables.Add(DtToExport);
                        ExportDataSet(ds, _fileName);
                    }
                }
            }
            finally
            {
                //CloseExcel();
            }
        }
        private void InitializeExcel()
        {           
            DtToExport = new DataTable();
        }
        private void SetHeader()
        {
            _columnIndex = 1;
            SetRiskHeader();
            SetCounterMHeader();
        }

        private void SetRiskHeader()
        {                      
            _columnIndex++;
            DtToExport.Columns.Add(RiskID);

            _columnIndex++;
            DtToExport.Columns.Add(RiskName);

            _columnIndex++;
            DtToExport.Columns.Add(RiskComments);

            _columnIndex++;
            DtToExport.Columns.Add(RiskProbability, typeof(decimal));

            _columnIndex++;
            DtToExport.Columns.Add(RiskStatus);

            _columnIndex++;
            DtToExport.Columns.Add(RiskFather);

            _columnIndex++;
            DtToExport.Columns.Add(RiskWBSName);

            SetDynamicHeader(false);
        }
        private void SetCounterMHeader()
        {
            _columnIndex++;
            DtToExport.Columns.Add(CMID);

            _columnIndex++;
            DtToExport.Columns.Add(CMName);

            _columnIndex++;
            DtToExport.Columns.Add(CMComments);

            _columnIndex++;
            DtToExport.Columns.Add(CMRiskRed, typeof(decimal));

            _columnIndex++;
            DtToExport.Columns.Add(CMStatus);

            SetDynamicHeader(true);
        }
        private void SetDynamicHeader(bool useCM)
        {
            foreach (var propertyType in _riskTreeDataSetTrader.RiskTypeList)
            {
                _columnIndex++;
                DtToExport.Columns.Add(useCM ? "CM-" + propertyType : propertyType, typeof(decimal));
            }
        }
        private void Fill(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            _rowIndex = BEGIN_AT_ROWINDEX;
            FillWithRisk(_riskTreeDataSetTrader.GetMainRiskChildList(), backgroundWorker, e, true);
        }
        private void FillWithRisk(IEnumerable<DataRow> mainRiskChildDataRowQuery, BackgroundWorker backgroundWorker, DoWorkEventArgs e, bool isMain)
        {
            drToExport = DtToExport.NewRow();
            foreach (DataRow riskDataRow in mainRiskChildDataRowQuery)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                _columnIndex = 0;               
                
                drToExport[RiskID] = riskDataRow[DT_Risk.ID];
                _columnIndex++;
                drToExport[RiskName] = riskDataRow[DT_Risk.NAMESHORT];
                _columnIndex++;
                drToExport[RiskComments] = riskDataRow[DT_Risk.COMMENTS];
                _columnIndex++;
                drToExport[RiskProbability] = riskDataRow[DT_Risk.PROBABILITY];
                _columnIndex++;
                drToExport[RiskStatus] = (bool)riskDataRow[DT_Risk.ENABLED] ? "Activated" : "No Activated";
                _columnIndex++;

                if (isMain)
                {
                    drToExport[RiskFather] = "";
                }
                else
                {
                    drToExport[RiskFather] = riskDataRow[DT_Risk.IDRISK_FATHER];
                }
                _columnIndex++;

                drToExport[RiskWBSName] = riskDataRow[DT_Risk.WBS_NAME];
                _columnIndex++;

                IEnumerable<DataRow> riskProperties = _riskTreeDataSetTrader.GetRiskPropertyList((int)riskDataRow[DT_Risk.ID]);

                foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
                {
                    string columnaNombre = DtToExport.Columns[_columnIndex].ColumnName;
                    drToExport[columnaNombre] = riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre)[DT_Risk_Damages.VALUE];
                    _columnIndex++;
                }
                int _counterMcolumnIndexBeginAt = _columnIndex;

                IEnumerable<DataRow> counterMeasureChildList = _riskTreeDataSetTrader.GetCounterMeasureChildList((int)riskDataRow[DT_Risk.ID]);

                if (!counterMeasureChildList.Any())
                {                   
                    DtToExport.Rows.Add(drToExport);
                    drToExport = DtToExport.NewRow();
                }
                foreach (DataRow counterMDataRow in counterMeasureChildList)
                {
                    _columnIndex = _counterMcolumnIndexBeginAt;
                    drToExport[CMID] = counterMDataRow[DT_CounterM.ID];
                    _columnIndex++;
                    drToExport[CMName] = counterMDataRow[DT_CounterM.NAMESHORT];
                    _columnIndex++;
                    drToExport[CMComments] = counterMDataRow[DT_CounterM.DETAIL];
                    _columnIndex++;
                    drToExport[CMRiskRed] = counterMDataRow[DT_CounterM.PROBABILITY];
                    _columnIndex++;
                    drToExport[CMStatus] = (bool)counterMDataRow[DT_CounterM.ENABLED] ? "Activated" : "No Activated";
                    _columnIndex++;

                    IEnumerable<DataRow> counterMProperties = _riskTreeDataSetTrader.GetCounterMPropertyList((int)counterMDataRow[DT_CounterM.ID]);

                    foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
                    {
                        string columnaNombre = DtToExport.Columns[_columnIndex].ColumnName;
                        drToExport[columnaNombre] = counterMProperties.FirstOrDefault(counterMProperty => counterMProperty[DT_CounterM_Damage.DAMAGE].ToString() == columnaNombre.Split('-')[1].ToString())[DT_CounterM_Damage.VALUE];
                        _columnIndex++;
                    }

                    int progressPercentage = (_rowIndex * 100 / _rowsCount);
                    backgroundWorker.ReportProgress(progressPercentage);
                    DtToExport.Rows.Add(drToExport);
                    drToExport = DtToExport.NewRow();
                    _rowIndex++;
                }

                IEnumerable<DataRow> riskChildList = _riskTreeDataSetTrader.GetRiskChildList((int)riskDataRow[DT_Risk.ID]);

                if (riskChildList.Any())
                {
                    FillWithRisk(riskChildList, backgroundWorker, e, false);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
