using DataMapping.Data;
using EnsureBusinesss;
using EnsureRisk.Classess;
using EnsureRisk.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsureRisk;

namespace EnsureRisk.Controllers.Import
{
    public class ImportFromExcel : IDisposable
    {
        #region Disposing
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

        private DataSet dsImporting, DsWBS, MainDS;
        private DataRow drDiagram;
        private decimal IdProject;
        private bool isMarkedAll, IsCustom;
        private DataTable dtExcel;
        private string keyPhrase;
        private List<HeaderExcelContent> MyList;


        public ImportFromExcel(DataSet dsImporting, DataSet MainDS, decimal IdProject, DataRow drDiagram, DataSet DsWBS, 
            bool isMarkedAll, bool IsCustom, string keyPhrase, DataTable dtExcel, List<HeaderExcelContent> MyList)
        {
            this.MainDS = MainDS;
            this.dsImporting = dsImporting;
            this.IdProject = IdProject;
            this.drDiagram = drDiagram;
            this.DsWBS = DsWBS;
            this.isMarkedAll = isMarkedAll;
            this.IsCustom = IsCustom;
            this.keyPhrase = keyPhrase;
            this.dtExcel = dtExcel;
            this.MyList = MyList;
        }

        public void ImportExcel(BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            if (!e.Cancel)
            {
                ServiceTopRiskController.WebServiceTopRisk wstop = new ServiceTopRiskController.WebServiceTopRisk();
                dsImporting.Tables[DT_Damage.TABLE_NAME].Merge(wstop.GetAllTopRisk().Tables[DT_Damage.TABLE_NAME]);
                wstop.Dispose();
                drDiagram[DT_Diagram.DIAGRAM_NAME] = "Imported Diagram at " + DateTime.Now;
                drDiagram[DT_Diagram.ID_DIAGRAM] = 0000;
                drDiagram[DT_Diagram.ID_PROJECT] = IdProject;
                dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);
                IEnumerable<HeaderExcelContent> countDamages = MyList.Where(x => x.IdClasification == (int)EnumExcelValue.RiskDamage);//Damages has 10 as ID
                ClasifyAndCreateDamages(dsImporting, drDiagram, countDamages);
                //Busco el diagrama que acabo de insertar, para agregarle el riesgo padre, para agregarle los riesgos y sus dannos
                DataRow theDiagram = dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Find(0000);
                //creo un riesgo root
                DataRow drRisk = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
                SetDataToMainRisk(drRisk, theDiagram);
                dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                //Asignarle al riesgo Root el rol admin
                AsignRoleAdminToLine(dsImporting, drRisk);

                //por cada daño del diagrama
                DamagesToMainRisk(dsImporting, drRisk, theDiagram);

                //Recorrer el Excel solo para llenar los riesgos
                HeaderExcelContent xIdRisk = FillDataRisk(dsImporting, IsCustom, dtExcel, MyList, countDamages, theDiagram, drRisk, DsWBS);
                FillWBSFromExcel(dsImporting, dtExcel, MyList, DsWBS);
                SetRisk_RiskFatherRelation(dsImporting, IsCustom, dtExcel, MyList, xIdRisk);
                SettingWBSInRisks(dsImporting, DsWBS);
                FillCM_Data(dsImporting, keyPhrase, dtExcel, MyList, countDamages, theDiagram, xIdRisk, DsWBS, isMarkedAll);
                WBSOperations.AddWBSTopToDiagram(dsImporting, (decimal)drDiagram[DT_Diagram.ID_DIAGRAM], DsWBS);
                TreeOperation.SetDiagramImportedPositions(dsImporting, (decimal)drDiagram[DT_Diagram.ID_DIAGRAM]);
                using (ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk())
                {
                    MainDS.Merge(ws.SaveRisk(dsImporting), true, MissingSchemaAction.Ignore);
                    MainDS.AcceptChanges();
                }
            }
            else
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

      
        /// <summary>
        /// Classify  the Damages in the file, validating if the damage exist or not in the global list of Damages
        /// </summary>
        /// <param name="dsImporting">The Dataset with all data</param>
        /// <param name="drDiagram">The data of the diagram created</param>
        /// <param name="countDamages">A list of all Damages detected in the file</param>
        public void ClasifyAndCreateDamages(DataSet dsImporting, DataRow drDiagram, IEnumerable<HeaderExcelContent> countDamages)
        {
            int colorvariant = 1;
            foreach (var itemDamage in countDamages)
            {
                string DamageName = itemDamage.MyContent;
                List<string> DamageUM = DamageNameParsing(DamageName);
                if (dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DamageUM[0] + "' and " + DT_Damage.UM + " = '" + DamageUM[1] + "'").Any())
                {
                    CreateDiagramDamagesExcel(dsImporting, dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DamageUM[0] + "' and " + DT_Damage.UM + " = '" + DamageUM[1] + "'")
                        .First(), drDiagram, itemDamage);
                }
                else
                {
                    CreateDiagramDamagesExcel(dsImporting, CreateNewDamageFromDamageName(dsImporting, ref colorvariant, DamageUM), drDiagram, itemDamage);
                }               
            }
        }

        /// <summary>
        /// Create the data in the relation Diagram-Damages. Validate if the damage is created or an existing Damage.
        /// </summary>
        /// <param name="dsImporting">The Dataset of all data</param>
        /// <param name="drDamage">The Damage datarow</param>
        /// <param name="Damage">The name of the Damage in the </param>
        /// <param name="drDiagram">The diagram data created</param>
        public void CreateDiagramDamagesExcel(DataSet dsImporting, DataRow drDamage, DataRow drDiagram, HeaderExcelContent damageContent)
        {
            DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].NewRow();//create a new Diagram with the Created Damages
            drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = drDamage[DT_Damage.ID_COLUMNA];
            drDamage_Diagram[DT_Diagram_Damages.COLOR] = drDamage[DT_Damage.COLORID_COLUMNA];
            drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
            drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drDamage_Diagram[DT_Diagram_Damages.TOP_RISK] = drDamage[DT_Damage.TOP_RISK_COLUMN];
            drDamage_Diagram[DT_Diagram_Damages.UM] = drDamage[DT_Damage.UM];
            drDamage_Diagram[DT_Diagram_Damages.DAMAGE] = drDamage[DT_Damage.TOP_RISK_COLUMN].ToString() + "(" + drDamage[DT_Damage.UM].ToString() + ")";
            dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Add(drDamage_Diagram);
            damageContent.MyContent = drDamage_Diagram[DT_Diagram_Damages.DAMAGE].ToString();
        }
        


        private List<string> DamageNameParsing(string damageName)
        {
            List<string> damageSplit = new List<string>();
            int startindex = 0;
            int endIndex = 0;
            int index = 0;
            foreach (var item in damageName)
            {
                index++;
                if (item == '(' || item == '[')
                {
                    startindex = index;
                }
                if (item == ')' || item == ']')
                {
                    endIndex = index;
                }
            }

            if (endIndex == 0 || startindex == 0)
            {
                damageSplit.Add(damageName);
                damageSplit.Add("UM");
            }
            else
            {
                if (startindex > 0)
                {
                    damageSplit.Add(damageName.Substring(0, startindex - 1));
                }
                if (endIndex > 0)
                {
                    damageSplit.Add(damageName.Substring(startindex, endIndex - (startindex + 1)));
                }
            }

            return damageSplit;
        }

        /// <summary>
        /// Create new damages from the damages that exist in the excel file, who doesn't appears in the Damage Global list.
        /// </summary>
        /// <param name="dsImporting">The data of the Damages, Risk and Cm in the excel</param>
        /// <param name="colorvariant">A numeric flag to know which color select</param>
        /// <param name="DamageName">The name of the Damage in the file</param>
        /// <returns>Return the new Datarow with the data of the Damage created</returns>
        private DataRow CreateNewDamageFromDamageName(DataSet dsImporting, ref int colorvariant, List<string> DamageUM)
        {
            DataRow drDamage = dsImporting.Tables[DT_Damage.TABLE_NAME].NewRow();//creo un nuevo daño
            drDamage[DT_Damage.TOP_RISK_COLUMN] = DamageUM[0];
            int[] R = new int[] { 255, 220, 40, 80, 54, 144, 54, 144, 158 };
            int[] G = new int[] { 50, 10, 150, 200, 54, 54, 158, 158, 135 };
            int[] B = new int[] { 60, 150, 25, 99, 158, 158, 130, 54, 54 };
            if (colorvariant < 10)
            {
                System.Windows.Media.Color color = new System.Windows.Media.Color
                {
                    R = Convert.ToByte(R[colorvariant]),
                    G = Convert.ToByte(G[colorvariant]),
                    B = Convert.ToByte(B[colorvariant]),
                    A = Convert.ToByte(255)
                };
                drDamage[DT_Damage.COLORID_COLUMNA] = color.ToString();
                colorvariant++;
            }
            else
            {
                drDamage[DT_Damage.COLORID_COLUMNA] = "#FF0000FF";
            }
            drDamage[DT_Damage.UM] = DamageUM[1];
            dsImporting.Tables[DT_Damage.TABLE_NAME].Rows.Add(drDamage);
            return drDamage;
        }

        /// <summary>
        /// Set to the Main risk in the Diagram, the automatic Data
        /// </summary>
        public void SetDataToMainRisk(DataRow drRisk, DataRow theDiagram)
        {
            drRisk[DT_Risk.ID] = 0000;
            drRisk[DT_Risk.NAMESHORT] = "Main Risk";
            drRisk[DT_Risk.COMMENTS] = "Main Risk of " + theDiagram[DT_Diagram.DIAGRAM_NAME];
            drRisk[DT_Risk.IS_ROOT] = true;
            drRisk[DT_Risk.IS_CM] = false;
            drRisk[DT_Risk.ISCOLLAPSED] = false;
            drRisk[DT_Risk.IS_ACTIVE] = true;
            drRisk[DT_Risk.PROBABILITY] = 100;
            drRisk[DT_Risk.POSITION] = 0;
            drRisk[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
        }

        /// <summary>
        /// Add to Main Risk, the Damages of the Diagram Created
        /// </summary>
        public void DamagesToMainRisk(DataSet dsImporting, DataRow drRisk, DataRow drDiagram)
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

        private decimal IDWBSByLevelAndName(DataSet DsWBS, string wbsToEvaluate)
        {
            decimal idToReturn = 0;
            foreach (DataRow rowWBS in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (wbsToEvaluate.Trim() == rowWBS[DT_WBS.NIVEL].ToString() + "" + rowWBS[DT_WBS.WBS_NAME].ToString())
                {
                    idToReturn = (decimal)rowWBS[DT_WBS.ID_WBS];
                }
            }
            return idToReturn;
        }

        private void FillWBSFromExcel(DataSet dsImporting, DataTable dtExcel, List<HeaderExcelContent> MyList, DataSet DsWBS)
        {
            var xWBS = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.WBSColumn);
            var xIdRisk = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.IdRisk);
            for (int rowPosition = 0; rowPosition < dtExcel.Rows.Count; rowPosition++)
            {
                //Si la linea en la columna de WBS no esta vacia
                if (xWBS != null && !string.IsNullOrWhiteSpace(dtExcel.Rows[rowPosition][xWBS.MyContent].ToString()))
                {
                    //Buscando WBS que coincida con nivel y nombre
                    decimal idWBS = IDWBSByLevelAndName(DsWBS, dtExcel.Rows[rowPosition][xWBS.MyContent.ToString()].ToString());
                    if (idWBS != 0)
                    {
                        decimal idRisk = 0;
                        if (xIdRisk != null && !string.IsNullOrWhiteSpace(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString()))
                        {
                            idRisk = General.ConvertStringID_inDecimal(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                        }
                        else
                        {
                            idRisk = (decimal)dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows[dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Count - 1][DT_RISK_WBS.ID_RISK];
                        }
                        if (!(dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { idRisk, idWBS })))
                        {
                            DataRow drRiskWBS = dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                            drRiskWBS[DT_RISK_WBS.ID_RISK] = idRisk;
                            drRiskWBS[DT_RISK_WBS.ID_WBS] = idWBS;
                            drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                            drRiskWBS[DT_RISK_WBS.PROBABILITY] = dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRisk)[DT_Risk.PROBABILITY];
                            dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                        }
                        foreach (DataRow itemAncestors in WBSOperations.GetAncestors(idWBS, DsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), DsWBS).Rows)
                        {
                            if (!(dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { idRisk, itemAncestors[DT_WBS.ID_WBS] })))
                            {
                                DataRow drRiskWBS = dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                drRiskWBS[DT_RISK_WBS.ID_RISK] = idRisk;
                                drRiskWBS[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                if (!(WBSOperations.HasParent((decimal)itemAncestors[DT_WBS.ID_WBS],DsWBS)))//pregunto si es el mas al tope y le doy la llave
                                {
                                    drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = true;
                                }
                                else
                                {
                                    drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                                }
                                drRiskWBS[DT_RISK_WBS.PROBABILITY] = dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRisk)[DT_Risk.PROBABILITY];
                                dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                            }
                        }
                    }                   
                }
            }
        }

        private void SettingWBSInRisks(DataSet dsImporting, DataSet DsWBS)
        {
            foreach (DataRow riskRow in dsImporting.Tables[DT_Risk.TABLE_NAME].Rows)
            {
                DataRow[] riskWBSData = dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + riskRow[DT_Risk.ID]);
                if (!(riskWBSData.Any()))
                {
                    SetTopWBSToLine(riskRow, dsImporting, DsWBS);//Se añaden los TopWBS al riesgo mas el damage
                }
                else
                {
                    foreach (DataRow rowRiskWBS in riskWBSData)
                    {
                        if (WBSOperations.IsRiskWBSLow(rowRiskWBS,DsWBS,dsImporting.Tables[DT_RISK_WBS.TABLE_NAME]))
                        {
                            foreach (var drRiskDamage in dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                            {
                                var drRiskWBSDamage = dsImporting.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = drRiskDamage[DT_Risk_Damages.DAMAGE];
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = drRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = riskRow[DT_Risk.ID];
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = rowRiskWBS[DT_RISK_WBS.ID_WBS];
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = drRiskDamage[DT_Risk_Damages.VALUE];
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = rowRiskWBS[DT_RISK_WBS.WBS];
                                drRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS_USER] = rowRiskWBS[DT_RISK_WBS.WBS_USER];
                                dsImporting.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drRiskWBSDamage);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the data of the Datatable created from excel (dtExcel) and creates new risks with these data
        /// </summary>
        /// <param name="dsImporting">The Dataset with all Data</param>
        /// <param name="isCustom">A flag for custom excel file</param>
        /// <param name="dtExcel">The data loaded from excel as Datatable</param>
        /// <param name="countDamages">The list of Damages</param>
        /// <param name="theDiagram">The data of the diagram created</param>
        /// <param name="drRisk">The Main Risk Data</param>
        /// <param name="DsWBS">The Dataset with the WBS</param>
        /// <returns></returns>
        public HeaderExcelContent FillDataRisk(DataSet dsImporting, bool isCustom, DataTable dtExcel, List<HeaderExcelContent> MyList,
            IEnumerable<HeaderExcelContent> countDamages, DataRow theDiagram, DataRow drRisk, DataSet DsWBS)
        {
            //TODO: Pon esa cosa horrorosa aqui o veras
            var xIdRisk = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.IdRisk);
            var xRiskShortName = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.RiskShortName);
            var xRiskDetail = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.RiskComments);
            var xRiskEnabled = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.RiskEnabled);
            var xRiskProb = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.RiskProbability);
            var xWBS = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.WBSColumn);

            for (int rowPosition = 0; rowPosition < dtExcel.Rows.Count; rowPosition++)
            {
                DataRow drRiskN = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();

                if (xIdRisk != null && dtExcel.Rows[rowPosition][xIdRisk.MyContent].ToString() != "")
                {
                    drRiskN[DT_Risk.ID] = General.ConvertStringID_inDecimal(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    drRiskN[DT_Risk.IS_CM] = false;
                    if (isCustom)
                    {                        
                        if (xRiskShortName != null && dtExcel.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString() != "")
                        {
                            drRiskN[DT_Risk.NAMESHORT] = dtExcel.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString();
                        }
                        if (xRiskDetail != null && dtExcel.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString() != "")
                        {
                            drRiskN[DT_Risk.COMMENTS] = dtExcel.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString();
                        }
                    }
                    else
                    {
                        if (dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
                        {
                            drRiskN[DT_Risk.NAMESHORT] = dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString();
                        }
                        if (dtExcel.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString() != "")
                        {
                            drRiskN[DT_Risk.NAMESHORT] = drRiskN[DT_Risk.NAMESHORT] + " " + dtExcel.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString();
                        }
                        if (xRiskDetail != null && dtExcel.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString() != "")
                        {
                            drRiskN[DT_Risk.COMMENTS] = dtExcel.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString();
                        }
                        drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
                    }
                    drRiskN[DT_Risk.IS_ACTIVE] = true;
                    drRiskN[DT_Risk.IS_ROOT] = false;
                    drRiskN[DT_Risk.ISCOLLAPSED] = false;
                    drRiskN[DT_Risk.POSITION] = 0;
                    drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];

                    if (xRiskProb != null && dtExcel.Rows[rowPosition][xRiskProb.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.PROBABILITY] = Convert.ToDecimal(dtExcel.Rows[rowPosition][xRiskProb.MyContent.ToString()]);
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
                        string DamageWithUM = itemDamages.MyContent;
                        decimal value = 0;
                        if (itemDamages != null && dtExcel.Rows[rowPosition][itemDamages.Column].ToString() != "")
                        {
                            value = Convert.ToDecimal(dtExcel.Rows[rowPosition][itemDamages.Column]);
                        }
                        else
                        {
                            value = 0;
                        }
                        DamagesToLine(dsImporting, DamageWithUM, drRiskN, value, theDiagram);
                    }
                    AsignRoleAdminToLine(dsImporting, drRiskN);
                    dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRiskN);
                    //SetTopWBSToLine(drRiskN, dsImporting, DsWBS);//esta linea la comento, pues luego se agregan los WBS en el Metodo FillWBSFromExcel
                }
            }
            return xIdRisk;
        }

        public void SetRisk_WBS_DamageValue(DataRow drRisk, DataSet dsImporting)
        {
            foreach (var drRiskDamage in dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + drRisk[DT_Risk.ID]))
            {
                foreach (var drRiskWBS in dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRisk[DT_Risk.ID]))
                {
                    var drRiskWBSDamage = dsImporting.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = drRiskDamage[DT_Risk_Damages.DAMAGE];
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = drRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = drRiskWBS[DT_RISK_WBS.ID_WBS];
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = drRiskDamage[DT_Risk_Damages.VALUE];
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = drRiskWBS[DT_RISK_WBS.WBS];
                    drRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS_USER] = drRiskWBS[DT_RISK_WBS.WBS_USER];
                    dsImporting.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drRiskWBSDamage);
                }
            }
        }

        /// <summary>
        /// Give the right RiskFather to a Risk.
        /// </summary>
        /// <param name="dsImporting">The Dataset with all data</param>
        /// <param name="isCustom">If the file is custom file</param>
        /// <param name="dtExcel">The table created by the file data</param>
        /// <param name="xIdRisk">The risk Father</param>
        public void SetRisk_RiskFatherRelation(DataSet dsImporting, bool isCustom, DataTable dtExcel, 
            List<HeaderExcelContent> MyList, HeaderExcelContent xIdRisk)
        {
            var xRiskFather = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.IdParentRisk);
            for (int rowposition = 0; rowposition < dtExcel.Rows.Count; rowposition++)
            {//ajustando estructura
                if (xRiskFather != null && dtExcel.Rows[rowposition][xRiskFather.MyContent.ToString()].ToString() != "" &&
                xIdRisk != null && dtExcel.Rows[rowposition][xIdRisk.MyContent.ToString()].ToString() != "")
                {
                    decimal idHijo;
                    decimal idPadre;
                    if (isCustom)
                    {
                        idHijo = General.ConvertStringID_inDecimal(dtExcel.Rows[rowposition][xIdRisk.MyContent.ToString()].ToString());
                        idPadre = General.ConvertStringID_inDecimal(dtExcel.Rows[rowposition][xRiskFather.MyContent.ToString()].ToString());
                    }
                    else
                    {
                        idPadre = General.ConvertStringID_inDecimal(dtExcel.Rows[rowposition][xIdRisk.MyContent.ToString()].ToString());
                        idHijo = General.ConvertStringID_inDecimal(dtExcel.Rows[rowposition][xRiskFather.MyContent.ToString()].ToString());
                    }
                    if (dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).Any())
                    {
                        dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).First()[DT_RiskStructure.IDRISK_FATHER] = idPadre;
                        dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Find(idHijo)[DT_Risk.IDRISK_FATHER] = idPadre;
                    }
                }
            }
        }

        /// <summary>
        /// Load the data of the Datatable created from excel (dtExcel) and creates new CMs with these data
        /// </summary>
        /// <param name="dsImporting">The Dataset with all data</param>
        /// <param name="statusKeyphrase">The text inserted by user as No Active </param>
        /// <param name="dtExcel">The data loaded from excel as Datatable</param>
        /// <param name="countDamages">The list of Damages</param>
        /// <param name="theDiagram">The data of the diagram created</param>
        /// <param name="xIdRisk">Data of the risk father</param>
        /// <param name="DsWBS">Dataset of the WBS</param>
        public void FillCM_Data(DataSet dsImporting, string statusKeyphrase, DataTable dtExcel, 
            List<HeaderExcelContent> MyList, IEnumerable<HeaderExcelContent> countDamages, DataRow theDiagram, 
            HeaderExcelContent xIdRisk, DataSet DsWBS, bool isMarkedAllAsActive)
        {
            HeaderExcelContent xCmShort = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.CMShortName);
            var xCmDetail = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.CM_Comments);
            var xCmReduction = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.CM_RiskReduction);
            var xCmActive = MyList.FindLast(x => x.IdClasification == (int)EnumExcelValue.CM_Status);
            for (int rowPosition = 0; rowPosition < dtExcel.Rows.Count; rowPosition++)
            {
                if (xCmShort != null && dtExcel.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString() != "")
                {
                    DataRow drCM = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
                    drCM[DT_Risk.NAMESHORT] = dtExcel.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString();
                    drCM[DT_Risk.IS_CM] = true;
                    drCM[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
                    if (xCmDetail != null && dtExcel.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString() != "")
                    {
                        drCM[DT_Risk.COMMENTS] = dtExcel.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString();
                    }

                    drCM[DT_Risk.IS_ACTIVE] = true;
                    drCM[DT_Risk.DIAGONAL] = false;
                    drCM[DT_Risk.POSITION] = 0;
                    if (xCmReduction != null && dtExcel.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString() != "")
                    {
                        drCM[DT_Risk.PROBABILITY] = dtExcel.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString();
                    }
                    else
                    {
                        drCM[DT_Risk.PROBABILITY] = 0;
                    }
                    if (xIdRisk != null && !string.IsNullOrWhiteSpace(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString()))
                    {
                        drCM[DT_Risk.IDRISK_FATHER] = General.ConvertStringID_inDecimal(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    }
                    else
                    {
                        drCM[DT_Risk.IDRISK_FATHER] = dsImporting.Tables[DT_Risk.TABLE_NAME].Rows[dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Count - 1][DT_Risk.IDRISK_FATHER];
                    }
                    
                    DataRow drStructure = dsImporting.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    drStructure[DT_RiskStructure.IDRISK] = drCM[DT_Risk.ID];
                    drStructure[DT_RiskStructure.IDRISK_FATHER] = drCM[DT_Risk.IDRISK_FATHER];
                    dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(drStructure);

                    if (isMarkedAllAsActive)
                    {
                        drCM[DT_Risk.IS_ACTIVE] = true;
                    }
                    else
                    {

                        if (xCmActive != null && dtExcel.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString() != "")
                        {
                            if (statusKeyphrase == dtExcel.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString())
                            {
                                drCM[DT_Risk.IS_ACTIVE] = false;
                            }
                            else
                            {
                                drCM[DT_Risk.IS_ACTIVE] = true;
                            }
                        }
                        else
                        {
                            drCM[DT_Risk.IS_ACTIVE] = true;
                        }
                    }
                    foreach (var itemDamages in countDamages.OrderBy(x => x.Column))
                    {
                        string TopRisk = itemDamages.MyContent;
                        DamagesToLine(dsImporting, TopRisk, drCM, 0, theDiagram);
                    }
                    AsignRoleAdminToLine(dsImporting, drCM);
                    dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drCM);
                    SetTopWBSToLine(drCM, dsImporting, DsWBS);
                }
            }
        }

        /// <summary>
        /// Add to Risk the Damages form the Diagram
        /// </summary>
        public void DamagesToLine(DataSet dsImporting, string Damage, DataRow drRiskN, decimal value, DataRow drDiagram)
        {
            DataRow drRiskDamageN = dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
            drRiskDamageN[DT_Risk_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.DAMAGE + " = '" + Damage + "'").First()[DT_Diagram_Damages.ID_DAMAGE];
            drRiskDamageN[DT_Risk_Damages.ID_RISK] = drRiskN[DT_Risk.ID];
            drRiskDamageN[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drRiskDamageN[DT_Risk_Damages.VALUE] = value;
            dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamageN);
        }
        
        /// <summary>
        /// Set to line The Admin Role
        /// </summary>
        public void AsignRoleAdminToLine(DataSet dsImporting, DataRow drRisk)
        {
            if (!(dsImporting.Tables[DT_Role_Risk.TABLE_NAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], 101 })))
            {
                DataRow drRiskRole = dsImporting.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                drRiskRole[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                drRiskRole[DT_Role_Risk.IDROL_COLUMN] = 101;
                dsImporting.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(drRiskRole);
            }
        }

        /// <summary>
        /// Set to Risk the Top WBS in the Project
        /// </summary>
        public void SetTopWBSToLine(DataRow drRisk, DataSet dsImporting, DataSet DsWBS)
        {
            bool primary = true;
            foreach (var item in WBSOperations.GetTopWBS(DsWBS))
            {
                if (!(dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_WBS.ID_WBS] })))
                {
                    DataRow drRiskWBS = dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    drRiskWBS[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                    drRiskWBS[DT_RISK_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                    drRiskWBS[DT_RISK_WBS.WBS] = item[DT_WBS.WBS_NAME];
                    drRiskWBS[DT_RISK_WBS.NIVEL] = item[DT_WBS.NIVEL];
                    drRiskWBS[DT_RISK_WBS.USERNAME] = item[DT_WBS.USERNAME];
                    drRiskWBS[DT_RISK_WBS.WBS_USER] = item[DT_WBS.WBS_NAME] + "[" + item[DT_WBS.USERNAME] + "]";
                    drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                    drRiskWBS[DT_RISK_WBS.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                    primary = false;
                    dsImporting.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                }
            }
            SetRisk_WBS_DamageValue(drRisk, dsImporting);
        }
    }
}
