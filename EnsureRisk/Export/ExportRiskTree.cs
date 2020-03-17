using DataMapping.Data;
//using EnsureRisk.Export.Contract;
using EnsureRisk.Export.Trader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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
            using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
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

                    List<DataColumn> columns = new List<DataColumn>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(new DataColumn(column.ColumnName, column.DataType));
                        Cell cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(column.ColumnName)                            
                        };
                        
                        headerRow.AppendChild(cell);                       
                    }
                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (var col in columns)
                        {
                            Cell cell = new Cell
                            {
                                DataType = CellValues.String,
                                CellValue = new CellValue(dsrow[col.ColumnName].ToString())
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
            }
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
