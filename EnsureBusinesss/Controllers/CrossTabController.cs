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
            }
            catch (Exception ex)
            {
                throw ex;
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
        
        public static void CleanDynamicCMColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "R. Reduct. (%)" && dataGrid.Columns[i].Header.ToString() != "Counter M." && dataGrid.Columns[i].Header.ToString() != "WBS Name"
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
