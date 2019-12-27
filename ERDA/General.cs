using System;
using System.Data.Common;

namespace ERDA
{
    public abstract class ERDA : IDisposable
    {
        #region Declaracion Propiedades

        protected DbDataAdapter adDataAdapter;
        protected DbTransaction adTransaction;
        protected Connection adConnection;

        protected DbCommand loadCommand;
        protected DbCommand insertCommand;
        protected DbCommand updateCommand;
        protected DbCommand deleteCommand;

        #endregion

        #region Constructores y Destructores

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (adDataAdapter != null)
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    if (adDataAdapter.SelectCommand.Connection != null)
                    {
                        adDataAdapter.SelectCommand.Connection.Dispose();
                    }
                    adDataAdapter.SelectCommand.Dispose();
                }
                adDataAdapter.Dispose();
                adDataAdapter = null;
            }
        }

        #endregion


    }

}
