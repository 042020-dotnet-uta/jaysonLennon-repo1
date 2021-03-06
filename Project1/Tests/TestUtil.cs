using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

using StoreApp.Data;
using StoreApp.Entity;

namespace TestStoreApp
{
    public class TestUtil
    {
        public static DbContextOptions<StoreContext> GetMemDbOptions(string dbName)
        {
            var sqlite = new SqliteConnection("Filename=:memory:");
            sqlite.Open();
            var options = new DbContextOptionsBuilder<StoreContext>()
                       //.UseInMemoryDatabase(databaseName: dbName)
                       .UseSqlite(sqlite)
                       .Options;

            using (var db = new StoreContext(options))
            {
                db.Database.Migrate();
                PopulateTestData(db);
            }

            return options;
        }

        public static User NewUser(StoreContext db) {
            var user = new User();
            user.FirstName = Guid.NewGuid().ToString();
            user.LastName = Guid.NewGuid().ToString();
            user.Login = Guid.NewGuid().ToString();
            user.Role = Role.Customer;

            db.Add(user);
            db.SaveChanges();

            return user;
        }

        public static Location NewLocation(StoreContext db) {
            var location = new Location(Guid.NewGuid().ToString());

            db.Add(location);
            db.SaveChanges();
            return location;
        }

        public static Product NewProduct(StoreContext db, double price) {
            var product = new Product(Guid.NewGuid().ToString(), price);

            db.Add(product);
            db.SaveChanges();
            return product;
        }

        public static LocationInventory AddToInventory(StoreContext db, Location location, Product product, int quantity) {
            var inventory = new LocationInventory(product, location, quantity);

            db.Add(inventory);
            db.SaveChanges();
            return inventory;
        }

        public static Order NewOrder(StoreContext db, User user, Location location) {
            var order = new Order();
            order.Location = location;
            order.User = user;
            order.TimeCreated = DateTime.Now;

            db.Add(order);
            db.SaveChanges();
            return order;
        }

        public static OrderLineItem AddToOrder(StoreContext db, Order order, Product product, int quantity) {
            var lineItem = new OrderLineItem(order, product);
            lineItem.Quantity = quantity; 

            db.Add(lineItem);
            db.SaveChanges();
            return lineItem;
        }

