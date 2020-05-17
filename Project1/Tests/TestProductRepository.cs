using Xunit;

using StoreApp.Data;
using StoreApp.Entity;
using StoreApp.Repository;

namespace TestStoreApp
{
    public class TestProductRepository
    {
        [Fact]
        public async void GetsProductById()
        {
            var options = TestUtil.GetMemDbOptions("TestProductRepository-GetsProductById");

            Product product;

            using (var db = new StoreContext(options))
            {
                product = TestUtil.NewProduct(db, 10);
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IProduct) new ProductRepository(db);
                var fromId = await repo.GetProductById(product.ProductId);
                Assert.Equal(product.ProductId, fromId.ProductId);
            }
        }
    }
}
