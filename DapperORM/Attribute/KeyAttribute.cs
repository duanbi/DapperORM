
namespace DapperORM.Attribute
{
    public class KeyAttribute : System.Attribute
    {
        public KeyType KeyType { get; private set; }

        public KeyAttribute(KeyType type)
        {
            this.KeyType = type;
        }
    }
}
