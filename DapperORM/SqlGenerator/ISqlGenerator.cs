using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM.SqlGenerator
{
    public interface ISqlGenerator<TEntity,TPrimaryKey> where TEntity:class,IEntity<TPrimaryKey>
    {
        string TableName { get; }

        IEnumerable<PropertyMetadata> KeyProperties { get; }

        IEnumerable<PropertyMetadata> BaseProperties { get; }

        PropertyMetadata IdentityProperty { get; }

        SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate);

        SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate);

        SqlQuery GetCount(Expression<Func<TEntity, bool>> predicate);

        SqlQuery GetDelete(Expression<Func<TEntity, bool>> predicate);
        SqlQuery GetUpdate(TEntity instance);
        SqlQuery GetInsert(TEntity instance);
        Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id);
    }
}
