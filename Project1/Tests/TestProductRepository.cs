using System;
using System.Linq;
using Xunit;
using Moq;

using StoreApp.Data;
using StoreApp.Repository;

namespace TestStoreApp
{
    public class TestProductRepository
    {
        [Fact]
        public async void GetsProductsAvailable()
        {
            var options = TestUtil.GetMemDbOptions("GetsProductsAvailable");

            using (var db = new StoreContext(options))
            {
                var locationId = Guid.Parse("B8B94F18-D101-4576-AF28-3CBF31EBD6B2");

                var productRepo = (IProduct) new ProductRepository(db);
                var locationRepo = (ILocation) new LocationRepository(db);
                var location = await locationRepo.GetById(locationId);
                var products = locationRepo.GetProductsAvailable(location);

                Assert.Equal(products.Count(), 9);

                var quantity = products
                              .Where(p => p.Item1.ProductId == Guid.Parse("C686DC98-FD97-4073-857F-17DBC48F7CA8"))
                              .Select(p => p.Item2)
                              .FirstOrDefault();
                
                Assert.Equal(quantity, 220);
            }
        }
    }
}
