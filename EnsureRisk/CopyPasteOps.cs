using DataMapping.Data;
using EnsureBusinesss.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk
{
    public static class CopyPasteOps
    {

        public static DataRow EstablecerValoresNuevoRiesgoCopiado(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drTargetRisk, bool isMain, int ID_Diagram, DataSet DsWBS)
        {
            DataRow drNewRisk = targetDataset.Tables[DT_Risk.TABLE_NAME].NewRow();
            drNewRisk[DT_Risk.COMMENTS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.COMMENTS];
            drNewRisk[DT_Risk.ENABLED] = true;
            drNewRisk[DT_Risk.FROM_TOP] = sourceRisk.FromTop;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceRisk.ShortName;
            drNewRisk[DT_Risk.POSITION] = isMain ? sourceRisk.Position : targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.POSITION];

            drNewRisk[DT_Risk.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.GROUPE_NAME]; ;
            drNewRisk[DT_Risk.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.ID_GROUPE]; ;
            drNewRisk[DT_Risk.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.ID_WBS];
            drNewRisk[DT_Risk.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.WBS_NAME];
            drNewRisk[DT_Risk.USER_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.USER_NAME];

            drNewRisk[DT_Risk.PROBABILITY] = 100;

            drNewRisk[DT_Risk.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Add(drNewRisk);

            DataRow rowstructure = targetDataset.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drNewRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

            SetDamagesNuevoRiesgoCopiado(targetDataset, drNewRisk, ID_Diagram);
            SetWBS_RiskNuevoRiesgoCopiado(targetDataset, drNewRisk, drTargetRisk);
            SetWBS_RISK_DamageNuevoRiskCopiado(targetDataset, drNewRisk, drTargetRisk, ID_Diagram, DsWBS);
            SetRoleRisk(targetDataset, drNewRisk, drTargetRisk);
            if (!(sourceRisk.IsLeaf()))
            {
                foreach (var risk in sourceRisk.Children)
                {
                    if (!risk.IsCM)
                    {
                        EstablecerValoresNuevoRiesgoCopiado(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS);
                    }
                    else
                    {
                        SetValoresCM(risk, targetDataset, drNewRisk, ID_Diagram, DsWBS);
                    }
                }
            }
            return drNewRisk;
        }

        public static DataRow SetValoresOriginalesRiesgoCopiado(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drTargetRisk, bool isMain, int ID_Diagram, DataSet DsWBS, List<RiskPolyLine> LinesList)
        {
            DataRow drNewRisk = targetDataset.Tables[DT_Risk.TABLE_NAME].NewRow();
            drNewRisk[DT_Risk.COMMENTS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.COMMENTS];
            drNewRisk[DT_Risk.ENABLED] = true;
            drNewRisk[DT_Risk.FROM_TOP] = sourceRisk.FromTop;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceRisk.ShortName;
            drNewRisk[DT_Risk.POSITION] = isMain ? LinesList.Find(r => r.ID == (int)drTargetRisk[DT_Risk.ID]).Children.Count : targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.POSITION];

            drNewRisk[DT_Risk.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME]; ;
            drNewRisk[DT_Risk.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE]; ;
            drNewRisk[DT_Risk.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
            drNewRisk[DT_Risk.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.WBS_NAME];
            drNewRisk[DT_Risk.USER_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.USER_NAME];

            drNewRisk[DT_Risk.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.PROBABILITY]; ;
            drNewRisk[DT_Risk.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Add(drNewRisk);
            DataRow rowstructure = targetDataset.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drNewRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

            SetOriginalAndNewDamagesRiesgoCopiado(sourceRisk, targetDataset, drNewRisk, ID_Diagram);
            SetWBS_RiskOriginalAndNuevoRiesgoCopiado(sourceRisk, targetDataset, drNewRisk, drTargetRisk);
            SetWBS_RISK_DamageOriginalAndNuevoRiskCopiado(sourceRisk, targetDataset, drNewRisk, drTargetRisk, ID_Diagram, DsWBS);
            SetNewAndOriginalRoleRisk(sourceRisk, targetDataset, drNewRisk, drTargetRisk);

            if (!(sourceRisk.IsLeaf()))
            {
                foreach (var risk in sourceRisk.Children)
                {
                    if (!risk.IsCM)
                    {
                        SetValoresOriginalesRiesgoCopiado(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS, LinesList);
                    }
                    else
                    {
                        SetValoresOriginalesAndNuevosCM(risk, targetDataset, drNewRisk, ID_Diagram, DsWBS);
                    }
                }
            }
            return drNewRisk;
        }

        #region ImportarVacio
        public static void SetDamagesNuevoRiesgoCopiado(DataSet targetDataset, DataRow drNewRisk, int ID_Diagram)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = diagramDamages[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.ID])[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drNewRisk[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = drNewRisk[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drNewRisk[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = 100;
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = drNewRisk[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = diagramDamages[DT_Diagram_Damages.RISK_TREE];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.USER_NAME];
                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        public static void SetWBS_RiskNuevoRiesgoCopiado(DataSet targetDataset, DataRow drNewRisk, DataRow drTargetRisk)
        {
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drNewRisk[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PRIMARY] = targetWBS[DT_RISK_WBS.PRIMARY];
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                    newRiskWBS[DT_RISK_WBS.RISK] = drNewRisk[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
        }

        public static void SetWBS_RISK_DamageNuevoRiskCopiado(DataSet targetDataset, DataRow drNewRisk, DataRow drTargetRisk, int ID_Diagram, DataSet DsWBS)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
                {
                    if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { targetWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] })))
                    {
                        if (WBS_isSheet((int)targetWBS[DT_RISK_WBS.ID_WBS], DsWBS))
                        {
                            DataRow newRiskWBSDamage = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = drNewRisk[DT_Risk.ID];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = targetWBS[DT_RISK_WBS.WBS];
                            targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRiskWBSDamage);
                        }
                    }
                }
            }
        }

        public static void SetRoleRisk(DataSet targetDataset, DataRow drNewRisk, DataRow drTargetRisk)
        {
            foreach (DataRow riskRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], riskRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drRiskRole = targetDataset.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskRole[DT_Role_Risk.IDROL_COLUMN];
                    drRiskRole[DT_Role_Risk.ID_RISK] = drNewRisk[DT_Risk.ID];
                    drRiskRole[DT_Role_Risk.NAME_SHORT] = drNewRisk[DT_Risk.NAMESHORT];
                    drRiskRole[DT_Role_Risk.Role] = riskRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                }
            }
        }

        public static void SetValoresCM(RiskPolyLine sourceCM, DataSet targetDataset, DataRow drRiskFather, int ID_Diagram, DataSet DsWBS)
        {
            DataRow drNewCM = targetDataset.Tables[DT_CounterM.TABLE_NAME].NewRow();
            drNewCM[DT_CounterM.DETAIL] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.DETAIL];
            drNewCM[DT_CounterM.ENABLED] = true;
            drNewCM[DT_CounterM.GROUPE_NAME] = drRiskFather[DT_Risk.GROUPE_NAME];
            drNewCM[DT_CounterM.ID_GROUPE] = drRiskFather[DT_Risk.ID_GROUPE];
            drNewCM[DT_CounterM.ID_RISK] = drRiskFather[DT_Risk.ID];
            drNewCM[DT_CounterM.ID_RISK_TREE] = ID_Diagram;
            drNewCM[DT_CounterM.ID_WBS] = drRiskFather[DT_Risk.ID_WBS]; ;
            drNewCM[DT_CounterM.NAMESHORT] = sourceCM.ShortName;
            drNewCM[DT_CounterM.POSITION] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.POSITION];
            drNewCM[DT_CounterM.PROBABILITY] = 0;
            drNewCM[DT_CounterM.RISK_NAMESHORT] = drRiskFather[DT_Risk.NAMESHORT];
            drNewCM[DT_CounterM.USER_NAME] = drRiskFather[DT_Risk.USER_NAME];
            drNewCM[DT_CounterM.WBS_NAME] = drRiskFather[DT_Risk.WBS_NAME];
            targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drNewCM);

            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drCMDamage = targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                    drCMDamage[DT_CounterM_Damage.COLOR] = diagramDamages[DT_Diagram_Damages.COLOR];
                    drCMDamage[DT_CounterM_Damage.COUNTERM_NAMESHORT] = drNewCM[DT_CounterM.NAMESHORT];
                    drCMDamage[DT_CounterM_Damage.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                    drCMDamage[DT_CounterM_Damage.GROUPENAME] = drNewCM[DT_CounterM.GROUPE_NAME];
                    drCMDamage[DT_CounterM_Damage.IDRISK] = drRiskFather[DT_Risk.ID];
                    drCMDamage[DT_CounterM_Damage.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drCMDamage[DT_CounterM_Damage.ID_GROUPE] = drRiskFather[DT_Risk.ID_GROUPE];
                    drCMDamage[DT_CounterM_Damage.ID_RISK_TREE] = ID_Diagram;
                    drCMDamage[DT_CounterM_Damage.ID_WBS] = drRiskFather[DT_Risk.ID_WBS];
                    drCMDamage[DT_CounterM_Damage.ID_COUNTERM] = drNewCM[DT_CounterM.ID];
                    drCMDamage[DT_CounterM_Damage.RISK] = drRiskFather[DT_Risk.NAMESHORT]; ;
                    drCMDamage[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                    drCMDamage[DT_CounterM_Damage.STATUS] = true;
                    drCMDamage[DT_CounterM_Damage.STATUS] = true;
                    drCMDamage[DT_CounterM_Damage.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drCMDamage[DT_CounterM_Damage.USERNAME] = drRiskFather[DT_Risk.USER_NAME];
                    drCMDamage[DT_CounterM_Damage.VALUE] = 0;
                    drCMDamage[DT_CounterM_Damage.WBS_NAME] = drRiskFather[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(drCMDamage);
                }
            }
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRiskFather[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newCMWBS = targetDataset.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                    newCMWBS[DT_CM_WBS.ID_CM] = drNewCM[DT_CounterM.ID];
                    newCMWBS[DT_CM_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newCMWBS[DT_CM_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                    newCMWBS[DT_CM_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newCMWBS[DT_CM_WBS.PRIMARY] = targetWBS[DT_RISK_WBS.PRIMARY];
                    newCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                    newCMWBS[DT_CM_WBS.CM] = drNewCM[DT_CounterM.NAMESHORT];
                    newCMWBS[DT_CM_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newCMWBS[DT_CM_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                }
                foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
                {
                    if (!(targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { targetWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewCM[DT_CounterM.ID] })))
                    {
                        if (WBS_isSheet((int)targetWBS[DT_RISK_WBS.ID_WBS], DsWBS))
                        {
                            DataRow newCMWBSDamage = targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                            newCMWBSDamage[DT_WBS_CM_Damage.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_CM] = drNewCM[DT_CounterM.ID];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                            newCMWBSDamage[DT_WBS_CM_Damage.VALUE] = 0;
                            newCMWBSDamage[DT_WBS_CM_Damage.WBS] = targetWBS[DT_RISK_WBS.WBS];
                            targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newCMWBSDamage);
                        }
                    }
                }
                foreach (DataRow riskRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + drRiskFather[DT_Risk.ID]))
                {
                    if (!(targetDataset.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], riskRole[DT_Role_Risk.IDROL_COLUMN] })))
                    {
                        DataRow drCmRole = targetDataset.Tables[DT_Role_CM.TABLENAME].NewRow();
                        drCmRole[DT_Role_CM.IDROL_COLUMN] = riskRole[DT_Role_Risk.IDROL_COLUMN];
                        drCmRole[DT_Role_CM.ID_CM] = drNewCM[DT_CounterM.ID];
                        drCmRole[DT_Role_CM.NAME_SHORT] = drNewCM[DT_CounterM.NAMESHORT];
                        drCmRole[DT_Role_CM.Role] = riskRole[DT_Role_Risk.Role];
                        targetDataset.Tables[DT_Role_CM.TABLENAME].Rows.Add(drCmRole);
                    }
                }
            }
        }
        #endregion
        
        private static bool WBS_isSheet(int ID_WBS, DataSet DsWBS)
        {
            return !DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + ID_WBS).Any();
        }
        
        #region ImportarTODO
        public static void AgregarDamageADiagram(DataSet targetDataset, DataRow[] drImportados, int ID_Diagram)
        {
            foreach (DataRow drDamage in drImportados)
            {
                if (!(targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Contains(new object[] { ID_Diagram, drDamage[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drAImportar = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].NewRow();
                    drAImportar[DT_Diagram_Damages.COLOR] = drDamage[DT_Diagram_Damages.COLOR];
                    drAImportar[DT_Diagram_Damages.DAMAGE] = drDamage[DT_Diagram_Damages.DAMAGE];
                    drAImportar[DT_Diagram_Damages.ID_DAMAGE] = drDamage[DT_Diagram_Damages.ID_DAMAGE];
                    drAImportar[DT_Diagram_Damages.ID_RISKTREE] = ID_Diagram;
                    drAImportar[DT_Diagram_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drAImportar[DT_Diagram_Damages.TOP_RISK] = drDamage[DT_Diagram_Damages.TOP_RISK];
                    drAImportar[DT_Diagram_Damages.UM] = drDamage[DT_Diagram_Damages.UM];
                    targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Add(drAImportar);
                }
            }
        }

        public static void AgregarNewDamagesAExistingRisk(DataSet targetDataset, DataRow[] drImportados, int ID_Diagram, DataSet DsWBS)
        {
            foreach (DataRow drDamage in drImportados)
            {
                foreach (DataRow risk in targetDataset.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram))
                {
                    if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { risk[DT_Risk.ID], drDamage[DT_Diagram_Damages.ID_DAMAGE] })))
                    {
                        DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                        drRiskDamage[DT_Risk_Damages.COLOR] = drDamage[DT_Diagram_Damages.COLOR];
                        drRiskDamage[DT_Risk_Damages.DAMAGE] = drDamage[DT_Diagram_Damages.DAMAGE];
                        drRiskDamage[DT_Risk_Damages.FATHER] = risk[DT_Risk.FATHER];
                        drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = risk[DT_Risk.GROUPE_NAME]; ;
                        drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = drDamage[DT_Diagram_Damages.ID_DAMAGE];
                        drRiskDamage[DT_Risk_Damages.ID_FATHER] = risk[DT_Risk.IDRISK_FATHER]; ;
                        drRiskDamage[DT_Risk_Damages.ID_GROUPE] = risk[DT_Risk.ID_GROUPE]; ;
                        drRiskDamage[DT_Risk_Damages.ID_RISK] = risk[DT_Risk.ID]; ;
                        drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                        drRiskDamage[DT_Risk_Damages.ID_WBS] = risk[DT_Risk.ID_WBS]; ;
                        drRiskDamage[DT_Risk_Damages.IS_ROOT] = risk[DT_Risk.IS_ROOT]; ;
                        drRiskDamage[DT_Risk_Damages.PROBABILITY] = risk[DT_Risk.PROBABILITY];
                        drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = risk[DT_Risk.NAMESHORT];
                        drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                        drRiskDamage[DT_Risk_Damages.STATUS] = risk[DT_Risk.ENABLED];
                        drRiskDamage[DT_Risk_Damages.TOP_RISK] = drDamage[DT_Diagram_Damages.TOP_RISK];
                        drRiskDamage[DT_Risk_Damages.USERNAME] = risk[DT_Risk.USER_NAME];
                        drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                        drRiskDamage[DT_Risk_Damages.WBS_NAME] = risk[DT_Risk.WBS_NAME];
                        targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                    }
                    foreach (DataRow riskWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + risk[DT_Risk.ID]))
                    {
                        if (WBS_isSheet((int)riskWBS[DT_RISK_WBS.ID_WBS], DsWBS))
                        {
                            if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { riskWBS[DT_RISK_WBS.ID_WBS], drDamage[DT_Diagram_Damages.ID_DAMAGE], risk[DT_Risk.ID] })))
                            {
                                DataRow drWBSDamageRisk = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                                drWBSDamageRisk[DT_WBS_RISK_DAMAGE.DAMAGE] = drDamage[DT_Diagram_Damages.DAMAGE];
                                drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = drDamage[DT_Diagram_Damages.ID_DAMAGE];
                                drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_RISK] = risk[DT_Risk.ID];
                                drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS] = riskWBS[DT_RISK_WBS.ID_WBS];
                                drWBSDamageRisk[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                                drWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS] = riskWBS[DT_RISK_WBS.WBS];
                                targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBSDamageRisk);
                            }
                        }
                    }
                }
            }
        }

        public static void SetOriginalAndNewDamagesRiesgoCopiado(RiskPolyLine sourceRisk,DataSet targetDataset, DataRow drNewRisk, int ID_Diagram)
        {
            //TODOS LOS DAMAGES DEL DIAGRAM SOURCE
            foreach (DataRow oldRiskDamage in targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] {ID_Diagram, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE]})[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE]; ;
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = oldRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drNewRisk[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drNewRisk[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { sourceRisk.ID, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.PROBABILITY]; ;
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = sourceRisk.ShortName;
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = oldRiskDamage[DT_Risk_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.USER_NAME];
                    if (targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { sourceRisk.ID, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] }))
                    {
                        drRiskDamage[DT_Risk_Damages.VALUE] = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { sourceRisk.ID, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.VALUE];
                    }
                    else
                    {
                        drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    }
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
            //LOS DAMAGES DEL DIAGRAM TARGET
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                //EVALUO QUE NO ESTE YA PARA NO SOBREEESCRIBIR 
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE];
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drNewRisk[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drNewRisk[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.PROBABILITY];
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = sourceRisk.ShortName;
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.USER_NAME];
                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drNewRisk[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        public static void SetWBS_RiskOriginalAndNuevoRiesgoCopiado(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drNewRisk, DataRow drTargetRisk)
        {
            //Estos son los WBS que tenia originalmente
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drNewRisk[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PRIMARY] = targetWBS[DT_RISK_WBS.PRIMARY];
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = targetWBS[DT_RISK_WBS.PROBABILITY];
                    newRiskWBS[DT_RISK_WBS.RISK] = drNewRisk[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
            //Estos son los WBS que heredo del padre
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drNewRisk[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PRIMARY] = "";
                    //Pongo la probabilidad original para que no me afecte el promedio de la probabilidad en la tabla del damage
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.PROBABILITY];
                    newRiskWBS[DT_RISK_WBS.RISK] = drNewRisk[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
        }

        public static void SetWBS_RISK_DamageOriginalAndNuevoRiskCopiado(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drNewRisk, DataRow drTargetRisk, int ID_Diagram, DataSet DsWBS)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                foreach (DataRow sourceWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + sourceRisk.ID))
                {
                    if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { sourceWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] })))
                    {
                        if (WBS_isSheet((int)sourceWBS[DT_RISK_WBS.ID_WBS], DsWBS))
                        {
                            DataRow newRiskWBSDamage = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = drNewRisk[DT_Risk.ID];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = sourceWBS[DT_RISK_WBS.ID_WBS];
                            //PONGO EL VALOR QUE TENIA EN EL OTRO DIAGRAMA
                            if (targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(
                                                                            new object[] { sourceWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], sourceRisk.ID }))
                            {
                                newRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(
                                                new object[] { sourceWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], sourceRisk.ID })[DT_WBS_RISK_DAMAGE.VALUE];
                            }
                            else
                            {
                                newRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                            }
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = sourceWBS[DT_RISK_WBS.WBS];
                            targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRiskWBSDamage);
                        }
                    }
                }
                //Los valores que va a tener que debio heredar del padre
                foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
                {
                    if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { targetWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] })))
                    {
                        if (WBS_isSheet((int)targetWBS[DT_RISK_WBS.ID_WBS], DsWBS))
                        {
                            DataRow newRiskWBSDamage = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = drNewRisk[DT_Risk.ID];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                            //PONE 0 COMO VALOR PORQUE ESTE WBS NO LO TENIA
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = targetWBS[DT_RISK_WBS.WBS];
                            targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRiskWBSDamage);
                        }
                    }
                }
            }
        }

        public static void SetNewAndOriginalRoleRisk(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drNewRisk, DataRow drTargetRisk)
        {
            //Original
            foreach (DataRow riskOriginalRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], riskOriginalRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drRiskRole = targetDataset.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskOriginalRole[DT_Role_Risk.IDROL_COLUMN];
                    drRiskRole[DT_Role_Risk.ID_RISK] = drNewRisk[DT_Risk.ID];
                    drRiskRole[DT_Role_Risk.NAME_SHORT] = drNewRisk[DT_Risk.NAMESHORT];
                    drRiskRole[DT_Role_Risk.Role] = riskOriginalRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                }
            }
            //New
            foreach (DataRow riskRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drNewRisk[DT_Risk.ID], riskRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drRiskRole = targetDataset.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskRole[DT_Role_Risk.IDROL_COLUMN];
                    drRiskRole[DT_Role_Risk.ID_RISK] = drNewRisk[DT_Risk.ID];
                    drRiskRole[DT_Role_Risk.NAME_SHORT] = drNewRisk[DT_Risk.NAMESHORT];
                    drRiskRole[DT_Role_Risk.Role] = riskRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                }
            }
        }

        public static void SetValoresOriginalesAndNuevosCM(RiskPolyLine sourceCM, DataSet targetDataset, DataRow drRiskFather, int ID_Diagram, DataSet DsWBS)
        {
            DataRow drNewCM = targetDataset.Tables[DT_CounterM.TABLE_NAME].NewRow();
            drNewCM[DT_CounterM.DETAIL] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.DETAIL];
            drNewCM[DT_CounterM.ENABLED] = true;
            drNewCM[DT_CounterM.GROUPE_NAME] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.GROUPE_NAME];
            drNewCM[DT_CounterM.ID_GROUPE] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.ID_GROUPE];
            drNewCM[DT_CounterM.ID_RISK] = drRiskFather[DT_Risk.ID];
            drNewCM[DT_CounterM.ID_RISK_TREE] = ID_Diagram;
            drNewCM[DT_CounterM.ID_WBS] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.ID_WBS];
            drNewCM[DT_CounterM.NAMESHORT] = sourceCM.ShortName;
            drNewCM[DT_CounterM.POSITION] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.POSITION];
            drNewCM[DT_CounterM.PROBABILITY] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.PROBABILITY];
            drNewCM[DT_CounterM.RISK_NAMESHORT] = drRiskFather[DT_Risk.NAMESHORT];
            drNewCM[DT_CounterM.USER_NAME] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.PROBABILITY];
            drNewCM[DT_CounterM.WBS_NAME] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.WBS_NAME];
            targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drNewCM);

            //TODOS LOS DAMAGES DEL DIAGRAM SOURCE
            foreach (DataRow oldCMDamage in targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + sourceCM.ID))
            {
                if (!(targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], oldCMDamage[DT_CounterM_Damage.ID_DAMAGE] })))
                {
                    DataRow drCMDamage = targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                    drCMDamage[DT_CounterM_Damage.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldCMDamage[DT_CounterM_Damage.ID_DAMAGE] })[DT_Diagram_Damages.COLOR];
                    drCMDamage[DT_CounterM_Damage.COUNTERM_NAMESHORT] = drNewCM[DT_CounterM.NAMESHORT];
                    drCMDamage[DT_CounterM_Damage.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldCMDamage[DT_CounterM_Damage.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE];
                    drCMDamage[DT_CounterM_Damage.GROUPENAME] = drNewCM[DT_CounterM.GROUPE_NAME];
                    drCMDamage[DT_CounterM_Damage.IDRISK] = drRiskFather[DT_Risk.ID];
                    drCMDamage[DT_CounterM_Damage.ID_DAMAGE] = oldCMDamage[DT_CounterM_Damage.ID_DAMAGE];
                    drCMDamage[DT_CounterM_Damage.ID_GROUPE] = drNewCM[DT_CounterM.ID_GROUPE];
                    drCMDamage[DT_CounterM_Damage.ID_RISK_TREE] = ID_Diagram;
                    drCMDamage[DT_CounterM_Damage.ID_WBS] = drNewCM[DT_CounterM.ID_WBS];
                    drCMDamage[DT_CounterM_Damage.ID_COUNTERM] = drNewCM[DT_CounterM.ID];
                    drCMDamage[DT_CounterM_Damage.RISK] = drRiskFather[DT_Risk.NAMESHORT];
                    drCMDamage[DT_CounterM_Damage.RISK_REDUCTION] = drNewCM[DT_CounterM.PROBABILITY];
                    drCMDamage[DT_CounterM_Damage.STATUS] = true;
                    drCMDamage[DT_CounterM_Damage.TOP_RISK] = oldCMDamage[DT_CounterM_Damage.TOP_RISK];
                    drCMDamage[DT_CounterM_Damage.USERNAME] = drNewCM[DT_CounterM.USER_NAME];
                    if (targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { sourceCM.ID, oldCMDamage[DT_CounterM_Damage.ID_DAMAGE] }))
                    {
                        drCMDamage[DT_CounterM_Damage.VALUE] = targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Find(new object[] { sourceCM.ID, oldCMDamage[DT_CounterM_Damage.ID_DAMAGE] })[DT_CounterM_Damage.VALUE];
                    }
                    else
                    {
                        drCMDamage[DT_CounterM_Damage.VALUE] = 0;
                    }
                    drCMDamage[DT_CounterM_Damage.WBS_NAME] = drNewCM[DT_CounterM.WBS_NAME];
                    targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(drCMDamage);
                }
            }
            //Los DAMAGES del TARGET DIAGRAM
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drCMDamage = targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                    drCMDamage[DT_CounterM_Damage.COLOR] = diagramDamages[DT_Diagram_Damages.COLOR];
                    drCMDamage[DT_CounterM_Damage.COUNTERM_NAMESHORT] = drNewCM[DT_CounterM.NAMESHORT];
                    drCMDamage[DT_CounterM_Damage.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                    drCMDamage[DT_CounterM_Damage.GROUPENAME] = drNewCM[DT_CounterM.GROUPE_NAME];
                    drCMDamage[DT_CounterM_Damage.IDRISK] = drRiskFather[DT_Risk.ID];
                    drCMDamage[DT_CounterM_Damage.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drCMDamage[DT_CounterM_Damage.ID_GROUPE] = drNewCM[DT_CounterM.ID_GROUPE];
                    drCMDamage[DT_CounterM_Damage.ID_RISK_TREE] = ID_Diagram;
                    drCMDamage[DT_CounterM_Damage.ID_WBS] = drNewCM[DT_CounterM.ID_WBS];
                    drCMDamage[DT_CounterM_Damage.ID_COUNTERM] = drNewCM[DT_CounterM.ID];
                    drCMDamage[DT_CounterM_Damage.RISK] = drRiskFather[DT_Risk.NAMESHORT];
                    drCMDamage[DT_CounterM_Damage.RISK_REDUCTION] = drNewCM[DT_CounterM.PROBABILITY];
                    drCMDamage[DT_CounterM_Damage.STATUS] = true;
                    drCMDamage[DT_CounterM_Damage.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drCMDamage[DT_CounterM_Damage.USERNAME] = drNewCM[DT_CounterM.USER_NAME];
                    drCMDamage[DT_CounterM_Damage.VALUE] = 0;
                    drCMDamage[DT_CounterM_Damage.WBS_NAME] = drNewCM[DT_CounterM.WBS_NAME];
                    targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(drCMDamage);
                }
            }

            foreach (DataRow sourceWBS in targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + sourceCM.ID))
            {
                if (!(targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], sourceWBS[DT_CM_WBS.ID_WBS] })))
                {
                    DataRow newCMWBS = targetDataset.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                    newCMWBS[DT_CM_WBS.ID_CM] = drNewCM[DT_CounterM.ID];
                    newCMWBS[DT_CM_WBS.ID_WBS] = sourceWBS[DT_CM_WBS.ID_WBS];
                    newCMWBS[DT_CM_WBS.IS_PRIMARY] = sourceWBS[DT_CM_WBS.IS_PRIMARY];
                    newCMWBS[DT_CM_WBS.NIVEL] = sourceWBS[DT_CM_WBS.NIVEL];
                    newCMWBS[DT_CM_WBS.PRIMARY] = sourceWBS[DT_CM_WBS.PRIMARY];
                    newCMWBS[DT_CM_WBS.PROBABILITY] = sourceWBS[DT_CM_WBS.PROBABILITY];
                    newCMWBS[DT_CM_WBS.CM] = drNewCM[DT_CounterM.NAMESHORT];
                    newCMWBS[DT_CM_WBS.USERNAME] = sourceWBS[DT_CM_WBS.USERNAME];
                    newCMWBS[DT_CM_WBS.WBS] = sourceWBS[DT_CM_WBS.WBS];
                    targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                }
                //LOS VALORES ORIGINALES 
                foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
                {
                    if (!(targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { sourceWBS[DT_CM_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewCM[DT_CounterM.ID] })))
                    {
                        if (WBS_isSheet((int)sourceWBS[DT_CM_WBS.ID_WBS], DsWBS))
                        {
                            DataRow newCMWBSDamage = targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                            newCMWBSDamage[DT_WBS_CM_Damage.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_CM] = drNewCM[DT_CounterM.ID];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_WBS] = sourceWBS[DT_CM_WBS.ID_WBS];
                            if (targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] {
                                                                    sourceWBS[DT_CM_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], sourceCM.ID}))
                            {
                                newCMWBSDamage[DT_WBS_CM_Damage.VALUE] = targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Find(new object[] {
                                                                    sourceWBS[DT_CM_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], sourceCM.ID})[DT_WBS_CM_Damage.VALUE];
                            }
                            newCMWBSDamage[DT_WBS_CM_Damage.VALUE] = 0;
                            newCMWBSDamage[DT_WBS_CM_Damage.WBS] = sourceWBS[DT_CM_WBS.WBS];
                            targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newCMWBSDamage);
                        }
                    }
                }
            }

            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRiskFather[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newCMWBS = targetDataset.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                    newCMWBS[DT_CM_WBS.ID_CM] = drNewCM[DT_CounterM.ID];
                    newCMWBS[DT_CM_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                    newCMWBS[DT_CM_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newCMWBS[DT_CM_WBS.PRIMARY] = "";
                    newCMWBS[DT_CM_WBS.PROBABILITY] = drNewCM[DT_CounterM.PROBABILITY];
                    newCMWBS[DT_CM_WBS.CM] = drNewCM[DT_CounterM.NAMESHORT];
                    newCMWBS[DT_CM_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newCMWBS[DT_CM_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                }
                foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
                {
                    if (!(targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { targetWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewCM[DT_CounterM.ID] })))
                    {
                        if (WBS_isSheet((int)targetWBS[DT_RISK_WBS.ID_WBS], DsWBS))
                        {
                            DataRow newCMWBSDamage = targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                            newCMWBSDamage[DT_WBS_CM_Damage.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_CM] = drNewCM[DT_CounterM.ID];
                            newCMWBSDamage[DT_WBS_CM_Damage.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                            newCMWBSDamage[DT_WBS_CM_Damage.VALUE] = 0;
                            newCMWBSDamage[DT_WBS_CM_Damage.WBS] = targetWBS[DT_RISK_WBS.WBS];
                            targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newCMWBSDamage);
                        }
                    }
                }
            }

            foreach (DataRow riskRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + drRiskFather[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], riskRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drCmRole = targetDataset.Tables[DT_Role_CM.TABLENAME].NewRow();
                    drCmRole[DT_Role_CM.IDROL_COLUMN] = riskRole[DT_Role_Risk.IDROL_COLUMN];
                    drCmRole[DT_Role_CM.ID_CM] = drNewCM[DT_CounterM.ID];
                    drCmRole[DT_Role_CM.NAME_SHORT] = drNewCM[DT_CounterM.NAMESHORT];
                    drCmRole[DT_Role_CM.Role] = riskRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_CM.TABLENAME].Rows.Add(drCmRole);
                }
            }
        }

        #endregion
    }
}
