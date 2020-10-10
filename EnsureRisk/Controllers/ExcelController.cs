using DataMapping.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EnsureBusinesss;
using EnsureRisk.Classess;
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
        public static DataTable ExcelToDataTable(string filpath)
        {
            DataTable dt = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(filpath, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0))
                {
                    dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
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
            return dt;
        }
        public static int CellReferenceToIndex(Cell cell)
        {
            int index = -1;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index + 1) * 26 + value;
                }
                else
                    return index;
            }
            return index;
        }

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

        public static List<HeaderExcelContent> SetColumnas(DataTable dtExcel)
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

        public static void SetValuesToCMInExcel(DataSet dsImporting, string EnableKeyWord, int rowPosition, DataRow theDiagram, DataTable dt, HeaderExcelContent xCmShort, HeaderExcelContent xCmDetail,
    HeaderExcelContent xCmReduction, HeaderExcelContent xIdRisk, HeaderExcelContent xCmActive, IEnumerable<HeaderExcelContent> countDamages, bool isCustom, DataSet DsWBS)
        {
            if (xCmShort != null && dt.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString() != "")
            {
                DataRow drCM = dsImporting.Tables[DT_CounterM.TABLE_NAME].NewRow();
                drCM[DT_CounterM.NAMESHORT] = dt.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString();
                drCM[DT_CounterM.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
                if (xCmDetail != null && dt.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString() != "")
                {
                    drCM[DT_CounterM.DETAIL] = dt.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString();
                }

                drCM[DT_CounterM.ENABLED] = true;
                drCM[DT_CounterM.DIAGONAL] = false;
                drCM[DT_CounterM.FROM_TOP] = true;
                drCM[DT_CounterM.POSITION] = 0;
                if (xCmReduction != null && dt.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString() != "")
                {
                    drCM[DT_CounterM.PROBABILITY] = dt.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString();
                }
                else
                {
                    drCM[DT_CounterM.PROBABILITY] = 0;
                }
                if (xIdRisk != null && dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
                {
                    if (isCustom)
                    {
                        drCM[DT_CounterM.ID_RISK] = Convert.ToDecimal(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    }
                    else
                    {
                        drCM[DT_CounterM.ID_RISK] = Convert.ToDecimal(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString().Split(new char[] { '-' })[1]);

                    }
                }
                else
                {
                    drCM[DT_CounterM.ID_RISK] = dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows[dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Count - 1][DT_CounterM.ID_RISK];
                }

                if (xCmActive != null && dt.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString() != "")
                {
                    if (EnableKeyWord == dt.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString())
                    {
                        drCM[DT_CounterM.ENABLED] = false;
                    }
                    else
                    {
                        drCM[DT_CounterM.ENABLED] = true;
                    }
                }
                else
                {
                    drCM[DT_CounterM.ENABLED] = true;
                }
                foreach (var itemDamages in countDamages.OrderBy(x => x.Column))
                {
                    string TopRisk = itemDamages.MyContent;
                    DamagesToCM(dsImporting, TopRisk, drCM, theDiagram);
                }
                AsignRoleToCM(dsImporting, drCM);
                dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                AsignarWBSDefaultToCM(drCM, dsImporting, DsWBS);
            }

        }

        public static void SetValuesToRiskInExcel(DataTable dt, DataRow theDiagram, DataRow drRisk, int rowPosition, DataRow drRiskN, string enableKeyWord, DataSet dsImporting, HeaderExcelContent xIdRisk,
    HeaderExcelContent xRiskShortName, HeaderExcelContent xRiskDetail, HeaderExcelContent xRiskEnabled, HeaderExcelContent xRiskProb, IEnumerable<HeaderExcelContent> countDamages, bool isCustom, DataSet DsWBS)
        {
            if (xIdRisk != null && dt.Rows[rowPosition][xIdRisk.MyContent].ToString() != "")
            {
                if (isCustom)
                {
                    drRiskN[DT_Risk.ID] = Convert.ToDecimal(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    if (xRiskShortName != null && dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.NAMESHORT] = dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString();
                    }
                    if (xRiskDetail != null && dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.COMMENTS] = dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString();
                    }
                }
                else
                {
                    drRiskN[DT_Risk.ID] = Convert.ToDecimal((dt.Rows[rowPosition][xIdRisk.MyContent.ToString()]).ToString().Split(new char[] { '-' })[1]);
                    if (dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.NAMESHORT] = dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString();
                    }
                    if (dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.NAMESHORT] = drRiskN[DT_Risk.NAMESHORT] + " " + dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString();
                    }
                    if (xRiskDetail != null && dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.COMMENTS] = dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString();
                    }
                    drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
                }
                if (xRiskEnabled != null && dt.Rows[rowPosition][xRiskEnabled.MyContent.ToString()].ToString() != "")
                {
                    if (enableKeyWord == dt.Rows[rowPosition][xRiskEnabled.MyContent.ToString()].ToString())
                    {
                        drRiskN[DT_Risk.ENABLED] = false;
                    }
                    else
                    {
                        drRiskN[DT_Risk.ENABLED] = true;
                    }
                }
                else
                {
                    drRiskN[DT_Risk.ENABLED] = true;
                }
                drRiskN[DT_Risk.IS_ROOT] = false;
                drRiskN[DT_Risk.ISCOLLAPSED] = false;
                drRiskN[DT_Risk.POSITION] = 0;
                drRiskN[DT_Risk.FROM_TOP] = false;
                drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];

                if (xRiskProb != null && dt.Rows[rowPosition][xRiskProb.MyContent.ToString()].ToString() != "")
                {
                    drRiskN[DT_Risk.PROBABILITY] = Convert.ToDecimal(dt.Rows[rowPosition][xRiskProb.MyContent.ToString()]);
                }
                else
                {
                    drRiskN[DT_Risk.PROBABILITY] = 0;
                }

                drRiskN[DT_Risk.IDRISK_FATHER] = drRisk[DT_Risk.ID];
                DataRow drStructure = dsImporting.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                drStructure[DT_RiskStructure.IDRISK] = drRiskN[DT_Risk.ID];
                drStructure[DT_RiskStructure.IDRISK_FATHER] = drRisk[DT_Risk.ID];
                dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(drStructure);

                foreach (var itemDamages in countDamages.OrderBy(x => x.Column))
                {
                    string TopRisk = itemDamages.MyContent;
                    decimal value = 0;
                    if (itemDamages != null && dt.Rows[rowPosition][itemDamages.MyContent.ToString()].ToString() != "")
                    {
                        value = Convert.ToDecimal(dt.Rows[rowPosition][itemDamages.MyContent.ToString()]);
                    }
                    else
                    {
                        value = 0;
                    }
                    DamagesToRisk(dsImporting, TopRisk, drRiskN, value, theDiagram);
                }

                //por cada riesgo, le agrega el rol admin
                AsignRoleAdminToRisk(dsImporting, drRiskN);
                dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRiskN);
                AsignarWBSDefaultToRisk(drRiskN, dsImporting, DsWBS);
            }

        }

        public static void SetRiskStructureInExcel(int rowPosition, bool isCustom, DataSet dsImporting, DataTable dt, HeaderExcelContent xIdRisk, HeaderExcelContent xRiskFather)
        {
            if (xRiskFather != null && dt.Rows[rowPosition][xRiskFather.MyContent.ToString()].ToString() != "" &&
                xIdRisk != null && dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
            {
                decimal idHijo;
                decimal idPadre;
                if (isCustom)
                {
                    idHijo = General.ConvertToDec(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    idPadre = General.ConvertToDec(dt.Rows[rowPosition][xRiskFather.MyContent.ToString()].ToString());
                }
                else
                {
                    idPadre = General.ConvertToDec(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    idHijo = General.ConvertToDec(dt.Rows[rowPosition][xRiskFather.MyContent.ToString()].ToString());
                }
                if (dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).Any())
                {
                    dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).First()[DT_RiskStructure.IDRISK_FATHER] = idPadre;
                }
            }
        }

        public static void SetDataToMainRisk(DataRow drRisk, DataRow theDiagram)
        {
            drRisk[DT_Risk.ID] = 0000;
            drRisk[DT_Risk.NAMESHORT] = "Root " + theDiagram[DT_Diagram.DIAGRAM_NAME];
            drRisk[DT_Risk.COMMENTS] = "Total Risk " + theDiagram[DT_Diagram.DIAGRAM_NAME];
            drRisk[DT_Risk.IS_ROOT] = true;

            drRisk[DT_Risk.ISCOLLAPSED] = false;
            drRisk[DT_Risk.ENABLED] = true;
            drRisk[DT_Risk.FROM_TOP] = false;
            drRisk[DT_Risk.PROBABILITY] = 100;
            drRisk[DT_Risk.POSITION] = 0;
            drRisk[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
        }

        public static void CreateDiagramDamagesExcel(DataSet dsImporting, DataRow drDamage, string Damage, DataRow drDiagram, bool existDamage)
        {
            if (existDamage)
            {
                DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].NewRow();//y creo un nuevo diagrama con los daños que acabo de crear
                drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = drDamage[DT_Damage.ID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.COLOR] = drDamage[DT_Damage.COLORID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
                drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drDamage_Diagram[DT_Diagram_Damages.TOP_RISK] = Damage;
                dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Add(drDamage_Diagram);
            }
            else
            {
                DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].NewRow();//y creo un nuevo diagrama con los daños que acabo de crear
                drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.ID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.COLOR] = dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.COLORID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
                drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drDamage_Diagram[DT_Diagram_Damages.TOP_RISK] = Damage;
                dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Add(drDamage_Diagram);
            }

        }

        public static void AsignRoleAdminToRisk(DataSet dsImporting, DataRow drRisk)
        {
            if (!(dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], 101 })))
            {
                DataRow drRiskRole = dsImporting.Tables[DT_Role_Risk.TABLENAME].NewRow();
                drRiskRole[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                drRiskRole[DT_Role_Risk.IDROL_COLUMN] = 101;
                dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
            }
        }

        public static void AsignRoleToCM(DataSet dsImporting, DataRow drCM)
        {
            if (!(dsImporting.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], 101 })))
            {
                DataRow drRoleCM = dsImporting.Tables[DT_Role_CM.TABLENAME].NewRow();
                drRoleCM[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                drRoleCM[DT_Role_CM.IDROL_COLUMN] = 101;
                dsImporting.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRoleCM);
            }
        }

        public static void DamagesToMainRisk(DataSet dsImporting, DataRow drRisk, DataRow drDiagram)
        {
            foreach (DataRow itemDiagramDamage in dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + drDiagram[DT_Diagram.ID_DIAGRAM]))
            {
                //darle valor 0, porque es la raiz
                DataRow drRiskDamage = dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = itemDiagramDamage[DT_Diagram_Damages.ID_DAMAGE];
                drRiskDamage[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
            }
        }

        public static void DamagesToCM(DataSet dsImporting, string Damage, DataRow drCM, DataRow drDiagram)
        {
            DataRow drCM_Damage = dsImporting.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
            drCM_Damage[DT_CounterM_Damage.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.ID_COLUMNA];
            drCM_Damage[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
            drCM_Damage[DT_CounterM_Damage.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drCM_Damage[DT_CounterM_Damage.VALUE] = 0;
            dsImporting.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(drCM_Damage);
        }

        public static void DamagesToRisk(DataSet dsImporting, string Damage, DataRow drRiskN, decimal value, DataRow drDiagram)
        {
            DataRow drRiskDamageN = dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
            drRiskDamageN[DT_Risk_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.ID_COLUMNA];
            drRiskDamageN[DT_Risk_Damages.ID_RISK] = drRiskN[DT_Risk.ID];
            drRiskDamageN[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drRiskDamageN[DT_Risk_Damages.VALUE] = value;
            dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamageN);
        }

        public static void AsignarWBSDefaultToRisk(DataRow drRisk, DataSet DsMain, DataSet DsWBS)
        {
            bool primary = true;
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
            {
                if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(DsMain.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRiskWBS = DsMain.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        drRiskWBS[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.WBS] = item[DT_WBS.WBS_NAME];
                        drRiskWBS[DT_RISK_WBS.NIVEL] = item[DT_WBS.NIVEL];
                        drRiskWBS[DT_RISK_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                        primary = false;
                        DsMain.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                    }
                }
            }
        }

        public static void AsignarWBSDefaultToCM(DataRow drCM, DataSet DsMain, DataSet DsWBS)
        {
            bool primary = true;
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(DsMain.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRCMWBS = DsMain.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                        drRCMWBS[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                        drRCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRCMWBS[DT_CM_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drRCMWBS[DT_CM_WBS.IS_PRIMARY] = primary;
                        drRCMWBS[DT_CM_WBS.PROBABILITY] = drCM[DT_CounterM.PROBABILITY];
                        primary = false;
                        DsMain.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(drRCMWBS);
                    }
                }
            }
        }
        #endregion

        #region ExportExcel

        #endregion
    }
}
