using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public class DbContext : IDbContext
    {
        
        private string _dbName;

        public IDbConnection Connection { private set; get; }

        public IDbTransaction DbTransaction { private set; get; }

        public DataBaseTypeEnum DataBaseType { get; set; }

        private ITransaction transaction;

        public ITransaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }

        public DataBaseTypeEnum DatabaseType { get; set; }

       
        public DbContext(string dbName= "default", DataBaseTypeEnum dataBaseType = DataBaseTypeEnum.Write, ITransaction transaction=null)
        {
            DataBaseType = dataBaseType;
            transaction = transaction;
            Connection = DataSource.GetConnection(dbName, dataBaseType);
        }


        /// <summary>
        /// 创建数据库连接
        /// </summary>
        public void InitConnection()
        {
            if (transaction == null)
            {
                Connection = DataSource.GetConnection(_dbName, DataBaseType);
                Connection.Open();
            }
            else if (transaction != null && transaction.DbConnection == null)
            {
                Connection = DataSource.GetConnection(_dbName, DataBaseType);
                Connection.Open();
                transaction.DbConnection = Connection;
                transaction.DbTransaction = Connection.BeginTransaction();
                DbTransaction = transaction.DbTransaction;
            }
            else if (transaction != null && transaction.DbConnection != null)
            {
                Connection = transaction.DbConnection;
                DbTransaction = transaction.DbTransaction;
            }
        }

        public IDbTransaction BeginTran(IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            if (this.Connection == null)
            {
                throw new Exception("Connection为null");
            }
            if (this.Connection.State != ConnectionState.Open)
            {
                this.Connection.Open();
            }
            DbTransaction = this.Connection.BeginTransaction();
            return DbTransaction;
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void Commit()
        {
            if (DbTransaction == null)
            {
                throw new Exception("未开启事务");
            }
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = null;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            if (DbTransaction == null)
            {
                throw new Exception("未开启事务");
            }
            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = null;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (transaction != null)
            {
                return;
            }
            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection = null;
            }
            if (DbTransaction != null)
            {
                DbTransaction = null;
            }
        }
    }
}
