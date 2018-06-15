using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DapperORM.Test.Repository;
using DapperORM.Test.TableEntity;

namespace DapperORM.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            
        }

        [TestMethod]
        public void InsertTest()
        {
            ProductRepository ProductRepository = new ProductRepository();
            var id = ProductRepository.Insert(new TableEntity.Product()
            {
                Name = "DapperOrmTest3",
                Price = 12.30M,
                CreateTime = DateTime.Now

            });

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void UpdateTest()
        {
            ProductRepository ProductRepository = new ProductRepository();
            var id = ProductRepository.Update(new TableEntity.Product()
            {
                Id = 2,
                Name = "Dappe_1111",
                Price = 1,
                CreateTime = DateTime.Now

            });

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void FindTest()
        {
            ProductRepository productRepository = new ProductRepository();
            var flag = true;
            try
            {
                var result1 = productRepository.FindAll();
                var result2 = productRepository.FindAll(x => x.Id == 1);
                var result3 = productRepository.FindById(1);
                var result4 = productRepository.FirstOrDefault(x => x.CreateTime < DateTime.Now);
            }
            catch (Exception ex)
            {
                flag = false;
            }
            
            Assert.IsTrue(flag);
        }


        [TestMethod]
        public void ExcuteQueryTest()
        {
            ProductRepository productRepository = new ProductRepository();
            var result = productRepository.GetProductList();

            //Assert.IsTrue(flag);
        }

    }
}
