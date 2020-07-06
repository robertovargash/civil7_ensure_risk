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
using EnsureBusinesss.Business;
using EnsureBusinesss;

namespace EnsureRisk.Export
{
    class RowRange
    {
        public int beginAt;
        public int endAt;
    }
    public class ExportRiskTree : IDisposable
    {
        private const int BEGIN_AT_ROWINDEX = 2;
        private readonly RiskTreeDataSetTrader _riskTreeDataSetTrader;

        public DataTable DtToExport { get; set; }
        private DataRow drToExport;
        private readonly string _fileName;
        public List<RiskPolyLine> Lines { get; set; }
        private int _columnIndex;
        private int _rowIndex;
        private readonly int _rowsCount;
        private List<char> _excelColumns = new List<char>();

        #region Constantes
        private const string RiskID = "Risk ID";
        private const string RiskName = "Risk Name";
        private const string RiskComments = "Risk Comments";
        private const string RiskProbability = "Probability";
        private const string RiskStatus = "Risk Status";
        private const string RiskFather = "Father";
        private const string RiskWBSName = "WBS Name";
        //private const string RiskAcumulatedValue = "Acumulated Value";

        private const string CMID = "CounterM ID";
        private const string CMName = "CM Name";
        private const string CMComments = "CM Comments";
        private const string CMRiskRed = "Risk Reduction";
        private const string CMStatus = "CM Status";
        //private const string CMAcumulatedDamage = "CM Acumulated Damage";

        #endregion