        public static void PopulateTestData(StoreContext db)
        {
            try
            {
                using (var trans = db.Database.BeginTransaction())
                {

                    db.Database.ExecuteSqlRaw("INSERT INTO Locations (LocationId, Name) VALUES ('B8B94F18-D101-4576-AF28-3CBF31EBD6B2', 'Choco Castle')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Locations (LocationId, Name) VALUES ('BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', 'Sweet Tooth')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Locations (LocationId, Name) VALUES ('DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', 'Candy Land')");

                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('59A3989B-8D6E-4B11-B360-52C4F94159B9', 'Gema', 'Halliday','721-555-2195', '123', 'gema@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('33CC1DEC-2B88-472D-8798-1D37D8823F0E', 'Abdul', 'Seneca', '616-555-2521', '123', 'abdul@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('A53B4693-D6AC-4D84-BB0A-9340D21D4779', 'Marx', 'Eckley', '312-555-0972', '123', 'marx@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('AE344F9D-29C6-4412-885B-F589873447ED', 'Leanna', 'Dibiase', '453-555-3116', '123', 'leanna@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('01A326C5-A5F5-463D-99D5-EBB91ADAD452', 'Dan', 'Iles', '525-555-6283', '123', 'dan@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('A23A4DDE-A0B7-4AA7-BC1A-BEDC4D965D26', 'Melissa', 'Alfonso', '697-555-6791', '123', 'melissa@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('6118C5C8-2F44-4634-8370-0C5623438BB7', 'Olin', 'Gillooly', '879-555-5982', '123', 'olin@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('653083A2-39A5-4D69-A615-3505710F3BB2', 'Tamica', 'Higgs', '669-555-9601', '123', 'tamica@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('E0E0B819-2D8D-484F-9013-AEB054F227B1', 'Sherry', 'Roa', '211-555-9292', '123', 'sherry@example.com', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, FirstName, LastName, PhoneNumber, Password, Login, Role) VALUES ('F91FA2F2-0E35-4476-BA81-DA325CBE6C32', 'Sara', 'Primm', '284-555-3831', '123', 'sara@example.com', 'Customer')");

                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('B52C8E10-A72B-4B74-80A3-7367A108BB46', '1', 'Lollipop', 'lollipop.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('34D34E7B-5911-4805-AF27-C0EE12C1EBA7', '2', 'Milk Chocolate Bar', 'milk-chocolate-bar.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('C2F49C8C-1E38-444C-9454-69D19C42FCB7', '2', 'Chocolate Bar with Almonds', 'chocolate-bar-with-almonds.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('681B47F6-A668-4465-B97D-001AE2C14B6F', '2', 'Dark Chocolate Bar', 'dark-chocolate-bar.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('8EE96987-4BD9-4BF4-9A61-08053B4ED64D', '3', 'Bag of Gummy Bears', 'bag-gummy-bears.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('67F25997-F989-41F3-931C-F54FE8172EC8', '4', 'Bag of Sour Gummies', 'bag-sour-gummies.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('C686DC98-FD97-4073-857F-17DBC48F7CA8', '3', 'Rock Candy', 'rock-candy.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('BCCCB075-AA60-443C-A098-820E3B3AAD65', '2', 'Toffee', 'toffee.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('6616320F-142A-41D6-B673-405779CAC6AB', '1', 'Giant Gumball', 'giant-gumball.jpg')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Products (ProductId, Price, Name, ImageName) VALUES ('3C7076EE-4887-4481-8E37-0D6B6BBE2D86', '1', 'Bag of Jelly Beans', 'bag-jelly-beans.jpg')");

                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('F2F69D85-D42D-48AD-84F0-A242C2490E67', 'B52C8E10-A72B-4B74-80A3-7367A108BB46', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '200')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('6C4C86E0-D692-43A7-B74F-31463BFFEC04', '34D34E7B-5911-4805-AF27-C0EE12C1EBA7', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '100')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('A08CDECA-C860-468B-8E12-2812D11C54F5', 'C2F49C8C-1E38-444C-9454-69D19C42FCB7', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '500')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('AFF3EE78-E6EC-427A-88BF-54BFD3A213DA', '681B47F6-A668-4465-B97D-001AE2C14B6F', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '400')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('76C43AAE-361F-4E35-9E90-6F96C8E0E18D', '8EE96987-4BD9-4BF4-9A61-08053B4ED64D', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '700')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('9254ACF8-6F45-4E3C-9B1D-345FFC56AFD2', '67F25997-F989-41F3-931C-F54FE8172EC8', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '300')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('E1AB6F82-102D-41C4-8ABC-3364A4991C1A', 'C686DC98-FD97-4073-857F-17DBC48F7CA8', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '220')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('07F17ADF-3EE2-49BB-B0C5-AC4A06EB7CF7', 'BCCCB075-AA60-443C-A098-820E3B3AAD65', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '190')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('E2E26F8C-2305-46A7-B15C-711EACF27746', '3C7076EE-4887-4481-8E37-0D6B6BBE2D86', 'B8B94F18-D101-4576-AF28-3CBF31EBD6B2', '440')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('25AD245A-546A-4CF2-8572-CFD61EA2774C', 'B52C8E10-A72B-4B74-80A3-7367A108BB46', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '550')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('BF88AE40-A339-4128-99C7-2C557B428B79', '34D34E7B-5911-4805-AF27-C0EE12C1EBA7', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '120')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('7903DBBF-5676-45FE-BD9C-86E55BB3D40D', 'C2F49C8C-1E38-444C-9454-69D19C42FCB7', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '100')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('679A204D-8F1B-4C8A-90FA-5C654FFC9616', '681B47F6-A668-4465-B97D-001AE2C14B6F', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '400')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('A86DFEA1-6C11-4481-BA5E-AD2CFACC8BF0', '8EE96987-4BD9-4BF4-9A61-08053B4ED64D', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '220')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('3BFCB4DB-2D35-49C2-9014-93E8C1DA7A18', '67F25997-F989-41F3-931C-F54FE8172EC8', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '670')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('DB22E0EC-C535-4E91-AABE-AF8BD2812CC1', 'C686DC98-FD97-4073-857F-17DBC48F7CA8', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '770')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('CDD2337F-9CAA-4D06-B34C-E6FB2889EBFA', 'BCCCB075-AA60-443C-A098-820E3B3AAD65', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '880')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('DFBBD5B9-F0E9-4E85-887F-F2E5F11DFF09', '6616320F-142A-41D6-B673-405779CAC6AB', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '400')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('C246B2DA-F9CB-43EA-8EAB-5FB1B38C7FF1', '3C7076EE-4887-4481-8E37-0D6B6BBE2D86', 'BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949', '320')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('1F189DE2-304E-4834-BF21-1503FE46EF41', 'B52C8E10-A72B-4B74-80A3-7367A108BB46', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '600')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('A19C268B-C65C-4A97-9B05-CDB2E6FF077B', '34D34E7B-5911-4805-AF27-C0EE12C1EBA7', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '300')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('CDB0801B-BE91-4900-8B65-6164DD0116F4', 'C2F49C8C-1E38-444C-9454-69D19C42FCB7', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '250')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('16E19FDF-9C17-4F00-BF3A-916A83D88792', '681B47F6-A668-4465-B97D-001AE2C14B6F', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '280')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('34D901B7-C46A-4309-B936-BBF2F00A5706', '8EE96987-4BD9-4BF4-9A61-08053B4ED64D', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '390')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('40BD668A-CBDC-43D3-B62E-EFFB07ED7E2B', '67F25997-F989-41F3-931C-F54FE8172EC8', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '320')");
                    db.Database.ExecuteSqlRaw("INSERT INTO LocationInventories (LocationInventoryId, ProductId, LocationId, Quantity) VALUES ('21BA560A-2517-46CB-AD95-97192CE17F1F', 'C686DC98-FD97-4073-857F-17DBC48F7CA8', 'DEA1BDEA-FB74-4372-A9D1-03BF26C8804D', '330')");

                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('37be6c2e-e61d-4e96-abf1-386169e00868', '401-873-4264', 'Tristan', 'Peltz', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5eda20eb-96ed-403f-ad2f-704234fbcaad', '392-506-3918', 'Junior', 'Kittleson', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1bb5372c-308d-4285-91c3-bd5ec49e7163', '182-524-5636', 'Adrianne', 'Mckissack', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a34260e5-838b-45ea-a694-614ba215be20', '036-038-6607', 'Reiko', 'Letson', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('11494f1e-d903-4691-a4df-0d67fe1dc76c', '061-079-7562', 'Duncan', 'Grosse', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e00a54d8-86d3-4e6e-9ea9-c05c4cbf9dd6', '462-019-8505', 'Zena', 'Lavery', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d76799c8-1f15-4371-a2cc-979f04670aaf', '720-900-0655', 'Carmela', 'Coryell', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('146fda38-e278-4dcd-90bb-f3eb05c81aa4', '549-118-5700', 'Shila', 'Stumpf', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('07e5685b-ddd4-4f0c-aa7e-66026f6da723', '792-660-5472', 'Renata', 'Mcelwee', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f1f9db73-8429-4273-8b46-1ac9e6551180', '981-120-9569', 'Nelida', 'Cluck', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2728ea08-4a67-4d54-bfda-1dcfa2cc3e74', '610-747-3891', 'Cara', 'Pancake', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('179475fb-251d-4c08-b614-05b141f3db80', '528-102-1958', 'Niesha', 'Segui', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('afe927bd-20b7-49e8-98ec-c18aba964f01', '122-132-3563', 'Tawana', 'Mei', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ec691c4d-34de-4df1-a3e5-4673e61ef277', '853-764-1239', 'Jocelyn', 'Blocker', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('30c56623-fc26-4e76-aae3-6fc122521445', '013-490-1704', 'Jeanine', 'Demo', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3ea029c2-7115-4b02-9bcb-f7d75c9051da', '351-960-4422', 'Maud', 'Lattea', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('88e26010-b2c6-4ef6-8016-8a721e54dcac', '239-799-8321', 'Toney', 'Eiler', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c1adc808-558f-488a-8907-60bd3856a407', '886-149-0511', 'Ayana', 'Bitton', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5cf791b7-133f-48b1-9006-b92a267af3ff', '498-532-6907', 'Takako', 'Jorgensen', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e758e289-43ad-44a9-acbb-de3a88be5cda', '352-313-9079', 'Eve', 'Gunn', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('93d49143-df95-4f8f-9d21-7c5f508d8b01', '582-737-0728', 'Luana', 'Sipos', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('144e3a95-b7e1-45d6-8842-44a87cd1cf7b', '868-347-5508', 'Vivan', 'Salamone', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f1a48ad4-fea1-4ccd-ae63-bf84fbb64fce', '525-054-6095', 'Cayla', 'Best', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b53b8eff-b3cd-41f2-8ed6-0fe9d44c2dc1', '409-651-8298', 'Maxine', 'Dryer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('208b831d-6271-4d6b-a1a8-5193d640a352', '437-596-1570', 'Marilee', 'Rowden', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('37d03814-d03c-4606-8ed7-7a9a2f8bd050', '196-079-5339', 'Janyce', 'Filip', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('37498b24-7014-4042-9437-4e29d1b143df', '898-080-8178', 'Sharen', 'Fickel', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1dc92922-4b32-41df-a256-0c8ff7a11afb', '207-226-0061', 'Williemae', 'Rohe', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ef03ac83-5584-4d44-b2ab-5870128d687d', '019-028-9130', 'Rufus', 'Wall', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8f69726c-57dc-4e01-abac-8504d5c523e3', '666-925-2046', 'Jaunita', 'Ramsden', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f021f317-cf51-47e4-a3b8-677a5f9b287d', '942-150-5245', 'Kori', 'Laman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('59fc1694-0894-402e-b132-1c890391e4a6', '602-020-9440', 'Jerrod', 'Shur', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b10d2ced-acd4-4319-9475-fa86cc2fd9ff', '295-912-5600', 'Brittaney', 'Likes', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('dccbf4c0-0765-406d-b62c-3f7fa7c850f4', '482-704-1190', 'Justin', 'Roderick', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('283de789-3316-4670-9b32-ca070abcd103', '397-732-3605', 'Deana', 'Lemonds', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e0837d7f-a263-4475-b9d2-cc70819aa61f', '690-254-3643', 'Marvis', 'Eley', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2d1358c7-1267-4789-93a7-b80b99d35ba9', '426-412-0898', 'Logan', 'Mcgaughy', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7b86115f-de2d-49eb-b2b2-7d794eb43bf3', '104-377-1757', 'Lizzette', 'Grand', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7a4da7c7-6021-4d0c-af96-5ea73296d130', '724-096-7705', 'Felicia', 'Mcroy', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ab3e6acd-5de8-4d5b-aff9-8234e0459e8d', '662-851-7099', 'Towanda', 'Pichardo', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8f6dc156-5e94-4d00-97f5-00fff0ff5814', '069-379-2201', 'Viki', 'Folden', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('63b0249e-c774-415f-9573-ea9767d40b32', '868-978-8433', 'Zola', 'Cruse', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('71af4c4c-ede4-411f-8bb3-a9338c51a67d', '111-551-9548', 'Rona', 'Stayton', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5b82592e-f1f1-411c-8a67-5036cd7af24d', '816-064-7647', 'Annika', 'Urbano', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b731f796-9105-4e65-9932-16e50f31fd89', '003-459-7951', 'Paola', 'Yeatman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('809b8bf7-415b-4054-83bc-ead0539c94d8', '361-728-2219', 'Benito', 'Tesar', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0ede5f34-56d7-4b2a-a7c4-7bd51f450b7f', '535-101-5835', 'Randal', 'Dittman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('fb289720-d7f0-4654-a440-818830bb8b9c', '047-325-9940', 'Reyes', 'Schlemmer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ee7677c4-e1cc-446e-870e-d13385880aa0', '922-065-9226', 'Lyla', 'Nez', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('eaea4616-a2f0-4c91-86b4-608195c38ff6', '058-329-9872', 'Antionette', 'Quinteros', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8173d056-2667-4c2e-86d0-6de562448e73', '249-806-0231', 'Darcie', 'Meese', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('67067c2d-df8d-4096-a399-68ef1e4cd7c1', '664-609-4364', 'Grisel', 'Bernardini', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1640166a-6fcc-4a05-bbe1-d485d371c2fe', '408-517-5320', 'Cathi', 'Chaidez', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('49845878-fdbe-494f-a637-3dde2d016067', '442-128-4152', 'Francie', 'Dudash', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ed158307-ad22-4845-8882-f8e83fd8490f', '957-602-5602', 'Linn', 'Faler', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('17690fec-84f8-4c2c-b4f3-88b02aa80e9c', '949-271-7478', 'Dorris', 'Haner', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('4ea5d429-d1c1-4828-b8e2-4da958ced48f', '955-057-9286', 'Jazmine', 'Stoneking', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e0f4512e-2cca-44fe-bbbf-c67e00f818f4', '264-636-4935', 'Lakeesha', 'Taubman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('4dd979dd-c26a-4c54-b30f-4ae2d347ba6b', '471-987-5261', 'Terrell', 'Michaelsen', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('810dace6-ec85-4278-b68e-24c5580ebcde', '424-403-4926', 'Rosina', 'Neary', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8d35d65e-1505-41ce-b822-654fda5e15b8', '506-923-5147', 'Breana', 'Linney', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f998deeb-9b0d-4e23-8839-e1b89b09e0df', '737-670-0609', 'Mellissa', 'Stemple', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('be7aafc7-65c8-462f-8605-f6853decf0ae', '410-383-7579', 'Paulita', 'Gains', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('39923f5f-3899-42a8-8988-cba22b5af205', '164-131-6473', 'Sebastian', 'Ronan', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1e2973cc-4b5b-4bb7-8753-381a2a7ee0f3', '520-495-8754', 'Mitzi', 'Wieland', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c2392e60-7bbb-4bfc-a5b3-cffdd3f6f617', '295-641-8552', 'Shoshana', 'Bellis', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('80317ffe-f214-4b4e-a32a-6f924a73915f', '389-345-3533', 'Ninfa', 'Wheelwright', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('66473027-a1bd-4e65-80da-22f74d08ef88', '347-513-7092', 'Lyndon', 'Shock', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c30ac5c4-2a58-4dfb-a360-588ad05f7957', '011-872-7965', 'Marybeth', 'Bash', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b462a9dc-7998-4931-a804-4a3b529fdb10', '123-977-0427', 'Brett', 'Restrepo', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c26fc04b-3a49-44f6-bb17-29c1cf7ec81b', '192-771-2788', 'Octavia', 'Comerford', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b61167eb-5738-44cd-a751-ed9c6cab62a8', '207-333-6604', 'Cecelia', 'Colburn', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('12b30637-1efb-476d-989a-65426c5ef367', '339-862-8671', 'Linnea', 'Corner', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('22dd08a9-f2a2-4e9d-a2cb-64521bca3445', '348-925-0738', 'Azzie', 'Devens', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2fab9310-1b26-4079-9fb4-388a2e5af42b', '021-976-5681', 'Ivan', 'Sen', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1fe69be4-cd00-4bf7-9c13-49f2964c35ad', '210-769-6943', 'Chia', 'Doverspike', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e13aa4e5-2b71-4da0-86d1-9e812b9a3287', '526-033-5611', 'Shawanna', 'Uhlman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a1dc24eb-0357-407b-8647-d4bb601f3e51', '789-174-6390', 'Marty', 'Robards', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('af95a8aa-e984-4dd6-9f66-dc7374a6b8dd', '406-569-2260', 'Aurelia', 'Eggers', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d329055a-639f-4616-9f6f-29f7bc3d75d9', '447-123-4507', 'Sammy', 'Attaway', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('01b6569e-fda2-4dec-a378-122e99d29d91', '857-722-1449', 'Talitha', 'Thornell', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('aa8580ba-0788-4589-a0a6-c93c18288a68', '820-106-7661', 'Rueben', 'Mcmains', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('dfa89bbc-2f14-42f1-aac7-a13f06189d90', '935-136-9292', 'Tajuana', 'Fye', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('94a22694-4be0-4adc-a917-5a4b38141e1a', '638-599-8893', 'Margherita', 'Plumer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('774d4aa6-1663-4df3-af12-ea1b0155384e', '461-210-1840', 'Verlene', 'Sawicki', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('497398f1-c540-4890-a8eb-b2b37f539b67', '048-624-3929', 'Tiffany', 'Leite', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('36d180c0-6119-44d3-bce6-c65a811e016e', '834-825-2128', 'Sau', 'Swofford', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('bbf599ed-d8b5-4ed5-9ba2-c7d93db88c95', '855-108-5333', 'Harriette', 'Luster', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('15d20b92-5d95-423c-a052-29791c35e81f', '611-620-5184', 'Clint', 'Redfearn', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('abb22af7-e5dc-4c1d-a2d4-54520b71abc2', '102-840-7476', 'Dianna', 'Hughley', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('880eae88-f307-436e-8742-549ea3de7799', '954-486-3200', 'Lara', 'Flora', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5c9f1c2f-6e7a-47d4-9093-feaf7dc8615f', '194-138-7947', 'Mathilda', 'Weller', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b302f332-897c-4526-a5ad-b4e050c11bca', '897-508-0292', 'Luanna', 'Roesler', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8307b8ef-0522-43d5-b114-24e7ad318aa7', '383-806-1972', 'Magnolia', 'Reulet', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5b733781-0663-432c-8e93-453fe9d758e6', '256-443-6777', 'Sadie', 'Burney', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('59a4644a-61f6-4bce-b0de-75916ecaeb2e', '945-771-8263', 'Steven', 'Mccartney', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8598b2f9-9f73-4370-8453-09a7505adb40', '793-654-4784', 'Bethel', 'Vansickle', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d82dce74-6f10-46d2-86fc-6c60edee006a', '216-289-6333', 'Evan', 'Mathes', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ce5fc24c-c89c-43b2-95e3-3938ce590154', '404-549-7354', 'Sanjuanita', 'Stafford', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('09da5d33-b6d1-4f30-8f73-55959d460997', '358-368-5731', 'Brinda', 'Holz', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('db4f2ed4-d88c-4de5-9256-8fa34e3c0121', '869-740-9860', 'Gertude', 'Jeter', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a0173269-3968-4bdb-bed4-0de508fd197c', '590-663-2137', 'Lindsay', 'Worthley', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c096c17d-b9d0-420b-a512-6376d39fe190', '883-680-6119', 'Bernardo', 'Wisecup', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8e8c0e7e-e294-4c00-bf68-5885b54bf181', '591-768-7975', 'Dina', 'Raskin', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c87f42bb-5928-4218-89a0-dcdbe6007698', '132-789-1239', 'Lan', 'Jacquet', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7a380f82-bf0c-41e3-8fdc-5fcbb39b9b86', '195-151-7346', 'Lyle', 'Baum', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ece1f52f-9eb3-400b-86ed-ae5bbede9d6b', '709-552-7420', 'Cathey', 'Waechter', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5314415f-e110-46d3-834c-a48a8c5ce24b', '495-209-8350', 'Renita', 'Darrington', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7ccb3c55-8ecc-4f65-94d6-6960f9020d81', '627-826-4968', 'Charley', 'Tiernan', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('74abcfdf-45c6-4b33-aa33-1e02a8751187', '799-081-9902', 'Lakeisha', 'Garbarino', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f644ccd6-d65e-4cb5-8405-53c779d0aeff', '318-958-5473', 'Vicenta', 'Swingle', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('983b0caf-9a0c-41b6-b615-79f88b5bdc28', '044-939-9995', 'Nida', 'Oppenheimer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('db8ef18d-39a1-4039-974d-f81f96020bf6', '977-369-6465', 'Christie', 'Parmentier', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a1e54abe-e047-4989-8ee7-52d1a489cc00', '312-644-9687', 'Jacki', 'Pavone', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ba412092-ca49-4b96-8aa8-337962432299', '404-273-2162', 'Shay', 'Love', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e20cc662-53fa-4d98-8a31-781423a7bf10', '817-469-1463', 'Dottie', 'Forgey', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5035cd77-47f4-4732-ac0a-dae28994a590', '188-229-2641', 'Eliz', 'Rulison', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6261f770-40b4-4b68-bc2a-3115e485feac', '574-316-5433', 'Saran', 'Cella', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7943f31f-f349-4f6f-b703-26558251ad02', '004-160-8783', 'Fred', 'Silk', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d5338803-773a-4139-9fdd-8ceed5172de2', '888-583-3900', 'Nydia', 'Mina', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('18766bd2-5f57-46d2-875a-a489283510a5', '463-004-7725', 'Myles', 'Justin', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f51d4a38-7203-486b-924e-a890f6693835', '370-445-5787', 'Melda', 'Sparr', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('9d163b2d-64d0-4741-9c2c-51e927c5f33b', '893-382-3510', 'Sheldon', 'Dammann', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1882d8bd-cdb4-45d4-a593-7dd9704d6c34', '809-949-4397', 'Kimberley', 'Tijerina', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8d7d0809-092a-4666-9be1-a0f7af502c42', '348-196-1151', 'Sharron', 'Eng', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c436a631-5d7b-4a6d-b052-89c8a2cb1d01', '843-980-9164', 'Manual', 'Bulluck', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3a34affb-f7b8-4ef8-ae15-5abfcf041bf7', '282-408-4003', 'Blaine', 'Eichner', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('4b946a22-57d0-452f-83d3-312ba23a457e', '123-264-1959', 'Sylvie', 'Maher', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b973740d-def6-481d-9727-530c970b330f', '771-891-3093', 'Florrie', 'Kleiner', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d6ef6b33-540f-49f7-bee8-70330a14edf8', '621-844-6064', 'Rocco', 'Reider', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b86accc6-6ea0-4864-a8e9-a2b9daef54af', '336-537-5574', 'Colby', 'Throckmorton', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c7b51f8c-2c61-4051-a1f5-1d26db93cf6e', '106-621-0022', 'Consuelo', 'Vandermeer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d92a6b53-4af5-4aa9-bc64-9cff804c8ac6', '259-219-4580', 'Robbin', 'Mckechnie', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ae98a80d-5c5a-4532-965b-001ab00364f1', '924-930-9368', 'Douglas', 'Kluge', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3cd11d59-da95-41c3-a91a-ca37d7c742d9', '205-538-3751', 'Jenise', 'Aldana', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a0a225d7-f055-434f-ba37-5ef8de522288', '721-555-3232', 'Jordan', 'Scaife', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a9ffce67-5b79-4cbb-8765-f6cd4df7c399', '767-701-0285', 'Bethany', 'Buzzard', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e5ba972d-b1d6-473f-a37f-84852bd7d9d4', '988-979-4066', 'Adriane', 'Haberle', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('9d5df300-d9c9-44af-9bbc-36db1d576702', '521-915-2362', 'Derek', 'Alt', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1f5164a9-bf98-4ce5-9acf-9f3e255f3548', '419-314-1996', 'Elia', 'Methvin', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('41099a31-f9b3-4f50-8722-71919ce19c9d', '452-898-9233', 'Taryn', 'Gatson', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('18707d5a-ebf3-47be-a12f-407eba0c782e', '374-668-6730', 'Eugenie', 'Belser', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e089ff34-b8a9-4e12-b50e-5b82b1c994e1', '622-099-0331', 'Florentino', 'Pantoja', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b0d79253-476b-426c-b2be-eb9ba3854eef', '237-658-4756', 'Libbie', 'Zertuche', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('30d0a31b-becc-4e0f-9725-e287f98988ef', '140-210-3188', 'Monte', 'Mote', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0f3ff6a7-1ee0-489c-a917-2f381a5f7dd3', '594-628-2605', 'Jean', 'Billie', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('30612124-2dd9-47be-b534-c2f2f5b0a94c', '804-346-1688', 'Karole', 'Morley', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('282f5735-a829-4529-8eb6-201f8ae707b0', '085-704-3028', 'Tianna', 'Rabin', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('bfb3c410-a48b-485c-8abf-d19c6b539a9b', '260-725-0498', 'Lilian', 'Scanlon', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2ee5a4a9-dd65-4311-91f1-8a90b4208566', '064-167-1456', 'Marita', 'Whitson', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('26d603aa-c685-4b18-ba2b-6b67c45f2326', '757-246-8098', 'Laticia', 'Jacinto', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2742ada9-2303-4614-a69f-519dea88312c', '591-106-7529', 'Abby', 'Murguia', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('460ce30b-cb1b-4387-a012-19deeb6ac91f', '898-634-1782', 'Leann', 'Munday', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('84a94c13-5b86-4e31-9548-8d4493ec8113', '262-646-1616', 'Desmond', 'Cassady', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8504bfa2-2d4f-4b91-b084-36dfc61451e3', '104-964-5443', 'Mindi', 'Alberts', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8beec707-0bdc-4184-bce4-98b96b9e6f55', '756-838-4205', 'Liza', 'Aubuchon', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d1d21382-dce1-41c2-af02-5e779c93ac96', '631-561-5097', 'Keisha', 'Tate', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('65d05186-ee34-4ad8-b872-2be3d3dda175', '024-954-7541', 'Arron', 'Cashin', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a5e9a413-0283-4fc6-a02f-ce477a0d4668', '834-269-5881', 'Leida', 'Fiorentino', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('546124d9-7b49-4278-b291-d62757f51b47', '737-739-1553', 'Wallace', 'Harville', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('bb593083-d8b4-4082-ad8b-d56dba4aa4f0', '042-095-7179', 'Bernardina', 'Campione', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6176e41c-4c13-4cff-a366-bc97649ce867', '755-695-7911', 'Cory', 'Sward', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('17558ad8-e3fa-483f-8e78-c4daec3326a5', '182-813-5175', 'Ramon', 'Hauger', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e6d1a75c-c29b-42d7-af54-de6f444f42ba', '011-839-8050', 'Carolina', 'Cueva', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('de14ea31-5f96-443a-a847-483d768d66a8', '021-447-1254', 'Mamie', 'Seaman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('308bc9e3-b49e-4893-82f4-7409e3ec22ad', '150-461-7777', 'Earlie', 'Pollman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3a1a4e02-43f2-407d-ae86-25e7453afa53', '838-390-5998', 'Garrett', 'Hosler', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('27802339-297b-4982-847f-3ee08a8980e8', '116-945-9467', 'Dyan', 'Bara', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f33e3885-2952-4c67-bb68-d9669df8af19', '911-946-7085', 'Sherly', 'Marx', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('45f03913-c7bc-4b61-9714-0894d37b558f', '428-172-5039', 'Jeanna', 'Harbison', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d7f8981c-7e44-49e4-b941-2f17e328a4c5', '740-485-2192', 'Faustina', 'Wasielewski', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d56ca6a1-a8ae-44c7-a8ac-ccbcbf96d7a5', '124-051-7189', 'Emerita', 'Outlaw', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('22fe335d-e5f3-4d0c-b071-de821035fac1', '167-915-2998', 'Kathlyn', 'Sumners', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('be030391-afd5-40dd-b739-cb623f7d482c', '916-163-7155', 'Lucretia', 'Hultquist', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('dcf2ef91-cedd-4b68-a096-f438a1577b27', '836-825-7536', 'Jillian', 'Plamondon', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5dd5b0de-c923-4e04-b64a-38bcb732225d', '763-826-6125', 'Janet', 'Deppe', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8e3f2cf6-a20e-42da-9ccc-d46f1a7e7bc3', '425-488-1000', 'Olympia', 'Wing', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0619d1ad-5e27-4783-b03d-7480de86df07', '905-796-9794', 'Louis', 'Hage', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e800d761-b363-458b-b71e-5c73e0f700d8', '671-792-6401', 'Elton', 'Desiderio', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5bbacdc4-23d0-4d8f-8f87-f2ee35d1bb94', '537-162-7651', 'Brendan', 'Gibb', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6b929207-9797-4402-a617-327e9fe8e41b', '645-467-6352', 'Danelle', 'Setliff', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('9fb7db44-e869-43ce-87d0-6b387a6c32a3', '101-248-3057', 'Esta', 'Ellen', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('747d882a-180f-464c-b80d-a44fe34934d4', '531-150-3992', 'Barney', 'Stratton', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0cda2cf7-82ef-4714-90b7-4e2031375cfd', '948-757-5016', 'Zonia', 'Lierman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0b3eb860-716f-4b8d-bac6-3d813ee5cb25', '273-249-6334', 'Clora', 'Weiner', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('82328873-2a6d-4994-8d1a-5ee1776f9655', '495-656-7728', 'Londa', 'Castrejon', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c2b6c7eb-451b-49ec-86de-6c1e9e1e3505', '424-716-6785', 'Regan', 'Drinkard', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c23983a1-b97b-4d15-9b47-b324db934805', '800-917-0702', 'Collene', 'Digiovanni', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d17a8f9e-76ae-4d1d-8647-e89413df5d7e', '412-068-8406', 'Mable', 'Catt', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('76f344d0-6370-4aed-96bc-cc707d07a393', '076-521-5728', 'Glady', 'Caruana', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d51fc540-8b2a-47a9-907f-d9f15c7aca04', '521-995-9722', 'Rosanne', 'Herrera', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f301d569-cddb-4b66-a714-679b36de5bfd', '257-759-0301', 'Rod', 'Moriarty', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b004f473-8d27-4892-9cee-a877b94e6fa1', '931-948-9973', 'Dolly', 'Perdomo', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('220794da-aca6-4b87-bbfb-8409f47013b0', '473-159-5430', 'Kendrick', 'Hutchcraft', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('96ff9bd7-1446-4e2f-8fa8-c1da34d75a6a', '127-227-4632', 'Eleonora', 'Siler', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ab4f5783-a4d3-4c67-afb7-a809ea369ec1', '724-217-0627', 'Tawny', 'Hebron', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('807437ac-1dcd-4913-bfef-ddfb44a55820', '089-769-2768', 'In', 'Powell', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8a2d07d6-ba28-4a41-a40e-b34a25072e46', '625-361-7119', 'Hettie', 'Winkleman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e1c6610e-7d28-4af4-a19e-a88374bd4b6e', '752-570-4890', 'Ada', 'Mcferron', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d16bc395-b015-4004-8bb3-c8172fd134c3', '551-270-9118', 'Lakisha', 'Gooch', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b22f1410-7d8e-4ed7-8271-1435dff3acd6', '899-175-1439', 'Julio', 'Winfrey', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3a7e7ed7-9a72-4860-9708-b09e966c588f', '949-267-2862', 'Stephnie', 'Moretz', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6c7d9e2d-fcf9-4902-b7df-3a4182e7fffa', '675-020-0881', 'Camille', 'Wayman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('eb70d945-580d-4c32-9808-851241c159de', '088-932-6872', 'Aisha', 'Golson', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('fc435433-387f-46b2-b927-5b3c4edf4900', '010-477-7359', 'Paul', 'Weise', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('bc9eadfc-4493-4fd7-9de4-05b69548d002', '844-128-6550', 'Crysta', 'Meyers', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('125031c9-ddc1-4a49-8358-fc5fdc884cb7', '488-958-3040', 'Nellie', 'Parmer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f32a2a4d-08d8-4f94-abd1-e672513aa25f', '760-680-5455', 'Deloise', 'Lanterman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('419d9986-9642-439c-aea4-1a2ca28e3b51', '640-307-1133', 'Betsy', 'Verduzco', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('862daafd-c7fe-4772-a367-8cfef4341a09', '086-217-0014', 'Clement', 'Tamura', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5261f56a-1ef8-4a66-96ae-a7113428e2ed', '780-468-0414', 'Yoko', 'Oshields', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('78c16636-af67-48b3-8138-2333d7811da0', '905-896-1310', 'Sanjuana', 'Collette', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('4879b466-77d0-4e58-921d-8445fde775bf', '452-079-8474', 'Kittie', 'Barcomb', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('08a9d89e-c872-4e9f-b66c-a2b498a5a787', '732-882-8866', 'Domingo', 'Bath', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1eb1e9a6-2b08-4faa-8241-b5df7ffec5dc', '088-552-3823', 'Christia', 'Rudy', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('068f29dc-4dbf-4712-ae21-e9eb44df652f', '654-738-8843', 'Ji', 'Brasch', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6b8479c6-8601-486c-838a-839878c7c6ec', '878-212-9184', 'Ruthe', 'Wineinger', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8703312f-2838-45ac-87bd-0cb08de53e8c', '636-513-5672', 'Mozella', 'Rusk', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3a1fe93d-759b-41ab-9797-106fca225b38', '772-831-5259', 'Vannessa', 'Bargas', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('da9fd126-a8cc-4074-a219-ee205b8c682a', '862-244-9437', 'Layne', 'Cochran', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('052abddf-6aa4-4434-878f-a16ab5b16237', '662-529-3159', 'Tommie', 'Smeltzer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8e5f2e98-1b0c-478f-bfab-216070993b3e', '947-250-7256', 'Cherrie', 'Respass', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d99e8ea0-76e8-4255-b16e-98f138df7221', '809-056-4842', 'Romelia', 'Mclamb', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a1f67fdc-3559-44c2-8070-61aa5560ccb8', '744-565-2806', 'Daryl', 'Ardis', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2178e12b-5c40-4542-af87-204351092049', '572-102-7894', 'Gertrudis', 'Hirsh', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d74191e9-d0be-4c23-9386-98bce1e45cc0', '883-608-5599', 'Austin', 'Britain', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('50ad72cb-95de-445f-91e0-8cf36f8c7e86', '574-100-1139', 'Doris', 'Strzelecki', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('39bba04e-256f-4a99-b066-3e1e76381962', '689-897-3915', 'Kandi', 'Plouffe', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('19d4aa15-dfcd-43d0-befb-0e4a604d97b3', '941-845-6113', 'Leia', 'Henze', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('002975bc-7bd6-4106-9871-16458689cbd7', '940-486-7160', 'Dominga', 'Liedtke', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8b9dbb9e-a4ae-4d8d-8049-c56135612324', '921-272-3317', 'Bethann', 'Adams', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('45f58815-b646-46e5-81ff-baf02b8736d5', '147-481-3291', 'Willetta', 'Albaugh', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('17809db5-5399-4c82-999d-f29da650f8b1', '334-570-5845', 'Vanna', 'Fiscus', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('be7c75f9-0b6c-4fc8-b069-208c846b3ff5', '799-803-6506', 'Lorretta', 'Oles', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('26b5a7cf-34ba-427d-927b-d8114f1edfb0', '906-882-5400', 'Pearline', 'Cate', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ed4d4cf4-da66-4201-b027-885d7d3490d4', '147-774-3657', 'Eura', 'Monnier', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d04a71ed-8b52-49b4-8f6d-95edad5f7694', '422-697-0024', 'Shaina', 'Cather', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6f1bea1e-e31b-4ca1-82c5-4b82028bfa04', '812-409-5248', 'Thomasine', 'Schwalb', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('efdaad5e-ceb4-4ad0-8709-d55964cfe3cb', '249-490-0226', 'Gilma', 'Showman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('974af5f0-2319-4cf6-9acf-a402433d5cca', '301-181-5251', 'Sharita', 'Schwandt', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a2b00364-fc81-41e4-96b8-e144c2764f69', '915-939-7273', 'Cassy', 'Nemitz', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c8e1dbf5-f04b-4d3a-91f3-c1e2ee57619b', '097-498-1913', 'Maryam', 'Nickle', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2eb09977-1947-432c-ab87-35538d12ede6', '843-804-4676', 'Phebe', 'Heimer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('80d9e736-c599-44ab-95d6-178a2e962659', '229-312-7139', 'Maurita', 'Elsea', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('da254c7d-da69-4a15-a044-b78d9a9a2ccc', '569-465-7453', 'Golda', 'Boon', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('95b8502a-022a-42da-8015-84c00810211a', '873-390-8874', 'Yolanda', 'Bragg', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1b2bf3b8-b805-45fd-b4db-0692c500c4ef', '225-599-9431', 'Clementine', 'Dash', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a289a5e8-f275-4177-9e2a-535a6440f218', '539-260-5376', 'Loyce', 'Roudebush', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b2b9aebb-4837-4ab9-9c4f-f471b74a63d2', '531-075-2217', 'Marcela', 'Worth', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('4fd7846d-9b18-484e-bb94-33fc64602cce', '377-205-0146', 'Jannie', 'Dupuy', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2d1820ec-2054-4819-b789-f65dca42276f', '234-889-3397', 'Hayden', 'Mcquaig', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('ee36bca8-046b-486a-8d7e-b1e4981a16be', '083-687-3832', 'Krista', 'Paredez', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('996b68f0-8298-4988-a64e-5aa8a6f50ba7', '740-360-1450', 'Malissa', 'Curry', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('24935915-d46f-4c95-9a33-3d6d9e7247fe', '912-813-6520', 'Ozell', 'Koo', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f8e40441-cb3d-456b-be9f-34c5651b6c52', '933-878-0593', 'Ramona', 'Mccrae', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7634e197-16bc-434d-a4d3-0d9c378ade90', '584-022-4247', 'Nola', 'Vivas', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d38ea13f-e072-4169-b538-139ab5ca4f5a', '250-587-7729', 'Rodolfo', 'Palombo', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('9d708802-e2db-467f-91cf-0e16f2b1d89a', '645-472-4704', 'Maricela', 'Fant', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('795d238b-f759-430d-9726-23d192703c80', '171-107-2186', 'Denis', 'Vecchio', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3623e5a1-290f-4bf0-8093-d38ece6f1e3e', '420-636-9006', 'Anita', 'Karter', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f38fce26-7cd7-4d30-ab61-e4714bdb1e3c', '785-338-9808', 'Hermina', 'Cassidy', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7f53ce21-cf57-4663-ba6a-c147345ecc25', '572-147-1738', 'Neville', 'Sigler', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d69cb9fc-67d9-4618-8f0b-c253f6a0b016', '277-636-6051', 'Rigoberto', 'Hiltner', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('7fed1bb0-cae1-4d15-8958-76c3284dca7a', '051-434-5128', 'Selene', 'Moffat', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('e273bf11-0c31-41ef-b1a0-a1d4325f7cde', '965-778-6258', 'Tangela', 'Rumsey', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('fe2a7a31-c1ba-4372-bea6-7e203279a726', '913-863-5820', 'Vanda', 'Britten', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('4d308926-2708-4ce0-bc1a-4cb1df18570b', '719-157-8085', 'Brant', 'Samford', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('9e27cccc-cb02-42d5-8bc3-2f826ad9c117', '774-233-8101', 'Dion', 'Bruss', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('808ed07e-c9b4-43ce-b557-3cd885b71ef2', '152-145-8547', 'Merna', 'Melia', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('6a9526b1-377c-4545-b41a-b4ec14ae40cd', '926-093-9744', 'Melony', 'Carico', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2ae2a1ac-33b8-462d-ae37-e02ee0291c5d', '597-558-7675', 'Sallie', 'Mullen', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('62b3dab2-c888-490c-90a5-309878e7e5e0', '038-739-0882', 'Lucia', 'Couture', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('db5e796b-f731-4763-b892-eaaa94d967ef', '652-974-9871', 'Augustus', 'Hellmann', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3b54ee8c-abdd-465f-a248-24de3d0f665e', '282-947-0291', 'Adalberto', 'Villasenor', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0f7d27f9-4602-407a-aabd-28f32de765ea', '359-318-6967', 'Isa', 'Agan', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('45b5fb6a-1231-4542-8f07-ae1e27b8a25c', '190-146-1776', 'Matilda', 'Phegley', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5d014c27-86cc-4a20-8eb7-18c6311b89b3', '869-466-4197', 'Dionne', 'Jaffe', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5d7af3ac-e400-41b1-8b01-4ba7a46ae90b', '326-353-0035', 'Preston', 'Giard', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('5132f42f-054d-451d-9309-544d7bc8e09d', '699-556-4917', 'Ta', 'Lowman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('35a0e972-9e14-43eb-8317-7e4f8c3a1e1d', '362-146-7939', 'Jenae', 'Borge', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0df4c730-1afa-4487-b276-72f19d429fbc', '332-118-3985', 'Ron', 'Tamplin', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('3da9395f-2fab-4155-9ba2-950c64bc04a3', '720-499-4124', 'Lyndsay', 'Pouliot', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a0662302-4edb-454f-a99f-912d9d0df476', '822-018-3457', 'Michal', 'Warnick', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('a821404d-512e-4b5f-9ff1-00dcea88a314', '332-412-5209', 'Lena', 'Spece', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('c3e6b315-b373-4c79-893f-cd98643c4dc9', '695-129-1427', 'Kylie', 'Sprenger', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('00bf609c-ad97-4205-8820-2cca761bbaf9', '678-807-2834', 'Christiana', 'Lauber', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8835f474-1159-492d-b6bf-013fd6d1c789', '912-442-9265', 'Beatrice', 'Ridgell', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('66b2f00f-285d-423a-8f32-66ca93bdd102', '612-139-6545', 'America', 'Cornett', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('10e173c5-1aa0-449b-bc41-03419a9960fc', '154-345-9118', 'Treasa', 'Ryder', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('bf17cec8-91a2-445b-8a30-a231414f041d', '279-968-4306', 'Selena', 'Mcminn', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('27fd262e-b3a2-4382-8b92-e4847be030a4', '721-776-6225', 'Vivien', 'Halvorsen', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('653409aa-06b0-41c1-b74f-fcf6862dabd5', '413-931-0656', 'Mai', 'Autry', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('8f3f68b0-3eb0-489e-820f-fad72b3e11ff', '211-528-3306', 'Mana', 'Stalzer', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('b616c977-c05c-4e47-86a1-4df88f97a61e', '263-005-0006', 'Carmelo', 'Kwiecien', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('0d40b0ef-b717-44d0-bdff-90c365120181', '226-094-5920', 'Delinda', 'Britto', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('04029f4e-2396-4fae-a481-1ad672019754', '808-087-4664', 'Karla', 'Broman', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('1f50e848-9ac0-4284-a8fc-6f87aafcdacd', '767-261-9393', 'Sherell', 'Balicki', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('d6ddae3d-46c1-46ad-8b05-9848efac26bf', '424-365-4772', 'Kirby', 'Kruchten', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('2136dd30-a52a-49ab-a82f-7fafed0fd376', '649-819-8503', 'Magali', 'Pertuit', 'Customer')");
                    db.Database.ExecuteSqlRaw("INSERT INTO Users (UserId, PhoneNumber, FirstName, LastName, Role) VALUES ('f75b6395-6acb-46e1-81f7-0ef6e077d1e6', '783-090-3150', 'Aleen', 'Scruggs', 'Customer')");
                    trans.Commit();
                }
            }
            catch
            {

            }
        }
    }
}
