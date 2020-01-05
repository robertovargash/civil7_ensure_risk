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
using Excel = Microsoft.Office.Interop.Excel;

namespace EnsureRisk.Export
{
    public class ExportRiskTree : IDisposable
    {
        private const int BEGIN_AT_ROWINDEX = 2;
        private RiskTreeDataSetTrader _riskTreeDataSetTrader;

        private string _fileName;

        private Excel.Application _excelApplication;
        private Excel.Workbook _workbook;
        private Excel.Worksheet _worksheet;

        private int _columnIndex;
        private int _rowIndex;
        private int _rowsCount;
 
        public ExportRiskTree(RiskTreeDataSetTrader riskTreeDataSetTrader, string fileName)
        {
            _riskTreeDataSetTrader = riskTreeDataSetTrader;
            _fileName = fileName;
            _rowsCount = _riskTreeDataSetTrader.RowsCount();
        }
        public void Export()
        {
            InitializeExcel();
            SetHeader();
            FreezeFirstRow();
            Fill();
            CloseExcel();
        }
        public void Export(BackgroundWorker backgroundWorker)
        {
            InitializeExcel();
            SetHeader();
            FreezeFirstRow();
            Fill(backgroundWorker);
            CloseExcel();
        }
        private void InitializeExcel()
        {
            _excelApplication = new Excel.Application();
            _workbook = _excelApplication.Workbooks.Add(Type.Missing);
            _worksheet = _workbook.Sheets[1];
        }
        private void SetHeader()
        {
            _columnIndex = 1;
            SetRiskHeader();
            SetCounterMHeader();
            _worksheet.Rows.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
        }
        private void FreezeFirstRow()
        {
            Excel.Range firstRow = (Excel.Range)_worksheet.Cells[1, 1];
            _worksheet.Activate();
            _worksheet.Application.ActiveWindow.SplitRow = 1;
            _worksheet.Application.ActiveWindow.SplitColumn = 1;
            firstRow.Application.ActiveWindow.FreezePanes = true;
        }
        private void SetRiskHeader()
        {
            _worksheet.Cells[1, _columnIndex] = "Risk ID";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Columns[_columnIndex].AutoFit();
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Risk Name";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Columns[_columnIndex].ColumnWidth = 40;
            _worksheet.Columns[_columnIndex].Style.WrapText = true;
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Comments";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Columns[_columnIndex].ColumnWidth = 100;
            _worksheet.Columns[_columnIndex].Style.WrapText = true;
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Probability";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Columns[_columnIndex].AutoFit();
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Status";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Columns[_columnIndex].ColumnWidth = 12;
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Father";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Columns[_columnIndex].AutoFit();
            _columnIndex++;

            SetDynamicHeader(false);
        }
        private void SetCounterMHeader()
        {
            _worksheet.Cells[1, _columnIndex] = "CounterM ID";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Cells[1, _columnIndex].Font.Italic = true;
            _worksheet.Columns[_columnIndex].AutoFit();
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "CM Name";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Cells[1, _columnIndex].Font.Italic = true;
            _worksheet.Columns[_columnIndex].ColumnWidth = 20;
            _worksheet.Columns[_columnIndex].Style.WrapText = true;
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Comments";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Cells[1, _columnIndex].Font.Italic = true;
            _worksheet.Columns[_columnIndex].ColumnWidth = 100;
            _worksheet.Columns[_columnIndex].Style.WrapText = true;
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Risk Reduction";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Cells[1, _columnIndex].Font.Italic = true;
            _worksheet.Columns[_columnIndex].AutoFit();
            _columnIndex++;
            _worksheet.Cells[1, _columnIndex] = "Status";
            _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
            _worksheet.Cells[1, _columnIndex].Font.Italic = true;
            _worksheet.Columns[_columnIndex].ColumnWidth = 12;
            _columnIndex++;

            SetDynamicHeader(true);
        }
        private void SetDynamicHeader(Boolean UseItalicFont)
        {
            foreach (var propertyType in _riskTreeDataSetTrader.RiskTypeList)
            {
                _worksheet.Cells[1, _columnIndex] = propertyType;
                _worksheet.Cells[1, _columnIndex].Font.FontStyle = "Bold";
                _worksheet.Cells[1, _columnIndex].Font.Italic = UseItalicFont;
                _worksheet.Columns[_columnIndex].AutoFit();
                _columnIndex++;
            }
        }
        private void Fill()
        {
            _rowIndex = BEGIN_AT_ROWINDEX;
            FillWithRisk(_riskTreeDataSetTrader.GetMainRiskChildList());
        }
        private void Fill(BackgroundWorker backgroundWorker)
        {
            _rowIndex = BEGIN_AT_ROWINDEX;
            FillWithRisk(_riskTreeDataSetTrader.GetMainRiskChildList(), backgroundWorker);
        }
        private void CloseExcel()
        {
            _workbook.SaveAs(_fileName);
            _workbook.Close();
            _excelApplication.Quit();

            Marshal.ReleaseComObject(_worksheet);
            Marshal.ReleaseComObject(_workbook);
            Marshal.ReleaseComObject(_excelApplication);
        }
        private void FillWithRisk(IEnumerable<DataRow> mainRiskChildDataRowQuery)
        {
            String columnHeader = String.Empty;

            foreach (DataRow riskDataRow in mainRiskChildDataRowQuery)
            {
                _columnIndex = 1;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.ID_COLUMNA];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.NAMESHORT_COLUMNA];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.DETAIL_COLUMNA];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.PROBABILITY_COLUMN];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.ENABLED_COLUMN];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.IDRISK_FATHER];
                _columnIndex++;

                IEnumerable<DataRow> riskProperties = _riskTreeDataSetTrader.GetRiskPropertyList((int)riskDataRow[DT_Risk.ID_COLUMNA]);

                foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
                {
                    columnHeader = _worksheet.Cells[1, _columnIndex].Value2;
                    _worksheet.Cells[_rowIndex, _columnIndex] = riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnHeader)[DT_Risk_Damages.VALUE];
                    _columnIndex++;
                }
                int _counterMcolumnIndexBeginAt = _columnIndex++;

                IEnumerable<DataRow> counterMeasureChildList = _riskTreeDataSetTrader.GetCounterMeasureChildList((int)riskDataRow[DT_Risk.ID_COLUMNA]);

                if (counterMeasureChildList.Count() == 0)
                    _rowIndex++;

                foreach (DataRow counterMDataRow in counterMeasureChildList)
                {
                    _columnIndex = _counterMcolumnIndexBeginAt;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.ID_COLUMNA];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.NAMESHORT_COLUMNA];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.DETAIL_COLUMNA];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.PROBABILITY_COLUMN];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.ENABLED_COLUMN];
                    _columnIndex++;

                    IEnumerable<DataRow> counterMProperties = _riskTreeDataSetTrader.GetCounterMPropertyList((int)counterMDataRow[DT_CounterM.ID_COLUMNA]);

                    foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
                    {
                        columnHeader = _worksheet.Cells[1, _columnIndex].Value2;
                        _worksheet.Cells[_rowIndex, _columnIndex] = counterMProperties.FirstOrDefault(counterMProperty => counterMProperty[DT_CounterM_Damage.DAMAGE].ToString() == columnHeader)[DT_CounterM_Damage.VALUE];
                        _columnIndex++;
                    }

                    _rowIndex++;
                }

                IEnumerable<DataRow> riskChildList = _riskTreeDataSetTrader.GetRiskChildList((int)riskDataRow[DT_Risk.ID_COLUMNA]);

                if (riskChildList.Count() > 0)
                {
                    FillWithRisk(riskChildList);
                }
            }
        }
        private void FillWithRisk(IEnumerable<DataRow> mainRiskChildDataRowQuery, BackgroundWorker backgroundWorker)
        {
            String columnHeader = String.Empty;

            foreach (DataRow riskDataRow in mainRiskChildDataRowQuery)
            {
                _columnIndex = 1;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.ID_COLUMNA];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.NAMESHORT_COLUMNA];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.DETAIL_COLUMNA];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.PROBABILITY_COLUMN];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.ENABLED_COLUMN];
                _columnIndex++;
                _worksheet.Cells[_rowIndex, _columnIndex] = riskDataRow[DT_Risk.IDRISK_FATHER];
                _columnIndex++;

                IEnumerable<DataRow> riskProperties = _riskTreeDataSetTrader.GetRiskPropertyList((int)riskDataRow[DT_Risk.ID_COLUMNA]);

                foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
                {
                    columnHeader = _worksheet.Cells[1, _columnIndex].Value2;
                    _worksheet.Cells[_rowIndex, _columnIndex] = riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnHeader)[DT_Risk_Damages.VALUE];
                    _columnIndex++;
                }
                int _counterMcolumnIndexBeginAt = _columnIndex++;

                IEnumerable<DataRow> counterMeasureChildList = _riskTreeDataSetTrader.GetCounterMeasureChildList((int)riskDataRow[DT_Risk.ID_COLUMNA]);

                if (counterMeasureChildList.Count() == 0)
                    _rowIndex++;

                foreach (DataRow counterMDataRow in counterMeasureChildList)
                {
                    _columnIndex = _counterMcolumnIndexBeginAt;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.ID_COLUMNA];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.NAMESHORT_COLUMNA];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.DETAIL_COLUMNA];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.PROBABILITY_COLUMN];
                    _columnIndex++;
                    _worksheet.Cells[_rowIndex, _columnIndex] = counterMDataRow[DT_CounterM.ENABLED_COLUMN];
                    _columnIndex++;

                    IEnumerable<DataRow> counterMProperties = _riskTreeDataSetTrader.GetCounterMPropertyList((int)counterMDataRow[DT_CounterM.ID_COLUMNA]);

                    foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
                    {
                        columnHeader = _worksheet.Cells[1, _columnIndex].Value2;
                        _worksheet.Cells[_rowIndex, _columnIndex] = counterMProperties.FirstOrDefault(counterMProperty => counterMProperty[DT_CounterM_Damage.DAMAGE].ToString() == columnHeader)[DT_CounterM_Damage.VALUE];
                        _columnIndex++;
                    }

                    int progressPercentage = (_rowIndex * 100 / _rowsCount);
                    backgroundWorker.ReportProgress(progressPercentage);

                    _rowIndex++;
                }

                IEnumerable<DataRow> riskChildList = _riskTreeDataSetTrader.GetRiskChildList((int)riskDataRow[DT_Risk.ID_COLUMNA]);

                if (riskChildList.Count() > 0)
                {
                    FillWithRisk(riskChildList, backgroundWorker);
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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ExportRiskTree()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
