using Dapper;
using DapperORM.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private DbContext _dbContext;
        private ISqlGenerator<TEntity, TPrimaryKey> _SqlGenerator;

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection
        {
            get
            {
                return _dbContext.Connection;
            }
        }

        /// <summary>
        /// Sql解析类
        /// </summary>
        private ISqlGenerator<TEntity, TPrimaryKey> SqlGenerator
        {
            get
            {
                if (_SqlGenerator == null)
                {
                    _SqlGenerator = new SqlGenerator<TEntity, TPrimaryKey>();
                }
                return _SqlGenerator;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext"></param>
        public Repository(string dbName, DataBaseTypeEnum dataBaseType)
        {
            _dbContext = new DbContext(dbName, dataBaseType);
        }

        /// <summary>
        /// 构造函数(默认连接读库)
        /// </summary>
        /// <param name="dbContext"></param>
        public Repository(string dbName)
        {
            _dbContext = new DbContext(dbName, DataBaseTypeEnum.Read);
        }

        /// <summary>
        /// 公用方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected virtual T Invoke<T>(Func<IDbConnection, T> func)
        {
            var conn = DbConnection;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                return func(conn);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        /// <summary>
        /// 获取第一个对象
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            var sqlQuery = SqlGenerator.GetSelectFirst(expression);
            return FindAll(sqlQuery).FirstOrDefault();
        }

        /// <summary>
        /// 根据主键查找
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public virtual TEntity FindById(TPrimaryKey primaryKey)
        {
            var expression = SqlGenerator.CreateEqualityExpressionForId(primaryKey);
            var sqlQuery = SqlGenerator.GetSelectFirst(expression);
            return FindAll(sqlQuery).FirstOrDefault();
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindAll()
        {
            var sqlQuery = SqlGenerator.GetSelectAll(null);
            return FindAll(sqlQuery);
        }

        /// <summary>
        /// linq查找对应所有数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate);
            return FindAll(sqlQuery);
        }

        /// <summary>
        /// 根据sql和参数查找所有实体
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        private IEnumerable<TEntity> FindAll(SqlQuery sqlQuery)
        {
            IEnumerable<TEntity> change = default(IEnumerable<TEntity>);
            Invoke(conn => change = conn.Query<TEntity>(sqlQuery.Sql, sqlQuery.Param));
            return change;
        }

        /// <summary>
        /// 查找数量
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            var sqlQuery = SqlGenerator.GetSelectAll(predicate);
            int primaryKey = 0;
            Invoke((conn) => primaryKey = conn.Query<TEntity>(sqlQuery.Sql, sqlQuery.Param).Count());
            return primaryKey;
        }

        /// <summary>
        /// 插入TEntity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TPrimaryKey Insert(TEntity entity)
        {
            TPrimaryKey primaryKey = default(TPrimaryKey);
            var sqlQuery = SqlGenerator.GetInsert(entity);
            Invoke((conn) => primaryKey = conn.ExecuteScalar<TPrimaryKey>(sqlQuery.Sql, sqlQuery.Param));
            if (primaryKey == null || primaryKey.Equals(default(TPrimaryKey)))
            {
                primaryKey = entity.Id;
            }
            return primaryKey;
        }

        /// <summary>
        /// 更新TEntity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Update(TEntity entity)
        {
            bool change = false;
            var sqlQuery = SqlGenerator.GetUpdate(entity);
            Invoke((conn) => change = conn.Execute(sqlQuery.Sql, sqlQuery.Param) > 0);
            return change;
        }
        
    }
}