        private void ExportDataSet(DataSet ds, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook))
            {
                workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook();
                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                sheetPart.Worksheet = new Worksheet();

                FreezeHeader(sheetPart);
                SheetFormatProperties sheetFormatProperties = new SheetFormatProperties()

                {
                    DefaultColumnWidth = 15,
                    DefaultRowHeight = 15D
                };

                sheetPart.Worksheet.Append(sheetFormatProperties);

                SetColumnsWidth(ds.Tables[0], sheetPart);

                var sheetData = sheetPart.Worksheet.AppendChild(new SheetData());
                var sheets = workbook.WorkbookPart.Workbook.AppendChild(new Sheets());
                sheets.AppendChild(new Sheet() { Id = workbook.WorkbookPart.GetIdOfPart(sheetPart), SheetId = 1, Name = ds.Tables[0].TableName });
                Row headerRow = new Row();
                List<DataColumn> dataColumnsList = CreateHeaderRow(ds.Tables[0], sheetData);

                int index = 2;
                RowRange rowRange = new RowRange();

                SetAndMergeRows(ds, sheetPart, sheetData, dataColumnsList, ref index, ref rowRange);

                AddStyleSheet(workbook);
                workbook.WorkbookPart.Workbook.Save();
            }

        }

        private void SetAndMergeRows(DataSet ds, WorksheetPart sheetPart, SheetData sheetData, List<DataColumn> dataColumnsList, ref int index, ref RowRange rowRange)
        {
            List<RowRange> rangesToMerge = new List<RowRange>();
            foreach (DataRow dsrow in ds.Tables[0].Rows)
            {
                Row newRow = new Row();

                if ((dsrow[0]).ToString() != string.Empty)
                {
                    rowRange = new RowRange
                    {
                        beginAt = index,
                        endAt = index
                    };
                    rangesToMerge.Add(rowRange);
                }
                else
                {
                    rowRange.endAt = index;
                }

                foreach (var col in dataColumnsList)
                {
                    Cell cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(dsrow[col.ColumnName].ToString()),
                        StyleIndex = Convert.ToUInt32(1)
                    };
                    if (col.DataType == typeof(decimal) || col.DataType == typeof(int))
                    {
                        cell.DataType = CellValues.Number;
                    }
                    newRow.AppendChild(cell);
                }
                sheetData.AppendChild(newRow);
                index++;
            }
            MergeRequiredCell(rangesToMerge, sheetPart);
        }

        private static void FreezeHeader(WorksheetPart sheetPart)
        {
            SheetViews sheetViews = new SheetViews();
            SheetView sheetView = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            Pane pane = new Pane() { ActivePane = PaneValues.BottomLeft, State = PaneStateValues.Frozen, TopLeftCell = "A2", VerticalSplit = 1D };
            Selection selection = new Selection() { Pane = PaneValues.BottomLeft };
            sheetView.Append(pane);
            sheetView.Append(selection);
            sheetViews.Append(sheetView);
            sheetPart.Worksheet.Append(sheetViews);
        }

        private static void SetColumnsWidth(DataTable dataTable, WorksheetPart sheetPart)
        {
            Columns columns = new Columns();
            for (uint i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[(int)i].ColumnName == "Risk Name")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 20, CustomWidth = true });
                }
                if (dataTable.Columns[(int)i].ColumnName == "Risk Comments")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 50, CustomWidth = true });
                }
                if (dataTable.Columns[(int)i].ColumnName == "CM Name")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 20, CustomWidth = true });
                }
                if (dataTable.Columns[(int)i].ColumnName == "CM Comments")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 50, CustomWidth = true });
                }
            }
            sheetPart.Worksheet.Append(columns);
        }

        private static List<DataColumn> CreateHeaderRow(DataTable table, SheetData sheetData)
        {
            Row headerRow = new Row();

            List<DataColumn> columns = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                Run run1 = new Run();
                run1.Append(new Text(column.ColumnName));
                //create runproperties and append a "Bold" to them
                RunProperties run1Properties = new RunProperties();
                run1Properties.Append(new Bold());
                //set the first runs RunProperties to the RunProperties containing the bold
                run1.RunProperties = run1Properties;
                columns.Add(new DataColumn(column.ColumnName, column.DataType));
                Cell cell = new Cell
                {
                    DataType = CellValues.InlineString,
                    StyleIndex = Convert.ToUInt32(1)
                };
                InlineString inlineString = new InlineString();
                inlineString.Append(run1);
                cell.Append(inlineString);
                headerRow.AppendChild(cell);
            }
            sheetData.AppendChild(headerRow);
            return columns;
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
            CellFormat cellformat1 = new CellFormat(new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Top });
            CellFormat cellformat2 = new CellFormat(new Alignment() { WrapText = true, Horizontal = HorizontalAlignmentValues.Justify }); // Style with textwrap set

            // <APPENDING CellFormats>
            CellFormats cellformats = new CellFormats();
            cellformats.Append(cellformat0);
            cellformats.Append(cellformat1);
            cellformats.Append(cellformat2);

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

        private void MergeRequiredCell(List<RowRange> rangesToMerge, WorksheetPart sheetPart)
        {
            //create a MergeCells class to hold each MergeCell
            MergeCells mergeCells = new MergeCells();
            foreach (var range in rangesToMerge)
            {
                foreach (var item in _excelColumns)
                {
                    //append a MergeCell to the mergeCells for each set of merged cells
                    MergeCell cellToMerge = new MergeCell() { Reference = new StringValue(item + range.beginAt.ToString() + ":" + item + range.endAt.ToString()) };
                    mergeCells.Append(cellToMerge);
                }
            }
            sheetPart.Worksheet.InsertAfter(mergeCells, sheetPart.Worksheet.Elements<SheetData>().First());
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
            for (int i = 0; i < _columnIndex - 2; i++)
            {
                _excelColumns.Add(Convert.ToChar(65 + i));
            }
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

            _columnIndex++;
            SetDynamicHeader(false);

           
        }
        private void SetCounterMHeader()
        {
            _columnIndex++;
            DtToExport.Columns.Add(CMID);

            _columnIndex++;
            DtToExport.Columns.Add(CMName);

            _columnIndex++;
            ADSetDynamicHeader();

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

        private void ADSetDynamicHeader()
        {
            int a = _columnIndex;
            
            foreach (var propertyType in _riskTreeDataSetTrader.RiskTypeList)
            {
                _columnIndex++;
                DtToExport.Columns.Add("After CM /" + propertyType, typeof(decimal));
            }
            for (int i = a - 3; i < _columnIndex - 3; i++)
            {
                _excelColumns.Add(Convert.ToChar(65 + i));
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
                bool flag = true;
                foreach (DataRow counterMDataRow in counterMeasureChildList)
                {
                    _columnIndex = _counterMcolumnIndexBeginAt;
                    drToExport[CMID] = counterMDataRow[DT_CounterM.ID];
                    _columnIndex++;
                    drToExport[CMName] = counterMDataRow[DT_CounterM.NAMESHORT];

                    AddColumnsADData(riskProperties, ref flag);


                    //_columnIndex++;
                    //drToExport[CMAcumulatedDamage] = _riskTreeDataSetTrader.CounterMeasureAcumulatedDamage((int)counterMDataRow[DT_CounterM.ID]);
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
                flag = true;
                IEnumerable<DataRow> riskChildList = _riskTreeDataSetTrader.GetRiskChildList((int)riskDataRow[DT_Risk.ID]);

                if (riskChildList.Any())
                {
                    FillWithRisk(riskChildList, backgroundWorker, e, false);
                }
            }
        }

        private void AddColumnsADData(IEnumerable<DataRow> riskProperties, ref bool flag)
        {
            foreach (var riskType in _riskTreeDataSetTrader.RiskTypeList)
            {
                _columnIndex++;
                if (flag)
                {
                    string columnaNombre = DtToExport.Columns[_columnIndex].ColumnName;
                    int id = (int)riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre.Split('/')[1].ToString())[DT_Risk_Damages.ID_RISK];
                    int idDamage = (int)riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre.Split('/')[1].ToString())[DT_Risk_Damages.ID_DAMAGE];

                    drToExport[columnaNombre] = General.MyRound(CalculateAD(id, idDamage), 1);                    
                }       
            }
            flag = false;
        }

        private decimal CalculateAD(int lineID, int IdDamageSelected)
        {
            decimal AcumDamage = 0;
            RiskPolyLine line = _riskTreeDataSetTrader.LinesDiagram.Find(l => l.ID == lineID);
            foreach (var itemI in TreeOperation.GetMeAndMyChildrenWithCM(line))
            {
                decimal value = 0;
                if (itemI.IsCM)
                {
                    if (_riskTreeDataSetTrader.SourceDataSet.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { itemI.ID, IdDamageSelected }))
                    {
                        value = (decimal)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { itemI.ID, IdDamageSelected })[DT_CounterM_Damage.VALUE];
                    }
                    AcumDamage += value;
                }
                else
                {
                    if (_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { itemI.ID, IdDamageSelected }))
                    {
                        value = (decimal)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { itemI.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                    }
                    AcumDamage += value * AcumulatedLikelihood(itemI);
                }
            }

            return AcumDamage;
        }

        public decimal AcumulatedLikelihood(RiskPolyLine LineFather)
        {
            decimal ValueToReturn;//This will be the value to return
            bool hasChildren = false;//the flag ill be activated if the risk has children,
            List<decimal> Probability_List = new List<decimal>();
            List<decimal> CM_Probabilities = new List<decimal>();
            foreach (var item in LineFather.Children)
            {
                if (item.IsCM)
                {
                    CM_Probabilities.Add(item.Probability);
                }
                else
                {
                    hasChildren = true;
                    if (item.IsLeaf())
                    {
                        Probability_List.Add(item.Probability);
                    }
                    else
                    {
                        Probability_List.Add(AcumulatedLikelihood(item));//else, call the function as recursive
                    }
                }
            }

            if (hasChildren)
            {
                //Here the formula, the probability of the father mult. by the probabilities of their children according with the In_Exclusion_Formula
                ValueToReturn = LineFather.Probability * EL_Inclusion_Exclusion(Probability_List);
                foreach (var item in CM_Probabilities)
                {
                    ValueToReturn *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            else
            {
                ValueToReturn = LineFather.Probability;//If don´t have child, Acum. Likelihood = its Probability
                foreach (var item in CM_Probabilities)
                {
                    ValueToReturn *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            if (ValueToReturn > 1)
            {
                return 1;
            }
            else
            {
                return ValueToReturn;
            }
        }

        public static decimal EL_Inclusion_Exclusion(List<decimal> p)
        {
            if (p.Count > 1)
            {
                decimal temp = 0;
                for (int i = 0; i < p.Count - 1; i++)
                {
                    temp = ProbabilityOr(p[i], p[i + 1]);
                    p[i + 1] = temp;
                }
                return temp;
            }
            else
            {
                return p[0];
            }
        }

        public static decimal ProbabilityOr(decimal A, decimal B)
        {
            return (A + B) - (A * B);
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
