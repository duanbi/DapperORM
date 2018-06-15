using System.ComponentModel;

namespace DapperORM.Attribute
{
    public enum KeyType
    {
        NotAKey,
        [Description("自增长")]
        Identity,
        [Description("Guid类型")]
        Guid,
        [Description("未分配")]
        Assigned
    }
}
