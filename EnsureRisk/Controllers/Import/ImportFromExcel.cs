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

        private DataSet dsImporting, DsWBS;
        private DataRow drDiagram;
        private decimal IdProject;
        private bool isMarkedAll, IsCustom;
        private DataTable dtExcel;
        private string keyPhrase;
        private List<HeaderExcelContent> MyList;


        public ImportFromExcel(DataSet dsImporting, decimal IdProject, DataRow drDiagram, DataSet DsWBS, 
            bool isMarkedAll, bool IsCustom, string keyPhrase, DataTable dtExcel, List<HeaderExcelContent> MyList)
        {
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
           ServiceTopRiskController.WebServiceTopRisk wstop = new ServiceTopRiskController.WebServiceTopRisk();
            dsImporting.Tables[DT_Damage.TABLE_NAME].Merge(wstop.GetAllTopRisk().Tables[DT_Damage.TABLE_NAME]);
            wstop.Dispose();
            drDiagram[DT_Diagram.DIAGRAM_NAME] = "Imported Diagram at " + DateTime.Now;
            drDiagram[DT_Diagram.ID_DIAGRAM] = 0000;
            drDiagram[DT_Diagram.ID_PROJECT] = IdProject;
            dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);
            IEnumerable<HeaderExcelContent> countDamages = MyList.Where(x => x.IdClasification == 10);//Damages has 10 as ID
            ClasifyAndCreateDamages(dsImporting, drDiagram, countDamages);
            //Busco el diagrama que acabo de insertar, para agregarle el riesgo padre, para agregarle los riesgos y sus dannos
            DataRow theDiagram = dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Find(0000);
            //creo un riesgo root
            DataRow drRisk = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
            SetDataToMainRisk(drRisk, theDiagram);
            dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

            //Asignarle al riesgo Root el rol admin
            AsignRoleAdminToRisk(dsImporting, drRisk);

            //por cada daño del diagrama
            DamagesToMainRisk(dsImporting, drRisk, theDiagram);

            //Recorrer el Excel solo para llenar los riesgos
            HeaderExcelContent xIdRisk = FillDataRisk(dsImporting, IsCustom, dtExcel, MyList, countDamages, theDiagram, drRisk, DsWBS);
            SetRisk_RiskFatherRelation(dsImporting, IsCustom, dtExcel, MyList, xIdRisk);
            FillCM_Data(dsImporting, IsCustom, keyPhrase, dtExcel, MyList, countDamages, theDiagram, xIdRisk, DsWBS, isMarkedAll);
            WBSOperations.AddWBSTopToDiagram(dsImporting, (decimal)drDiagram[DT_Diagram.ID_DIAGRAM], DsWBS);
            TreeOperation.SetDiagramImportedPositions(dsImporting, (decimal)drDiagram[DT_Diagram.ID_DIAGRAM]);
            if (dsImporting.HasChanges())
            {
                using (ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk())
                {
                    DataSet temp = dsImporting.GetChanges();
                    temp = ws.SaveRisk(temp);
                    dsImporting.Merge(temp);
                    dsImporting.AcceptChanges();
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
            foreach (var itemDamages in countDamages)
            {
                string DamageName = itemDamages.MyContent;
                List<string> DamageUM = DamageNameParsing(DamageName);
                if (dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DamageUM[0] + "' and " + DT_Damage.UM + " = '" + DamageUM[1] + "'").Any())
                {
                    CreateDiagramDamagesExcel(dsImporting, dsImporting.Tables[DT_Damage.TABLE_NAME].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DamageUM[0] + "' and " + DT_Damage.UM + " = '" + DamageUM[1] + "'")
                        .First(), drDiagram);
                }
                else
                {
                    CreateDiagramDamagesExcel(dsImporting, CreateNewDamageFromDamageName(dsImporting, ref colorvariant, DamageUM), drDiagram);
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
        public void CreateDiagramDamagesExcel(DataSet dsImporting, DataRow drDamage, DataRow drDiagram)
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


        /// <summary>
        /// Load the data of the Datatable created from excel (dtExcel) and creates new risks with these data
        /// </summary>
        /// <param name="dsImporting">The Dataset with all Data</param>
        /// <param name="isCustom">A flag for custom excel file</param>
        /// <param name="isActiveKeyword">The text inserted by user as No Active </param>
        /// <param name="dtExcel">The data loaded from excel as Datatable</param>
        /// <param name="whc">The header parameters</param>
        /// <param name="countDamages">The list of Damages</param>
        /// <param name="theDiagram">The data of the diagram created</param>
        /// <param name="drRisk">The Main Risk Data</param>
        /// <param name="DsWBS">The Dataset with the WBS</param>
        /// <returns></returns>
        public HeaderExcelContent FillDataRisk(DataSet dsImporting, bool isCustom, DataTable dtExcel, List<HeaderExcelContent> MyList,
            IEnumerable<HeaderExcelContent> countDamages, DataRow theDiagram, DataRow drRisk, DataSet DsWBS)
        {
            var xIdRisk = MyList.FindLast(x => x.IdClasification == 1);
            var xRiskShortName = MyList.FindLast(x => x.IdClasification == 2);
            var xRiskDetail = MyList.FindLast(x => x.IdClasification == 3);
            var xRiskEnabled = MyList.FindLast(x => x.IdClasification == 4);
            var xRiskProb = MyList.FindLast(x => x.IdClasification == 11);
            for (int rowPosition = 0; rowPosition < dtExcel.Rows.Count; rowPosition++)
            {
                DataRow drRiskN = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();

                if (xIdRisk != null && dtExcel.Rows[rowPosition][xIdRisk.MyContent].ToString() != "")
                {
                    drRiskN[DT_Risk.ID] = General.ConvertToDec(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
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
                    drRiskN[DT_Risk.ENABLED] = true;
                    drRiskN[DT_Risk.IS_ROOT] = false;
                    drRiskN[DT_Risk.ISCOLLAPSED] = false;
                    drRiskN[DT_Risk.POSITION] = 0;
                    drRiskN[DT_Risk.FROM_TOP] = false;
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
                        if (itemDamages != null && dtExcel.Rows[rowPosition][itemDamages.MyContent.ToString()].ToString() != "")
                        {
                            value = Convert.ToDecimal(dtExcel.Rows[rowPosition][itemDamages.MyContent.ToString()]);
                        }
                        else
                        {
                            value = 0;
                        }
                        DamagesToRisk(dsImporting, DamageWithUM, drRiskN, value, theDiagram);
                    }
                    AsignRoleAdminToRisk(dsImporting, drRiskN);
                    dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRiskN);
                    SetTopWBSToRisk(drRiskN, dsImporting, DsWBS);
                }
            }

            return xIdRisk;
        }

        /// <summary>
        /// Add to Risk the Damages form the Diagram
        /// </summary>
        public void DamagesToRisk(DataSet dsImporting, string Damage, DataRow drRiskN, decimal value, DataRow drDiagram)
        {
            DataRow drRiskDamageN = dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
            drRiskDamageN[DT_Risk_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.DAMAGE + " = '" + Damage + "'").First()[DT_Diagram_Damages.ID_DAMAGE];
            drRiskDamageN[DT_Risk_Damages.ID_RISK] = drRiskN[DT_Risk.ID];
            drRiskDamageN[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drRiskDamageN[DT_Risk_Damages.VALUE] = value;
            dsImporting.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamageN);
        }

        /// <summary>
        /// Set to risk The Admin Role
        /// </summary>
        public void AsignRoleAdminToRisk(DataSet dsImporting, DataRow drRisk)
        {
            if (!(dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], 101 })))
            {
                DataRow drRiskRole = dsImporting.Tables[DT_Role_Risk.TABLENAME].NewRow();
                drRiskRole[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                drRiskRole[DT_Role_Risk.IDROL_COLUMN] = 101;
                dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
            }
        }

        /// <summary>
        /// Set to Risk the Top WBS in the Project
        /// </summary>
        public void SetTopWBSToRisk(DataRow drRisk, DataSet dsImporting, DataSet DsWBS)
        {
            bool primary = true;
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
            {
                if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
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
            }
            SetRisk_WBS_DamageValue(drRisk, dsImporting);
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
        /// <param name="whc">The excel headers</param>
        /// <param name="xIdRisk">The risk Father</param>
        public void SetRisk_RiskFatherRelation(DataSet dsImporting, bool isCustom, DataTable dtExcel, 
            List<HeaderExcelContent> MyList, HeaderExcelContent xIdRisk)
        {
            var xRiskFather = MyList.FindLast(x => x.IdClasification == 5);
            for (int rowposition = 0; rowposition < dtExcel.Rows.Count; rowposition++)
            {//ajustando estructura
                if (xRiskFather != null && dtExcel.Rows[rowposition][xRiskFather.MyContent.ToString()].ToString() != "" &&
                xIdRisk != null && dtExcel.Rows[rowposition][xIdRisk.MyContent.ToString()].ToString() != "")
                {
                    decimal idHijo;
                    decimal idPadre;
                    if (isCustom)
                    {
                        idHijo = General.ConvertToDec(dtExcel.Rows[rowposition][xIdRisk.MyContent.ToString()].ToString());
                        idPadre = General.ConvertToDec(dtExcel.Rows[rowposition][xRiskFather.MyContent.ToString()].ToString());
                    }
                    else
                    {
                        idPadre = General.ConvertToDec(dtExcel.Rows[rowposition][xIdRisk.MyContent.ToString()].ToString());
                        idHijo = General.ConvertToDec(dtExcel.Rows[rowposition][xRiskFather.MyContent.ToString()].ToString());
                    }
                    if (dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).Any())
                    {
                        dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).First()[DT_RiskStructure.IDRISK_FATHER] = idPadre;
                    }
                }
            }
        }

        /// <summary>
        /// Load the data of the Datatable created from excel (dtExcel) and creates new CMs with these data
        /// </summary>
        /// <param name="dsImporting">The Dataset with all data</param>
        /// <param name="isCustom">If the excel file loaded is custom or not</param>
        /// <param name="statusKeyphrase">The text inserted by user as No Active </param>
        /// <param name="dtExcel">The data loaded from excel as Datatable</param>
        /// <param name="whc">The header parameters</param>
        /// <param name="countDamages">The list of Damages</param>
        /// <param name="theDiagram">The data of the diagram created</param>
        /// <param name="xIdRisk">Data of the risk father</param>
        /// <param name="DsWBS">Dataset of the WBS</param>
        public void FillCM_Data(DataSet dsImporting, bool isCustom, string statusKeyphrase, DataTable dtExcel, 
            List<HeaderExcelContent> MyList, IEnumerable<HeaderExcelContent> countDamages, DataRow theDiagram, 
            HeaderExcelContent xIdRisk, DataSet DsWBS, bool isMarkedAllAsActive)
        {
            HeaderExcelContent xCmShort = MyList.FindLast(x => x.IdClasification == 8);
            var xCmDetail = MyList.FindLast(x => x.IdClasification == 9);
            var xCmReduction = MyList.FindLast(x => x.IdClasification == 12);
            var xCmActive = MyList.FindLast(x => x.IdClasification == 14);
            for (int rowPosition = 0; rowPosition < dtExcel.Rows.Count; rowPosition++)
            {
                if (xCmShort != null && dtExcel.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString() != "")
                {
                    DataRow drCM = dsImporting.Tables[DT_CounterM.TABLE_NAME].NewRow();
                    drCM[DT_CounterM.NAMESHORT] = dtExcel.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString();
                    drCM[DT_CounterM.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
                    if (xCmDetail != null && dtExcel.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString() != "")
                    {
                        drCM[DT_CounterM.DETAIL] = dtExcel.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString();
                    }

                    drCM[DT_CounterM.ENABLED] = true;
                    drCM[DT_CounterM.DIAGONAL] = false;
                    drCM[DT_CounterM.FROM_TOP] = true;
                    drCM[DT_CounterM.POSITION] = 0;
                    if (xCmReduction != null && dtExcel.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString() != "")
                    {
                        drCM[DT_CounterM.PROBABILITY] = dtExcel.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString();
                    }
                    else
                    {
                        drCM[DT_CounterM.PROBABILITY] = 0;
                    }
                    if (xIdRisk != null && !string.IsNullOrWhiteSpace(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString()))
                    {
                        drCM[DT_CounterM.ID_RISK] = General.ConvertToDec(dtExcel.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    }
                    else
                    {
                        drCM[DT_CounterM.ID_RISK] = dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows[dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Count - 1][DT_CounterM.ID_RISK];
                    }

                    if (isMarkedAllAsActive)
                    {
                        drCM[DT_CounterM.ENABLED] = true;
                    }
                    else
                    {

                        if (xCmActive != null && dtExcel.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString() != "")
                        {
                            if (statusKeyphrase == dtExcel.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString())
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
                    }
                    foreach (var itemDamages in countDamages.OrderBy(x => x.Column))
                    {
                        string TopRisk = itemDamages.MyContent;
                        DamagesToCM(dsImporting, TopRisk, drCM, theDiagram);
                    }
                    AsignRoleToCM(dsImporting, drCM);
                    dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                    SetTopWBSToCM(drCM, dsImporting, DsWBS);
                }
            }
        }

        /// <summary>
        /// Add to CM the Damages from the Diagram
        /// </summary>
        public static void DamagesToCM(DataSet dsImporting, string Damage, DataRow drCM, DataRow drDiagram)
        {
            DataRow drCM_Damage = dsImporting.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
            drCM_Damage[DT_CounterM_Damage.ID_DAMAGE] = dsImporting.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.DAMAGE + " = '" + Damage + "'").First()[DT_Diagram_Damages.ID_DAMAGE];
            drCM_Damage[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
            drCM_Damage[DT_CounterM_Damage.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drCM_Damage[DT_CounterM_Damage.VALUE] = 0;
            dsImporting.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(drCM_Damage);
        }

        /// <summary>
        /// Set to CM the Admin Role
        /// </summary>
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

        /// <summary>
        /// Set to Cm the Top WBS in the Project
        /// </summary>
        public static void SetTopWBSToCM(DataRow drCM, DataSet dsImporting, DataSet DsWBS)
        {
            bool primary = true;
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(dsImporting.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRCMWBS = dsImporting.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                        drRCMWBS[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                        drRCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRCMWBS[DT_CM_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drRCMWBS[DT_CM_WBS.WBS] = item[DT_WBS.WBS_NAME];
                        drRCMWBS[DT_CM_WBS.IS_PRIMARY] = primary;
                        drRCMWBS[DT_CM_WBS.WBS_USER] = drRCMWBS[DT_CM_WBS.WBS] + "[" + drRCMWBS[DT_CM_WBS.USERNAME] + "]";
                        drRCMWBS[DT_CM_WBS.PROBABILITY] = drCM[DT_CounterM.PROBABILITY];
                        primary = false;
                        dsImporting.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(drRCMWBS);
                    }
                }
            }
            SetCM_WBS_DamageValue(drCM, dsImporting);
        }
        public static void SetCM_WBS_DamageValue(DataRow drCM, DataSet dsImporting)
        {
            foreach (var drCMDamage in dsImporting.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + drCM[DT_CounterM.ID]))
            {
                foreach (var drCMWBS in dsImporting.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + drCM[DT_CounterM.ID]))
                {
                    var drRiskWBSDamage = dsImporting.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                    drRiskWBSDamage[DT_WBS_CM_Damage.DAMAGE] = drCMDamage[DT_CounterM_Damage.DAMAGE];
                    drRiskWBSDamage[DT_WBS_CM_Damage.ID_DAMAGE] = drCMDamage[DT_CounterM_Damage.ID_DAMAGE];
                    drRiskWBSDamage[DT_WBS_CM_Damage.ID_CM] = drCM[DT_CounterM.ID];
                    drRiskWBSDamage[DT_WBS_CM_Damage.ID_WBS] = drCMWBS[DT_CM_WBS.ID_WBS];
                    drRiskWBSDamage[DT_WBS_CM_Damage.VALUE] = drCMDamage[DT_CounterM_Damage.VALUE];
                    drRiskWBSDamage[DT_WBS_CM_Damage.WBS] = drCMWBS[DT_CM_WBS.WBS];
                    drRiskWBSDamage[DT_WBS_CM_Damage.WBS_USER] = drCMWBS[DT_CM_WBS.WBS_USER];
                    dsImporting.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(drRiskWBSDamage);
                }
            }
        }

    }
}
