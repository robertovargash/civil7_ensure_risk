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
        public static DataRow CopyRiskWithoutSourceData(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drTargetRisk, bool isMain, decimal ID_Diagram, DataSet DsWBS)
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
            drNewRisk[DT_Risk.ENABLED] = true;
            drNewRisk[DT_Risk.FROM_TOP] = sourceRisk.FromTop;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceRisk.IsRoot ? DiagramName : sourceRisk.ShortName;
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

            SetDamageNewCopiedRisk(targetDataset, drNewRisk, ID_Diagram);
            SetWBS_RiskNewCopiedRisk(targetDataset, drNewRisk, drTargetRisk, DsWBS);
            SetWBS_RISK_DamageNewCopiedRisk(targetDataset, drNewRisk, DsWBS);
            SetRoleRisk(targetDataset, drNewRisk, drTargetRisk);
            if (!(sourceRisk.IsLeaf()))
            {
                foreach (var risk in sourceRisk.Children)
                {
                    if (!risk.IsCM)
                    {
                        CopyRiskWithoutSourceData(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS);
                    }
                    else
                    {
                        SetNewCMValues(risk, targetDataset, drNewRisk, ID_Diagram, DsWBS);
                    }
                }
            }
            return drNewRisk;
        }

        /// <summary>
        /// Create a Risk as child of the drtargetRisk Data, with the sourceRisk WBS and Damage Data 
        /// </summary>
        /// <param name="sourceRisk">The source Line</param>
        /// <param name="targetDataset">Dataset</param>
        /// <param name="drTargetRisk">Target data of the risk</param>
        /// <param name="isMain">A flag to control the position to copy</param>
        /// <param name="ID_Diagram">The current Diagram ID</param>
        /// <param name="DsWBS">The WBS Dataset</param>
        /// <param name="LinesList">The Lines of the Diagram</param>
        /// <returns>Return the data of the line created as copy</returns>
        public static DataRow CopyAllValuesFromLineSource(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drTargetRisk, bool isMain, 
            decimal ID_Diagram, DataSet DsWBS, List<RiskPolyLine> LinesList, bool onlyWBSData)
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
            drNewRisk[DT_Risk.ENABLED] = true;
            drNewRisk[DT_Risk.FROM_TOP] = sourceRisk.FromTop;
            drNewRisk[DT_Risk.ID_DIAGRAM] = ID_Diagram;
            drNewRisk[DT_Risk.ISCOLLAPSED] = false;
            drNewRisk[DT_Risk.IS_ROOT] = false;
            drNewRisk[DT_Risk.NAMESHORT] = sourceRisk.IsRoot ? DiagramName : sourceRisk.ShortName;
            drNewRisk[DT_Risk.POSITION] = isMain ? LinesList.Find(r => r.ID == (decimal)drTargetRisk[DT_Risk.ID]).Children.Count : targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.POSITION];

            drNewRisk[DT_Risk.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
            drNewRisk[DT_Risk.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
            drNewRisk[DT_Risk.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_WBS];
            drNewRisk[DT_Risk.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.WBS_NAME];
            drNewRisk[DT_Risk.USER_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.USER_NAME];
            drNewRisk[DT_Risk.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.PROBABILITY];
            drNewRisk[DT_Risk.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Add(drNewRisk);
            DataRow rowstructure = targetDataset.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drNewRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = drTargetRisk[DT_Risk.ID];
            targetDataset.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
            if (onlyWBSData)
            {
                SetDamageNewCopiedRisk(targetDataset, drNewRisk, ID_Diagram);
            }
            else
            {
                SetOriginalAndNewDamagesCopiedRisk(sourceRisk, targetDataset, drNewRisk, ID_Diagram);
            }
            SetWBS_RiskOriginalAndNewCopiedRisk(sourceRisk, targetDataset, drNewRisk, drTargetRisk, DsWBS);
            SetWBS_RISK_DamageNewCopiedRisk(targetDataset, drNewRisk, DsWBS);
            SetNewAndOriginalRoleRisk(sourceRisk, targetDataset, drNewRisk, drTargetRisk);

            if (!(sourceRisk.IsLeaf()))
            {
                foreach (var risk in sourceRisk.Children)
                {
                    if (!risk.IsCM)
                    {
                        CopyAllValuesFromLineSource(risk, targetDataset, drNewRisk, false, ID_Diagram, DsWBS, LinesList, onlyWBSData);
                    }
                    else
                    {
                        SetValuesOriginalAndNewCopiedCM(risk, targetDataset, drNewRisk, ID_Diagram, DsWBS, onlyWBSData);
                    }
                }
            }
            return drNewRisk;
        }

        /// <summary>
        /// Copy the WBS, Role and WBS_Damage from the Child to the Father and the Ancestors
        /// </summary>
        /// <param name="drRiskCopied">The Risk copied</param>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drTargetRisk">data of the Risk Father</param>
        /// <param name="DsWBS">Dataset with the WBS</param>
        public static void SetValuesFromChildToFather(DataRow drRiskCopied, DataSet targetDataset, DataRow drTargetRisk, DataSet DsWBS)
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
                    newRiskWBS[DT_RISK_WBS.PRIMARY] = "";
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
                        SetValuesFromChildToFather(drTargetRisk, targetDataset, targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(idFather), DsWBS);
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
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.ID])[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCopied[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = drRiskCopied[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCopied[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
                    drRiskDamage[DT_Risk_Damages.ID_WBS] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCopied[DT_Risk.IDRISK_FATHER])[DT_Risk.ID_WBS];
                    drRiskDamage[DT_Risk_Damages.IS_ROOT] = false;
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = 100;
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
                        newRiskWBS[DT_RISK_WBS.PRIMARY] = targetWBS[DT_RISK_WBS.PRIMARY];
                        newRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
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
                    foreach (DataRow wbs in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
                    {
                        if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + wbs[DT_WBS.ID_WBS]).Any()))
                        {
                            if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCopied[DT_Risk.ID], wbs[DT_WBS.ID_WBS] })))
                            {
                                DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCopied[DT_Risk.ID];
                                newRiskWBS[DT_RISK_WBS.ID_WBS] = wbs[DT_WBS.ID_WBS];
                                newRiskWBS[DT_RISK_WBS.WBS] = wbs[DT_WBS.WBS_NAME];
                                newRiskWBS[DT_RISK_WBS.NIVEL] = wbs[DT_WBS.NIVEL];
                                newRiskWBS[DT_RISK_WBS.USERNAME] = wbs[DT_WBS.USERNAME];
                                newRiskWBS[DT_RISK_WBS.WBS_USER] = wbs[DT_WBS.WBS_NAME] + "[" +  wbs[DT_WBS.USERNAME] + "]";
                                newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                                newRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                                primary = false;
                                targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                            }
                        }
                    }
                }
            }
            else
            {
                bool primary = true;
                //foreach (var topWBS in BuscarWBSSinPadre(DsWBS))
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
                        newRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                        newRiskWBS[DT_RISK_WBS.PRIMARY] = primary ? "PRIMARY" : "";
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

        /// <summary>
        /// Add to the CM that belong to the Risk Father Source, the values from the father (Damages, WBS and Role)
        /// </summary>
        /// <param name="sourceCM">The source CM</param>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskFather">Data of the Risk Father</param>
        /// <param name="ID_Diagram">The Current Diagram ID</param>
        /// <param name="DsWBS">The WBS Dataset</param>
        /// <returns></returns>
        public static DataRow SetNewCMValues(RiskPolyLine sourceCM, DataSet targetDataset, DataRow drRiskFather, decimal ID_Diagram, DataSet DsWBS)
        {
            DataRow drNewCM = targetDataset.Tables[DT_CounterM.TABLE_NAME].NewRow();
            drNewCM[DT_CounterM.DETAIL] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.DETAIL];
            drNewCM[DT_CounterM.ENABLED] = true;
            drNewCM[DT_CounterM.GROUPE_NAME] = drRiskFather[DT_Risk.GROUPE_NAME];
            drNewCM[DT_CounterM.ID_GROUPE] = drRiskFather[DT_Risk.ID_GROUPE];
            drNewCM[DT_CounterM.ID_RISK] = drRiskFather[DT_Risk.ID];
            drNewCM[DT_CounterM.ID_DIAGRAM] = ID_Diagram;
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
                    newCMWBS[DT_CM_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" +  targetWBS[DT_RISK_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                }
            }
            if (!targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + drNewCM[DT_CounterM.ID]).Any())
            {
                bool primary = true;
                foreach (DataRow wbs in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
                {
                    if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + wbs[DT_WBS.ID_WBS]).Any()))
                    {
                        if (!(targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], wbs[DT_WBS.ID_WBS] })))
                        {
                            DataRow newCMWBS = targetDataset.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                            newCMWBS[DT_CM_WBS.ID_CM] = drNewCM[DT_CounterM.ID];
                            newCMWBS[DT_CM_WBS.ID_WBS] = wbs[DT_WBS.ID_WBS];
                            newCMWBS[DT_CM_WBS.WBS] = wbs[DT_WBS.WBS_NAME];
                            newCMWBS[DT_CM_WBS.NIVEL] = wbs[DT_WBS.NIVEL];
                            newCMWBS[DT_CM_WBS.USERNAME] = wbs[DT_WBS.USERNAME];
                            newCMWBS[DT_CM_WBS.WBS_USER] = wbs[DT_WBS.WBS_NAME] + "[" + wbs[DT_WBS.USERNAME] + "]";
                            newCMWBS[DT_CM_WBS.IS_PRIMARY] = primary;
                            newCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                            primary = false;
                            targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                        }
                    }
                }
            }
            foreach (DataRow newCMWBS in targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + drNewCM[DT_CounterM.ID]))
            {
                if (WBSOperations.IsCMWBSLow(newCMWBS, DsWBS, targetDataset.Tables[DT_CM_WBS.TABLE_NAME]))
                {
                    AddWBS_CM_Damage(targetDataset, ID_Diagram, drNewCM, newCMWBS);
                }//no hace falta validar que no sea lowlevel, pues estos WBS, son los Main WBS
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
            return drNewCM;
        }

        /// <summary>
        /// If the WBS that belong to that CM, is in the lowest level, Add a damage
        /// </summary>
        private static void AddWBS_CM_Damage(DataSet targetDataset, decimal ID_Diagram, DataRow drNewCM, DataRow newCMWBS)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { newCMWBS[DT_CM_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewCM[DT_CounterM.ID] })))
                {
                    DataRow newCMWBSDamage = targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                    newCMWBSDamage[DT_WBS_CM_Damage.DAMAGE] = diagramDamages[DT_Diagram_Damages.DAMAGE];
                    newCMWBSDamage[DT_WBS_CM_Damage.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    newCMWBSDamage[DT_WBS_CM_Damage.ID_CM] = drNewCM[DT_CounterM.ID];
                    newCMWBSDamage[DT_WBS_CM_Damage.ID_WBS] = newCMWBS[DT_CM_WBS.ID_WBS];
                    newCMWBSDamage[DT_WBS_CM_Damage.VALUE] = 0;
                    newCMWBSDamage[DT_WBS_CM_Damage.WBS] = newCMWBS[DT_CM_WBS.WBS];
                    newCMWBSDamage[DT_WBS_CM_Damage.WBS_USER] = newCMWBS[DT_CM_WBS.WBS_USER];
                    targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newCMWBSDamage);
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
                        drRiskDamage[DT_Risk_Damages.STATUS] = risk[DT_Risk.ENABLED];
                        drRiskDamage[DT_Risk_Damages.TOP_RISK] = drDamage[DT_Diagram_Damages.TOP_RISK];
                        drRiskDamage[DT_Risk_Damages.USERNAME] = risk[DT_Risk.USER_NAME];
                        drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                        drRiskDamage[DT_Risk_Damages.WBS_NAME] = risk[DT_Risk.WBS_NAME];
                        targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                    }
                    foreach (DataRow riskWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + risk[DT_Risk.ID]))
                    {
                        if (WBSOperations.IsRiskWBSLow(riskWBS, DsWBS, targetDataset.Tables[DT_RISK_WBS.TABLE_NAME]))
                        {
                            if (!(targetDataset.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { riskWBS[DT_RISK_WBS.ID_WBS], drDamage[DT_Diagram_Damages.ID_DAMAGE], risk[DT_Risk.ID] })))
                            {
                                AddWBS_Risk_Damage(risk, targetDataset, riskWBS);
                            }
                        }
                        else
                        {
                            DeleteWBS_Risk_Damage(targetDataset, risk, riskWBS);
                        }
                    }
                }
            }
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
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = oldRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCreated[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = ID_Diagram;
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
                    drRiskDamage[DT_Risk_Damages.FATHER] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.NAMESHORT];
                    drRiskDamage[DT_Risk_Damages.GROUPE_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.GROUPE_NAME];
                    drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = diagramDamages[DT_Diagram_Damages.ID_DAMAGE];
                    drRiskDamage[DT_Risk_Damages.ID_FATHER] = drRiskCreated[DT_Risk.IDRISK_FATHER];
                    drRiskDamage[DT_Risk_Damages.ID_GROUPE] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.ID_GROUPE];
                    drRiskDamage[DT_Risk_Damages.ID_RISK] = drRiskCreated[DT_Risk.ID];
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
                    drRiskDamage[DT_Risk_Damages.WBS_NAME] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(drRiskCreated[DT_Risk.IDRISK_FATHER])[DT_Risk.WBS_NAME];
                    targetDataset.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drRiskDamage);
                }
            }
        }

        /// <summary>
        /// Set in the New Risk Copied, the WBS from the Risk Source and The WBS of the Risk Father
        /// </summary>
        /// <param name="sourceRisk">Risk Source</param>
        /// <param name="targetDataset">The Dataset</param>
        /// <param name="drRiskCreated">Data of the Risk Created</param>
        /// <param name="drTargetRisk">Data of the Risk Father</param>
        /// <param name="DsWBS">WBS Dataset</param>
        public static void SetWBS_RiskOriginalAndNewCopiedRisk(RiskPolyLine sourceRisk, DataSet targetDataset, DataRow drRiskCreated, DataRow drTargetRisk, DataSet DsWBS)
        {
            //Estos son los WBS que tenia originalmente
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + sourceRisk.ID))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = targetWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PRIMARY] = targetWBS[DT_RISK_WBS.PRIMARY];
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = targetWBS[DT_RISK_WBS.PROBABILITY];
                    newRiskWBS[DT_RISK_WBS.RISK] = drRiskCreated[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    newRiskWBS[DT_RISK_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" + targetWBS[DT_RISK_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
            //Estos son los WBS que heredo del padre
            foreach (DataRow targetWBS in targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drTargetRisk[DT_Risk.ID]))
            {
                if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], targetWBS[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCreated[DT_Risk.ID];
                    newRiskWBS[DT_RISK_WBS.ID_WBS] = targetWBS[DT_RISK_WBS.ID_WBS];
                    newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                    newRiskWBS[DT_RISK_WBS.NIVEL] = targetWBS[DT_RISK_WBS.NIVEL];
                    newRiskWBS[DT_RISK_WBS.PRIMARY] = "";
                    //Pongo la probabilidad original para que no me afecte el promedio de la probabilidad en la tabla del damage
                    newRiskWBS[DT_RISK_WBS.PROBABILITY] = targetDataset.Tables[DT_Risk.TABLE_NAME].Rows.Find(sourceRisk.ID)[DT_Risk.PROBABILITY];
                    newRiskWBS[DT_RISK_WBS.RISK] = drRiskCreated[DT_Risk.NAMESHORT];
                    newRiskWBS[DT_RISK_WBS.USERNAME] = targetWBS[DT_RISK_WBS.USERNAME];
                    newRiskWBS[DT_RISK_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    newRiskWBS[DT_RISK_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" + targetWBS[DT_RISK_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                }
            }
            if (!targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRiskCreated[DT_Risk.ID]).Any())
            {
                bool primary = true;
                foreach (DataRow wbs in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
                {
                    if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + wbs[DT_WBS.ID_WBS]).Any()))
                    {
                        if (!(targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskCreated[DT_Risk.ID], wbs[DT_WBS.ID_WBS] })))
                        {
                            DataRow newRiskWBS = targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                            newRiskWBS[DT_RISK_WBS.ID_RISK] = drRiskCreated[DT_Risk.ID];
                            newRiskWBS[DT_RISK_WBS.ID_WBS] = wbs[DT_WBS.ID_WBS];
                            newRiskWBS[DT_RISK_WBS.WBS] = wbs[DT_WBS.WBS_NAME];
                            newRiskWBS[DT_RISK_WBS.NIVEL] = wbs[DT_WBS.NIVEL];
                            newRiskWBS[DT_RISK_WBS.USERNAME] = wbs[DT_WBS.USERNAME];
                            newRiskWBS[DT_RISK_WBS.WBS_USER] = wbs[DT_WBS.WBS_NAME] + "[" +  wbs[DT_WBS.USERNAME] + "]";
                            newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                            newRiskWBS[DT_RISK_WBS.PROBABILITY] = drRiskCreated[DT_Risk.PROBABILITY];
                            primary = false;
                            targetDataset.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                        }
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

        /// <summary>
        /// Create a Copy of the CM child of The Risk Copied, The Damage, WBS, Role and the Values
        /// </summary>
        /// <param name="sourceCM">Source CM Child</param>
        /// <param name="targetDataset">The DataSet</param>
        /// <param name="drRiskFather">The Data of the Risk Copied</param>
        /// <param name="ID_Diagram">The Current Diagram ID</param>
        /// <param name="DsWBS">WBS Dataset</param>
        /// <returns></returns>
        public static DataRow SetValuesOriginalAndNewCopiedCM(RiskPolyLine sourceCM, DataSet targetDataset,
            DataRow drRiskFather, decimal ID_Diagram, DataSet DsWBS, bool onlyWBSData)
        {
            DataRow drNewCM = targetDataset.Tables[DT_CounterM.TABLE_NAME].NewRow();
            drNewCM[DT_CounterM.DETAIL] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.DETAIL];
            drNewCM[DT_CounterM.ENABLED] = true;
            drNewCM[DT_CounterM.GROUPE_NAME] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.GROUPE_NAME];
            drNewCM[DT_CounterM.ID_GROUPE] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.ID_GROUPE];
            drNewCM[DT_CounterM.ID_RISK] = drRiskFather[DT_Risk.ID];
            drNewCM[DT_CounterM.ID_DIAGRAM] = ID_Diagram;
            drNewCM[DT_CounterM.ID_WBS] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.ID_WBS];
            drNewCM[DT_CounterM.NAMESHORT] = sourceCM.ShortName;
            drNewCM[DT_CounterM.POSITION] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.POSITION];
            drNewCM[DT_CounterM.PROBABILITY] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.PROBABILITY];
            drNewCM[DT_CounterM.RISK_NAMESHORT] = drRiskFather[DT_Risk.NAMESHORT];
            drNewCM[DT_CounterM.USER_NAME] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.PROBABILITY];
            drNewCM[DT_CounterM.WBS_NAME] = targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Find(sourceCM.ID)[DT_CounterM.WBS_NAME];
            targetDataset.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drNewCM);
            if (onlyWBSData)
            {
                ImportDamagesFromTargetDiagram(targetDataset, drRiskFather, ID_Diagram, drNewCM);
            }
            else
            {
                ImportDamageFromCMSource(sourceCM, targetDataset, drRiskFather, ID_Diagram, drNewCM);
            }
            //Los DAMAGES del TARGET DIAGRAM
            ImportWBSFromSourceCM(sourceCM, targetDataset, drNewCM);
            ImportWBSFromRiskNewFather(targetDataset, drRiskFather, drNewCM);
            foreach (DataRow myWBS in targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + drNewCM[DT_CounterM.ID]))
            {
                if (WBSOperations.IsCMWBSLow(myWBS, DsWBS, targetDataset.Tables[DT_CM_WBS.TABLE_NAME]))
                {
                    AddWBS_CM_Damage(targetDataset, ID_Diagram, drNewCM, myWBS);
                }
                else
                {
                    foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
                    {
                        if (targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { myWBS[DT_CM_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewCM[DT_CounterM.ID] }))
                        {
                            targetDataset.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Find(new object[] { myWBS[DT_CM_WBS.ID_WBS], diagramDamages[DT_Diagram_Damages.ID_DAMAGE], drNewCM[DT_CounterM.ID] }).Delete();
                        }
                    }
                }
            }
            ImportRoleFromNewRiskFather(targetDataset, drRiskFather, drNewCM);
            return drNewCM;
        }

        private static void ImportRoleFromNewRiskFather(DataSet targetDataset, DataRow drRiskFather, DataRow drNewCM)
        {
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

        private static void ImportWBSFromRiskNewFather(DataSet targetDataset, DataRow drRiskFather, DataRow drNewCM)
        {
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
                    newCMWBS[DT_CM_WBS.WBS_USER] = targetWBS[DT_RISK_WBS.WBS] + "[" + targetWBS[DT_RISK_WBS.USERNAME] + "]";
                    newCMWBS[DT_CM_WBS.WBS] = targetWBS[DT_RISK_WBS.WBS];
                    targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                }
            }
        }

        private static void ImportWBSFromSourceCM(RiskPolyLine sourceCM, DataSet targetDataset, DataRow drNewCM)
        {
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
                    newCMWBS[DT_CM_WBS.WBS_USER] = sourceWBS[DT_CM_WBS.WBS] + "[" + sourceWBS[DT_CM_WBS.USERNAME] + "]";
                    targetDataset.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                }
            }
        }

        private static void ImportDamagesFromTargetDiagram(DataSet targetDataset, DataRow drRiskFather, decimal ID_Diagram, DataRow drNewCM)
        {
            foreach (DataRow diagramDamages in targetDataset.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram))
            {
                if (!(targetDataset.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { drNewCM[DT_CounterM.ID], diagramDamages[DT_Diagram_Damages.ID_DAMAGE] })))
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
        }

        private static void ImportDamageFromCMSource(RiskPolyLine sourceCM, DataSet targetDataset, DataRow drRiskFather, decimal ID_Diagram, DataRow drNewCM)
        {
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
                top[DT_Risk_Damages.STATUS] = drRisk[DT_Risk.ENABLED];
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
                rwbs[DT_RISK_WBS.PRIMARY] = itemi[DT_RISK_WBS.PRIMARY];
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
                    rwbs[DT_RISK_WBS.PRIMARY] = "";
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
            foreach (DataRow itemi in dsSource.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmLine.ID))
            {
                DataRow top = dsTarget.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                top[DT_CounterM_Damage.COLOR] = itemi[DT_CounterM_Damage.COLOR];
                top[DT_CounterM_Damage.ID_DAMAGE] = itemi[DT_CounterM_Damage.ID_DAMAGE];
                top[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
                top[DT_CounterM_Damage.VALUE] = itemi[DT_CounterM_Damage.VALUE];
                top[DT_CounterM_Damage.TOP_RISK] = itemi[DT_CounterM_Damage.TOP_RISK];
                top[DT_CounterM_Damage.COUNTERM_NAMESHORT] = drCM[DT_CounterM.NAMESHORT];
                top[DT_CounterM_Damage.DAMAGE] = itemi[DT_CounterM_Damage.DAMAGE];
                top[DT_CounterM_Damage.GROUPENAME] = itemi[DT_CounterM_Damage.GROUPENAME];
                top[DT_CounterM_Damage.IDRISK] = drCM[DT_CounterM.ID_RISK];
                top[DT_CounterM_Damage.ID_GROUPE] = itemi[DT_CounterM_Damage.ID_GROUPE];
                top[DT_CounterM_Damage.ID_RISK_TREE] = idDiagram;
                top[DT_CounterM_Damage.RISK] = riskFather.ShortName;
                top[DT_CounterM_Damage.RISK_REDUCTION] = drCM[DT_CounterM.PROBABILITY];
                top[DT_CounterM_Damage.STATUS] = itemi[DT_CounterM_Damage.STATUS];
                top[DT_CounterM_Damage.ID_WBS] = itemi[DT_CounterM_Damage.ID_WBS];
                top[DT_CounterM_Damage.WBS_NAME] = itemi[DT_CounterM_Damage.WBS_NAME];
                top[DT_CounterM_Damage.USERNAME] = itemi[DT_CounterM_Damage.USERNAME];
                dsTarget.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(top);
            }
        }

        private static void FillCMWBS(DataSet dsSource, DataSet dsResult, RiskPolyLine item, DataRow drCM, RiskPolyLine riskfather)
        {
            //Los valores de WBS que tenia originalmente
            foreach (DataRow itemi in dsSource.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                rwbs[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                rwbs[DT_CM_WBS.ID_WBS] = itemi[DT_CM_WBS.ID_WBS];
                rwbs[DT_CM_WBS.NIVEL] = itemi[DT_CM_WBS.NIVEL];
                rwbs[DT_CM_WBS.CM] = drCM[DT_CounterM.NAMESHORT];
                rwbs[DT_CM_WBS.WBS] = itemi[DT_CM_WBS.WBS];
                rwbs[DT_CM_WBS.IS_PRIMARY] = itemi[DT_CM_WBS.IS_PRIMARY];
                rwbs[DT_CM_WBS.PRIMARY] = itemi[DT_CM_WBS.PRIMARY];
                rwbs[DT_CM_WBS.USERNAME] = itemi[DT_CM_WBS.USERNAME];
                rwbs[DT_CM_WBS.WBS_USER] = itemi[DT_CM_WBS.WBS] + "[" + itemi[DT_CM_WBS.USERNAME] +"]";
                rwbs[DT_CM_WBS.CM] = itemi[DT_CM_WBS.CM];
                rwbs[DT_CM_WBS.PROBABILITY] = itemi[DT_CM_WBS.PROBABILITY];
                dsResult.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(rwbs);
            }
            //Los valores de WBS que tenia el padre
            //foreach (DataRow itemi in dsSource.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + item.Father.ID))
            foreach (DataRow wbsFather in dsResult.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + riskfather.ID))
            {
                if (!(dsResult.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], wbsFather[DT_CM_WBS.ID_WBS] })))
                {
                    DataRow rwbs = dsResult.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                    rwbs[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                    rwbs[DT_CM_WBS.ID_WBS] = wbsFather[DT_RISK_WBS.ID_WBS];
                    rwbs[DT_CM_WBS.NIVEL] = wbsFather[DT_RISK_WBS.NIVEL];
                    rwbs[DT_CM_WBS.CM] = drCM[DT_CounterM.NAMESHORT];
                    rwbs[DT_CM_WBS.WBS] = wbsFather[DT_RISK_WBS.WBS];
                    rwbs[DT_CM_WBS.IS_PRIMARY] = false;
                    rwbs[DT_CM_WBS.PRIMARY] = "";
                    rwbs[DT_CM_WBS.USERNAME] = wbsFather[DT_RISK_WBS.USERNAME];
                    rwbs[DT_CM_WBS.WBS_USER] = wbsFather[DT_RISK_WBS.WBS] + "[" + wbsFather[DT_RISK_WBS.USERNAME] + "]";
                    rwbs[DT_CM_WBS.CM] = drCM[DT_CounterM.NAMESHORT];
                    rwbs[DT_CM_WBS.PROBABILITY] = 0;
                    dsResult.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(rwbs);
                }
            }
            //Los valores WBSDamage que tenia originalmente
            foreach (DataRow itemi in dsSource.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                rwbs[DT_WBS_CM_Damage.ID_CM] = drCM[DT_CounterM.ID];
                rwbs[DT_WBS_CM_Damage.ID_WBS] = itemi[DT_WBS_CM_Damage.ID_WBS];
                rwbs[DT_WBS_CM_Damage.ID_DAMAGE] = itemi[DT_WBS_CM_Damage.ID_DAMAGE];
                rwbs[DT_WBS_CM_Damage.DAMAGE] = itemi[DT_WBS_CM_Damage.DAMAGE];
                rwbs[DT_WBS_CM_Damage.VALUE] = itemi[DT_WBS_CM_Damage.VALUE];
                rwbs[DT_WBS_CM_Damage.WBS] = itemi[DT_WBS_CM_Damage.WBS];
                rwbs[DT_WBS_CM_Damage.WBS_USER] = itemi[DT_WBS_CM_Damage.WBS_USER];
                dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(rwbs);
            }
            //Los valores WBSDamage que tiene el padre originalmente
            foreach (DataRow fatherWBSValues in dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + riskfather.ID))
            {
                if (!(dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] {fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_WBS],
                                                        fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drCM[DT_CounterM.ID]})))
                {
                    DataRow rwbs = dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                    rwbs[DT_WBS_CM_Damage.ID_CM] = drCM[DT_CounterM.ID];
                    rwbs[DT_WBS_CM_Damage.ID_WBS] = fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_WBS];
                    rwbs[DT_WBS_CM_Damage.ID_DAMAGE] = fatherWBSValues[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                    rwbs[DT_WBS_CM_Damage.DAMAGE] = fatherWBSValues[DT_WBS_RISK_DAMAGE.DAMAGE];
                    rwbs[DT_WBS_CM_Damage.VALUE] = 0;
                    rwbs[DT_WBS_CM_Damage.WBS] = fatherWBSValues[DT_WBS_RISK_DAMAGE.WBS];
                    rwbs[DT_WBS_CM_Damage.WBS_USER] = fatherWBSValues[DT_WBS_RISK_DAMAGE.WBS_USER];
                    dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(rwbs);
                }
            }
        }

        private static void FillCMRoles(DataSet dsSource, DataSet dsResult, RiskPolyLine risk, DataRow drCM, RiskPolyLine item)
        {
            //foreach role that its father have
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + risk.ID))
            {
                if (!(dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_CM.TABLENAME].NewRow();
                    newRow[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                    newRow[DT_Role_CM.Role] = itemo[DT_Role_CM.Role];
                    newRow[DT_Role_CM.IDROL_COLUMN] = itemo[DT_Role_CM.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_CM.TABLENAME].Rows.Add(newRow);
                }
            }
            //for each role that original have 
            foreach (DataRow itemo in dsSource.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + item.ID))
            {
                if (!(dsResult.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], itemo[DT_Role_CM.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_CM.TABLENAME].NewRow();
                    newRow[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                    newRow[DT_Role_CM.Role] = itemo[DT_Role_CM.Role];
                    newRow[DT_Role_CM.IDROL_COLUMN] = itemo[DT_Role_CM.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_CM.TABLENAME].Rows.Add(newRow);
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
                        drRisk[DT_Risk.ENABLED] = true;
                        drRisk[DT_Risk.FROM_TOP] = childLine.FromTop;
                        drRisk[DT_Risk.ID_DIAGRAM] = idDiagram;
                        drRisk[DT_Risk.ISCOLLAPSED] = childLine.Collapsed;
                        drRisk[DT_Risk.IS_ROOT] = false;
                        drRisk[DT_Risk.NAMESHORT] = childLine.ShortName;
                        drRisk[DT_Risk.POSITION] = childLine.Position;
                        drRisk[DT_Risk.PROBABILITY] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.PROBABILITY];
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
                        DataRow drCM = dsResult.Tables[DT_CounterM.TABLE_NAME].NewRow();
                        drCM[DT_CounterM.DETAIL] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(childLine.ID)[DT_CounterM.DETAIL];
                        drCM[DT_CounterM.ENABLED] = true;
                        drCM[DT_CounterM.FROM_TOP] = childLine.FromTop;
                        drCM[DT_CounterM.ID_DIAGRAM] = idDiagram;
                        drCM[DT_CounterM.ID_RISK] = riskFather.ID;
                        drCM[DT_CounterM.NAMESHORT] = childLine.ShortName;
                        drCM[DT_CounterM.POSITION] = childLine.Position;
                        drCM[DT_CounterM.PROBABILITY] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(childLine.ID)[DT_Risk.PROBABILITY];
                        drCM[DT_CounterM.RISK_NAMESHORT] = childLine.ShortName;
                        drCM[DT_CounterM.ID_WBS] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(childLine.ID)[DT_CounterM.ID_WBS];
                        drCM[DT_CounterM.WBS_NAME] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(childLine.ID)[DT_CounterM.WBS_NAME];
                        drCM[DT_CounterM.USER_NAME] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(childLine.ID)[DT_CounterM.USER_NAME];
                        dsResult.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                        FillCMDamages(dsSource, dsResult, riskFather, drCM, childLine, idDiagram);
                        FillCMWBS(dsSource, dsResult, childLine, drCM, riskFather);
                        FillCMRoles(dsSource, dsResult, riskFather, drCM, childLine);
                        childLine.ID = (decimal)drCM[DT_CounterM.ID];
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
