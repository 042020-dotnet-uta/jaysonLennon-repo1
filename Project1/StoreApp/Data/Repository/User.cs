using System;
using StoreApp.Data;
using StoreApp.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace StoreApp.Repository
{
    /// <summary>
    /// The return value of <c>CreateUserAccount()</c>
    /// </summary>
    public enum CreateUserAccountResult
    {
        /// <summary>The account was created successfully.</summary>
        Ok,

        /// <summary>Failed to create account: another account with that name already exists.</summary>
        AccountNameExists,

        /// <summary>Failed to create account: no login name was present in <c>User</c> object.</summary>
        MissingLogin,

        /// <summary>Failed to create account: no password was present in the <c>User</c> object.</summary>
        MissingPassword,
    }

    public interface IUserData
    {
        string GetFirstName();
        string GetLastName();
        string GetAddressLine1();
        string GetAddressLine2();
        string GetCity();
        string GetState();
        string GetZip();
    }

    public class UserQueryResultWithRevenue
    {
        /// <summary>Used to highlight results.</summary>
        public string QueryItem1 { get; set; }

        /// <summary>Used to highlight results.</summary>
        public string QueryItem2 { get; set; }

        /// <summary>
        /// Whether this query searches both first and last name, or specific fields.
        /// 
        /// When this is True, then both the first and last name fields have been searched
        /// and QueryItem1 and QueryItem2 should be highlighted in either name fields.
        /// 
        /// When this is False, QueryItem1 represents the first name of the customer,
        /// and QueryItem2 represents the last name of the customer. Highlighting should
        /// occur in the appropriate fields when this is the case.
        /// </summary>
        public bool IsOmniQuery { get; set; }
        public IEnumerable<Tuple<User, double>> Users { get; set; }
    }
    public interface IUser
    {
        public IEnumerable<User> FindUserQuery(string nameQuery);
        public UserQueryResultWithRevenue FindUserQueryIncludeRevenue(string nameQuery);
        public IEnumerable<User> FindUserByFirstName(string firstName);
        public IEnumerable<User> FindUserByLastName(string lastName);
        public Task<User> GetUserByLogin(string login);
        public Task<User> GetUserById(Guid userId);
        public Task<Address> GetAddressByuserId(Guid userId);
        public Task<bool> LoginExists(string login);
        public Task<CreateUserAccountResult> Add(User user);
        public Task<bool> VerifyUserLogin(string login);
        public void SetDefaultLocation(User user, Location location);
        public Task<Location> GetDefaultLocation(User user);
        public Task<Location> GetDefaultLocation(Guid userId);
        public Task<Order> GetOpenOrder(User user, Location location);
        public Task<User> VerifyCredentials(string login, string plainPassword);
        public Task<bool> UpdateUserInfo(Guid userId, IUserData newData);
        public Task<int> CountProductsInCart(Guid userId);
    }


    public class UserRepository : Repository.IUser
    {
        private StoreContext _context;

        public UserRepository(StoreContext context)
        {
            this._context = context;
        }

        public async Task<Location> GetDefaultLocation(User user)
        {
            return await _context.Users
                                 .Include(c => c.DefaultLocation)
                                 .Where(c => c.UserId == user.UserId)
                                 .Select(c => c.DefaultLocation)
                                 .SingleOrDefaultAsync();
        }

        public async Task<Location> GetDefaultLocation(Guid userId)
        {
            return await _context.Users
                .Include(c => c.DefaultLocation)
                .Where(c => c.UserId == userId)
                .Select(c => c.DefaultLocation)
                .SingleOrDefaultAsync();
        }

        public async Task<CreateUserAccountResult> Add(User user)
        {
            var hashed = StoreApp.Util.Hash.Sha256(user.Password);
            user.Password = hashed;
            if (String.IsNullOrEmpty(user.Login)) return CreateUserAccountResult.MissingLogin;
            if (String.IsNullOrEmpty(user.Password)) return CreateUserAccountResult.MissingPassword;

            var loginExists = await _context.Users.Where(c => c.Login == user.Login.ToLower()).SingleOrDefaultAsync();
            if (loginExists != null) return CreateUserAccountResult.AccountNameExists;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return CreateUserAccountResult.Ok;
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users
                                 .Where(c => c.UserId == id)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByLogin(string login)
        {
            login = login.ToLower();
            return await _context.Users
                                 .Where(c => c.Login.ToLower() == login)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }


        public async Task<Order> GetOpenOrder(User user, Location location)
        {
            var currentOrder = await _context.Orders
                                             .Include(o => o.Location)
                                             .Where(o => o.User.UserId == user.UserId)
                                             .Where(o => o.TimeSubmitted == null)
                                             .Where(o => o.Location.LocationId == location.LocationId)
                                             .Select(o => o)
                                             .SingleOrDefaultAsync();
            if (currentOrder == null)
            {
                var newOrder = new Entity.Order(user, location);
                _context.Add(newOrder);
                await _context.SaveChangesAsync();
                return newOrder;
            }
            else
            {
                return currentOrder;
            }
        }

        public async Task<bool> LoginExists(string login)
        {
            login = login.ToLower();
            return await _context.Users
                           .Where(c => c.Login.ToLower() == login)
                           .Select(c => c)
                           .SingleOrDefaultAsync() != null;
        }

        public async void SetDefaultLocation(User user, Location location)
        {
            user.DefaultLocation = location;
            await _context.SaveChangesAsync();
        }

        public async Task<User> VerifyCredentials(string login, string plainPassword)
        {
            login = login.ToLower();
            var hashed = StoreApp.Util.Hash.Sha256(plainPassword);
            return await _context.Users
                                 .Where(c => c.Login.ToLower() == login)
                                 .Where(c => c.Password == hashed)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }

        public async Task<bool> VerifyUserLogin(string login)
        {
            if (String.IsNullOrEmpty(login)) return false;
            login = login.ToLower();
            // TODO: additional validation rules
            var exists = await _context.Users
                                       .Where(c => c.Login.ToLower() == login)
                                       .SingleOrDefaultAsync();
            return exists == null;
        }

        public async Task<bool> UpdateUserInfo(Guid userId, IUserData newData)
        {
            // TODO: make this function less terrible

            var cityString = newData.GetCity();
            Entity.City cityEntity = null;
            if (cityString != null)
            {
                cityEntity = await _context.Addresses
                    .Where(a => a.City.Name.ToLower() == cityString.ToLower().Trim())
                    .Select(a => a.City)
                    .SingleOrDefaultAsync();
                if (cityEntity == null)
                {
                    cityEntity = new Entity.City();
                    cityEntity.Name = cityString.Trim();
                }
            }

            // TODO: validate states
            var stateString = newData.GetState();
            Entity.State stateEntity = null;
            if (stateString != null)
            {
                stateEntity = await _context.Addresses
                    .Where(a => a.State.Name.ToLower() == stateString.ToLower().Trim())
                    .Select(a => a.State)
                    .SingleOrDefaultAsync();
                if (stateEntity == null)
                {
                    stateEntity = new Entity.State();
                    stateEntity.Name = stateString.Trim();
                }
            }


            var zipString = newData.GetZip();
            if (zipString != null)
            {
                // Match any amount of numbers, optionally followed by a dash and any amount of numbers.
                var zipValidator = new Regex(@"^[0-9]{5}(-?[0-9]{4})?$");
                if (!zipValidator.IsMatch(newData.GetZip().Trim())) return false;
            }

            Entity.ZipCode zipEntity = null;
            if (zipString != null)
            {
                zipEntity = await _context.Addresses
                    .Where(a => a.Zip.Zip == zipString.Trim())
                    .Select(a => a.Zip)
                    .SingleOrDefaultAsync();
                if (zipEntity == null)
                {
                    zipEntity = new Entity.ZipCode();
                    zipEntity.Zip = zipString.Trim();
                }
            }

            var addressLine1String = newData.GetAddressLine1();
            Entity.AddressLine1 addressLine1Entity = null;
            if (addressLine1String != null)
            {
                addressLine1Entity = await _context.AddressLine1s
                    .Where(l => l.Data.ToLower() == addressLine1String.ToLower().Trim())
                    .Select(l => l)
                    .SingleOrDefaultAsync();
                if (addressLine1Entity == null)
                {
                    addressLine1Entity = new Entity.AddressLine1();
                    addressLine1Entity.Data = addressLine1String.Trim();
                }
            }

            var addressLine2String = newData.GetAddressLine2();
            Entity.AddressLine2 addressLine2Entity = null;
            if (addressLine2String != null)
            {
                addressLine2Entity = await _context.AddressLine2s
                    .Where(l => l.Data.ToLower() == addressLine2String.ToLower().Trim())
                    .Select(l => l)
                    .SingleOrDefaultAsync();
                if (addressLine2Entity == null)
                {
                    addressLine2Entity = new Entity.AddressLine2();
                    addressLine2Entity.Data = addressLine2String.Trim();
                }
            }

            var user = await _context.Users
                .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                .Include(c => c.Address)
                    .ThenInclude(a => a.State)
                .Include(c => c.Address)
                    .ThenInclude(a => a.Zip)
                .Include(c => c.Address)
                    .ThenInclude(a => a.Line1)
                .Include(c => c.Address)
                    .ThenInclude(a => a.Line2)
                .Where(c => c.UserId == userId)
                .Select(c => c)
                .SingleOrDefaultAsync();

            Entity.Address address = null;
            if (user.Address == null)
            {
                address = new Entity.Address();
                _context.Add(address);
                user.Address = address;
            }
            else
            {
                address = await _context.Addresses
                    .Where(a => a.AddressId == user.Address.AddressId)
                    .Select(a => a)
                    .SingleOrDefaultAsync();
            }

            if (newData.GetFirstName() != null) user.FirstName = newData.GetFirstName().Trim();
            else user.FirstName = null;

            if (newData.GetLastName() != null) user.LastName = newData.GetLastName().Trim();
            else user.LastName = null;

            address.City = cityEntity;
            address.State = stateEntity;
            address.Zip = zipEntity;
            address.Line1 = addressLine1Entity;
            address.Line2 = addressLine2Entity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Address> GetAddressByuserId(Guid userId)
        {
            return await _context.Users
                .Include(c => c.Address)
                    .ThenInclude(a => a.City)
                .Include(c => c.Address)
                    .ThenInclude(a => a.State)
                .Include(c => c.Address)
                    .ThenInclude(a => a.Zip)
                .Include(c => c.Address)
                    .ThenInclude(a => a.Line1)
                .Include(c => c.Address)
                    .ThenInclude(a => a.Line2)
                .Where(c => c.UserId == userId)
                .Select(c => c.Address)
                .SingleOrDefaultAsync();
        }

        public async Task<int> CountProductsInCart(Guid userId)
        {
            return await _context.OrderLineItems
                .Where(ol => ol.Order.TimeSubmitted == null)
                .Where(ol => ol.Order.User.UserId == userId)
                .SumAsync(ol => ol.Quantity);
        }

        public IEnumerable<User> FindUserQuery(string nameQuery)
        {
            nameQuery = nameQuery.ToLower();

            // Search by either first name or last name when a space is present between two
            // search terms. Both search terms must be present in either the first or last
            // name (or both) of the customer in order to be considered a match.
            var nameComponents = nameQuery.Split(' ', 2);
            if (nameComponents.Length == 2)
            {
                var query1 = FindUserQuery(nameComponents[0].Trim());
                var query2 = FindUserQuery(nameComponents[1].Trim());
                return query1.Intersect(query2);
            }

            // Search for names in a "lastName,firstName" fashion when a comma is present
            // in the search query.
            var lastThenFirst = nameQuery.Split(',', 2);
            if (lastThenFirst.Length == 2)
            {
                var query1 = FindUserByLastName(lastThenFirst[0].Trim());
                var query2 = FindUserByFirstName(lastThenFirst[1].Trim());
                return query1.Intersect(query2);
            }

            // Standard single term search checks both first and last names.
            return _context.Users
                .Where(u => u.FirstName.ToLower().Contains(nameQuery) || u.LastName.ToLower().Contains(nameQuery))
                .Select(u => u);
        }

        public UserQueryResultWithRevenue FindUserQueryIncludeRevenue(string nameQuery)
        {
            nameQuery = nameQuery.ToLower();

            // Search by either first name or last name when a space is present between two
            // search terms. Both search terms must be present in either the first or last
            // name (or both) of the customer in order to be considered a match.
            var nameComponents = nameQuery.Split(' ', 2);
            if (nameComponents.Length == 2)
            {
                var queryItem1 = nameComponents[0].Trim();
                var queryItem2 = nameComponents[1].Trim();
                var query1 = FindUserQuery(queryItem1);
                var query2 = FindUserQuery(queryItem2);
                return new UserQueryResultWithRevenue {
                    Users = query1.Intersect(query2)
                            .Select(u => new Tuple<User, double>(
                                u,
                                _context.Orders
                                    .Where(o => o.User.UserId == u.UserId)
                                    .Sum(o => o.AmountPaid) ?? 0.0
                                )
                            ),
                    QueryItem1 = queryItem1,
                    QueryItem2 = queryItem2,
                    IsOmniQuery = true,
                };
            }

            // Search for names in a "lastName,firstName" fashion when a comma is present
            // in the search query.
            var lastThenFirst = nameQuery.Split(',', 2);
            if (lastThenFirst.Length == 2)
            {
                var queryItem1 = lastThenFirst[0].Trim();
                var queryItem2 = lastThenFirst[1].Trim();
                var query1 = FindUserByLastName(queryItem1);
                var query2 = FindUserByFirstName(queryItem2);
                return new UserQueryResultWithRevenue {
                    Users = query1.Intersect(query2)
                            .Select(u => new Tuple<User, double>(
                                u,
                                _context.Orders
                                    .Where(o => o.User.UserId == u.UserId)
                                    .Sum(o => o.AmountPaid) ?? 0.0
                                )
                            ),
                    QueryItem1 = queryItem1,
                    QueryItem2 = queryItem2,
                    IsOmniQuery = false,
                };
            }

            // Standard single term search checks both first and last names.
            var singleTermResults = _context.Users
                .Where(u => u.FirstName.ToLower().Contains(nameQuery) || u.LastName.ToLower().Contains(nameQuery))
                .Select(u => new Tuple<User, double>(
                    u,
                    _context.Orders
                        .Where(o => o.User.UserId == u.UserId)
                        .Sum(o => o.AmountPaid) ?? 0.0
                    )
                );

            return new UserQueryResultWithRevenue {
                Users = singleTermResults,
                QueryItem1 = nameQuery,
                QueryItem2 = null,
                IsOmniQuery = true,
            };
        }

        public IEnumerable<User> FindUserByFirstName(string firstName)
        {
            firstName = firstName.ToLower();
            return _context.Users
                .Where(u => u.FirstName.ToLower().Contains(firstName))
                .Select(u => u);
        }

        public IEnumerable<User> FindUserByLastName(string lastName)
        {
            lastName = lastName.ToLower();
            return _context.Users
                .Where(u => u.LastName.ToLower().Contains(lastName))
                .Select(u => u);
        }
    }
}
