using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace DapperORM
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// 执行查询sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IEnumerable<TTEntity> ExcuteQuery<TTEntity>(this DbContext context, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = default(int?))
        {
            if (IsHaveChangeSql(sql))
            {
                throw new Exception("执行查询操作，不能含有修改关键词");
            }
            IEnumerable<TTEntity> change = default(IEnumerable<TTEntity>);
            using (context)
            {
                change = context.Connection.Query<TTEntity>(sql, param, transaction, buffered,commandTimeout);
            }
            return change;
        }

        /// <summary>
        /// 执行更新操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int ExcuteNonQuery(this DbContext context, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            if (IsHaveQuerySql(sql))
            {
                throw new Exception("执行更新操作，不能含有select/delete关键词");
            }
            int change = 0;
            using (context)
            {
                change = context.Connection.Execute(sql, param, transaction, commandTimeout);
            }
            return change;
        }



        /// <summary>
        /// 返回单个数据
        /// </summary>
        /// <typeparam name="TTEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static TTEntity ExecuteScalar<TTEntity>(this DbContext context,string sql, object param = null,IDbTransaction transaction = null, int? commandTimeout = default(int?))
        {
            if (IsHaveChangeSql(sql))
            {
                throw new Exception("执行查询操作，不能含有insert|delete|update修改关键词");
            }
            TTEntity change = default(TTEntity);
            using (context) {
                change = context.Connection.ExecuteScalar<TTEntity>(sql, param, transaction, commandTimeout);
            }
            return change;
        }


        ///// <summary>
        ///// 执行SQL操作
        ///// </summary>
        ///// <param name="sql"></param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static int Excute(this DbContext context, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?))
        //{
        //    int change = 0;
        //    using (context)
        //    {
        //        change = context.Connection.Execute(sql, param, transaction, commandTimeout);
        //    }
        //    return change;
        //}

        /// <summary>
        /// 查询语句过滤
        /// </summary>
        /// <returns></returns>
        private static bool IsHaveQuerySql(string sql)
        {
            string word = "select|delete";
            if (sql == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((sql.ToLower().IndexOf(i + " ") > -1) || (sql.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 修改语句过滤
        /// </summary>
        /// <returns></returns>
        private static bool IsHaveChangeSql(string sql)
        {
            string word = "insert|delete|update";
            if (sql == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((sql.ToLower().IndexOf(i + " ") > -1) || (sql.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
