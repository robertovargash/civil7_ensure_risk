using DataMapping.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureBusinesss
{
    public class CrossTabController
    {
        public static void AjustarProbabilidadRisk(DataRow drRiskDamage, DataTable dtRisk_WBS, decimal IdWBSFilter)
        {
            try
            {
                if (dtRisk_WBS.Rows.Contains(new object[] {drRiskDamage[DT_Risk_Damages.ID_RISK], IdWBSFilter }))
                {
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = dtRisk_WBS.Rows.Find(new object[] { drRiskDamage[DT_Risk_Damages.ID_RISK], IdWBSFilter })[DT_RISK_WBS.PROBABILITY];

                }
                if ((decimal)drRiskDamage[DT_Risk_Damages.PROBABILITY] == 0)
                {
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AjustarProbabilidadCM(DataRow drCMDamage, DataTable dtCM_WBS, decimal IdWBSFilter)
        {
            try
            {
                if (dtCM_WBS.Rows.Contains(new object[] {drCMDamage[DT_CounterM_Damage.ID_COUNTERM], IdWBSFilter }))
                {
                    drCMDamage[DT_CounterM_Damage.RISK_REDUCTION] = dtCM_WBS.Rows.Find(new object[] { drCMDamage[DT_CounterM_Damage.ID_COUNTERM], IdWBSFilter })[DT_CM_WBS.PROBABILITY];
                }
            }
            catch
            {
                throw new Exception("Error ajustando ProbCM");
            }
        }

        public static void AjustarDamagesRisk(DataRow drRiskDamage, DataTable dtRisk_WBS_Damage, decimal IdWBSFilter)
        {
            try
            {
                if (dtRisk_WBS_Damage.Rows.Contains(new object[] { IdWBSFilter, drRiskDamage[DT_Risk_Damages.ID_DAMAGE], drRiskDamage[DT_Risk_Damages.ID_RISK] }))
                {
                    drRiskDamage[DT_Risk_Damages.VALUE] = dtRisk_WBS_Damage.Rows.Find(new object[] { IdWBSFilter, drRiskDamage[DT_Risk_Damages.ID_DAMAGE], drRiskDamage[DT_Risk_Damages.ID_RISK] })[DT_WBS_RISK_DAMAGE.VALUE];
                }
                if ((decimal)drRiskDamage[DT_Risk_Damages.VALUE] == 0)
                {
                    drRiskDamage[DT_Risk_Damages.VALUE] = DBNull.Value;
                }
            }
            catch
            {
                throw new Exception("Error ajustandoDamagesRisk");
            }
        }

        public static void AjustarDamagesCM(DataRow drCMDamage, DataTable dtCM_WBS_Damage, decimal IdWBSFilter)
        {
            try
            {
                if (dtCM_WBS_Damage.Rows.Contains(new object[] {IdWBSFilter, drCMDamage[DT_CounterM_Damage.ID_DAMAGE], drCMDamage[DT_CounterM_Damage.ID_COUNTERM] }))
                {
                    drCMDamage[DT_CounterM_Damage.VALUE] = dtCM_WBS_Damage.Rows.Find(new object[] { IdWBSFilter, drCMDamage[DT_CounterM_Damage.ID_DAMAGE], drCMDamage[DT_CounterM_Damage.ID_COUNTERM] })[DT_WBS_CM_Damage.VALUE];
                }
            }
            catch
            {
                throw new Exception("Error ajustando DmaagesCM");
            }
        }

        public static void ColumnasTextoRiskReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() == "Risk")
                    {
                        item.IsReadOnly = !flag;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ColumnasNumerosRiskReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() != "Risk" && item.Header.ToString() != "WBS Name" && item.Header.ToString() != "User" && item.Header.ToString() != "Father" && item.Header.ToString() != "Active?")
                    {
                        item.IsReadOnly = !flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ColumnasTextoCMReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() == "Counter M.")
                    {
                        item.IsReadOnly = !flag;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ColumnasNumerosCMReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() != "Counter M." && item.Header.ToString() != "WBS Name" && item.Header.ToString() != "User" && item.Header.ToString() != "Risk" && item.Header.ToString() != "Status")
                    {
                        item.IsReadOnly = !flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CleanDynamicCMColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "Risk Reduction(%)" && dataGrid.Columns[i].Header.ToString() != "Counter M." && dataGrid.Columns[i].Header.ToString() != "WBS Name"
                    && dataGrid.Columns[i].Header.ToString() != "User" && dataGrid.Columns[i].Header.ToString() != "Risk" && dataGrid.Columns[i].Header.ToString() != "Active?")
                {
                    dataGrid.Columns.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }

        public static void CleanDynamicRiskColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "Probability (%)" && dataGrid.Columns[i].Header.ToString() != "Risk" && dataGrid.Columns[i].Header.ToString() != "WBS Name"
                    && dataGrid.Columns[i].Header.ToString() != "User" && dataGrid.Columns[i].Header.ToString() != "Father" && dataGrid.Columns[i].Header.ToString() != "Active?")
                {
                    dataGrid.Columns.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }
    }
}
