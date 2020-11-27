using EnsureBusinesss;
using EnsureBusinesss.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataMapping.Data;

namespace EnsureRisk.Export.Trader
{
    public class RiskAndCm
    {
        public bool isCM;
        public decimal id;
        public decimal value;
    }

    /// <summary>
    /// Controller to fetch data from diagram dataset source
    /// </summary>
    public class RiskTreeDataSetTrader : DataSetTrader, IDisposable
    {

        public RiskAndCm[] AcumulatedValueList { get; set; }
        public List<RiskPolyLine> LinesDiagram { get; set; }

        /// <summary>
        /// Risk data container
        /// </summary>
        public DataTable RiskDataTable
        {
            get
            {
                return _riskTreeDataSet.Tables[RISK_TABLENAME];
            }
        }

        /// <summary>
        /// Counter measure data container
        /// </summary>
        public DataTable CMDataTable
        {
            get
            {
                return _riskTreeDataSet.Tables[COUNTERM_TABLENAME];
            }
        }

        /// <summary>
        /// Risk top risk data container
        /// </summary>
        public DataTable RiskTopRiskDataTable
        {
            get
            {
                return _riskTreeDataSet.Tables[RISK_TOPRISK_TABLENAME];
            }
        }
        public IEnumerable<String> RiskTypeList
        {
            get
            {
                return GetRiskTypeList();
            }
        }

        public DataRow[] DamagesRow { get; set; }
        private DataSet _riskTreeDataSet;

        private const string MAINRISKNAME = "Main Branch";
        private const string RiskName = "Risk Name";
        //private const string RiskAcumulatedValue = "Acumulated Value";

        private const string CMName = "CM Name";


        private const string RISKTREE_TABLENAME = "RiskTree";
        private const string RISKTREE_TOPRISK_TABLENAME = "RiskTree_TopRisk";
        private const string TOPRISK_TABLENAME = "TopRisk";
        private const string RISK_TABLENAME = "Risk";
        private const string RISK_TOPRISK_TABLENAME = "Risk_TopRisk";
        private const string COUNTERM_TABLENAME = "CounterM";
        private const string COUNTERM_TOPRISK_TABLENAME = "CounterM_TopRisk";

        private const string IDRISKTREE_COLUMNNAME = "idRiskTree";
        private const string IDTOPRISK_COLUMNNAME = "idTopRisk";
        private const string POSITION_COLUMNNAME = "position";
        private const string IDRISK_COLUMNNAME = "idRisk";
        private const string IDCOUNTERM_COLUMNNAME = "idCounterM";
        private const string DAMAGE_COLUMNNAME = "Damage";
        private const string ISROOT_COLUMNNAME = "isRoot";
        private const string IDRISKFATHER_COLUMNNAME = "idRiskFather";

