using System.Linq;
using System;
using Xunit;
using Moq;

using StoreApp.Data;
using StoreApp.Repository;
using StoreApp.Entity;

namespace TestStoreApp
{
    public class TestLocationRepository
    {
        [Fact]
        public void GetsById()
        {
            var options = TestUtil.GetMemDbOptions("TestLocationRepository-GetsById");

            Location location;
            using (var db = new StoreContext(options))
            {
                location = TestUtil.NewLocation(db);
            }

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var locationQueryResult = repo.GetById(location.LocationId);
                Assert.NotNull(location);
            }
        }

        [Fact]
        public void GetsMostStockedLocation()
        {
            var options = TestUtil.GetMemDbOptions("TestLocationRepository-GetsMostStockedLocation");

            Location location;
            using (var db = new StoreContext(options))
            {
                location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 999999);
            }

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var mostStockedLocation = repo.GetMostStocked();
                Assert.Equal(location.LocationId, mostStockedLocation.LocationId);
            }
        }

        [Fact]
        public void GetsProductsAvailable()
        {
            var options = TestUtil.GetMemDbOptions("TestLocationRepository-GetsProductsAvailable");

            Location location;
            Product product1;
            Product product2;
            Product product3;

            var product1Qty = 10;
            var product2Qty = 5;
            var product3Qty = 0;

            using (var db = new StoreContext(options))
            {
                location = TestUtil.NewLocation(db);
                product1 = TestUtil.NewProduct(db, 10);
                product2 = TestUtil.NewProduct(db, 10);
                product3 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product1, product1Qty);
                TestUtil.AddToInventory(db, location, product2, product2Qty);
                TestUtil.AddToInventory(db, location, product3, product3Qty);
            }

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var available = repo.GetProductsAvailable(location);

                Assert.Equal(2, available.Count());

                var availableMapped = available
                    .Select(p => new {
                        ProductId = p.Item1.ProductId,
                        Quantity = p.Item2
                    });

                {
                    var productExpected = new {
                        ProductId = product1.ProductId,
                        Quantity = product1Qty,
                    };

                    Assert.Contains(productExpected, availableMapped);
                }

                {
                    var productExpected = new {
                        ProductId = product2.ProductId,
                        Quantity = product2Qty,
                    };

                    Assert.Contains(productExpected, availableMapped);
                }
            }
        }

        [Fact]
        public async void GetsStock()
        {
            var options = TestUtil.GetMemDbOptions("TestLocationRepository-GetsStock");

            Location location;
            Product product;
            var stocked = 12;

            using (var db = new StoreContext(options))
            {
                location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, stocked);
            }

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var quantityInStock = await repo.GetStock(location, product);
                Assert.Equal(stocked, quantityInStock);
            }
        }
    }
}
