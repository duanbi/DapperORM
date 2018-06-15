using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM
{
    public interface IRepository<TEntity, TPrimaryKey> where TEntity :
       class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 查找元素
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        TEntity FindById(TPrimaryKey primaryKey);

        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 插入操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TPrimaryKey Insert(TEntity entity);

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(TEntity entity);
    }
}
