using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public interface IDbContext : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction DbTransaction { get; }

        IDbTransaction BeginTran(IsolationLevel isolation = IsolationLevel.ReadCommitted);

        void InitConnection();

        void Commit();

        void Rollback();
    }
}