        public RiskTreeDataSetTrader(DataSet dataSet, decimal id) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
        }
        public RiskTreeDataSetTrader(DataSet dataSet, decimal id, List<RiskPolyLine> linesDiagram) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
            LinesDiagram = linesDiagram;
        }

        /// <summary>
        /// Populate RiskTreeDataSetTrader with diagram data
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="id">Diagram id</param>
        /// <param name="linesDiagram"></param>
        /// <param name="drDamages"></param>
        public RiskTreeDataSetTrader(DataSet dataSet, decimal id, List<RiskPolyLine> linesDiagram, DataRow[] drDamages) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
            LinesDiagram = linesDiagram;
            DamagesRow = drDamages;
        }
        public RiskTreeDataSetTrader(DataSet dataSet, decimal id, RiskAndCm[] acumulatedValueList) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
            AcumulatedValueList = acumulatedValueList;
        }

        #region ShortExcel
        
        private int MaxValue(int A, int B)
        {
            if (A > B)
            {
                return A;
            }
            else
            {
                return B;
            }
        }

        /// <summary>
        /// Return main risk descendants
        /// </summary>
        /// <param name="mainRisk">Main risk descendant</param>
        /// <returns></returns>
        public DataTable GetMainRisksDescendants(DataRow mainRisk)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Risk_ID");
            dataTable.Columns.Add("Main Branch");
            dataTable.Columns.Add("Risk Name");
            dataTable.Columns.Add("CM Name");
            List<DataRow> mainRisksRiskDescendats = new List<DataRow>();
            List<DataRow> mainRisksCMDescendats = new List<DataRow>();
            foreach (var descendant in TreeOperation.GetOnlyMyChildrenWithCM(LinesDiagram.Find(l => l.ID == (decimal)mainRisk[IDRISK_COLUMNNAME])))
            {
                if (descendant.IsCM)
                {
                    mainRisksCMDescendats.Add(CMDataTable.Rows.Find(descendant.ID));
                }
                else
                {
                    mainRisksRiskDescendats.Add(RiskDataTable.Rows.Find(descendant.ID));
                }
            }            
            int max = MaxValue(mainRisksRiskDescendats.Count, mainRisksCMDescendats.Count);
            if (max == 0)
            {
                DataRow drNewTable = dataTable.NewRow();
                drNewTable["Main Branch"] = mainRisk[DT_Risk.NAMESHORT];
                drNewTable["Risk_ID"] = mainRisk[DT_Risk.ID];
                dataTable.Rows.Add(drNewTable);
            }
            else
            {
                for (int i = 0; i < max; i++)
                {
                    DataRow drNewTable = dataTable.NewRow();
                    if (i == 0)
                    {
                        drNewTable["Main Branch"] = mainRisk[DT_Risk.NAMESHORT];
                        drNewTable["Risk_ID"] = mainRisk[DT_Risk.ID];
                    }
                    if (mainRisksRiskDescendats.Count > i)
                    {
                        drNewTable["Risk Name"] = mainRisksRiskDescendats[i][DT_Risk.NAMESHORT];
                    }
                    if (mainRisksCMDescendats.Count > i)
                    {
                        drNewTable["CM Name"] = mainRisksCMDescendats[i][DT_CounterM.NAMESHORT];
                    }
                    dataTable.Rows.Add(drNewTable);
                }
            }
            return dataTable;
        }
        #endregion

        private IEnumerable<String> GetRiskTypeList()
        {
            IEnumerable<String> riskPropertiesTypeQuery =
                //from riskTopRiskDataRow in _riskTreeDataSet.Tables[RISK_TOPRISK_TABLENAME].AsEnumerable()
                from riskTopRiskDataRow in DamagesRow.AsEnumerable()
                select riskTopRiskDataRow.Field<String>(DAMAGE_COLUMNNAME);

            return riskPropertiesTypeQuery.Distinct();
        }

        public IEnumerable<DataRow> ObtenerTiposDamagesRisk()
        {
            IEnumerable<String> riskPropertiesTypeQuery =
                from riskTopRiskDataRow in _riskTreeDataSet.Tables[RISK_TOPRISK_TABLENAME].AsEnumerable()
                //from riskTopRiskDataRow in DamagesRow.AsEnumerable()
                select riskTopRiskDataRow.Field<String>(DAMAGE_COLUMNNAME);

            List<DataRow> p = new List<DataRow>();
            foreach (var item in DamagesRow)
            {
                foreach (var itemi in _riskTreeDataSet.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_DAMAGE + "=" + item[DT_Damage.ID_COLUMNA]))
                {
                    p.Add(itemi);
                }                
            }
            return p;
        }

        public IEnumerable<DataRow> GetShortRiskTypeList()
        {
            IEnumerable<DataRow> riskPropertiesTypeQuery =
                //from riskTopRiskDataRow in _riskTreeDataSet.Tables[RISK_TOPRISK_TABLENAME].AsEnumerable()
                from riskTopRiskDataRow in DamagesRow.AsEnumerable()
                select riskTopRiskDataRow;

            return riskPropertiesTypeQuery.Distinct();
        }

        /// <summary>
        /// Return main risk decendants
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataRow> GetMainRiskChildList()
        {
            DataRow mainRisk = RiskDataTable.AsEnumerable().FirstOrDefault(riskDataRow => (bool)(riskDataRow[ISROOT_COLUMNNAME]) == true);

            IEnumerable<DataRow> mainRiskChildDataRowQuery =
                from mainRiskChildDataRow in RiskDataTable.AsEnumerable()
                where mainRiskChildDataRow.Field<decimal?>(IDRISKFATHER_COLUMNNAME) == (decimal)mainRisk[IDRISK_COLUMNNAME]
                select mainRiskChildDataRow;
            return mainRiskChildDataRowQuery;
        }
        public IEnumerable<DataRow> GetRiskPropertyList(decimal riskId)
        {
            IEnumerable<DataRow> riskTopRiskDataRowQuery =
                from riskTopRiskDataRow in RiskTopRiskDataTable.AsEnumerable()
                where riskTopRiskDataRow.Field<decimal>(IDRISK_COLUMNNAME) == riskId
                select riskTopRiskDataRow;
            return riskTopRiskDataRowQuery;
        }

        public IEnumerable<DataRow> GetShortRiskPropertyList(decimal riskId)
        {
            IEnumerable<DataRow> riskTopRiskDataRowQuery =
                from riskTopRiskDataRow in RiskTopRiskDataTable.AsEnumerable()
                where riskTopRiskDataRow.Field<decimal>(IDRISK_COLUMNNAME) == riskId
                select riskTopRiskDataRow;
            return riskTopRiskDataRowQuery;
        }


        public IEnumerable<DataRow> GetCounterMPropertyList(decimal counterMId)
        {
            IEnumerable<DataRow> counterMTopRiskDataRowQuery =
                from counterMTopRiskDataRow in _riskTreeDataSet.Tables[COUNTERM_TOPRISK_TABLENAME].AsEnumerable()
                where counterMTopRiskDataRow.Field<decimal>(IDCOUNTERM_COLUMNNAME) == counterMId
                select counterMTopRiskDataRow;
            return counterMTopRiskDataRowQuery;
        }
        public IEnumerable<DataRow> GetRiskChildList(decimal riskId)
        {
            IEnumerable<DataRow> riskChildDataRowQuery =
                   from riskChildDataRow in RiskDataTable.AsEnumerable()
                   where riskChildDataRow.Field<decimal?>(IDRISKFATHER_COLUMNNAME) == riskId
                   select riskChildDataRow;
            return riskChildDataRowQuery;
        }
        public IEnumerable<DataRow> GetCounterMeasureChildList(decimal riskId)
        {
            IEnumerable<DataRow> counterMChildDataRowQuery =
                   from counterMChildDataRow in _riskTreeDataSet.Tables[COUNTERM_TABLENAME].AsEnumerable()
                   where counterMChildDataRow.Field<decimal?>(IDRISK_COLUMNNAME) == riskId
                   orderby counterMChildDataRow.Field<int?>(POSITION_COLUMNNAME)
                   select counterMChildDataRow;
            return counterMChildDataRowQuery;
        }

        /// <summary>
        /// Return diagram riskTreeID dataset
        /// </summary>
        /// <param name="riskTreeID">Diagram to return</param>
        /// <returns></returns>
        private DataSet GetRiskTreeDataSet(decimal riskTreeID)
        {
            _riskTreeDataSet = new DataSet();

            _riskTreeDataSet.Tables.Add(GetRiskTreeDataTable(riskTreeID));
            _riskTreeDataSet.Tables.Add(GetRiskTreeTopRiskDataTable(riskTreeID));
            _riskTreeDataSet.Tables.Add(GetTopRiskDataTable(_riskTreeDataSet.Tables[RISKTREE_TOPRISK_TABLENAME]));
            _riskTreeDataSet.Tables.Add(GetRiskDataTable(riskTreeID));
            _riskTreeDataSet.Tables.Add(GetRiskTopRiskDataTable(_riskTreeDataSet.Tables[RISK_TABLENAME]));
            _riskTreeDataSet.Tables.Add(GetCounterMDataTable(riskTreeID));
            _riskTreeDataSet.Tables.Add(GetCounterMTopRiskDataTable(_riskTreeDataSet.Tables[COUNTERM_TABLENAME]));

            return _riskTreeDataSet;
        }

        /// <summary>
        /// Return RiskTree datatable
        /// </summary>
        /// <param name="riskTreeID">Diagram id</param>
        /// <returns></returns>
        private DataTable GetRiskTreeDataTable(decimal riskTreeID)
        {
            //RiskTree
            DataTable dataTable = SourceDataSet.Tables[RISKTREE_TABLENAME].Clone();
            dataTable.ImportRow(SourceDataSet.Tables[RISKTREE_TABLENAME].Rows.Find(riskTreeID));
            return dataTable;
        }

        /// <summary>
        /// Return RiskTreeTopRisk datatable
        /// </summary>
        /// <param name="riskTreeID">Diagram id</param>
        /// <returns></returns>
        private DataTable GetRiskTreeTopRiskDataTable(decimal riskTreeID)
        {
            //RiskTree_TopRisk
            DataTable dataTable = SourceDataSet.Tables[RISKTREE_TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> riskTreeDamageDataRowQuery =
                from riskTreeDamageDataRow in SourceDataSet.Tables[RISKTREE_TOPRISK_TABLENAME].AsEnumerable()
                where riskTreeDamageDataRow.Field<decimal>(IDRISKTREE_COLUMNNAME) == riskTreeID
                select riskTreeDamageDataRow;
            riskTreeDamageDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }

        /// <summary>
        /// Return TopRisk datatable
        /// </summary>
        /// <param name="riskTreeTopRiskDataTable">RiskTreeTopRisk datatable</param>
        /// <returns></returns>
        private DataTable GetTopRiskDataTable(DataTable riskTreeTopRiskDataTable)
        {
            //TopRisk 
            DataTable dataTable = SourceDataSet.Tables[TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> topRiskDataRowQuery =
                from topRiskDataRow in SourceDataSet.Tables[TOPRISK_TABLENAME].AsEnumerable()
                join riskTreeDamageDataRow in riskTreeTopRiskDataTable.AsEnumerable()
                on topRiskDataRow.Field<decimal>(IDTOPRISK_COLUMNNAME) equals riskTreeDamageDataRow.Field<decimal>(IDTOPRISK_COLUMNNAME)
                select topRiskDataRow;
            topRiskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }

        /// <summary>
        /// Get risk datatable
        /// </summary>
        /// <param name="riskTreeID">Diagram id</param>
        /// <returns></returns>
        private DataTable GetRiskDataTable(decimal riskTreeID)
        {
            //Risk
            DataTable dataTable = SourceDataSet.Tables[RISK_TABLENAME].Clone();
            IEnumerable<DataRow> riskDataRowQuery =
                from riskDataRow in SourceDataSet.Tables[RISK_TABLENAME].AsEnumerable()
                where riskDataRow.Field<decimal>(IDRISKTREE_COLUMNNAME) == riskTreeID
                orderby riskDataRow.Field<int?>(POSITION_COLUMNNAME)
                select riskDataRow;
            riskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            //TODO: Ojo con esto
            return dataTable;
        }

        /// <summary>
        /// Return RiskTopRisk datatable
        /// </summary>
        /// <param name="riskDataTable">Risk datatable</param>
        /// <returns></returns>
        private DataTable GetRiskTopRiskDataTable(DataTable riskDataTable)
        {
            //Risk_TopRisk
            DataTable dataTable = SourceDataSet.Tables[RISK_TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> riskTopRiskDataRowQuery =
                from riskTopRiskDataRow in SourceDataSet.Tables[RISK_TOPRISK_TABLENAME].AsEnumerable()
                join riskDataRow in riskDataTable.AsEnumerable() on riskTopRiskDataRow.Field<decimal>(IDRISK_COLUMNNAME) equals riskDataRow.Field<decimal>(IDRISK_COLUMNNAME)
                orderby riskDataRow.Field<int?>(POSITION_COLUMNNAME), riskTopRiskDataRow.Field<decimal>(IDRISK_COLUMNNAME)
                select riskTopRiskDataRow;
            riskTopRiskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }

        /// <summary>
        /// Return Counter measure datatable
        /// </summary>
        /// <param name="riskTreeID">Diagram id</param>
        /// <returns></returns>
        private DataTable GetCounterMDataTable(decimal riskTreeID)
        {
            //CounterM
            DataTable dataTable = SourceDataSet.Tables[COUNTERM_TABLENAME].Clone();
            IEnumerable<DataRow> counterMesureDataRowQuery =
                from counterMesureDataRow in SourceDataSet.Tables[COUNTERM_TABLENAME].AsEnumerable()
                where counterMesureDataRow.Field<decimal>(IDRISKTREE_COLUMNNAME) == riskTreeID
                select counterMesureDataRow;
            counterMesureDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }

        /// <summary>
        /// Return counter measure top risk datatable
        /// </summary>
        /// <param name="counterMDataTable">Xounter measure datatable</param>
        /// <returns></returns>
        private DataTable GetCounterMTopRiskDataTable(DataTable counterMDataTable)
        {
            //CounterM_TopRisk
            DataTable dataTable = SourceDataSet.Tables[COUNTERM_TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> counterMeasureTopRiskDataRowQuery =
                from counterMeasureTopRiskDataRow in SourceDataSet.Tables[COUNTERM_TOPRISK_TABLENAME].AsEnumerable()
                join counterMesureDataRow in counterMDataTable.AsEnumerable() on counterMeasureTopRiskDataRow.Field<decimal>(IDCOUNTERM_COLUMNNAME) equals counterMesureDataRow.Field<decimal>(IDCOUNTERM_COLUMNNAME)
                orderby counterMesureDataRow.Field<int>(POSITION_COLUMNNAME), counterMeasureTopRiskDataRow.Field<decimal>(IDCOUNTERM_COLUMNNAME)
                select counterMeasureTopRiskDataRow;
            counterMeasureTopRiskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }

        /// <summary>
        /// Count risk and counter measure
        /// </summary>
        /// <returns></returns>
        public int RowsCount()
        {
            int riskCount = _riskTreeDataSet.Tables[RISK_TABLENAME].Rows.Count;

            var query = from counterMDataRow in _riskTreeDataSet.Tables[COUNTERM_TABLENAME].AsEnumerable()
                        group counterMDataRow by counterMDataRow.Field<decimal>(IDRISK_COLUMNNAME) into counterMGroup
                        select new
                        {
                            idRisk = counterMGroup.Key,
                            CounterMCount = counterMGroup.Count() - 1
                        };
            int counterMCount = query.Sum(counterMGroup => counterMGroup.CounterMCount);

            return riskCount + counterMCount;
        }
        public decimal RiskAcumulatedValue(decimal riskId)
        {
            RiskAndCm riskFound = AcumulatedValueList.Where(value => !value.isCM && value.id == riskId).FirstOrDefault();

            return (riskFound != null) ? riskFound.value : 0;
        }
        public decimal CounterMeasureAcumulatedDamage(decimal counterMeasureId)
        {
            RiskAndCm riskFound = AcumulatedValueList.Where(value => value.isCM && value.id == counterMeasureId).FirstOrDefault();

            return (riskFound != null) ? riskFound.value : 0;
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }
                disposedValue = true;
            }
        }

        // ~RiskTreeDataSetTrader()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
