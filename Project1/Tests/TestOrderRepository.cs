using Xunit;
using System;
using System.Linq;

using StoreApp.Data;
using StoreApp.Entity;
using StoreApp.Repository;

namespace TestStoreApp
{
    public class TestOrderRepository
    {
        [Fact]
        public async void PlacesValidOrder()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-PlacesOrder");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                TestUtil.AddToOrder(db, order, product, 5);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.PlaceOrder(user.UserId, order.OrderId);
                Assert.Equal(PlaceOrderResult.Ok, result);
            }
        }

        [Fact]
        public async void RejectsOrderWhenOutOfStock()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsOrderWhenOutofStock");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                TestUtil.AddToOrder(db, order, product, 50);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.PlaceOrder(user.UserId, order.OrderId);
                Assert.Equal(PlaceOrderResult.OutOfStock, result);
            }
        }

        [Fact]
        public async void RejectsEmptyOrders()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsEmptyOrders");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.PlaceOrder(user.UserId, order.OrderId);
                Assert.Equal(PlaceOrderResult.NoLineItems, result);
            }
        }

        [Fact]
        public async void RejectsOrdersWithInvalidIds()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsOrdersWithInvalidIds");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                TestUtil.AddToOrder(db, order, product, 50);
                TestUtil.AddToOrder(db, order, product2, 1);
                order.TimeSubmitted = DateTime.Now;

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);

                // Bad user id
                {
                    var result = await repo.PlaceOrder(Guid.NewGuid(), order.OrderId);
                    Assert.Equal(PlaceOrderResult.OrderNotFound, result);
                }

                // Bad order id
                {
                    var result = await repo.PlaceOrder(user.UserId, Guid.NewGuid());
                    Assert.Equal(PlaceOrderResult.OrderNotFound, result);
                }

                // Both bad ids
                {
                    var result = await repo.PlaceOrder(Guid.NewGuid(), Guid.NewGuid());
                    Assert.Equal(PlaceOrderResult.OrderNotFound, result);
                }
            }
        }

        [Fact]
        public void GetsSubmittedOrders()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-GetsSubmittedOrders");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                // A total of 6 items are in our order.
                TestUtil.AddToOrder(db, order, product, 5);
                TestUtil.AddToOrder(db, order, product2, 1);

                // This order should not appear in out results since it was not submitted.
                var orderNotSubmitted = TestUtil.NewOrder(db, user, location);
                TestUtil.AddToOrder(db, orderNotSubmitted, product2, 7);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var orders = repo.GetSubmittedOrders(user.UserId);
                var mappedOrders = orders
                    .Select(o => new {
                        OrderId = o.Item1.OrderId,
                        Quantity = o.Item2
                    });

                Assert.Single(orders);

                var mappedOrder = new {
                    OrderId = order.OrderId,
                    Quantity = 6
                };

                Assert.Contains(mappedOrder, mappedOrders);
            }
        }

        [Fact]
        public void GetsOrderLines()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-GetsOrderLines");

            Order order;
            User user;
            OrderLineItem line1;
            OrderLineItem line2;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                line1 = TestUtil.AddToOrder(db, order, product, 5);
                line2 = TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var orderLines = repo.GetOrderLines(user.UserId, order.OrderId);
                var mappedOrderLines = orderLines
                    .Select(ol => new {
                        OrderLineId = ol.OrderLineItemId,
                        Quantity = ol.Quantity,
                    });
                
                Assert.Equal(2, mappedOrderLines.Count());

                {
                    var mappedLine = new {
                        OrderLineId = line1.OrderLineItemId,
                        Quantity = line1.Quantity,
                    };
                    Assert.Contains(mappedLine, mappedOrderLines);
                }

                {
                    var mappedLine = new {
                        OrderLineId = line2.OrderLineItemId,
                        Quantity = line2.Quantity,
                    };
                    Assert.Contains(mappedLine, mappedOrderLines);
                }
            }
        }

        [Fact]
        public void RejectsOrderLinesWithBadIds()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsOrderLinesWithBadIds");

            Order order;
            User user;
            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                TestUtil.AddToOrder(db, order, product, 5);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);

                // Bad user id
                {
                    var orderLines = repo.GetOrderLines(Guid.NewGuid(), order.OrderId);
                    Assert.Empty(orderLines);
                }

                // Bad order id
                {
                    var orderLines = repo.GetOrderLines(user.UserId, Guid.NewGuid());
                    Assert.Empty(orderLines);
                }

                // Bad order id and user id
                {
                    var orderLines = repo.GetOrderLines(Guid.NewGuid(), Guid.NewGuid());
                    Assert.Empty(orderLines);
                }
            }
        }

        [Fact]
        public async void SetsLineItemQuantity()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-SetsLineItemQuantity");

            Order order;
            User user;
            Product product;
            OrderLineItem line1;
            OrderLineItem line2;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                line1 = TestUtil.AddToOrder(db, order, product, 5);
                line2 = TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            var newQuantity = 7;
            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.SetLineItemQuantity(user.UserId, order, product.ProductId, newQuantity);
                Assert.Equal(SetLineItemQuantityResult.Ok, result);
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var orderLines = repo.GetOrderLines(user.UserId, order.OrderId);
                var mappedOrderLines = orderLines
                    .Select(ol => new {
                        OrderLineId = ol.OrderLineItemId,
                        Quantity = ol.Quantity,
                    });
                {
                    var mappedLine = new {
                        OrderLineId = line1.OrderLineItemId,
                        Quantity = newQuantity,
                    };
                    Assert.Contains(mappedLine, mappedOrderLines);
                }
            }
        }

        [Fact]
        public async void RejectsLineItemAdjustmentWhenItExceedsStock()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsLineItemAdjustmentWhenItExceedsStock");

            Order order;
            User user;
            Product product;
            OrderLineItem line1;

            var originalQuantity = 5;
            var newQuantity = 999;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                line1 = TestUtil.AddToOrder(db, order, product, originalQuantity);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.SetLineItemQuantity(user.UserId, order, product.ProductId, newQuantity);
                Assert.Equal(SetLineItemQuantityResult.ExceedsStock, result);
            }

            // make sure that the line item is unchanged.
            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var orderLines = repo.GetOrderLines(user.UserId, order.OrderId);
                var mappedOrderLines = orderLines
                    .Select(ol => new {
                        OrderLineId = ol.OrderLineItemId,
                        Quantity = ol.Quantity,
                    });
                {
                    var mappedLine = new {
                        OrderLineId = line1.OrderLineItemId,
                        Quantity = originalQuantity,
                    };
                    Assert.Contains(mappedLine, mappedOrderLines);
                }
            }
        }

        [Fact]
        public async void RejectsLineItemAdjustmentWithBadIds()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsLineItemAdjustmentWithBadIds");

            Order order;
            User user;
            Product product;
            OrderLineItem line1;

            var originalQuantity = 5;
            var newQuantity = 999;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                line1 = TestUtil.AddToOrder(db, order, product, originalQuantity);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);

                // Bad product id
                {
                    var result = await repo.SetLineItemQuantity(user.UserId, order, Guid.NewGuid(), newQuantity);
                    Assert.Equal(SetLineItemQuantityResult.ProductMissing, result);
                }

                // Bad user id
                {
                    var result = await repo.SetLineItemQuantity(Guid.NewGuid(), order, product.ProductId, newQuantity);
                    Assert.Equal(SetLineItemQuantityResult.ProductMissing, result);
                }

                // missing order
                {
                    var result = await repo.SetLineItemQuantity(user.UserId, null, product.ProductId, newQuantity);
                    Assert.Equal(SetLineItemQuantityResult.OrderMissing, result);
                }
            }
        }

        // This one is broken for some reason.
        public async void AddsLineItem()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-AddsLineItem");

            Order order;
            User user;
            Product product;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.AddLineItem(user.UserId, order, product, 5);
                Assert.Equal(AddLineItemResult.Ok, result);

                var orderLines = repo.GetOrderLines(user.UserId, order.OrderId);
                Assert.Single(orderLines);
            }
        }

        [Fact]
        public async void RejectsNewLineItemWhenItExceedsStock()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsNewLineItemWhenItExceedsStock");

            Order order;
            User user;
            Product product;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var result = await repo.AddLineItem(user.UserId, order, product, 999);
                Assert.Equal(AddLineItemResult.ExceedsStock, result);

                var orderLines = repo.GetOrderLines(user.UserId, order.OrderId);
                Assert.Empty(orderLines);
            }
        }

        [Fact]
        public async void RejectsNewLineItemWithBadIds()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-RejectsNewLineItemWithBadIds");

            Order order;
            Product product;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                // Bad order
                {
                    var result = await repo.AddLineItem(user.UserId, null, product, 3);
                    Assert.Equal(AddLineItemResult.OrderMissing, result);
                }
                // Bad product
                {
                    var result = await repo.AddLineItem(user.UserId, order, null, 3);
                    Assert.Equal(AddLineItemResult.ProductMissing, result);
                }
            }
        }

        [Fact]
        public async void DeletesLineItem()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-DeletesLineItem");

            Order order;
            User user;
            Product product;
            OrderLineItem line1;
            OrderLineItem line2;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                line1 = TestUtil.AddToOrder(db, order, product, 5);
                line2 = TestUtil.AddToOrder(db, order, product2, 3);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                // Normal
                {
                    var result = await repo.DeleteLineItem(user.UserId, order, line1.Product.ProductId);
                    Assert.True(result);
                }
                // Bad user id
                {
                    var result = await repo.DeleteLineItem(Guid.NewGuid(), order, line1.Product.ProductId);
                    Assert.False(result);
                }
                // Bad order id
                {
                    var result = await repo.DeleteLineItem(user.UserId, null, line1.Product.ProductId);
                    Assert.False(result);
                }
                // Bad product id
                {
                    var result = await repo.DeleteLineItem(user.UserId, order, Guid.NewGuid());
                    Assert.False(result);
                }
            }
        }


        [Fact]
        public async void GetsOrderById()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-GetsOrderById");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                TestUtil.AddToOrder(db, order, product, 5);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var orderResult = await repo.GetOrderById(user.UserId, order.OrderId);
                Assert.Equal(order.OrderId, orderResult.OrderId);
            }
        }

        [Fact]
        public async void DoesNotReturnOrderWithWrongUserId()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-DoesNotReturnOrderWithWrongUserId");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                TestUtil.AddToOrder(db, order, product, 5);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                var orderResult = await repo.GetOrderById(Guid.NewGuid(), order.OrderId);
                Assert.Null(orderResult);
            }
        }

        [Fact]
        public async void ChecksIfOrderExists()
        {
            var options = TestUtil.GetMemDbOptions("TestOrderRepository-ChecksIfOrderExists");

            Order order;
            User user;

            using (var db = new StoreContext(options))
            {
                user = TestUtil.NewUser(db);
                var location = TestUtil.NewLocation(db);
                var product = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product, 10);
                var product2 = TestUtil.NewProduct(db, 10);
                TestUtil.AddToInventory(db, location, product2, 10);

                order = TestUtil.NewOrder(db, user, location);
                order.TimeSubmitted = DateTime.Now;
                TestUtil.AddToOrder(db, order, product, 5);
                TestUtil.AddToOrder(db, order, product2, 1);

                db.SaveChanges();
            }

            using (var db = new StoreContext(options))
            {
                var repo = (IOrder) new OrderRepository(db);
                // OK
                {
                    var orderResult = await repo.Exists(order.OrderId);
                    Assert.True(orderResult);
                }
                // Nonexisting order
                {
                    var orderResult = await repo.Exists(Guid.NewGuid());
                    Assert.False(orderResult);
                }
            }
        }
  
    }
}