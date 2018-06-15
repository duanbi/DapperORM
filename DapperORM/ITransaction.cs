using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public interface ITransaction
    {
        IDbConnection DbConnection { get; set; }

        IDbTransaction DbTransaction { get; set; }

        void Commit();
    }
}
