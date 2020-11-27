using DataMapping.Data;
using EnsureBusinesss.Business;
using EnsureRisk.Classess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk
{
    public class SavingAsController
    {
        /// <summary>
        /// Add damages from source diagram to target Diagram
        /// </summary>
        /// <param name="dsTarget">Dataset only with the data of the target Diagram</param>
        /// <param name="drDiagram">Datarow with data of the created diagram</param>
        /// <param name="dsSource">Dataset with the original source data</param>
        /// <param name="originalLayout">Source layout</param>
        public static void CreateDiagramDamages(DataSet dsTarget, DataRow drDiagram, DataSet dsSource, MyLayoutDocumentt originalLayout)
        {
            foreach (DataRow drDiagramDamage in dsSource.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + originalLayout.ID_Diagram))
            {
                DataRow newTreeDamage = dsTarget.Tables[DT_Diagram_Damages.TABLE_NAME].NewRow();
                newTreeDamage[DT_Diagram_Damages.COLOR] = drDiagramDamage[DT_Diagram_Damages.COLOR];
                newTreeDamage[DT_Diagram_Damages.TOP_RISK] = drDiagramDamage[DT_Diagram_Damages.TOP_RISK];
                newTreeDamage[DT_Diagram_Damages.ID_DAMAGE] = drDiagramDamage[DT_Diagram_Damages.ID_DAMAGE];
                newTreeDamage[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                newTreeDamage[DT_Diagram_Damages.RISK_TREE] = drDiagram[DT_Diagram.DIAGRAM_NAME];
                newTreeDamage[DT_Diagram_Damages.UM] = drDiagramDamage[DT_Diagram_Damages.UM];
                newTreeDamage[DT_Diagram_Damages.DAMAGE] = drDiagramDamage[DT_Diagram_Damages.DAMAGE];
                dsTarget.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Add(newTreeDamage);
            }
        }

        /// <summary>
        /// Create a List of Risk, using the source layout as reference.
        /// </summary>
        /// <param name="originalLayout">Layout of reference</param>
        /// <param name="listToFill">List of risk to fill</param>
        /// <param name="menuRisk">Menu for all risks</param>
        public static void CreateRisks(MyLayoutDocumentt originalLayout, List<RiskPolyLine> listToFill, ContextMenu menuRisk)
        {
            foreach (DataRow item in originalLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + originalLayout.ID_Diagram))
            {
                int myPosition = item[DT_Risk.POSITION] == DBNull.Value ? 0 : (int)item[DT_Risk.POSITION];
                RiskPolyLine riskLine = new RiskPolyLine(new Grid(), menuRisk, false)
                {
                    ShortName = item[DT_Risk.NAMESHORT].ToString(),
                    ID = (decimal)item[DT_Risk.ID],
                    Position = myPosition,
                    IsRoot = (bool)item[DT_Risk.IS_ROOT],
                    Collapsed = (bool)item[DT_Risk.ISCOLLAPSED],
                    Probability = (decimal)item[DT_Risk.PROBABILITY],
                    IsCM = false
                };
                if (originalLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains((decimal)item[DT_Risk.ID]))
                {
                    if (originalLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (decimal)item[DT_Risk.ID]).Any())
                    {
                        riskLine.IdRiskFather = (decimal)originalLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (decimal)item[DT_Risk.ID]).First()[DT_RiskStructure.IDRISK_FATHER];
                    }
                    else
                    {
                        riskLine.IdRiskFather = 0;
                    }
                }
                else { riskLine.IdRiskFather = 0; }
                listToFill.Add(riskLine);
            }
        }

        /// <summary>
        /// Create a List of CM, using the source layout as reference.
        /// </summary>
        /// <param name="originalLayout">Layout of reference</param>
        /// <param name="cmPolyLines">List of CM to fill</param>
        /// <param name="menuCM">Menu for all CMs</param>
        public static void CreateCMs(MyLayoutDocumentt originalLayout, List<RiskPolyLine> cmPolyLines, ContextMenu menuCM)
        {
            foreach (DataRow item in originalLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_DIAGRAM + " = " + originalLayout.ID_Diagram))
            {
                RiskPolyLine cmline = new RiskPolyLine(new Grid(), menuCM, true)
                {
                    IsCM = true,
                    Position = (int)item[DT_CounterM.POSITION],
                    FromTop = false,
                    ShortName = item[DT_CounterM.NAMESHORT].ToString(),
                    IdRiskFather = (decimal)item[DT_CounterM.ID_RISK],
                    ID = (decimal)item[DT_CounterM.ID],
                    Probability = (decimal)item[DT_CounterM.PROBABILITY],
                    IsActivated = (bool)item[DT_CounterM.ENABLED]
                };
                cmPolyLines.Add(cmline);
            }
        }

        /// <summary>
        /// Create the WBS, Damage and Role to the risk (drRisk), usually the Main Risk, to all risk of the diagram heir the data.
        /// </summary>
        /// <param name="targetDS">Dataset to copy the data from source Dataset</param>
        /// <param name="sourceDS">Dataset where info are copied from</param>
        /// <param name="drRisk">Data of the risk</param>
        /// <param name="drDiagram">Data of the diagram</param>
        /// <param name="diagramName">Source diagram name</param>
        /// <param name="lineToCopy">Source line to copy values</param>
        public static void CreateDamageWBSRoleRisks(DataSet targetDS, DataSet sourceDS, DataRow drRisk, DataRow drDiagram, string diagramName, RiskPolyLine lineToCopy)
        {
            foreach (DataRow itemi in sourceDS.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + lineToCopy.ID))
            {
                DataRow topR = targetDS.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                topR[DT_Risk_Damages.COLOR] = itemi[DT_Risk_Damages.COLOR];
                topR[DT_Risk_Damages.ID_DAMAGE] = itemi[DT_Risk_Damages.ID_DAMAGE];
                topR[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                topR[DT_Risk_Damages.VALUE] = itemi[DT_Risk_Damages.VALUE];
                topR[DT_Risk_Damages.TOP_RISK] = itemi[DT_Risk_Damages.TOP_RISK];
                topR[DT_Risk_Damages.DAMAGE] = itemi[DT_Risk_Damages.DAMAGE];
                topR[DT_Risk_Damages.FATHER] = "";
                topR[DT_Risk_Damages.GROUPE_NAME] = itemi[DT_Risk_Damages.GROUPE_NAME];
                topR[DT_Risk_Damages.ID_FATHER] = drRisk[DT_Risk.IDRISK_FATHER];
                topR[DT_Risk_Damages.ID_GROUPE] = itemi[DT_Risk_Damages.ID_GROUPE];
                topR[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                topR[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                topR[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                topR[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                topR[DT_Risk_Damages.RISK_TREE] = diagramName;
                topR[DT_Risk_Damages.STATUS] = itemi[DT_Risk_Damages.STATUS];

                targetDS.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(topR);
            }

            foreach (DataRow item in sourceDS.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + lineToCopy.ID))
            {
                if (!(targetDS.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = targetDS.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                    newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                    targetDS.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
            foreach (DataRow item in sourceDS.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + lineToCopy.ID))
            {
                if (!(targetDS.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRow = targetDS.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRow[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                    newRow[DT_RISK_WBS.WBS] = item[DT_RISK_WBS.WBS];
                    newRow[DT_RISK_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                    newRow[DT_RISK_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                    newRow[DT_RISK_WBS.PRIMARY] = item[DT_RISK_WBS.PRIMARY];
                    newRow[DT_RISK_WBS.IS_PRIMARY] = item[DT_RISK_WBS.IS_PRIMARY];
                    newRow[DT_RISK_WBS.PROBABILITY] = item[DT_RISK_WBS.PROBABILITY];
                    newRow[DT_RISK_WBS.WBS_USER] = item[DT_RISK_WBS.WBS] + "[" + item[DT_RISK_WBS.USERNAME] + "]";
                    targetDS.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRow);
                }
            }
            foreach (DataRow item in sourceDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + lineToCopy.ID))
            {
                DataRow newRow = targetDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                newRow[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                newRow[DT_WBS_RISK_DAMAGE.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                newRow[DT_WBS_RISK_DAMAGE.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                newRow[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.VALUE] = item[DT_WBS_RISK_DAMAGE.VALUE];
                newRow[DT_WBS_RISK_DAMAGE.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.WBS_USER] = item[DT_WBS_RISK_DAMAGE.WBS_USER];
                targetDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRow);
            }
        }


        public static void CreateNewDiagram(DataSet sourceDs, DataSet targetDS, DataRow drSourceDiagram, DataRow drTargetDiagram)
        {
            using (DataSet lahostia = sourceDs.Copy())
            {
                foreach (var riskRow in lahostia.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + drSourceDiagram[DT_Diagram.ID_DIAGRAM]))
                {
                    foreach (var riskDamageRow in lahostia.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                    {
                        DataRow drRiskDamage = targetDS.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                        if ((decimal)riskDamageRow[DT_Risk_Damages.ID_RISK] >= 0)
                        {
                            drRiskDamage[DT_Risk_Damages.ID_RISK] = (decimal)riskDamageRow[DT_Risk_Damages.ID_RISK] * (-1);
                        }
                        else
                        {
                            drRiskDamage[DT_Risk_Damages.ID_RISK] = riskDamageRow[DT_Risk_Damages.ID_RISK];
                        }
                        drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = riskDamageRow[DT_Risk_Damages.ID_DAMAGE];
                        drRiskDamage[DT_Risk_Damages.ID_WBS] = riskDamageRow[DT_Risk_Damages.ID_WBS];
                        drRiskDamage[DT_Risk_Damages.VALUE] = riskDamageRow[DT_Risk_Damages.VALUE];
                        targetDS.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                    }
                    foreach (var riskWBSRow in lahostia.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                    {
                        DataRow drRiskWBS = targetDS.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        if ((decimal)riskWBSRow[DT_RISK_WBS.ID_RISK] >= 0)
                        {
                            drRiskWBS[DT_RISK_WBS.ID_RISK] = (decimal)riskWBSRow[DT_RISK_WBS.ID_RISK] * (-1);
                        }
                        else
                        {
                            drRiskWBS[DT_RISK_WBS.ID_RISK] = riskWBSRow[DT_RISK_WBS.ID_RISK];
                        }
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = riskWBSRow[DT_RISK_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = riskWBSRow[DT_RISK_WBS.IS_PRIMARY];
                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = riskWBSRow[DT_RISK_WBS.PROBABILITY];
                        targetDS.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                    }
                    foreach (var riskRoleRow in lahostia.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                    {
                        DataRow drRiskRole = targetDS.Tables[DT_Role_Risk.TABLENAME].NewRow();
                        if ((decimal)riskRoleRow[DT_Role_Risk.ID_RISK] >= 0)
                        {
                            drRiskRole[DT_Role_Risk.ID_RISK] = (decimal)riskRoleRow[DT_Role_Risk.ID_RISK] * (-1);
                        }
                        else
                        {
                            drRiskRole[DT_Role_Risk.ID_RISK] = riskRoleRow[DT_Role_Risk.ID_RISK];
                        }
                        drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskRoleRow[DT_Role_Risk.IDROL_COLUMN];
                        targetDS.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                    }
                    foreach (var riskDamageWBSRow in lahostia.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                    {
                        DataRow drRiskDamageWBS = targetDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                        if ((decimal)riskDamageWBSRow[DT_WBS_RISK_DAMAGE.ID_RISK] >= 0)
                        {
                            drRiskDamageWBS[DT_WBS_RISK_DAMAGE.ID_RISK] = (decimal)riskDamageWBSRow[DT_WBS_RISK_DAMAGE.ID_RISK] * (-1);
                        }
                        else
                        {
                            drRiskDamageWBS[DT_WBS_RISK_DAMAGE.ID_RISK] = riskDamageWBSRow[DT_WBS_RISK_DAMAGE.ID_RISK];
                        }
                        drRiskDamageWBS[DT_WBS_RISK_DAMAGE.ID_WBS] = riskDamageWBSRow[DT_WBS_RISK_DAMAGE.ID_WBS];
                        drRiskDamageWBS[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = riskDamageWBSRow[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                        drRiskDamageWBS[DT_WBS_RISK_DAMAGE.VALUE] = riskDamageWBSRow[DT_WBS_RISK_DAMAGE.VALUE];
                        targetDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drRiskDamageWBS);
                    }
                    foreach (var riskFather in lahostia.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + riskRow[DT_Risk.ID]))
                    {
                        DataRow drFather = targetDS.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                        if ((decimal)riskFather[DT_RiskStructure.IDRISK] >= 0)
                        {
                            drFather[DT_RiskStructure.IDRISK] = (decimal)riskFather[DT_RiskStructure.IDRISK] * (-1);
                        }
                        else
                        {
                            drFather[DT_RiskStructure.IDRISK] = riskFather[DT_RiskStructure.IDRISK];
                        }
                        if ((decimal)riskFather[DT_RiskStructure.IDRISK_FATHER] >= 0)
                        {
                            drFather[DT_RiskStructure.IDRISK_FATHER] = (decimal)riskFather[DT_RiskStructure.IDRISK_FATHER] * (-1);
                        }
                        else
                        {
                            drFather[DT_RiskStructure.IDRISK_FATHER] = riskFather[DT_RiskStructure.IDRISK_FATHER];
                        }
                        targetDS.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(drFather);
                    }
                    DataRow drRisk = targetDS.Tables[DT_Risk.TABLE_NAME].NewRow();                    
                    if ((decimal)riskRow[DT_Risk.ID] >= 0)
                    {
                        drRisk[DT_Risk.ID] = (decimal)riskRow[DT_Risk.ID] * (-1);
                    }
                    else
                    {
                        drRisk[DT_Risk.ID] = riskRow[DT_Risk.ID];
                    }
                    drRisk[DT_Risk.ID_DIAGRAM] = drTargetDiagram[DT_Diagram.ID_DIAGRAM];
                    drRisk[DT_Risk.NAMESHORT] = riskRow[DT_Risk.NAMESHORT];
                    drRisk[DT_Risk.COMMENTS] = riskRow[DT_Risk.COMMENTS];
                    drRisk[DT_Risk.ISCOLLAPSED] = riskRow[DT_Risk.ISCOLLAPSED];
                    drRisk[DT_Risk.IS_ROOT] = riskRow[DT_Risk.IS_ROOT];
                    drRisk[DT_Risk.FROM_TOP] = riskRow[DT_Risk.FROM_TOP];
                    drRisk[DT_Risk.PROBABILITY] = riskRow[DT_Risk.PROBABILITY];
                    drRisk[DT_Risk.POSITION] = riskRow[DT_Risk.POSITION];
                    drRisk[DT_Risk.ENABLED] = riskRow[DT_Risk.ENABLED];
                    drRisk[DT_Risk.ID_GROUPE] = riskRow[DT_Risk.ID_GROUPE];
                    drRisk[DT_Risk.ID_WBS] = riskRow[DT_Risk.ID_WBS];
                    targetDS.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);
                }                

                foreach (var cmRow in lahostia.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_DIAGRAM + " = " + drSourceDiagram[DT_Diagram.ID_DIAGRAM]))
                {
                    foreach (var cmDamageRow in lahostia.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmRow[DT_CounterM.ID]))
                    {
                        DataRow drCMDamage = targetDS.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                        if ((decimal)cmDamageRow[DT_CounterM_Damage.ID_COUNTERM] >= 0)
                        {
                            drCMDamage[DT_CounterM_Damage.ID_COUNTERM] = (decimal)cmDamageRow[DT_CounterM_Damage.ID_COUNTERM] * (-1);
                        }
                        else
                        {
                            drCMDamage[DT_CounterM_Damage.ID_COUNTERM] = cmDamageRow[DT_CounterM_Damage.ID_COUNTERM];
                        }
                        drCMDamage[DT_CounterM_Damage.ID_DAMAGE] = cmDamageRow[DT_CounterM_Damage.ID_DAMAGE];
                        drCMDamage[DT_CounterM_Damage.VALUE] = cmDamageRow[DT_CounterM_Damage.VALUE];
                        targetDS.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(drCMDamage);
                    }
                    foreach (var cmWBSRow in lahostia.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + cmRow[DT_CounterM.ID]))
                    {
                        DataRow drCMWBS = targetDS.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                        if ((decimal)cmWBSRow[DT_CM_WBS.ID_CM] >= 0)
                        {
                            drCMWBS[DT_CM_WBS.ID_CM] = (decimal)cmWBSRow[DT_CM_WBS.ID_CM] * (-1);
                        }
                        else
                        {
                            drCMWBS[DT_CM_WBS.ID_CM] = cmWBSRow[DT_CM_WBS.ID_CM];
                        }

                        drCMWBS[DT_CM_WBS.ID_WBS] = cmWBSRow[DT_CM_WBS.ID_WBS];
                        drCMWBS[DT_CM_WBS.IS_PRIMARY] = cmWBSRow[DT_CM_WBS.IS_PRIMARY];
                        drCMWBS[DT_CM_WBS.PROBABILITY] = cmWBSRow[DT_CM_WBS.PROBABILITY];
                        targetDS.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(drCMWBS);
                    }
                    foreach (var cmRoleRow in lahostia.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + cmRow[DT_CounterM.ID]))
                    {
                        DataRow drCMRole = targetDS.Tables[DT_Role_CM.TABLENAME].NewRow();
                        if ((decimal)cmRoleRow[DT_Role_CM.ID_CM] >= 0)
                        {
                            drCMRole[DT_Role_CM.ID_CM] = (decimal)cmRoleRow[DT_Role_CM.ID_CM] * (-1);
                        }
                        else
                        {
                            drCMRole[DT_Role_CM.ID_CM] = cmRoleRow[DT_Role_CM.ID_CM];
                        }
                        drCMRole[DT_Role_CM.IDROL_COLUMN] = cmRoleRow[DT_Role_CM.IDROL_COLUMN];
                        targetDS.Tables[DT_Role_CM.TABLENAME].Rows.Add(drCMRole);
                    }
                    foreach (var cmDamageWBSRow in lahostia.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + cmRow[DT_CounterM.ID]))
                    {
                        DataRow drCMWBSDamage = targetDS.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                        if ((decimal)cmDamageWBSRow[DT_WBS_CM_Damage.ID_CM] >= 0)
                        {
                            drCMWBSDamage[DT_WBS_CM_Damage.ID_CM] = (decimal)cmDamageWBSRow[DT_WBS_CM_Damage.ID_CM] * (-1);
                        }
                        else
                        {
                            drCMWBSDamage[DT_WBS_CM_Damage.ID_CM] = cmDamageWBSRow[DT_WBS_CM_Damage.ID_CM];
                        }
                        drCMWBSDamage[DT_WBS_CM_Damage.ID_WBS] = cmDamageWBSRow[DT_WBS_CM_Damage.ID_WBS];
                        drCMWBSDamage[DT_WBS_CM_Damage.ID_DAMAGE] = cmDamageWBSRow[DT_WBS_CM_Damage.ID_DAMAGE];
                        drCMWBSDamage[DT_WBS_CM_Damage.VALUE] = cmDamageWBSRow[DT_WBS_CM_Damage.VALUE];
                        targetDS.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(drCMWBSDamage);
                    }
                    DataRow drCM = targetDS.Tables[DT_CounterM.TABLE_NAME].NewRow();
                    drCM[DT_CounterM.ID_DIAGRAM] = drTargetDiagram[DT_Diagram.ID_DIAGRAM];
                    if ((decimal)cmRow[DT_CounterM.ID] >= 0)
                    {
                        drCM[DT_CounterM.ID] = (decimal)cmRow[DT_CounterM.ID] * (-1);
                    }
                    else
                    {
                        drCM[DT_CounterM.ID] = cmRow[DT_CounterM.ID];
                    }
                    if ((decimal)cmRow[DT_CounterM.ID_RISK] >= 0)
                    {
                        drCM[DT_CounterM.ID_RISK] = (decimal)cmRow[DT_CounterM.ID_RISK] * (-1);
                    }
                    else
                    {
                        drCM[DT_CounterM.ID_RISK] = cmRow[DT_CounterM.ID_RISK];
                    }
                    drCM[DT_CounterM.NAMESHORT] = cmRow[DT_CounterM.NAMESHORT];
                    drCM[DT_CounterM.DETAIL] = cmRow[DT_CounterM.DETAIL];
                    drCM[DT_CounterM.FROM_TOP] = cmRow[DT_CounterM.FROM_TOP];
                    drCM[DT_CounterM.PROBABILITY] = cmRow[DT_CounterM.PROBABILITY];
                    drCM[DT_CounterM.POSITION] = cmRow[DT_CounterM.POSITION];
                    drCM[DT_CounterM.ENABLED] = cmRow[DT_CounterM.ENABLED];
                    drCM[DT_CounterM.ID_GROUPE] = cmRow[DT_CounterM.ID_GROUPE];
                    drCM[DT_CounterM.ID_WBS] = cmRow[DT_CounterM.ID_WBS];
                    targetDS.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                }
            }
        }
    }
}
