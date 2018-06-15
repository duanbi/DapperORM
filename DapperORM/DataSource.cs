using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public class DataSource
    {
        private static string DbName { get; set; }

        private static string connStr = string.Empty;
        public static string ConnectionString
        {
            get { return string.IsNullOrEmpty(connStr) ? DbConnectionStringFactory.GetDbConnectionConfig(DbName)["connectionString"] : connStr; }
            set { connStr = value; }
        }

        //默认 SQL Server
        private static string providerName = "System.Data.SqlClient";
        public static string ProviderName
        {
            get { return  DbConnectionStringFactory.GetDbConnectionConfig(DbName)["providerName"] ?? providerName; }
            set { providerName = value; }
        }


        public static IDbConnection GetConnection(string dbName, DataBaseTypeEnum dataBaseType)
        {
            DbName = dbName;
            var factory = DbProviderFactories.GetFactory(ProviderName);

            var dbConnection = factory.CreateConnection();
            dbConnection.ConnectionString = ConnectionString;
            return dbConnection;
        }

    }
}
