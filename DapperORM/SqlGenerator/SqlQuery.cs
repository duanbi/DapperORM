using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM.SqlGenerator
{
    /// <summary>
    /// 代表一个sql语句，包括sql语句和参数
    /// </summary>
    public class SqlQuery
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        public SqlQuery(string sql, dynamic param)
        {
            Param = param;
            Sql = sql;
        }


        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; private set; }

        /// <summary>
        /// sql查询参数
        /// </summary>
        public object Param { get; private set; }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="param"></param>
        public void SetParam(dynamic param)
        {
            Param = param;
        }
        /// <summary>
        /// 附加语句
        /// </summary>
        /// <param name="sqlString"></param>
        public void AppendToSql(string sqlString)
        {
            var sqlBuilder = new StringBuilder(Sql);
            sqlBuilder.AppendLine(sqlString);
            Sql = sqlBuilder.ToString();
        }
        /// <summary>
        /// 附加多条sql
        /// </summary>
        /// <param name="sqlStrings"></param>
        public void AppendToSql(IEnumerable<string> sqlStrings)
        {
            var sqlBuilder = new StringBuilder(Sql);
            foreach (var s in sqlStrings)
            {
                sqlBuilder.AppendLine(s);
            }
            Sql = sqlBuilder.ToString();
        }

    }
}
