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
       
    }
}
