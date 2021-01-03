using DataMapping.Data;
using EnsureBusinesss.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnsureBusinesss
{
    public class CopyPasteController
    {
        #region Copy&Paste/Move/Import_WithSourceData

        #region KeepAllData
        public static DataRow KeepAllData(RiskPolyLine sourceLine, DataSet targetDataset, DataRow drTargetRisk, bool isMain,
            decimal ID_Diagram, DataSet DsWBS, List<RiskPolyLine> LinesList)
        {
            decimal sourceID_Diagram;
            string DiagramName = "";
            if (sourceLine.IsRoot)
            {
                sourceID_Diagram = (decimal)targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.ID_DIAGRAM];
                DiagramName = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(sourceID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
            }
            DataRow drNewRisk = targetDataset.Tables[DT_Risk.TABLE_NAME].NewRow();
            drNewRisk[DT_Risk.COMMENTS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.COMMENTS];
            drNewRisk[DT_Risk.IS_ACTIVE] = true;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceLine.IsRoot ? DiagramName + " Main Risk" : sourceLine.ShortName;
            drNewRisk[DT_Risk.POSITION] = isMain ? LinesList.Find(r => r.ID == (decimal)drTargetRisk[DT_Risk.ID]).Children.Count : targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.POSITION];

            drNewRisk[DT_Risk.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.GROUPE_NAME];
            drNewRisk[DT_Risk.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.ID_GROUPE];
            drNewRisk[DT_Risk.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.ID_WBS];
            drNewRisk[DT_Risk.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.WBS_NAME];
            drNewRisk[DT_Risk.USER_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.USER_NAME];
            drNewRisk[DT_Risk.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.PROBABILITY];
            drNewRisk[DT_Risk.IS_CM] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.IS_CM];
            drNewRisk[DT_Risk.FATHER] = drTargetRisk[DT_Risk.NAMESHORT];
            drNewRisk[DT_Risk.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Add(drNewRisk);
            DataRow rowstructure = targetDataset.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drNewRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

            SetOriginalAndNewDamagesCopiedRisk(sourceLine, targetDataset, drNewRisk, ID_Diagram);

            SetWBSFromSourceLine(sourceLine, targetDataset, drNewRisk);
            SetWBS_Risk_DamageFromSourceLine(targetDataset, drNewRisk, sourceLine, DsWBS);            

            SetNewAndOriginalRoleRisk(sourceLine, targetDataset, drNewRisk, drTargetRisk);

            if (!(sourceLine.IsLeaf()))
            {
                foreach (var risk in sourceLine.Children)
                {
                    KeepAllData(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS, LinesList);
                }
            }
            return drNewRisk;
        }

        /// <summary>
        /// Add to the Damages values of the New Risk the values of the Source Risk, and the New Damages existing in the current Diagram. Evaluate for copy/import in many diagrams.
        /// </summary>
        /// <param name="sourceRisk">Risk Copied</param>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCreated">Data of the Created Risk</param>
        /// <param name="ID_Diagram">The Id of the Current Diagram</param>
        public static void SetOriginalAndNewDamagesCopiedRisk(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drRiskCreated, decimal ID_Diagram)
        {
            //TODOS LOS DAMAGES DEL DIAGRAM SOURCE
            foreach (DataRow oldRiskDamage in targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE]; ;
                    drRiskDamage[DT_Risk_Damages.FATHER] = drRiskCreated[DT_Risk.FATHER];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = oldRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCreated[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.IS_CM] = drRiskCreated[DT_Risk.IS_CM];
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { sourceRisk.ID, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.PROBABILITY]; ;
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = sourceRisk.ShortName;
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = oldRiskDamage[DT_Risk_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.USER_NAME];
                    if (targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { sourceRisk.ID, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] }))
                    {
                        drRiskDamage[DT_Risk_Damages.VALUE] = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { sourceRisk.ID, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.VALUE];
                    }
                    else
                    {
                        drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    }
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
            //LOS DAMAGES DEL DIAGRAM TARGET
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                //EVALUO QUE NO ESTE YA PARA NO SOBREEESCRIBIR 
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE];
                    drRiskDamage[DT_Risk_Damages.FATHER] = drRiskCreated[DT_Risk.FATHER];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCreated[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.IS_CM] = drRiskCreated[DT_Risk.IS_CM];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.FATHER] = drRiskCreated[DT_Risk.FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.PROBABILITY];
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = sourceRisk.ShortName;
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.USER_NAME];
                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        private static void SetWBSFromSourceLine(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drRiskCreated)
        {
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = targetWBS[DT_RISK_WBS.PROBABILITY];
                    newRiskWBS[DT_RISK_WBS.RISK] = drRiskCreated[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    newRiskWBS[DT_RISK_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" + targetWBS[DT_RISK_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
        }

        public static void SetWBS_Risk_DamageFromSourceLine(DataSet targetDataset, DataRow drRiskCreated, RiskPolyLine sourceLine, DataSet DsWBS)
        {
            foreach (DataRow rowWBSDamageRisk in targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + sourceLine.ID))
            {
                if (!targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(
                    new object[] { rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS], rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drRiskCreated[DT_Risk.ID] }))
                {
                    DataRow drWBSDamageRisk = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.DAMAGE] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.DAMAGE];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.VALUE] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.VALUE];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS_USER] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS_USER];
                    targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBSDamageRisk);
                }
            }
            SetWBS_Risk_DamageFromNewDamages(drRiskCreated, targetDataset, DsWBS);
        }

        #endregion

        #region KeepOnlyWBS
        public static DataRow KeepOnlyWBS(RiskPolyLine sourceLine, DataSet targetDataset, DataRow drTargetRisk, bool isMain,
    decimal ID_Diagram, DataSet DsWBS, List<RiskPolyLine> LinesList)
        {
            decimal sourceID_Diagram;
            string DiagramName = "";
            if (sourceLine.IsRoot)
            {
                sourceID_Diagram = (decimal)targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.ID_DIAGRAM];
                DiagramName = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(sourceID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
            }
            DataRow drNewRisk = targetDataset.Tables[DT_Risk.TABLE_NAME].NewRow();
            drNewRisk[DT_Risk.COMMENTS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.COMMENTS];
            drNewRisk[DT_Risk.IS_ACTIVE] = true;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceLine.IsRoot ? DiagramName + " Main Risk" : sourceLine.ShortName;
            drNewRisk[DT_Risk.POSITION] = isMain ? LinesList.Find(r => r.ID == (decimal)drTargetRisk[DT_Risk.ID]).Children.Count : targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.POSITION];

            drNewRisk[DT_Risk.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.GROUPE_NAME];
            drNewRisk[DT_Risk.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.ID_GROUPE];
            drNewRisk[DT_Risk.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.ID_WBS];
            drNewRisk[DT_Risk.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.WBS_NAME];
            drNewRisk[DT_Risk.USER_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.USER_NAME];
            drNewRisk[DT_Risk.IS_CM] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceLine.ID)[DT_Risk.IS_CM];
            drNewRisk[DT_Risk.PROBABILITY] = (bool)drNewRisk[DT_Risk.IS_CM] ? 0 : 100;
            drNewRisk[DT_Risk.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            drNewRisk[DT_Risk.FATHER] = drTargetRisk[DT_Risk.NAMESHORT];
            targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Add(drNewRisk);
            DataRow rowstructure = targetDataset.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drNewRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

            SetDamageNewCopiedRiskInCero(targetDataset, drNewRisk, ID_Diagram);

            SetWBSFromSourceLineInCero(sourceLine, targetDataset, drNewRisk);

            SetWBS_Risk_DamageFromSourceLineInCero(targetDataset, drNewRisk, sourceLine, DsWBS);

            SetRoleRisk(targetDataset, drNewRisk, drTargetRisk);

            if (!sourceLine.IsLeaf())
            {
                foreach (RiskPolyLine risk in sourceLine.Children)
                {
                    KeepOnlyWBS(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS, LinesList);
                }
            }
            return drNewRisk;
        }

        /// <summary>
        /// Add to New Risk Copied the damages of the current Diagram
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCopied">The new Risk Copied</param>
        /// <param name="ID_Diagram">ID of the Current Diagram</param>
        public static void SetDamageNewCopiedRiskInCero(DataSet targetDataset, DataRow drRiskCopied, decimal ID_Diagram)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = diagramDamages[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                    drRiskDamage[DT_Risk_Damages.FATHER] = drRiskCopied[DT_Risk.FATHER];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.ID])[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCopied[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = drRiskCopied[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCopied[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.FATHER] = drRiskCopied[DT_Risk.FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = (bool)drRiskCopied[DT_Risk.IS_CM] ? 0 : 100;
                    drRiskDamage[DT_Risk_Damages.IS_CM] = drRiskCopied[DT_Risk.IS_CM];
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = drRiskCopied[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = diagramDamages[DT_Diagram_Damages.RISK_TREE];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.USER_NAME];
                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        public static void SetOriginalAndNewDamagesCopiedRiskInCero(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drRiskCreated, decimal ID_Diagram)
        {
            //TODOS LOS DAMAGES DEL DIAGRAM SOURCE
            foreach (DataRow oldRiskDamage in targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, oldRiskDamage[DT_Risk_Damages.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE]; ;
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = oldRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCreated[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = sourceRisk.IsCM ? 0 : 100;
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = sourceRisk.ShortName;
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = oldRiskDamage[DT_Risk_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.USER_NAME];

                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;

                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
            //LOS DAMAGES DEL DIAGRAM TARGET
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                //EVALUO QUE NO ESTE YA PARA NO SOBREEESCRIBIR 
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })[DT_Diagram_Damages.DAMAGE];
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCreated[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = sourceRisk.IsCM ? 0 : 100;
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = sourceRisk.ShortName;
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.USER_NAME];
                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        private static void SetWBSFromSourceLineInCero(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drRiskCreated)
        {
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = (bool)drRiskCreated[DT_Risk.IS_CM] ? 0 : 100;
                    newRiskWBS[DT_RISK_WBS.RISK] = drRiskCreated[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    newRiskWBS[DT_RISK_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" + targetWBS[DT_RISK_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
        }

        public static void SetWBS_Risk_DamageFromSourceLineInCero(DataSet targetDataset, DataRow drRiskCreated, RiskPolyLine sourceLine, DataSet DsWBS)
        {
            foreach (DataRow rowWBSDamageRisk in targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + sourceLine.ID))
            {
                if (!targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(
                    new object[] { rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS], rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drRiskCreated[DT_Risk.ID] }))
                {
                    DataRow drWBSDamageRisk = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.DAMAGE] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.DAMAGE];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.ID_WBS];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS];
                    drWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS_USER] = rowWBSDamageRisk[DT_WBS_RISK_DAMAGE.WBS_USER];
                    targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBSDamageRisk);
                }
            }
            SetWBS_Risk_DamageFromNewDamages(drRiskCreated, targetDataset, DsWBS);
        }


        #endregion

        #region KeepNothing
        /// <summary>
        /// Create a copy of the source Risk with the basic data (Short Name and coments), without the WBS and Damage data. The values will be by default as 0
        /// </summary>
        /// <param name="sourceRisk">The source Line</param>
        /// <param name="targetDataset">Dataset</param>
        /// <param name="drTargetRisk">Target data of the risk</param>
        /// <param name="isMain">A flag to control the position to copy</param>
        /// <param name="ID_Diagram">The current Diagram ID</param>
        /// <param name="DsWBS">The WBS Dataset</param>
        /// <returns>>Return the data of the line created as copy</returns>
        public static DataRow KeepNothing(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drTargetRisk, bool isMain, decimal ID_Diagram, DataSet DsWBS)
        {
            decimal sourceID_Diagram;
            string DiagramName = "";
            if (sourceRisk.IsRoot)
            {
                sourceID_Diagram = (decimal)targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_DIAGRAM];
                DiagramName = targetDataset.Tables[DT_Diagram.TABLE_NAME].Rows.Find(sourceID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
            }
            DataRow drNewRisk = targetDataset.Tables[DT_Risk.TABLE_NAME].NewRow();
            drNewRisk[DT_Risk.COMMENTS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.COMMENTS];
            drNewRisk[DT_Risk.IS_ACTIVE] = true;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceRisk.IsRoot ? DiagramName + " Main Risk" : sourceRisk.ShortName;
            drNewRisk[DT_Risk.POSITION] = isMain ? sourceRisk.Position : targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.POSITION];

            drNewRisk[DT_Risk.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.GROUPE_NAME]; ;
            drNewRisk[DT_Risk.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.ID_GROUPE]; ;
            drNewRisk[DT_Risk.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.ID_WBS];
            drNewRisk[DT_Risk.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.WBS_NAME];
            drNewRisk[DT_Risk.USER_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drTargetRisk[DT_Risk.ID])[DT_Risk.USER_NAME];
            drNewRisk[DT_Risk.IS_CM] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.IS_CM];

            drNewRisk[DT_Risk.PROBABILITY] = (bool)drNewRisk[DT_Risk.IS_CM] ? 0 : 100;

            drNewRisk[DT_Risk.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            drNewRisk[DT_Risk.FATHER] = drTargetRisk[DT_Risk.NAMESHORT];

            targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Add(drNewRisk);

            DataRow rowstructure = targetDataset.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drNewRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

            SetDamageNewCopiedRisk(targetDataset, drNewRisk, ID_Diagram);
            SetWBS_RiskNewCopiedRisk(targetDataset, drNewRisk, drTargetRisk, DsWBS);
            SetWBS_RISK_DamageNewCopiedRisk(targetDataset, drNewRisk, DsWBS);
            SetRoleRisk(targetDataset, drNewRisk, drTargetRisk);
            if (!(sourceRisk.IsLeaf()))
            {
                foreach (var risk in sourceRisk.Children)
                {
                    KeepNothing(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS);
                }
            }
            return drNewRisk;
        }

        /// <summary>
        /// Add to New Risk Copied the damages of the current Diagram
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCopied">The new Risk Copied</param>
        /// <param name="ID_Diagram">ID of the Current Diagram</param>
        public static void SetDamageNewCopiedRisk(DataSet targetDataset, DataRow drRiskCopied, decimal ID_Diagram)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow drRiskDamage = targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    drRiskDamage[DT_Risk_Damages.COLOR] = diagramDamages[DT_Diagram_Damages.COLOR];
                    drRiskDamage[DT_Risk_Damages.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.ID])[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCopied[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.FATHER] = drRiskCopied[DT_Risk.FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = drRiskCopied[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCopied[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = (bool)drRiskCopied[DT_Risk.IS_CM] ? 0 : 100;
                    drRiskDamage[DT_Risk_Damages.IS_CM] = drRiskCopied[DT_Risk.IS_CM];
                    drRiskDamage[DT_Risk_Damages.RISK_NAMESHORT] = drRiskCopied[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.RISK_TREE] = diagramDamages[DT_Diagram_Damages.RISK_TREE];
                    drRiskDamage[DT_Risk_Damages.STATUS] = true;
                    drRiskDamage[DT_Risk_Damages.TOP_RISK] = diagramDamages[DT_Diagram_Damages.TOP_RISK];
                    drRiskDamage[DT_Risk_Damages.USERNAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.USER_NAME];
                    drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        /// <summary>
        /// Add to the New Risk Copied the existing WBS from the Father Risk
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCopied">The data of the New Risk Copied</param>
        /// <param name="drTargetRisk">The data of the Risk Father</param>
        /// <param name="DsWBS">The WBS Dataset</param>
        public static void SetWBS_RiskNewCopiedRisk(DataSet targetDataset, DataRow drRiskCopied, DataRow drTargetRisk, DataSet DsWBS)
        {
            if (targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]).Any())
            {
                foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
                {
                    if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                    {
                        DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCopied[DT_Risk.ID];
                        newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                        newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                        newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                        newRiskWBS[DT_RISK_WBS.PROBABILITY] = (bool)drRiskCopied[DT_Risk.IS_CM] ? 0 : 100;
                        newRiskWBS[DT_RISK_WBS.RISK] = drRiskCopied[DT_Risk.NAMESHORT];
                        newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                        newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                        newRiskWBS[DT_RISK_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" + targetWBS[DT_RISK_WBS.USERNAME] + "]";
                        targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                    }
                }
                if (!targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRiskCopied[DT_Risk.ID]).Any())
                {
                    bool primary = true;
                    foreach (var wbs in WBSOperations.GetTopWBS(DsWBS))
                    {
                        if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], wbs[DT_WBS.ID_WBS] })))
                        {
                            DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                            newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCopied[DT_Risk.ID];
                            newRiskWBS[DT_RISK_WBS.ID_WBS] = wbs[DT_WBS.ID_WBS];
                            newRiskWBS[DT_RISK_WBS.WBS] = wbs[DT_WBS.WBS_NAME];
                            newRiskWBS[DT_RISK_WBS.NIVEL] = wbs[DT_WBS.NIVEL];
                            newRiskWBS[DT_RISK_WBS.USERNAME] = wbs[DT_WBS.USERNAME];
                            newRiskWBS[DT_RISK_WBS.WBS_USER] = wbs[DT_WBS.WBS_NAME] + "[" + wbs[DT_WBS.USERNAME] + "]";
                            newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                            newRiskWBS[DT_RISK_WBS.PROBABILITY] = (bool)drRiskCopied[DT_Risk.IS_CM] ? 0 : 100; ;
                            primary = false;
                            targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                        }
                    }
                }
            }
            else
            {
                bool primary = true;
                foreach (var topWBS in WBSOperations.GetTopWBS(DsWBS))
                {
                    if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], topWBS[DT_WBS.ID_WBS] })))
                    {
                        DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCopied[DT_Risk.ID];
                        newRiskWBS[DT_RISK_WBS.ID_WBS] = topWBS[DT_WBS.ID_WBS];
                        newRiskWBS[DT_RISK_WBS.WBS] = topWBS[DT_WBS.WBS_NAME];
                        newRiskWBS[DT_RISK_WBS.NIVEL] = topWBS[DT_WBS.NIVEL];
                        newRiskWBS[DT_RISK_WBS.USERNAME] = topWBS[DT_WBS.USERNAME];
                        newRiskWBS[DT_RISK_WBS.WBS_USER] = topWBS[DT_WBS.WBS_NAME] + "[" + topWBS[DT_WBS.USERNAME] + "]";
                        newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                        newRiskWBS[DT_RISK_WBS.PROBABILITY] = (bool)drRiskCopied[DT_Risk.IS_CM] ? 0 : 100;
                        primary = false;
                        targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                    }
                }
            }
        }
               
        /// <summary>
        /// Add Damage Values for each WBS with the lowest level in the Risk Created
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCreated">Data of the Risk Created</param>
        /// <param name="DsWBS">WBS Dataset</param>
        public static void SetWBS_RISK_DamageNewCopiedRisk(DataSet targetDataset, DataRow drRiskCreated, DataSet DsWBS)
        {
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRiskCreated[DT_Risk.ID]))
            {
                if (WBSOperations.IsRiskWBSLow(targetWBS, DsWBS, targetDataset.Tables[DT_RISK_WBS.TABLE_NAME]))
                {
                    AddWBS_Risk_Damage(drRiskCreated, targetDataset, targetWBS);
                }
                else
                {
                    DeleteWBS_Risk_Damage(targetDataset, drRiskCreated, targetWBS);
                }
            }
        }

        /// <summary>
        /// Set to the New Risk Created the role from the Risk Father
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCreated">The Data of Risk Created</param>
        /// <param name="drTargetRisk">The Data of the Risk Father </param>
        public static void SetRoleRisk(DataSet targetDataset, DataRow drRiskCreated, DataRow drTargetRisk)
        {
            foreach (DataRow riskRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], riskRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drRiskRole = targetDataset.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskRole[DT_Role_Risk.IDROL_COLUMN];
                    drRiskRole[DT_Role_Risk.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drRiskRole[DT_Role_Risk.NAME_SHORT] = drRiskCreated[DT_Risk.NAMESHORT];
                    drRiskRole[DT_Role_Risk.Role] = riskRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                }
            }
        }

        #endregion

        /// <summary>
        /// Copy the WBS, Role and WBS_Damage from the Child to the Father and the Ancestors
        /// </summary>
        /// <param name="drRiskCopied">The Risk copied</param>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drTargetRisk">data of the Risk Father</param>
        /// <param name="DsWBS">Dataset with the WBS</param>
        public static void SetValuesFromChildToAncestors(DataRow drRiskCopied, DataSet targetDataset, DataRow drTargetRisk, DataSet DsWBS, bool isCM)
        {
            foreach (DataRow rowRiskWbs in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRiskCopied[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drTargetRisk[DT_Risk.ID], rowRiskWbs[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drTargetRisk[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = rowRiskWbs[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                    newRiskWBS[DT_RISK_WBS.NIVEL] = rowRiskWbs[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = drTargetRisk[DT_Risk.PROBABILITY];
                    newRiskWBS[DT_RISK_WBS.RISK] = drTargetRisk[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = rowRiskWbs[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = rowRiskWbs[DT_RISK_WBS.WBS];
                    newRiskWBS[DT_RISK_WBS.WBS_USER] = rowRiskWbs[DT_RISK_WBS.WBS] + "[" + rowRiskWbs[DT_RISK_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
            foreach (DataRow rowRiskWbs in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (WBSOperations.IsRiskWBSLow(rowRiskWbs, DsWBS, targetDataset.Tables[DT_RISK_WBS.TABLE_NAME]))
                {
                    AddWBS_Risk_Damage(drTargetRisk, targetDataset, rowRiskWbs);
                }
                else
                {
                    DeleteWBS_Risk_Damage(targetDataset, drTargetRisk, rowRiskWbs);
                }
            }
            if (!(bool)drTargetRisk[DT_Risk.IS_ROOT])
            {
                if (targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + drTargetRisk[DT_Risk.ID]).Any())
                {
                    decimal idFather = (decimal)targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + drTargetRisk[DT_Risk.ID]).First()[DT_RiskStructure.IDRISK_FATHER];
                    if (targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Contains(idFather))
                    {
                        SetValuesFromChildToAncestors(drTargetRisk, targetDataset, targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(idFather), DsWBS, false);
                    }
                }
            }
        }

        private static void DeleteWBS_Risk_Damage(DataSet targetDataset, DataRow drNewRisk, DataRow sourceWBS)
        {
            foreach (DataRow itemDamage in targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + drNewRisk[DT_Risk.ID]))
            {
                if (targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { sourceWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] }))
                {
                    targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(new object[] { sourceWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] }).Delete();
                }
            }
        }

        private static void AddWBS_Risk_Damage(DataRow drNewRisk, DataSet targetDataset, DataRow targetWBS)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + drNewRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { targetWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Risk_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] })))
                {
                    DataRow newRiskWBSDamage = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = diagramDamages[DT_Risk_Damages.DAMAGE];
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = diagramDamages[DT_Risk_Damages.ID_DAMAGE];
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = drNewRisk[DT_Risk.ID];
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS_USER] = targetWBS[DT_RISK_WBS.WBS_USER];
                    targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRiskWBSDamage);
                }
            }
        }
        #endregion

        #region Copy&Paste/Move/Import_EMPTY

        private static void SetWBS_Risk_DamageFromNewDamages(DataRow drNewRisk, DataSet targetDataset, DataSet DsWBS)
        {            
            foreach (DataRow rowWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drNewRisk[DT_Risk.ID]))
            {
                if (WBSOperations.IsRiskWBSLow(rowWBS, DsWBS, targetDataset.Tables[DT_RISK_WBS.TABLE_NAME]))
                {
                    foreach (DataRow diagramDamages in targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + drNewRisk[DT_Risk.ID]))
                    {
                        if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { rowWBS[DT_RISK_WBS.ID_WBS], diagramDamages[DT_Risk_Damages.ID_DAMAGE], drNewRisk[DT_Risk.ID] })))
                        {
                            DataRow newRiskWBSDamage = targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = diagramDamages[DT_Risk_Damages.DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = diagramDamages[DT_Risk_Damages.ID_DAMAGE];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = drNewRisk[DT_Risk.ID];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = rowWBS[DT_RISK_WBS.ID_WBS];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS] = rowWBS[DT_RISK_WBS.WBS];
                            newRiskWBSDamage[DT_WBS_RISK_DAMAGE.WBS_USER] = rowWBS[DT_RISK_WBS.WBS_USER];
                            targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRiskWBSDamage);
                        }
                    }
                }
            }
        }

        #endregion

        #region Function_For_Import
        /// <summary>
        /// Add to the current Diagram the imported damages from Source Diagram
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="importedDamages">Imported Damages from the Source Diagram</param>
        /// <param name="ID_Diagram">The current Diagram ID</param>
        public static void AddDamageToDiagram(DataSet targetDataset, DataRow[] importedDamages, decimal ID_Diagram)
        {
            foreach (DataRow drDamage in importedDamages)
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

        /// <summary>
        /// Add to all Risk in the Current Diagram the Imported Damages from the Source Diagram
        /// </summary>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="importedDamages">The list of the imported Damages from the Source Diagram</param>
        /// <param name="ID_Diagram">The Current Diagram ID</param>
        /// <param name="DsWBS">The WBS Dataste</param>
        public static void AddImportedDamagesToAllRisk(DataSet targetDataset, DataRow[] importedDamages, decimal ID_Diagram, DataSet DsWBS)
        {
            foreach (DataRow drDamage in importedDamages)
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
                        drRiskDamage[DT_Risk_Damages.STATUS] = risk[DT_Risk.IS_ACTIVE];
                        drRiskDamage[DT_Risk_Damages.TOP_RISK] = drDamage[DT_Diagram_Damages.TOP_RISK];
                        drRiskDamage[DT_Risk_Damages.USERNAME] = risk[DT_Risk.USER_NAME];
                        drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                        drRiskDamage[DT_Risk_Damages.WBS_NAME] = risk[DT_Risk.WBS_NAME];
                        targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                    }
                }
            }
        }

        /// <summary>
        /// Copy in the Created Risk, the source Risk´s roles and the roles of the New Risk inherit  from the Father
        /// </summary>
        /// <param name="sourceRisk">The Source Risk</param>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCopied">The data of the Risk Copied</param>
        /// <param name="drTargetRisk"></param>
        public static void SetNewAndOriginalRoleRisk(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drRiskCopied, DataRow drTargetRisk)
        {
            //Original
            foreach (DataRow riskOriginalRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], riskOriginalRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drRiskRole = targetDataset.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskOriginalRole[DT_Role_Risk.IDROL_COLUMN];
                    drRiskRole[DT_Role_Risk.ID_RISK] = drRiskCopied[DT_Risk.ID];
                    drRiskRole[DT_Role_Risk.NAME_SHORT] = drRiskCopied[DT_Risk.NAMESHORT];
                    drRiskRole[DT_Role_Risk.Role] = riskOriginalRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                }
            }
            //New
            foreach (DataRow riskRole in targetDataset.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], riskRole[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow drRiskRole = targetDataset.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = riskRole[DT_Role_Risk.IDROL_COLUMN];
                    drRiskRole[DT_Role_Risk.ID_RISK] = drRiskCopied[DT_Risk.ID];
                    drRiskRole[DT_Role_Risk.NAME_SHORT] = drRiskCopied[DT_Risk.NAMESHORT];
                    drRiskRole[DT_Role_Risk.Role] = riskRole[DT_Role_Risk.Role];
                    targetDataset.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
                }
            }
        }

        #endregion

        #region PasteForSavingAs&ReopenDiagram
        private static void FillRiskDamages(DataSet dsSource, DataSet dsTarget, RiskPolyLine itemRisk, RiskPolyLine riskFather, DataRow drRisk, decimal idDiagram)
        {
            foreach (DataRow itemi in dsSource.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + itemRisk.ID))
            {
                DataRow top = dsTarget.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                top[DT_Risk_Damages.COLOR] = itemi[DT_Risk_Damages.COLOR];
                top[DT_Risk_Damages.DAMAGE] = itemi[DT_Risk_Damages.DAMAGE];
                top[DT_Risk_Damages.FATHER] = riskFather.ShortName;
                top[DT_Risk_Damages.GROUPE_NAME] = drRisk[DT_Risk.GROUPE_NAME];
                top[DT_Risk_Damages.ID_FATHER] = riskFather.ID;
                top[DT_Risk_Damages.ID_GROUPE] = drRisk[DT_Risk.ID_GROUPE];
                top[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                top[DT_Risk_Damages.ID_RISK_TREE] = idDiagram;
                top[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                top[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                top[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                top[DT_Risk_Damages.RISK_TREE] = dsTarget.Tables[DT_Diagram.TABLE_NAME].Rows.Find(idDiagram)[DT_Diagram.DIAGRAM_NAME];
                top[DT_Risk_Damages.STATUS] = drRisk[DT_Risk.IS_ACTIVE];
                top[DT_Risk_Damages.ID_DAMAGE] = itemi[DT_Risk_Damages.ID_DAMAGE];
                top[DT_Risk_Damages.VALUE] = itemi[DT_Risk_Damages.VALUE];
                top[DT_Risk_Damages.TOP_RISK] = itemi[DT_Risk_Damages.TOP_RISK];
                top[DT_Risk_Damages.ID_WBS] = itemi[DT_Risk_Damages.ID_WBS];
                top[DT_Risk_Damages.WBS_NAME] = itemi[DT_Risk_Damages.WBS_NAME];
                top[DT_Risk_Damages.USERNAME] = itemi[DT_Risk_Damages.USERNAME];
                dsTarget.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(top);
            }
        }

        private static void FillRiskWBS(DataSet dsSource, DataSet dsTarget, RiskPolyLine item, DataRow drRisk, RiskPolyLine riskFather)
        {
            //Los WBS que tenia originalmente
            foreach (DataRow itemi in dsSource.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID))
            {
                DataRow rwbs = dsTarget.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                rwbs[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                rwbs[DT_RISK_WBS.ID_WBS] = itemi[DT_RISK_WBS.ID_WBS];
                rwbs[DT_RISK_WBS.NIVEL] = itemi[DT_RISK_WBS.NIVEL];
                rwbs[DT_RISK_WBS.RISK] = drRisk[DT_Risk.NAMESHORT];
                rwbs[DT_RISK_WBS.WBS] = itemi[DT_RISK_WBS.WBS];
                rwbs[DT_RISK_WBS.IS_PRIMARY] = itemi[DT_RISK_WBS.IS_PRIMARY];
                rwbs[DT_RISK_WBS.USERNAME] = itemi[DT_RISK_WBS.USERNAME];
                rwbs[DT_RISK_WBS.WBS_USER] = itemi[DT_RISK_WBS.WBS] + "[" + itemi[DT_RISK_WBS.USERNAME] + "]";
                rwbs[DT_RISK_WBS.PROBABILITY] = itemi[DT_RISK_WBS.PROBABILITY];
                dsTarget.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(rwbs);
            }
            //Los WBS que adquiere del padre
            foreach (DataRow wbsFather in dsTarget.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + riskFather.ID))
            {
                if (!(dsTarget.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], wbsFather[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow rwbs = dsTarget.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    rwbs[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                    rwbs[DT_RISK_WBS.ID_WBS] = wbsFather[DT_RISK_WBS.ID_WBS];
                    rwbs[DT_RISK_WBS.NIVEL] = wbsFather[DT_RISK_WBS.NIVEL];
                    rwbs[DT_RISK_WBS.RISK] = drRisk[DT_Risk.NAMESHORT];
                    rwbs[DT_RISK_WBS.WBS] = wbsFather[DT_RISK_WBS.WBS];
                    rwbs[DT_RISK_WBS.IS_PRIMARY] = false;
                    rwbs[DT_RISK_WBS.USERNAME] = wbsFather[DT_RISK_WBS.USERNAME];
                    rwbs[DT_RISK_WBS.WBS_USER] = wbsFather[DT_RISK_WBS.WBS] + "[" + wbsFather[DT_RISK_WBS.USERNAME] + "]";
                    rwbs[DT_RISK_WBS.PROBABILITY] = 0;
                    dsTarget.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(rwbs);
                }
            }
            //Los WBS values que tiene originalmente
            foreach (DataRow itemi in dsSource.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + item.ID))
            {
                DataRow rwbs = dsTarget.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                rwbs[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                rwbs[DT_WBS_RISK_DAMAGE.ID_WBS] = itemi[DT_WBS_RISK_DAMAGE.ID_WBS];
                rwbs[DT_WBS_RISK_DAMAGE.WBS] = itemi[DT_WBS_RISK_DAMAGE.WBS];
                rwbs[DT_WBS_RISK_DAMAGE.VALUE] = itemi[DT_WBS_RISK_DAMAGE.VALUE];
                rwbs[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemi[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                rwbs[DT_WBS_RISK_DAMAGE.DAMAGE] = itemi[DT_WBS_RISK_DAMAGE.DAMAGE];
                rwbs[DT_WBS_RISK_DAMAGE.WBS_USER] = itemi[DT_WBS_RISK_DAMAGE.WBS_USER];
                dsTarget.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(rwbs);
            }
            //Los WBS values que faltan por completar
            foreach (DataRow fatherWBSValues in dsTarget.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + riskFather.ID))
            {
                if (!(dsTarget.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] {fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_WBS],
                    fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drRisk[DT_Risk.ID] })))
                {
                    DataRow rwbs = dsTarget.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    rwbs[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                    rwbs[DT_WBS_RISK_DAMAGE.ID_WBS] = fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_WBS];
                    rwbs[DT_WBS_RISK_DAMAGE.WBS] = fatherWBSValues[DT_WBS_RISK_DAMAGE.WBS];
                    rwbs[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                    rwbs[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                    rwbs[DT_WBS_RISK_DAMAGE.DAMAGE] = fatherWBSValues[DT_WBS_RISK_DAMAGE.DAMAGE];
                    rwbs[DT_WBS_RISK_DAMAGE.WBS_USER] = fatherWBSValues[DT_WBS_RISK_DAMAGE.WBS_USER];
                    dsTarget.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(rwbs);
                }
            }
        }

        private static void FillRiskRole(DataSet dsSource, DataSet dsTarget, RiskPolyLine riskFather, DataRow drRisk, RiskPolyLine rourceRisk)
        {
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + riskFather.ID))
            {
                if (!(dsTarget.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsTarget.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    dsTarget.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
            //for each role that original have 
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + rourceRisk.ID))
            {
                if (!(dsTarget.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsTarget.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    dsTarget.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
        }

        private static void FillCMDamages(DataSet dsSource, DataSet dsTarget, RiskPolyLine riskFather, DataRow drCM, RiskPolyLine cmLine, decimal idDiagram)
        {
            foreach (DataRow itemi in dsSource.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + cmLine.ID))
            {
                DataRow top = dsTarget.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                top[DT_Risk_Damages.COLOR] = itemi[DT_Risk_Damages.COLOR];
                top[DT_Risk_Damages.ID_DAMAGE] = itemi[DT_Risk_Damages.ID_DAMAGE];
                top[DT_Risk_Damages.IS_CM] = itemi[DT_Risk_Damages.IS_CM];
                top[DT_Risk_Damages.ID_RISK] = drCM[DT_Risk.ID];
                top[DT_Risk_Damages.VALUE] = itemi[DT_Risk_Damages.VALUE];
                top[DT_Risk_Damages.TOP_RISK] = itemi[DT_Risk_Damages.TOP_RISK];
                top[DT_Risk_Damages.RISK_NAMESHORT] = drCM[DT_Risk.NAMESHORT];
                top[DT_Risk_Damages.DAMAGE] = itemi[DT_Risk_Damages.DAMAGE];
                top[DT_Risk_Damages.GROUPE_NAME] = itemi[DT_Risk_Damages.GROUPE_NAME];
                top[DT_Risk_Damages.ID_FATHER] = drCM[DT_Risk.IDRISK_FATHER];
                top[DT_Risk_Damages.ID_GROUPE] = itemi[DT_Risk_Damages.ID_GROUPE];
                top[DT_Risk_Damages.ID_RISK_TREE] = idDiagram;
                top[DT_Risk_Damages.FATHER] = riskFather.ShortName;
                top[DT_Risk_Damages.PROBABILITY] = drCM[DT_Risk.PROBABILITY];
                top[DT_Risk_Damages.STATUS] = itemi[DT_Risk_Damages.STATUS];
                top[DT_Risk_Damages.ID_WBS] = itemi[DT_Risk_Damages.ID_WBS];
                top[DT_Risk_Damages.WBS_NAME] = itemi[DT_Risk_Damages.WBS_NAME];
                top[DT_Risk_Damages.USERNAME] = itemi[DT_Risk_Damages.USERNAME];
                dsTarget.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(top);
            }
        }

        private static void FillCMWBS(DataSet dsSource, DataSet dsResult, RiskPolyLine item, DataRow drCM, RiskPolyLine riskfather)
        {
            //Los valores de WBS que tenia originalmente
            foreach (DataRow itemi in dsSource.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                rwbs[DT_RISK_WBS.ID_RISK] = drCM[DT_Risk.ID];
                rwbs[DT_RISK_WBS.ID_WBS] = itemi[DT_RISK_WBS.ID_WBS];
                rwbs[DT_RISK_WBS.NIVEL] = itemi[DT_RISK_WBS.NIVEL];
                rwbs[DT_RISK_WBS.RISK] = drCM[DT_Risk.NAMESHORT];
                rwbs[DT_RISK_WBS.WBS] = itemi[DT_RISK_WBS.WBS];
                rwbs[DT_RISK_WBS.IS_PRIMARY] = itemi[DT_RISK_WBS.IS_PRIMARY];
                rwbs[DT_RISK_WBS.USERNAME] = itemi[DT_RISK_WBS.USERNAME];
                rwbs[DT_RISK_WBS.WBS_USER] = itemi[DT_RISK_WBS.WBS] + "[" + itemi[DT_RISK_WBS.USERNAME] +"]";
                rwbs[DT_RISK_WBS.PROBABILITY] = itemi[DT_RISK_WBS.PROBABILITY];
                dsResult.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(rwbs);
            }
            //Los valores de WBS que tenia el padre
            //foreach (DataRow itemi in dsSource.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_CM + " = " + item.Father.ID))
            foreach (DataRow wbsFather in dsResult.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + riskfather.ID))
            {
                if (!(dsResult.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drCM[DT_Risk.ID], wbsFather[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow rwbs = dsResult.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    rwbs[DT_RISK_WBS.ID_RISK] = drCM[DT_Risk.ID];
                    rwbs[DT_RISK_WBS.ID_WBS] = wbsFather[DT_RISK_WBS.ID_WBS];
                    rwbs[DT_RISK_WBS.NIVEL] = wbsFather[DT_RISK_WBS.NIVEL];
                    rwbs[DT_RISK_WBS.RISK] = drCM[DT_Risk.NAMESHORT];
                    rwbs[DT_RISK_WBS.WBS] = wbsFather[DT_RISK_WBS.WBS];
                    rwbs[DT_RISK_WBS.IS_PRIMARY] = false;
                    rwbs[DT_RISK_WBS.USERNAME] = wbsFather[DT_RISK_WBS.USERNAME];
                    rwbs[DT_RISK_WBS.WBS_USER] = wbsFather[DT_RISK_WBS.WBS] + "[" + wbsFather[DT_RISK_WBS.USERNAME] + "]";
                    rwbs[DT_RISK_WBS.PROBABILITY] = 0;
                    dsResult.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(rwbs);
                }
            }
            //Los valores WBSDamage que tenia originalmente
            foreach (DataRow itemi in dsSource.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                rwbs[DT_WBS_RISK_DAMAGE.ID_RISK] = drCM[DT_Risk.ID];
                rwbs[DT_WBS_RISK_DAMAGE.ID_WBS] = itemi[DT_WBS_RISK_DAMAGE.ID_WBS];
                rwbs[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemi[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                rwbs[DT_WBS_RISK_DAMAGE.DAMAGE] = itemi[DT_WBS_RISK_DAMAGE.DAMAGE];
                rwbs[DT_WBS_RISK_DAMAGE.VALUE] = itemi[DT_WBS_RISK_DAMAGE.VALUE];
                rwbs[DT_WBS_RISK_DAMAGE.WBS] = itemi[DT_WBS_RISK_DAMAGE.WBS];
                rwbs[DT_WBS_RISK_DAMAGE.WBS_USER] = itemi[DT_WBS_RISK_DAMAGE.WBS_USER];
                dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(rwbs);
            }
            //Los valores WBSDamage que tiene el padre originalmente
            foreach (DataRow fatherWBSValues in dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + riskfather.ID))
            {
                if (!(dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] {fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_WBS],
                                                        fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drCM[DT_Risk.ID]})))
                {
                    DataRow rwbs = dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    rwbs[DT_WBS_RISK_DAMAGE.ID_RISK] = drCM[DT_Risk.ID];
                    rwbs[DT_WBS_RISK_DAMAGE.ID_WBS] = fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_WBS];
                    rwbs[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                    rwbs[DT_WBS_RISK_DAMAGE.DAMAGE] = fatherWBSValues[DT_WBS_RISK_DAMAGE.DAMAGE];
                    rwbs[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                    rwbs[DT_WBS_RISK_DAMAGE.WBS] = fatherWBSValues[DT_WBS_RISK_DAMAGE.WBS];
                    rwbs[DT_WBS_RISK_DAMAGE.WBS_USER] = fatherWBSValues[DT_WBS_RISK_DAMAGE.WBS_USER];
                    dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(rwbs);
                }
            }
        }

        private static void FillCMRoles(DataSet dsSource, DataSet dsResult, RiskPolyLine risk, DataRow drCM, RiskPolyLine item)
        {
            //foreach role that its father have
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + risk.ID))
            {
                if (!(dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drCM[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drCM[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
            //for each role that original have 
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + item.ID))
            {
                if (!(dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drCM[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drCM[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
        }

        public static void Paste(RiskPolyLine riskFather, DataSet dsSource, DataSet dsResult, decimal idDiagram)
        {
            try
            {
                foreach (var childLine in riskFather.Children)
                {
                    if (!(childLine.IsCM))
                    {
                        DataRow drRisk = dsResult.Tables[DT_Risk.TABLE_NAME].NewRow();
                        drRisk[DT_Risk.COMMENTS] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.COMMENTS];
                        drRisk[DT_Risk.IS_ACTIVE] = true;
                        drRisk[DT_Risk.ID_DIAGRAM] = idDiagram;
                        drRisk[DT_Risk.ISCOLLAPSED] = childLine.Collapsed;
                        drRisk[DT_Risk.IS_ROOT] = false;
                        drRisk[DT_Risk.NAMESHORT] = childLine.ShortName;
                        drRisk[DT_Risk.POSITION] = childLine.Position;
                        drRisk[DT_Risk.PROBABILITY] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.PROBABILITY];
                        drRisk[DT_Risk.IS_CM] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.IS_CM];
                        drRisk[DT_Risk.IDRISK_FATHER] = riskFather.ID;
                        drRisk[DT_Risk.ID_WBS] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.ID_WBS];
                        drRisk[DT_Risk.WBS_NAME] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.WBS_NAME];
                        drRisk[DT_Risk.USER_NAME] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.USER_NAME];
                        dsResult.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);
                        DataRow rowstructure = dsResult.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                        rowstructure[DT_RiskStructure.IDRISK] = drRisk[DT_Risk.ID];
                        rowstructure[DT_RiskStructure.IDRISK_FATHER] = riskFather.ID;
                        dsResult.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                        FillRiskDamages(dsSource, dsResult, childLine, riskFather, drRisk, idDiagram);
                        FillRiskWBS(dsSource, dsResult, childLine, drRisk, riskFather);
                        FillRiskRole(dsSource, dsResult, riskFather, drRisk, childLine);
                        childLine.ID = (decimal)drRisk[DT_Risk.ID];
                        if (!childLine.IsLeaf())
                        {
                            foreach (var item2 in childLine.Children)
                            {
                                item2.IdRiskFather = (decimal)drRisk[DT_Risk.ID];
                            }
                            Paste(childLine, dsSource, dsResult, idDiagram);
                        }
                    }
                    else
                    {
                        DataRow drCM = dsResult.Tables[DT_Risk.TABLE_NAME].NewRow();
                        drCM[DT_Risk.COMMENTS] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.COMMENTS];
                        drCM[DT_Risk.IS_ACTIVE] = true;
                        drCM[DT_Risk.ID_DIAGRAM] = idDiagram;
                        drCM[DT_Risk.IDRISK_FATHER] = riskFather.ID;
                        drCM[DT_Risk.NAMESHORT] = childLine.ShortName;
                        drCM[DT_Risk.POSITION] = childLine.Position;
                        drCM[DT_Risk.IS_CM] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.IS_CM];

                        drCM[DT_Risk.PROBABILITY] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.PROBABILITY];
                        drCM[DT_Risk.FATHER] = childLine.ShortName;
                        drCM[DT_Risk.ID_WBS] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.ID_WBS];
                        drCM[DT_Risk.WBS_NAME] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.WBS_NAME];
                        drCM[DT_Risk.USER_NAME] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.USER_NAME];
                        dsResult.Tables[DT_Risk.TABLE_NAME].Rows.Add(drCM);
                        DataRow rowstructure = dsResult.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                        rowstructure[DT_RiskStructure.IDRISK] = drCM[DT_Risk.ID];
                        rowstructure[DT_RiskStructure.IDRISK_FATHER] = riskFather.ID;
                        dsResult.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                        FillCMDamages(dsSource, dsResult, riskFather, drCM, childLine, idDiagram);
                        FillCMWBS(dsSource, dsResult, childLine, drCM, riskFather);
                        FillCMRoles(dsSource, dsResult, riskFather, drCM, childLine);
                        childLine.ID = (decimal)drCM[DT_Risk.ID];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
