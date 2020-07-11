using EnsureBusinesss.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EnsureRisk.Export.Trader
{
    public class RiskAndCm
    {
        public bool isCM;
        public int id;
        public decimal value;
    }
    public class RiskTreeDataSetTrader : DataSetTrader, IDisposable
    {
        //public List<RiskPolyLine> LinesList
        //{
        //    get
        //    {
        //        return _linesList;
        //    }
        //}
        //private IEnumerable<RiskAndCm> _acumulatedValueList;
        public RiskAndCm[] _acumulatedValueList { get; set; }
        public List<RiskPolyLine> LinesDiagram { get; set; }
        //object _riskList;
        //object _cmList;

        public DataTable RiskDataTable
        {
            get
            {
                return _riskTreeDataSet.Tables[RISK_TABLENAME];
            }
        }
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
        private DataSet _riskTreeDataSet;
        //private List<RiskPolyLine> _linesList;

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

        public RiskTreeDataSetTrader(DataSet dataSet, int id) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
        }
        public RiskTreeDataSetTrader(DataSet dataSet, int id, List<RiskPolyLine> linesDiagram) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
            LinesDiagram = linesDiagram;
        }
        public RiskTreeDataSetTrader(DataSet dataSet, int id, RiskAndCm[] acumulatedValueList) : base(dataSet)
        {
            GetRiskTreeDataSet(id);
            _acumulatedValueList = acumulatedValueList;
        }
        private IEnumerable<String> GetRiskTypeList()
        {
            IEnumerable<String> riskPropertiesTypeQuery =
                from riskTopRiskDataRow in _riskTreeDataSet.Tables[RISK_TOPRISK_TABLENAME].AsEnumerable()
                select riskTopRiskDataRow.Field<String>(DAMAGE_COLUMNNAME);

            return riskPropertiesTypeQuery.Distinct();
        }
        public IEnumerable<DataRow> GetMainRiskChildList()
        {
            DataRow mainRisk = RiskDataTable.AsEnumerable().FirstOrDefault(riskDataRow => (Boolean)(riskDataRow[ISROOT_COLUMNNAME]) == true);

            IEnumerable<DataRow> mainRiskChildDataRowQuery =
                from mainRiskChildDataRow in RiskDataTable.AsEnumerable()
                where mainRiskChildDataRow.Field<int?>(IDRISKFATHER_COLUMNNAME) == (int)mainRisk[IDRISK_COLUMNNAME]
                select mainRiskChildDataRow;
            return mainRiskChildDataRowQuery;
        }
        public IEnumerable<DataRow> GetRiskPropertyList(int riskId)
        {
            IEnumerable<DataRow> riskTopRiskDataRowQuery =
                from riskTopRiskDataRow in RiskTopRiskDataTable.AsEnumerable()
                where riskTopRiskDataRow.Field<int>(IDRISK_COLUMNNAME) == riskId
                select riskTopRiskDataRow;
            return riskTopRiskDataRowQuery;
        }
        public IEnumerable<DataRow> GetCounterMPropertyList(int counterMId)
        {
            IEnumerable<DataRow> counterMTopRiskDataRowQuery =
                from counterMTopRiskDataRow in _riskTreeDataSet.Tables[COUNTERM_TOPRISK_TABLENAME].AsEnumerable()
                where counterMTopRiskDataRow.Field<int>(IDCOUNTERM_COLUMNNAME) == counterMId
                select counterMTopRiskDataRow;
            return counterMTopRiskDataRowQuery;
        }
        public IEnumerable<DataRow> GetRiskChildList(int riskId)
        {
            IEnumerable<DataRow> riskChildDataRowQuery =
                   from riskChildDataRow in RiskDataTable.AsEnumerable()
                   where riskChildDataRow.Field<int?>(IDRISKFATHER_COLUMNNAME) == riskId
                   select riskChildDataRow;
            return riskChildDataRowQuery;
        }
        public IEnumerable<DataRow> GetCounterMeasureChildList(int riskId)
        {
            IEnumerable<DataRow> counterMChildDataRowQuery =
                   from counterMChildDataRow in _riskTreeDataSet.Tables[COUNTERM_TABLENAME].AsEnumerable()
                   where counterMChildDataRow.Field<int?>(IDRISK_COLUMNNAME) == riskId
                   orderby counterMChildDataRow.Field<int?>(POSITION_COLUMNNAME)
                   select counterMChildDataRow;
            return counterMChildDataRowQuery;
        }
        private DataSet GetRiskTreeDataSet(int riskTreeID)
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
        private DataTable GetRiskTreeDataTable(int riskTreeID)
        {
            //RiskTree
            DataTable dataTable = SourceDataSet.Tables[RISKTREE_TABLENAME].Clone();
            dataTable.ImportRow(SourceDataSet.Tables[RISKTREE_TABLENAME].Rows.Find(riskTreeID));
            return dataTable;
        }
        private DataTable GetRiskTreeTopRiskDataTable(int riskTreeID)
        {
            //RiskTree_TopRisk
            DataTable dataTable = SourceDataSet.Tables[RISKTREE_TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> riskTreeDamageDataRowQuery =
                from riskTreeDamageDataRow in SourceDataSet.Tables[RISKTREE_TOPRISK_TABLENAME].AsEnumerable()
                where riskTreeDamageDataRow.Field<int>(IDRISKTREE_COLUMNNAME) == riskTreeID
                select riskTreeDamageDataRow;
            riskTreeDamageDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }
        private DataTable GetTopRiskDataTable(DataTable riskTreeTopRiskDataTable)
        {
            //TopRisk 
            DataTable dataTable = SourceDataSet.Tables[TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> topRiskDataRowQuery =
                from topRiskDataRow in SourceDataSet.Tables[TOPRISK_TABLENAME].AsEnumerable()
                join riskTreeDamageDataRow in riskTreeTopRiskDataTable.AsEnumerable()
                on topRiskDataRow.Field<int>(IDTOPRISK_COLUMNNAME) equals riskTreeDamageDataRow.Field<int>(IDTOPRISK_COLUMNNAME)
                select topRiskDataRow;
            topRiskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }
        private DataTable GetRiskDataTable(int riskTreeID)
        {
            //Risk
            DataTable dataTable = SourceDataSet.Tables[RISK_TABLENAME].Clone();
            IEnumerable<DataRow> riskDataRowQuery =
                from riskDataRow in SourceDataSet.Tables[RISK_TABLENAME].AsEnumerable()
                where riskDataRow.Field<int>(IDRISKTREE_COLUMNNAME) == riskTreeID
                orderby riskDataRow.Field<int?>(POSITION_COLUMNNAME)
                select riskDataRow;
            riskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }
        private DataTable GetRiskTopRiskDataTable(DataTable riskDataTable)
        {
            //Risk_TopRisk
            DataTable dataTable = SourceDataSet.Tables[RISK_TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> riskTopRiskDataRowQuery =
                from riskTopRiskDataRow in SourceDataSet.Tables[RISK_TOPRISK_TABLENAME].AsEnumerable()
                join riskDataRow in riskDataTable.AsEnumerable() on riskTopRiskDataRow.Field<int>(IDRISK_COLUMNNAME) equals riskDataRow.Field<int>(IDRISK_COLUMNNAME)
                orderby riskDataRow.Field<int?>(POSITION_COLUMNNAME), riskTopRiskDataRow.Field<int>(IDRISK_COLUMNNAME)
                select riskTopRiskDataRow;
            riskTopRiskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }
        private DataTable GetCounterMDataTable(int riskTreeID)
        {
            //CounterM
            DataTable dataTable = SourceDataSet.Tables[COUNTERM_TABLENAME].Clone();
            IEnumerable<DataRow> counterMesureDataRowQuery =
                from counterMesureDataRow in SourceDataSet.Tables[COUNTERM_TABLENAME].AsEnumerable()
                where counterMesureDataRow.Field<int>(IDRISKTREE_COLUMNNAME) == riskTreeID
                select counterMesureDataRow;
            counterMesureDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }
        private DataTable GetCounterMTopRiskDataTable(DataTable counterMDataTable)
        {
            //CounterM_TopRisk
            DataTable dataTable = SourceDataSet.Tables[COUNTERM_TOPRISK_TABLENAME].Clone();
            IEnumerable<DataRow> counterMeasureTopRiskDataRowQuery =
                from counterMeasureTopRiskDataRow in SourceDataSet.Tables[COUNTERM_TOPRISK_TABLENAME].AsEnumerable()
                join counterMesureDataRow in counterMDataTable.AsEnumerable() on counterMeasureTopRiskDataRow.Field<int>(IDCOUNTERM_COLUMNNAME) equals counterMesureDataRow.Field<int>(IDCOUNTERM_COLUMNNAME)
                orderby counterMesureDataRow.Field<int>(POSITION_COLUMNNAME), counterMeasureTopRiskDataRow.Field<int>(IDCOUNTERM_COLUMNNAME)
                select counterMeasureTopRiskDataRow;
            counterMeasureTopRiskDataRowQuery.CopyToDataTable<DataRow>(dataTable, LoadOption.OverwriteChanges);
            return dataTable;
        }
        public int RowsCount()
        {
            int riskCount = _riskTreeDataSet.Tables[RISK_TABLENAME].Rows.Count;

            var query = from counterMDataRow in _riskTreeDataSet.Tables[COUNTERM_TABLENAME].AsEnumerable()
                        group counterMDataRow by counterMDataRow.Field<int>(IDRISK_COLUMNNAME) into counterMGroup
                        select new
                        {
                            idRisk = counterMGroup.Key,
                            CounterMCount = counterMGroup.Count() - 1
                        };
            int counterMCount = query.Sum(counterMGroup => counterMGroup.CounterMCount);

            return riskCount + counterMCount;
        }
        public decimal RiskAcumulatedValue(int riskId)
        {
            RiskAndCm riskFound = _acumulatedValueList.Where(value => !value.isCM && value.id == riskId).FirstOrDefault();

            return (riskFound != null) ? riskFound.value : 0;
        }
        public decimal CounterMeasureAcumulatedDamage(int counterMeasureId)
        {
            RiskAndCm riskFound = _acumulatedValueList.Where(value => value.isCM && value.id == counterMeasureId).FirstOrDefault();

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
