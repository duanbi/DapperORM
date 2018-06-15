using DapperORM;
using DapperORM.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM.Test.TableEntity
{
    [Table("Product")]
    public class Product : IEntity<int>
    {
        [Key(KeyType.Identity), Column("Id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public DateTime CreateTime { get; set; }

        [Ignore]
        public string Title { get; set; }
    }
}
