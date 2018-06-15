
namespace DapperORM.Attribute
{
    public class ColumnAttribute : System.Attribute
    {
        public string ColumnName { get; set; }

        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
