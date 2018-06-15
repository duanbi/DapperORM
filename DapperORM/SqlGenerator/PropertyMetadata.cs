using DapperORM.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM.SqlGenerator
{
    public class PropertyMetadata
    {
        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 属性别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        public string Name
        {
            get
            {
                return PropertyInfo.Name;
            }
        }
        /// <summary>
        /// 属性对应数据库的列名
        /// </summary>
        public string ColumnName
        {
            get
            {
                return string.IsNullOrEmpty(this.Alias) ? this.PropertyInfo.Name : this.Alias;
            }
        }
        /// <summary>
        /// 构造信息
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyMetadata(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            var alias = PropertyInfo.GetCustomAttribute<ColumnAttribute>();
            Alias = alias != null ? alias.ColumnName : string.Empty;
        }
    }
}
