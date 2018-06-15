
namespace DapperORM.Attribute
{
    public class TableAttribute : System.Attribute
    {
        public string TableName { get; set; }

        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
