using DapperORM.Attribute;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM.SqlGenerator
{
    public class SqlGenerator<TEntity, TPrimaryKey> : ISqlGenerator<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {

        public string TableName { get; private set; }

        public PropertyInfo[] AllProperties { get; private set; }

        public PropertyMetadata IdentityProperty { get; private set; }

        public IEnumerable<PropertyMetadata> KeyProperties { get; private set; }

        public IEnumerable<PropertyMetadata> BaseProperties { get; private set; }


        public SqlGenerator()
        {
            var entityType = typeof(TEntity);
            var entityTypeInfo = entityType.GetTypeInfo();
            var aliasAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();

            this.TableName = aliasAttribute != null ? aliasAttribute.TableName : entityTypeInfo.Name;
            AllProperties = entityType.GetProperties();
            //Load all the "primitive" entity properties
            var props = AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            //Filter the non stored properties
            this.BaseProperties = props.Where(p => !p.GetCustomAttributes<IgnoreAttribute>().Any()).Select(p => new PropertyMetadata(p));

            //Filter key properties
            this.KeyProperties = props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any()).Select(p => new PropertyMetadata(p));

            //Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
            this.IdentityProperty = identityProperty != null ? new PropertyMetadata(identityProperty) : null;
        }


        
        /// <summary>
        /// 查询第一个返回结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return GetSelect(predicate, true);
        }

        /// <summary>
        /// 查询全部结果集
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate)
        {
            return GetSelect(predicate, false);
        }

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SqlQuery GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("SELECT Count(1) FROM {0} With(Nolock)", TableName));
            return GetSqlQuery(predicate, builder);
        }

        /// <summary>
        /// 获取Delete查询对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SqlQuery GetDelete(Expression<Func<TEntity, bool>> predicate)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format("Delete FROM {0} ", TableName));
            return GetSqlQuery(predicate, builder);
        }

        /// <summary>
        /// 获取Update查询对象
        /// </summary>
        /// <param name="instantce"></param>
        /// <returns></returns>
        public SqlQuery GetUpdate(TEntity instance)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            var builder = new StringBuilder();
            builder.AppendFormat("UPDATE TOP (1) {0} SET ", TableName);
            var pro = BaseProperties.Where(e => !e.PropertyInfo.GetCustomAttributes<KeyAttribute>().Any());
            for (int i = 0; i < pro.Count(); i++)
            {
                var item = pro.ElementAt(i);
                builder.AppendFormat(" {0}=@{0}{1}", pro.ElementAt(i).ColumnName, i != pro.Count() - 1 ? "," : "").AppendLine();
                expando[item.ColumnName] = item.PropertyInfo.GetValue(instance);
            }
            expando[IdentityProperty.ColumnName] = IdentityProperty.PropertyInfo.GetValue(instance);
            builder.AppendFormat(" WHERE {0}=@{0}", IdentityProperty.ColumnName);
            return new SqlQuery(builder.ToString(), expando);
        }

        /// <summary>
        /// 获取Insert查询对象
        /// </summary>
        /// <param name="instantce"></param>
        /// <returns></returns>
        public SqlQuery GetInsert(TEntity instance)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            var cloumnbuilder = new StringBuilder();
            var valueBuilder = new StringBuilder();
            for (int i = 0; i < BaseProperties.Count(); i++)
            {
                var item = BaseProperties.ElementAt(i);
                var keyAttribute = item.PropertyInfo.GetCustomAttribute<KeyAttribute>();
                if (keyAttribute != null && keyAttribute.KeyType == KeyType.Identity)
                {
                    //是主键 且自增长则跳过
                    continue;
                }
                cloumnbuilder.AppendFormat(" {0},", item.ColumnName);
                valueBuilder.AppendFormat(" @{0},", item.ColumnName);
                if (keyAttribute != null && keyAttribute.KeyType == KeyType.Guid &&
                    string.IsNullOrEmpty(item.PropertyInfo.GetValue(instance).ToString()))
                {
                    expando[item.ColumnName] = Guid.NewGuid().ToString();
                }
                else
                {
                    expando[item.ColumnName] = item.PropertyInfo.GetValue(instance);
                }
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(" INSERT INTO {0} ({1})VALUES({2})  SELECT @@IDENTITY AS ReturnId", TableName, cloumnbuilder.ToString().TrimEnd(','), valueBuilder.ToString().TrimEnd(','));
            return new SqlQuery(builder.ToString(), expando);
        }

        /// <summary>
        /// 生产表达式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            ParameterExpression lambdaParam = Expression.Parameter(typeof(TEntity));

            BinaryExpression lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        /// <summary>
        /// Fill query properties
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private void FillQueryProperties(BinaryExpression body, ExpressionType linkingType, ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                string propertyName = BaseProperties.First(e => e.Name == ExpressionHelper.GetPropertyName(body)).ColumnName;
                object propertyValue = ExpressionHelper.GetValue(body.Right);
                string opr = ExpressionHelper.GetSqlOperator(body.NodeType);
                string link = ExpressionHelper.GetSqlOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Left), body.NodeType, ref queryProperties);
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Right), body.NodeType, ref queryProperties);
            }
        }

        private StringBuilder InitBuilderSelect(bool firstOnly)
        {
            var builder = new StringBuilder();
            var select = "SELECT ";

            if (firstOnly)
                select += "TOP 1 ";
            builder.Append(string.Format("{0} {1}", select, GetFieldsSelect(TableName, BaseProperties)));
            return builder;
        }

        private static string GetFieldsSelect(string tableName, IEnumerable<PropertyMetadata> properties)
        {
            Func<PropertyMetadata, string> projectionFunction = (p) => !string.IsNullOrEmpty(p.Alias)
                ? string.Format(" {0}.{1} AS {2} ", tableName, p.ColumnName, p.Name)
                : string.Format("{0}.{1} ", tableName, p.ColumnName);

            return string.Join(", ", properties.Select(projectionFunction));
        }

        /// <summary>
        /// 构建选择查询的表
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="firstOnly"></param>
        /// <returns></returns>
        private SqlQuery GetSelect(Expression<Func<TEntity, bool>> predicate, bool firstOnly)
        {
            var builder = InitBuilderSelect(firstOnly);
            builder.Append(string.Format(" FROM {0} WITH(NOLOCK)", TableName));
            return GetSqlQuery(predicate, builder);
        }

        /// <summary>
        /// 构建查询条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        private SqlQuery GetSqlQuery(Expression<Func<TEntity, bool>> predicate, StringBuilder builder)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            if (predicate != null)
            {
                // WHERE
                var queryProperties = new List<QueryParameter>();
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(predicate.Body), ExpressionType.Default,
                    ref queryProperties);

                builder.Append(" WHERE ");


                for (int i = 0; i < queryProperties.Count; i++)
                {
                    var item = queryProperties[i];

                    if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                    {
                        builder.Append(string.Format("{0} {1}.{2} {3} @{2} ", item.LinkingOperator, TableName, item.PropertyName,
                            item.QueryOperator));
                    }
                    else
                    {
                        builder.Append(string.Format("{0}.{1} {2} @{1} ", TableName, item.PropertyName, item.QueryOperator));
                    }

                    expando[item.PropertyName] = item.PropertyValue;
                }
            }
            return new SqlQuery(builder.ToString().TrimEnd(), expando);
        }
    }
}
