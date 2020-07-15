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
    class Entity
    {
        public string Color;
        public int Index;
        public Entity()
        {

        }
    }
    public class ExportRiskTree : IDisposable
    {
        private const int BEGIN_AT_ROWINDEX = 2;
        private readonly RiskTreeDataSetTrader _riskTreeDataSetTrader;
        public CellFormats CellFormats { get; set; }
        public Fills Fills { get; set; }
        public DataTable DtToExport { get; set; }
        private DataRow drToExport;
        private readonly string _fileName;
        public List<RiskPolyLine> Lines { get; set; }
        private int _columnIndex;
        private int _rowIndex;
        private readonly int _rowsCount;
        private List<char> _excelColumns = new List<char>();
        private List<int> columnsDamages = new List<int>();
        private List<Entity> colorsDamages = new List<Entity>();
        private const int NumberFormats = 3;
        private const int FillsCount = 2;

        #region Constantes
        private const string RiskID = "Risk_ID";
        private const string MAINRISKNAME = "Main Branch";
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

                //SetColumnsWidth(ds.Tables[0], sheetPart);//este es del exportar viejo
                SetColumnsWidthShort(ds.Tables[0], sheetPart);

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
                int contadorColores = NumberFormats;
                for (int i = 0; i < dataColumnsList.Count; i++)
                {
                    Cell cell = new Cell
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(dsrow[dataColumnsList[i].ColumnName].ToString())
                    };
                    if (dsrow[dataColumnsList[0].ColumnName].ToString() == string.Empty)
                    {
                        cell.StyleIndex = Convert.ToUInt32(2);
                    }
                    else
                    {
                        cell.StyleIndex = Convert.ToUInt32(1);
                    }
                    if (columnsDamages.Contains(i+1))
                    {
                        if (contadorColores - NumberFormats > columnsDamages.Count)
                        {
                            contadorColores = NumberFormats;                            
                        }
                        cell.StyleIndex = Convert.ToUInt32(contadorColores);
                        contadorColores++;
                    }
                    if (dataColumnsList[i].DataType == typeof(decimal) || dataColumnsList[i].DataType == typeof(int))
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

        private static void SetColumnsWidthShort(DataTable dataTable, WorksheetPart sheetPart)
        {
            Columns columns = new Columns();
            for (uint i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[(int)i].ColumnName == "Risk_ID")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 20, CustomWidth = true, Hidden=true});
                }
                if (dataTable.Columns[(int)i].ColumnName == "Main Branch")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 50, CustomWidth = true });
                }
                if (dataTable.Columns[(int)i].ColumnName == "Risk Name")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 40, CustomWidth = true });
                }
                if (dataTable.Columns[(int)i].ColumnName == "CM Name")
                {
                    columns.Append(new Column() { Min = i + 1, Max = i + 1, Width = 40, CustomWidth = true });
                }
            }
            sheetPart.Worksheet.Append(columns);
        }

        private List<DataColumn> CreateHeaderRow(DataTable table, SheetData sheetData)
        {
            Row headerRow = new Row();

            List<DataColumn> columns = new List<DataColumn>();
            int contadorColores = NumberFormats;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Run run1 = new Run();
                run1.Append(new Text(table.Columns[i].ColumnName));
                //create runproperties and append a "Bold" to them
                RunProperties run1Properties = new RunProperties();
                run1Properties.Append(new Bold());
                //set the first runs RunProperties to the RunProperties containing the bold
                run1.RunProperties = run1Properties;
                columns.Add(new DataColumn(table.Columns[i].ColumnName, table.Columns[i].DataType));
                Cell cell = new Cell
                {
                    DataType = CellValues.InlineString,
                    StyleIndex = Convert.ToUInt32(1)
                };

                if (columnsDamages.Contains(i + 1))
                {
                    if (contadorColores - NumberFormats > columnsDamages.Count)
                    {
                        contadorColores = NumberFormats;
                    }
                    cell.StyleIndex = Convert.ToUInt32(contadorColores);
                    contadorColores++;
                }
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

            Fonts fonts = new Fonts(new Font( ));

            Fills.Append(new Fill(new PatternFill() { PatternType = PatternValues.None }));
            Fills.Append(new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }));

            foreach (var item in colorsDamages)
            {
                Fills.Append(new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = item.Color.ToString().Substring(1) } }) { PatternType = PatternValues.Solid }));
            }
            //int intValue = 182;
            //// Convert integer 182 as a hex in a string variable
            //string hexValue = intValue.ToString("X");

            LeftBorder LB = new LeftBorder(){Style = BorderStyleValues.Medium};
            RightBorder RB = new RightBorder(){Style = BorderStyleValues.Medium};
            TopBorder TB = new TopBorder(){Style = BorderStyleValues.Medium};
            BottomBorder BB = new BottomBorder(){Style = BorderStyleValues.Medium};

            Border defaultBorder = new Border();// Defualt border
            Border coloredBorders = new Border(new OpenXmlElement[] {LB,RB,TB,BB });
            Border borderMerged = new Border(new OpenXmlElement[] { new LeftBorder() { Style = BorderStyleValues.Thin }, new RightBorder() { Style = BorderStyleValues.Thin }, new TopBorder() { Style = BorderStyleValues.Thin } });
            Border emptyBorder = new Border(new OpenXmlElement[] { new LeftBorder() { Style = BorderStyleValues.Thin }, new RightBorder() { Style = BorderStyleValues.Thin } });

            emptyBorder.Append();
            emptyBorder.Append();

            Borders borders = new Borders();    // <APPENDING Borders>
            borders.Append(defaultBorder);
            borders.Append(coloredBorders);
            borders.Append(borderMerged);
            borders.Append(emptyBorder);

            // <CellFormats>
            CellFormat formatDefault = new CellFormat();//DefaultFormat
            CellFormat cellformat1 = new CellFormat() { FontId = 0, FillId = 0, BorderId = 2, ApplyFill = false, Alignment = new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Center } };
            CellFormat format2 = new CellFormat() { FontId = 0, FillId = 0, BorderId = 3, ApplyFill = false, Alignment = new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Center } }; ;

            CellFormats.Append(formatDefault);
            CellFormats.Append(cellformat1);
            CellFormats.Append(format2);
            for (int i = FillsCount; i < colorsDamages.Count + FillsCount; i++)
            {
                CellFormats.Append(new CellFormat() { FontId = 0, FillId = Convert.ToUInt32(i), BorderId = 1, ApplyFill = true, Alignment = new Alignment() { WrapText = true, Vertical = VerticalAlignmentValues.Center } });
            }

            //// <APPENDING CellFormats>
            //CellFormats cellformats = new CellFormats();


            // Append FONTS, FILLS , BORDERS & CellFormats to stylesheet <Preserve the ORDER>
            workbookstylesheet.Append(fonts);
            workbookstylesheet.Append(Fills);
            workbookstylesheet.Append(borders);
            workbookstylesheet.Append(CellFormats);

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
            CellFormats = new CellFormats();
            Fills = new Fills();
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

        public void ExportShortExcel(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            InitializeExcel();
            SetHeaderShort();
            FillShort(backgroundWorker, e);
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
            
            SetRiskHeader();
            SetCounterMHeader();
        }

        private void SetHeaderShort()
        {
            SetRiskHeaderShort();
            SetCounterMHeaderShort();
        }

        private void SetRiskHeader()
        {
            _columnIndex = 1;
            DtToExport.Columns.Add(RiskID);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskName);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskComments);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskProbability, typeof(decimal));
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskStatus);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskFather);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskWBSName);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            SetDynamicHeader(false);           
        }

        private void SetRiskHeaderShort()
        {
            _columnIndex = 1;
            DtToExport.Columns.Add(RiskID);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(MAINRISKNAME);
            _excelColumns.Add(Convert.ToChar(64 + _columnIndex));

            _columnIndex++;
            DtToExport.Columns.Add(RiskName);
           
            SetDynamicHeader(false);
        }

        private void SetCounterMHeaderShort()
        {
            _columnIndex++;
            DtToExport.Columns.Add(CMName);

            ADSetDynamicHeader();
        }


        private void SetCounterMHeader()
        {
            _columnIndex++;
            DtToExport.Columns.Add(CMID);

            _columnIndex++;
            DtToExport.Columns.Add(CMName);

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
                columnsDamages.Add(_columnIndex);
                //colorsDamages.Add(_columnIndex.ToString());
                
                DtToExport.Columns.Add(useCM ? "CM-" + propertyType : propertyType, typeof(decimal));
                if (!useCM)
                {
                    _excelColumns.Add(Convert.ToChar(64 + _columnIndex));
                    colorsDamages.Add(new Entity() { Color = "", Index = _columnIndex });
                }
            }
        }

        private void ADSetDynamicHeader()
        {
            foreach (var propertyType in _riskTreeDataSetTrader.RiskTypeList)
            {
                _columnIndex++;
                DtToExport.Columns.Add("After CM /" + propertyType, typeof(decimal));
                _excelColumns.Add(Convert.ToChar(64 + _columnIndex));               
            }
        }

        private void Fill(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            _rowIndex = BEGIN_AT_ROWINDEX;
            FillWithRisk(_riskTreeDataSetTrader.GetMainRiskChildList(), backgroundWorker, e, true);
        }

        private void FillShort(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            _rowIndex = BEGIN_AT_ROWINDEX;
            FillWithRiskShort(_riskTreeDataSetTrader.GetMainRiskChildList(), backgroundWorker, e, true);
        }

        private void FillWithRiskShort(IEnumerable<DataRow> mainRiskChildDataRowQuery, BackgroundWorker backgroundWorker, DoWorkEventArgs e, bool isMain)
        {
            IEnumerable<DataRow> riskProperties = _riskTreeDataSetTrader.ObtenerTiposDamagesRisk();
            foreach (DataRow riskDataRow in mainRiskChildDataRowQuery)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                DtToExport.Merge(_riskTreeDataSetTrader.GetMainRisksDescendants(riskDataRow));
                _columnIndex = 3;
                foreach (var damage in _riskTreeDataSetTrader.DamagesRow)
                {
                    string columnaNombre = DtToExport.Columns[_columnIndex].ColumnName;
                    int idRisk = (int)riskDataRow[DT_Risk.ID];
                    DtToExport.Select(RiskID + " = " + idRisk).First()[columnaNombre] = riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre)[DT_Risk_Damages.VALUE];
                    int a = _columnIndex + 1;
                    if (colorsDamages.FindIndex(c => c.Index == a) >= 0)
                    {
                        colorsDamages[colorsDamages.FindIndex(c => c.Index == a)].Color = riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre)[DT_Risk_Damages.COLOR].ToString();
                    }
                    _columnIndex++;
                }
                _columnIndex++;//La columna de la CM
                foreach (var damage in _riskTreeDataSetTrader.DamagesRow)
                {
                    string columnaNombre = DtToExport.Columns[_columnIndex].ColumnName;
                    int id = (int)riskDataRow[DT_Risk.ID];
                    int idDamage = (int)riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre.Split('/')[1].ToString())[DT_Risk_Damages.ID_DAMAGE];

                    DtToExport.Select(RiskID + " = " + id).First()[columnaNombre] = General.MyRound(CalculateAD(id, idDamage), 1);
                    _columnIndex++;
                }
            }
           
            
            
           
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
                    int a = _columnIndex + 1;
                    if (colorsDamages.FindIndex(c => c.Index == a)>=0)
                    {
                        colorsDamages[colorsDamages.FindIndex(c => c.Index == a)].Color = riskProperties.FirstOrDefault(riskProperty => riskProperty[DT_Risk_Damages.DAMAGE].ToString() == columnaNombre)[DT_Risk_Damages.COLOR].ToString();
                    }
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
                    if (_riskTreeDataSetTrader.SourceDataSet.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemI.ID, IdDamageSelected }))
                    {
                        value = (decimal)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Find(new object[] { itemI.ID, IdDamageSelected })[DT_CounterM_Damage.VALUE];
                    }
                    AcumDamage += value;
                }
                else
                {
                    if (_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { itemI.ID, IdDamageSelected }))
                    {
                        value = (decimal)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { itemI.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                    }
                    AcumDamage += value * AcumulatedLikelihood(_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemI.ID));
                }
            }
            return AcumDamage;
        }

        private decimal AcumulatedLikelihood(DataRow drRFather)
        {
            decimal ValueToReturn;
            bool hasChildren = false;//the flag ill be activated if the risk has children,
            List<decimal> Probability_List = new List<decimal>();
            List<decimal> CM_Probabilities = new List<decimal>();
            foreach (DataRow drChildCM in _riskTreeDataSetTrader.SourceDataSet.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK + " = " + drRFather[DT_Risk.ID]))
            {
                if ((bool)drChildCM[DT_CounterM.ENABLED])
                {
                    CM_Probabilities.Add((decimal)drChildCM[DT_CounterM.PROBABILITY]/100);
                }
            }
            foreach (DataRow drChildRisk in _riskTreeDataSetTrader.SourceDataSet.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK_FATHER + " = " + drRFather[DT_Risk.ID]))
            {
                hasChildren = true;
                if (_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Contains(drChildRisk[DT_RiskStructure.IDRISK]))
                {
                    if (IsLeaf(_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Find(drChildRisk[DT_RiskStructure.IDRISK]),_riskTreeDataSetTrader.SourceDataSet.Tables[DT_RiskStructure.TABLE_NAME]))
                    {
                        if (!((bool)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Find(drChildRisk[DT_RiskStructure.IDRISK])[DT_Risk.ENABLED]))
                        {
                            Probability_List.Add(1);
                        }
                        else
                        {
                            Probability_List.Add((decimal)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Find(drChildRisk[DT_RiskStructure.IDRISK])[DT_Risk.PROBABILITY]/100);
                        }
                    }
                    else
                    {
                        if (((bool)_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Find(drChildRisk[DT_RiskStructure.IDRISK])[DT_Risk.ENABLED]))
                        {
                            Probability_List.Add(AcumulatedLikelihood(_riskTreeDataSetTrader.SourceDataSet.Tables[DT_Risk.TABLE_NAME].Rows.Find(drChildRisk[DT_RiskStructure.IDRISK])));
                        }
                        else
                        {
                            Probability_List.Add(1);
                        }
                    }
                }                
            }
            if (hasChildren)
            {
                //Here the formula, the probability of the father mult. by the probabilities of their children according with the In_Exclusion_Formula
                ValueToReturn = (decimal)drRFather[DT_Risk.PROBABILITY]/100 * EL_Inclusion_Exclusion(Probability_List);
            }
            else
            {
                ValueToReturn = (decimal)drRFather[DT_Risk.PROBABILITY]/100;//If don´t have child, Acum. Likelihood = its Probability                
            }
            foreach (var item in CM_Probabilities)
            {
                ValueToReturn *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
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

        private bool IsLeaf(DataRow drChild, DataTable dtStructure)
        {
            return !dtStructure.Select(DT_Risk.IDRISK_FATHER + " = " + drChild[DT_Risk.ID]).Any();
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

                }                
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
