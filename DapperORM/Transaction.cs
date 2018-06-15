using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public class Transaction : ITransaction, IDisposable
    {
        public IDbConnection DbConnection { get; set; }

        public IDbTransaction DbTransaction { get; set; }

        public void Commit()
        {
            if (this.DbTransaction != null)
            {
                this.DbTransaction.Commit();
            }
        }

        public void Rollback()
        {
            if (this.DbTransaction != null)
            {
                this.DbTransaction.Rollback();
            }
        }

        public void Dispose()
        {
            if (this.DbConnection != null)
            {
                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
                this.DbConnection.Dispose();
                this.DbConnection = null;
            }
            if (this.DbTransaction != null)
            {
                this.DbTransaction.Dispose();
                this.DbTransaction = null;
            }
        }
    }
}
