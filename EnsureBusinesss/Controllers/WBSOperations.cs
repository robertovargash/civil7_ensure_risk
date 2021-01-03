using DataMapping.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnsureBusinesss
{
    public class WBSOperations
    {
        /// <summary>
        /// Return the WBS children of the WBSFatherRow
        /// </summary>
        /// <param name="WBSFatherRow">Data of the WBS</param>
        /// <param name="dtWBS">Table of WBS</param>
        /// <returns>  Return the WBS children of the WBSFatherRow</returns>
        public static List<DataRow> MyWBSChildren(DataRow WBSFatherRow, DataTable dtWBS)
        {
            List<DataRow> returnList = new List<DataRow>();

            foreach (DataRow item in dtWBS.Select(DT_WBS.ID_FATHER + " = " + WBSFatherRow[DT_WBS.ID_WBS].ToString()))
            {
                returnList.Add(item);
            }
            return returnList;
        }

        public static List<DataRow> MyWBSDescendants(DataRow WBSFatherRow, DataTable dtWBS)
        {
            List<DataRow> returnList = new List<DataRow>();

            foreach (DataRow item in dtWBS.Select(DT_WBS.ID_FATHER + " = " + WBSFatherRow[DT_WBS.ID_WBS].ToString()))
            {
                returnList.Add(item);
                returnList.AddRange(MyWBSDescendants(item, dtWBS));
            }
            return returnList;
        }

        /// <summary>
        /// Delete from entire List of WBS, all its children
        /// </summary>
        /// <param name="wbsList">List to delete children</param>
        /// <param name="dtWBS">Table of WBS</param>
        /// <param name="dtStrucutre">Table of the hierarquical structure of the WBS</param>
        public static void DeleteMyWBSChildren(List<DataRow> wbsList, DataTable dtWBS)
        {
            foreach (DataRow item in wbsList)
            {
                if (dtWBS.Select(DT_WBS.ID_FATHER + " = " + item[DT_WBS.ID_WBS]).Any())
                {
                    DeleteMyWBSChildren(MyWBSChildren(item, dtWBS), dtWBS);
                }
                dtWBS.Rows.Find(item[DT_WBS.ID_WBS]).Delete();
            }
        }

        /// <summary>
        /// Return if the current WBS is the lowest in the Risk Selected
        /// </summary>
        /// <param name="rowRiskWBS">WBS of the Risk row</param>
        /// <param name="dsWBS">DataSet</param>
        /// <param name="dtRisk_WBS">Table of the WBS Risk row</param>
        /// <returns>Return if the current WBS is the lowest in the Risk Selected</returns>
        public static bool IsRiskWBSLow(DataRow rowRiskWBS, DataSet dsWBS, DataTable dtRisk_WBS)
        {
            foreach (DataRow riskWBS in dtRisk_WBS.Select(DT_RISK_WBS.ID_RISK + " = " + rowRiskWBS[DT_RISK_WBS.ID_RISK]))
            {
                if (rowRiskWBS[DT_RISK_WBS.ID_WBS] != riskWBS[DT_RISK_WBS.ID_WBS])
                {
                    if (dsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_FATHER + " = " + rowRiskWBS[DT_RISK_WBS.ID_WBS] + " and " + DT_WBS.ID_WBS + " = " + riskWBS[DT_RISK_WBS.ID_WBS]).Any())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Return IF the WBS is in the lowest Level of thw WBS Tree
        /// </summary>
        /// <param name="ID_WBS"></param>
        /// <param name="dsWBS"></param>
        /// <returns> Return IF the WBS is in the lowest Level of thw WBS Tree</returns>
        public static bool WBS_isSheet(decimal ID_WBS, DataSet dsWBS)
        {
            if (dsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_FATHER + " = " + ID_WBS).Any())
            {
                return false;
            }
            else
            { return true; }
        }

        /// <summary>
        /// Return the ancestors the current WBS
        /// </summary>
        /// <param name="idWBS">Selected WBS</param>
        /// <param name="dtWBSAncestors">DT of the Ancestors(editable)</param>
        /// <param name="DsWBS">Dataset</param>
        /// <returns>Return the ancestors the current WBS</returns>
        public static DataTable GetAncestors(decimal idWBS, DataTable dtWBSAncestors, DataSet DsWBS)
        {
            if (HasParent(idWBS, DsWBS))
            {
                dtWBSAncestors.ImportRow(GetParent(idWBS, DsWBS));
                return GetAncestors((decimal)GetParent(idWBS, DsWBS)[DT_WBS.ID_WBS], dtWBSAncestors, DsWBS);
            }
            else
            {
                return dtWBSAncestors;
            }
        }

        /// <summary>
        /// Return If the Selected WBS has a Parent
        /// </summary>
        /// <param name="idWBS">Id of the selected WBS</param>
        /// <param name="DsWBS">Dataset</param>
        /// <returns>Return If the Selected WBS has a Parent</returns>
        public static bool HasParent(decimal idWBS, DataSet DsWBS)
        {
            if (DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_WBS + " = " + idWBS).First()[DT_WBS.ID_FATHER] != DBNull.Value)
            {
                if ((decimal)DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_WBS + " = " + idWBS).First()[DT_WBS.ID_FATHER] != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return the Parent of the Selected WBS
        /// </summary>
        /// <param name="idWBS">ID of the Selected WBS</param>
        /// <param name="DsWBS">Dataset</param>
        /// <returns>Return the Parent of the Selected WBS</returns>
        private static DataRow GetParent(decimal idWBS, DataSet DsWBS)
        {
            decimal idPadre = (decimal)DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_WBS + " = " + idWBS).First()[DT_WBS.ID_FATHER];
            return DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(idPadre);
        }

        /// <summary>
        /// Add to the Table DT_WBS_RISK_DAMAGE or WBS_CM_DAMAGE in case of Risk or CM, a new row of this table
        /// </summary>
        /// <param name="itemWBS">WBS data for insert</param>
        /// <param name="idLine">ID of Risk or CM for Insert</param>
        /// <param name="isCM">If the line is Risk or CM</param>
        /// <param name="Ds">Dataset</param>
        public static void TabAddWBS_LINE_Damage(DataRow itemWBS, decimal idLine, bool isCM, DataSet Ds)
        {
            foreach (DataRow itemDamage in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + idLine))
            {
                if (!(Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], idLine })))
                {
                    DataRow drWBS_RISK_Damage = Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_RISK] = idLine;
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS] = itemWBS[DT_RISK_WBS.WBS];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS_USER] = itemWBS[DT_RISK_WBS.WBS] + "[" + itemWBS[DT_RISK_WBS.USERNAME] + "]";
                    Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBS_RISK_Damage);
                }
            }
        }

        /// <summary>
        /// Delete from the Table DT_WBS_RISK_DAMAGE or WBS_CM_DAMAGE in case of Risk or CM, an existing row in this table
        /// </summary>
        /// <param name="itemWBS">WBS data for delete</param>
        /// <param name="idLine">ID of Risk or CM for Delete</param>
        /// <param name="isCM">If the line is Risk or CM</param>
        /// <param name="Ds">Dataset</param>
        public static void TabDeleteWBS_LINE_Damage(DataRow itemWBS, decimal idLine, bool isCM, DataSet Ds)
        {
            foreach (DataRow itemDamage in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + idLine))
            {
                if (Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], idLine }))
                {
                    Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], idLine }).Delete();
                }
            }
        }

        public static void SetAsPrimaryWhoDelete(DataRow rowWBStoDelete, DataSet Ds, string logguedUser, decimal iDriskSelected, DataSet DsWBS)
        {
            bool isPrimary = false;
            if ((bool)rowWBStoDelete[DT_RISK_WBS.IS_PRIMARY])
            {
                isPrimary = true;
            }
            else
            {
                foreach (DataRow descendant in MyWBSDescendants(DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(rowWBStoDelete[DT_RISK_WBS.ID_WBS]), DsWBS.Tables[DT_WBS.TABLE_NAME]))
                {
                    if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { iDriskSelected, descendant[DT_WBS.ID_WBS] }))
                    {
                        if ((bool)Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { iDriskSelected, descendant[DT_WBS.ID_WBS] })[DT_RISK_WBS.IS_PRIMARY])
                        {
                            isPrimary = true; break;
                        }
                    }
                }
            }

            if (isPrimary && Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + iDriskSelected).Any())
            {
                foreach (DataRow ancestor in GetAncestors((decimal)rowWBStoDelete[DT_RISK_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), DsWBS).Rows)
                {
                    if (ancestor[DT_WBS.USERNAME].ToString() == logguedUser)
                    {
                        if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { iDriskSelected, ancestor[DT_WBS.ID_WBS] }))
                        {
                            DataRow RiskRow = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(iDriskSelected);
                            Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { iDriskSelected, ancestor[DT_WBS.ID_WBS] })[DT_RISK_WBS.IS_PRIMARY] = true;
                            RiskRow[DT_Risk.ID_WBS] = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { iDriskSelected, ancestor[DT_WBS.ID_WBS] })[DT_RISK_WBS.ID_WBS];
                            RiskRow[DT_Risk.USER_NAME] = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { iDriskSelected, ancestor[DT_WBS.ID_WBS] })[DT_RISK_WBS.USERNAME];
                            RiskRow[DT_Risk.WBS_NAME] = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { iDriskSelected, ancestor[DT_WBS.ID_WBS] })[DT_RISK_WBS.WBS];
                            foreach (var riskrow in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                            {
                                riskrow[DT_Risk_Damages.ID_WBS] = RiskRow[DT_Risk.ID_WBS];
                                riskrow[DT_Risk_Damages.WBS_NAME] = RiskRow[DT_Risk.WBS_NAME];
                                riskrow[DT_Risk_Damages.USERNAME] = RiskRow[DT_Risk.USER_NAME];
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set as primary first WBS that found
        /// </summary>
        /// <param name="cmWBSrow">CM_WBS datarow</param>
        /// <param name="isCM">if the selected line is Risk or CM</param>
        /// <param name="Ds">Dataset</param>
        /// <param name="IDSelected">ID of the selected line (Risk or CM)</param>
        public static void SetDefaultWBSPrimary(DataRow cmWBSrow, DataSet Ds, decimal IDSelected)
        {
            if ((bool)cmWBSrow[DT_RISK_WBS.IS_PRIMARY] && Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + IDSelected).Any())
            {
                DataRow RiskRow = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(IDSelected);
                Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]).First()[DT_RISK_WBS.IS_PRIMARY] = true;
                RiskRow[DT_Risk.ID_WBS] = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]).First()[DT_RISK_WBS.ID_WBS];
                RiskRow[DT_Risk.USER_NAME] = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]).First()[DT_RISK_WBS.USERNAME];
                RiskRow[DT_Risk.WBS_NAME] = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]).First()[DT_RISK_WBS.WBS];
                foreach (var riskrow in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    riskrow[DT_Risk_Damages.ID_WBS] = RiskRow[DT_Risk.ID_WBS];
                    riskrow[DT_Risk_Damages.WBS_NAME] = RiskRow[DT_Risk.WBS_NAME];
                    riskrow[DT_Risk_Damages.USERNAME] = RiskRow[DT_Risk.USER_NAME];
                }
            }
        }

        /// <summary>
        /// Search all the TOP WBS
        /// </summary>
        /// <param name="ds">The Current WBS Dataset</param>
        /// <returns></returns>
        public static List<DataRow> GetTopWBS(DataSet ds)
        {
            List<DataRow> ListaRow = new List<DataRow>();
            foreach (DataRow item in ds.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (item[DT_WBS.ID_FATHER] == DBNull.Value || (decimal)item[DT_WBS.ID_FATHER] == 0)
                {
                    ListaRow.Add(item);
                }
                //if (ds.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_WBS + " = 0").Any())
                //{

                //}
            }
            return ListaRow;
        }

        /// <summary>
        /// Add to each Risk in the current Diagram the Top WBS in the WBS Structure.
        /// </summary>
        /// <param name="ds">Expect the dataset of the Current Diagram</param>
        public static void AddWBSTopToDiagram(DataSet ds, decimal Id_Diagram, DataSet DsWBS)
        {
            bool primaryy = true;
            foreach (var rowWBS in GetTopWBS(DsWBS))
            {
                foreach (DataRow riskRow in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + Id_Diagram))
                {
                    if (!(ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { riskRow[DT_Risk.ID], rowWBS[DT_WBS.ID_WBS] })))
                    {
                        DataRow newRiskWBS = ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        newRiskWBS[DT_RISK_WBS.ID_RISK] = riskRow[DT_Risk.ID];
                        newRiskWBS[DT_RISK_WBS.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                        newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primaryy;
                        newRiskWBS[DT_RISK_WBS.NIVEL] = rowWBS[DT_WBS.NIVEL];
                        newRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                        newRiskWBS[DT_RISK_WBS.RISK] = riskRow[DT_Risk.NAMESHORT];
                        newRiskWBS[DT_RISK_WBS.USERNAME] = rowWBS[DT_WBS.USERNAME];
                        newRiskWBS[DT_RISK_WBS.WBS] = rowWBS[DT_WBS.WBS_NAME];
                        newRiskWBS[DT_RISK_WBS.WBS_USER] = rowWBS[DT_WBS.WBS_NAME] + "[" + rowWBS[DT_WBS.USERNAME] + "]";
                        ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                        foreach (DataRow rowRiskDamage in ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                        {
                            rowRiskDamage[DT_Risk_Damages.WBS_NAME] = rowWBS[DT_WBS.NIVEL].ToString() + " " + rowWBS[DT_WBS.WBS_NAME].ToString();
                            rowRiskDamage[DT_Risk_Damages.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                            rowRiskDamage[DT_Risk_Damages.USERNAME] = rowWBS[DT_WBS.USERNAME];
                        }
                    }
                    riskRow[DT_Risk.WBS_NAME] = rowWBS[DT_WBS.NIVEL].ToString() + " " + rowWBS[DT_WBS.WBS_NAME].ToString();
                    riskRow[DT_Risk.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                    riskRow[DT_Risk.USER_NAME] = rowWBS[DT_WBS.USERNAME];
                }
            }
        }

        public static DataTable GetMyDescendantsAndDeleteExistents(DataTable dtWBS_Codif, DataTable dtRiskWBS, DataTable dtWBSExistents, string LoginUser)
        {
            DataTable dtReturn = dtWBS_Codif.Clone();
            if (dtRiskWBS.Select(DT_RISK_WBS.USERNAME + " = '" + LoginUser + "'").Any())
            {
                decimal IDWBS = (decimal)dtRiskWBS.Select(DT_RISK_WBS.USERNAME + " = '" + LoginUser + "'").First()[DT_RISK_WBS.ID_WBS];
                foreach (DataRow descendant in WBSOperations.MyWBSDescendants(dtWBS_Codif.Rows.Find(IDWBS), dtWBS_Codif))
                {
                    if (dtWBSExistents.Select(DT_WBS.ID_WBS + " = " + descendant[DT_WBS.ID_WBS]).Any())
                    {
                        dtReturn.ImportRow(descendant);
                    }
                }
            }
            return dtReturn;
        }

        public static decimal RiskWBSValidations(DataRow RiskRow, DataSet Ds, string LoginUser, DataSet DsWBS, bool hasAccess, bool isCM)
        {
            decimal probability = 0;
            List<decimal> Probabilities = new List<decimal>();
            foreach (DataRow rowRiskWbs in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
            {
                rowRiskWbs[DT_RISK_WBS.CanDelete] = false;
                rowRiskWbs[DT_RISK_WBS.CanEditPrimary] = false;
                rowRiskWbs[DT_RISK_WBS.IsProbabReadOnly] = false;
                if (IsRiskWBSLow(rowRiskWbs, DsWBS, Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                {
                    if (rowRiskWbs[DT_RISK_WBS.PROBABILITY] == DBNull.Value)
                    {
                        if (isCM)
                        {
                            Probabilities.Add(0);
                        }
                        else
                        {
                            Probabilities.Add(100);
                        }
                    }
                    else
                    {
                        Probabilities.Add((decimal)rowRiskWbs[DT_RISK_WBS.PROBABILITY]);
                    }
                }
                else
                {
                    rowRiskWbs[DT_RISK_WBS.PROBABILITY] = 0;
                }
            }
            if (Probabilities.Count > 0)
            {
                probability = Probabilities.Sum() / Probabilities.Count;
            }
            else
            {
                probability = isCM ? 100 : 0;
            }

            foreach (DataRow riskWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " and "
                + DT_RISK_WBS.USERNAME + " = '" + LoginUser + "'"))
            {
                riskWBS[DT_RISK_WBS.CanEditPrimary] = hasAccess;
                if (IsRiskWBSLow(riskWBS, DsWBS, Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                {
                    riskWBS[DT_RISK_WBS.IsProbabReadOnly] = true;
                }
                foreach (DataRow descendant in MyWBSDescendants(riskWBS, DsWBS.Tables[DT_WBS.TABLE_NAME]))
                {
                    if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], descendant[DT_WBS.ID_WBS] }))
                    {
                        DataRow drRiskWBStoSet = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { RiskRow[DT_Risk.ID], descendant[DT_WBS.ID_WBS] });

                        if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], descendant[DT_WBS.ID_WBS] }))
                        {
                            drRiskWBStoSet[DT_RISK_WBS.CanDelete] = true;
                            drRiskWBStoSet[DT_RISK_WBS.CanEditPrimary] = hasAccess;
                        }
                    }
                }
            }
            return probability;
        }

    }
}