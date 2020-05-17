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

        [Fact]
        public void GetsOrders()
        {
            var options = TestUtil.GetMemDbOptions("TestLocationRepository-GetsOrders");

            Location location;
            Order order;

            using (var db = new StoreContext(options))
            {
                var user = TestUtil.NewUser(db);
                location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                TestUtil.AddToOrder(db, order, product, 5);

                // Second order was not submitted, so should not be in query.
                var untrackedOrder = TestUtil.NewOrder(db, user, location);
                TestUtil.AddToOrder(db, untrackedOrder, product, 2);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var orders = repo.GetOrders(location.LocationId);
                Assert.Single(orders);
                Assert.Equal(order.OrderId, orders.ElementAt(0).Item1.OrderId);

            }
        }

        [Fact]
        public void GetsOrderLineItems()
        {
            var options = TestUtil.GetMemDbOptions("TestLocationRepository-GetsOrderLineItems");

            Order order;
            OrderLineItem orderLine1;
            OrderLineItem orderLine2;

            using (var db = new StoreContext(options))
            {
                var user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                orderLine1 = TestUtil.AddToOrder(db, order, product, 5);
                orderLine2 = TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var orderLines = repo.GetOrderLineItems(order.OrderId);

                Assert.Equal(2, orderLines.Count());

                var mappedOrderLines = orderLines
                    .Select(ol => new {
                        OrderLineId = ol.OrderLineItemId,
                    });

                {
                    var mappedOrderLine = new {
                        OrderLineId = orderLine1.OrderLineItemId,
                    };
                    Assert.Contains(mappedOrderLine, mappedOrderLines);
                }
                {
                    var mappedOrderLine = new {
                        OrderLineId = orderLine1.OrderLineItemId,
                    };
                    Assert.Contains(mappedOrderLine, mappedOrderLines);
                }

            }
        }
    }
}
