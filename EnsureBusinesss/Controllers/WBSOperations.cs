using DataMapping.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureBusinesss
{
    public class WBSOperations
    {
        /// <summary>
        /// Return the WBS children of the WBSFatherRow
        /// </summary>
        /// <param name="WBSFatherRow">Data of the WBS</param>
        /// <param name="dtWBS">Table of WBS</param>
        /// <param name="dtStrucutre">Table of the hierarquical structure of the WBS</param>
        /// <returns>  Return the WBS children of the WBSFatherRow</returns>
        public static List<DataRow> MyWBSChildren(DataRow WBSFatherRow, DataTable dtWBS, DataTable dtStructure)
        {
            List<DataRow> returnList = new List<DataRow>();

            foreach (DataRow item in dtStructure.Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + WBSFatherRow[DT_WBS.ID_WBS].ToString()))
            {
                returnList.Add(dtWBS.Rows.Find(item[DT_WBS_STRUCTURE.ID_CHILD]));
            }
            return returnList;
        }

        /// <summary>
        /// Delete from entire List of WBS, all its children
        /// </summary>
        /// <param name="wbsList">List to delete children</param>
        /// <param name="dtWBS">Table of WBS</param>
        /// <param name="dtStrucutre">Table of the hierarquical structure of the WBS</param>
        public static void DeleteMyWBSChildren(List<DataRow> wbsList, DataTable dtWBS, DataTable dtStrucutre)
        {
            foreach (DataRow item in wbsList)
            {
                if (dtStrucutre.Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + item[DT_WBS.ID_WBS]).Any())
                {
                    DeleteMyWBSChildren(MyWBSChildren(item, dtWBS, dtStrucutre), dtWBS, dtStrucutre);
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
                if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + rowRiskWBS[DT_RISK_WBS.ID_WBS] + " and " + DT_WBS_STRUCTURE.ID_CHILD + " = " + riskWBS[DT_RISK_WBS.ID_WBS]).Any())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Return if the current WBS is the lowest in the CM Selected
        /// </summary>
        /// <param name="rowCMWBS">WBS of the CM row</param>
        /// <param name="dsWBS">Dataset</param>
        /// <param name="dtCM_WBS">Table of the WBS CM row</param>
        /// <returns>Return if the current WBS is the lowest in the CM Selected</returns>
        public static bool IsCMWBSLow(DataRow rowCMWBS, DataSet dsWBS, DataTable dtCM_WBS)
        {
            foreach (DataRow cmWBS in dtCM_WBS.Select(DT_CM_WBS.ID_CM + " = " + rowCMWBS[DT_CM_WBS.ID_CM]))
            {
                if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + rowCMWBS[DT_CM_WBS.ID_WBS] + " and " + DT_WBS_STRUCTURE.ID_CHILD + " = " + cmWBS[DT_CM_WBS.ID_WBS]).Any())
                {
                    return false;
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
            if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + ID_WBS).Any())
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
            return DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + idWBS).Any();
        }

        /// <summary>
        /// Return the Parent of the Selected WBS
        /// </summary>
        /// <param name="idWBS">ID of the Selected WBS</param>
        /// <param name="DsWBS">Dataset</param>
        /// <returns>Return the Parent of the Selected WBS</returns>
        private static DataRow GetParent(decimal idWBS, DataSet DsWBS)
        {
            decimal idPadre = (decimal)DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + idWBS).First()[DT_WBS_STRUCTURE.ID_FATHER];
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
            if (isCM)
            {
                foreach (DataRow itemDamage in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + idLine))
                {
                    DataRow drWBS_CM_Damage = Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_CM] = idLine;
                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_WBS] = itemWBS[DT_CM_WBS.ID_WBS];
                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                    drWBS_CM_Damage[DT_WBS_CM_Damage.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                    drWBS_CM_Damage[DT_WBS_CM_Damage.VALUE] = 0;
                    drWBS_CM_Damage[DT_WBS_CM_Damage.WBS] = itemWBS[DT_CM_WBS.WBS];
                    if (!(Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], idLine })))
                    {
                        Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(drWBS_CM_Damage);
                    }
                }
            }
            else
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
                        Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBS_RISK_Damage);
                    }
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
            if (isCM)
            {
                foreach (DataRow itemDamage in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + idLine))
                {
                    if (Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], idLine }))
                    {
                        Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Find(new object[] { itemWBS[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], idLine }).Delete();
                    }
                }
            }

            foreach (DataRow itemDamage in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + idLine))
            {
                if (Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], idLine }))
                {
                    Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], idLine }).Delete();
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
        public static void SetDefaultWBSPrimary(DataRow cmWBSrow, bool isCM, DataSet Ds, decimal IDSelected)
        {
            if (isCM)
            {
                if ((bool)cmWBSrow[DT_CM_WBS.IS_PRIMARY] && Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + IDSelected).Any())
                {
                    DataRow cmRow = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(IDSelected);
                    Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + cmRow[DT_CounterM.ID]).First()[DT_CM_WBS.IS_PRIMARY] = true;
                    cmRow[DT_CounterM.ID_WBS] = Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + cmRow[DT_CounterM.ID]).First()[DT_CM_WBS.ID_WBS];
                    cmRow[DT_CounterM.USER_NAME] = Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + cmRow[DT_CounterM.ID]).First()[DT_CM_WBS.USERNAME];
                    cmRow[DT_CounterM.WBS_NAME] = Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + cmRow[DT_CounterM.ID]).First()[DT_CM_WBS.WBS];
                    foreach (var riskrow in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmRow[DT_CounterM.ID]))
                    {
                        riskrow[DT_CounterM_Damage.ID_WBS] = cmRow[DT_CounterM.ID_WBS];
                        riskrow[DT_CounterM_Damage.WBS_NAME] = cmRow[DT_CounterM.WBS_NAME];
                        riskrow[DT_CounterM_Damage.USERNAME] = cmRow[DT_CounterM.USER_NAME];
                    }
                }
            }
            else
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
                if (!(ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    ListaRow.Add(item);
                }
            }
            return ListaRow;
        }

        /// <summary>
        /// Add to each Risk in the current Diagram the Top WBS in the WBS Structure.
        /// </summary>
        /// <param name="ds">Expect the dataset of the Current Diagram</param>
        public static void AddWBSTopToDiagram(DataSet ds, decimal Id_Diagram, DataSet DsWBS)
        {
            foreach (DataRow rowWBS in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (!(HasParent((decimal)rowWBS[DT_WBS.ID_WBS], DsWBS)))
                {
                    foreach (DataRow riskRow in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + Id_Diagram))
                    {
                        if (!(ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { riskRow[DT_Risk.ID], rowWBS[DT_WBS.ID_WBS] })))
                        {
                            DataRow newRiskWBS = ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                            newRiskWBS[DT_RISK_WBS.ID_RISK] = riskRow[DT_Risk.ID];
                            newRiskWBS[DT_RISK_WBS.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                            newRiskWBS[DT_RISK_WBS.IS_PRIMARY] = true;
                            newRiskWBS[DT_RISK_WBS.NIVEL] = rowWBS[DT_WBS.NIVEL];
                            newRiskWBS[DT_RISK_WBS.PRIMARY] = "Primary";
                            newRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                            newRiskWBS[DT_RISK_WBS.RISK] = riskRow[DT_Risk.NAMESHORT];
                            newRiskWBS[DT_RISK_WBS.USERNAME] = rowWBS[DT_WBS.USERNAME];
                            newRiskWBS[DT_RISK_WBS.WBS] = rowWBS[DT_WBS.WBS_NAME];
                            ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRiskWBS);
                        }
                        riskRow[DT_Risk.WBS_NAME] = rowWBS[DT_WBS.NIVEL].ToString() + " " + rowWBS[DT_WBS.WBS_NAME].ToString();
                        riskRow[DT_Risk.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                        riskRow[DT_Risk.USER_NAME] = rowWBS[DT_WBS.USERNAME];
                        foreach (DataRow rowRiskDamage in ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + riskRow[DT_Risk.ID]))
                        {
                            rowRiskDamage[DT_Risk_Damages.WBS_NAME] = rowWBS[DT_WBS.NIVEL].ToString() + " " + rowWBS[DT_WBS.WBS_NAME].ToString();
                            rowRiskDamage[DT_Risk_Damages.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                            rowRiskDamage[DT_Risk_Damages.USERNAME] = rowWBS[DT_WBS.USERNAME];
                        }
                    }
                    foreach (DataRow rowCM in ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + Id_Diagram))
                    {
                        if (!(ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { rowCM[DT_CounterM.ID], rowWBS[DT_WBS.ID_WBS] })))
                        {
                            DataRow newCMWBS = ds.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                            newCMWBS[DT_CM_WBS.ID_CM] = rowCM[DT_CounterM.ID];
                            newCMWBS[DT_CM_WBS.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                            newCMWBS[DT_CM_WBS.IS_PRIMARY] = true;
                            newCMWBS[DT_CM_WBS.NIVEL] = rowWBS[DT_WBS.NIVEL];
                            newCMWBS[DT_CM_WBS.PRIMARY] = "Primary";
                            newCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                            newCMWBS[DT_CM_WBS.CM] = rowCM[DT_CounterM.NAMESHORT];
                            newCMWBS[DT_CM_WBS.USERNAME] = rowWBS[DT_WBS.USERNAME];
                            newCMWBS[DT_CM_WBS.WBS] = rowWBS[DT_WBS.WBS_NAME];
                            ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newCMWBS);
                        }
                        rowCM[DT_CounterM.WBS_NAME] = rowWBS[DT_WBS.NIVEL].ToString() + " " + rowWBS[DT_WBS.WBS_NAME].ToString();
                        rowCM[DT_CounterM.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                        rowCM[DT_CounterM.USER_NAME] = rowWBS[DT_WBS.USERNAME];
                        foreach (DataRow rowCMDamage in ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + rowCM[DT_CounterM.ID]))
                        {
                            rowCMDamage[DT_CounterM_Damage.WBS_NAME] = rowWBS[DT_WBS.NIVEL].ToString() + " " + rowWBS[DT_WBS.WBS_NAME].ToString();
                            rowCMDamage[DT_CounterM_Damage.ID_WBS] = rowWBS[DT_WBS.ID_WBS];
                            rowCMDamage[DT_CounterM_Damage.USERNAME] = rowWBS[DT_WBS.USERNAME];
                        }
                    }
                }
            }
        }

    }
}
