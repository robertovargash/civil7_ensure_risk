using DataMapping.Data;
using EnsureBusinesss.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureBusinesss
{
    public class ThicknessProvider
    {
        public DataSet Ds { get; set; }
        public int ID_Diagram { get; set; }
        public int ID_TopRisk { get; set; }
        public List<RiskPolyLine> LinesList { get; set; }

        public ThicknessProvider()
        {
        }
        public ThicknessProvider(int diagramID, DataSet diagramsDataSet, List<RiskPolyLine> polyLines, int id_TopRisk)
        {
            ID_Diagram = diagramID;
            Ds = diagramsDataSet;
            LinesList = polyLines;
            ID_TopRisk = id_TopRisk;
        }

        public void UpdateThickness()
        {
            DataTable dtRisk = Ds.Tables[DT_Risk.TABLE_NAME].Clone();
            foreach (var item in Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_RISK_TREE + " = " + ID_Diagram))
            {
                dtRisk.ImportRow(item);
            }
            General.UpdateThickness(ID_TopRisk, dtRisk, LinesList, Ds.Tables[DT_Risk_Damages.TABLENAME],
                           Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLENAME]);
        }
    }
}
