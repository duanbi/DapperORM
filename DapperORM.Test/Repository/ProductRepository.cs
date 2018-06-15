using DapperORM;
using DapperORM.Test.TableEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperORM.Test.Repository
{
    public class ProductRepository:Repository<Product, int>
    {

        public ProductRepository() : base("Demo")
        {

        }

        public List<Product> GetProductList()
        {
            var result = new List<Product>();
            var sql = @"SELECT  [Id] ,
                                    [Name] ,
                                    [Price] ,
                                    [CreateTime]
                            FROM    [dbo].[Product] WITH ( NOLOCK )
                            WHERE   Id = @id ";
            var paramter = new { id = 1 };
            //var result = ExcuteQuery<Product>(sql, paramter);

            using (var dbContext = new DbContext("Demo", DataBaseTypeEnum.Write))
            {
                result = dbContext.ExcuteQuery<Product>(sql, paramter).ToList();
            }
            return result;
        }

    }
}
