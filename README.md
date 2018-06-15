# DapperORM

在dapper 的基础上简化操作。

单表的增删改查都可以采用labuda 表达式进行。

具体使用方法：

前提：
在App_Data中添加
connectionStrings.config


继承：Repository
如：public class ProductRepository:Repository<Product, int>

添加一个构造器：base中添加库名

 public ProductRepository() : base("Demo")
 {
 }

新增：
ProductRepository ProductRepository = new ProductRepository();
var id = ProductRepository.Insert(new TableEntity.Product()
 {
    Name = "DapperOrmTest3",
    Price = 12.30M,
    CreateTime = DateTime.Now

});


修改：
ProductRepository ProductRepository = new ProductRepository();
var id = ProductRepository.Update(new TableEntity.Product()
{
    Id = 2,
    Name = "Dappe_1111",
    Price = 1,
    CreateTime = DateTime.Now

});

查询：
ProductRepository productRepository = new ProductRepository();
var result1 = productRepository.FindAll();
var result2 = productRepository.FindAll(x => x.Id == 1);
var result3 = productRepository.FindById(1);
var result4 = productRepository.FirstOrDefault(x => x.CreateTime < DateTime.Now);


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


