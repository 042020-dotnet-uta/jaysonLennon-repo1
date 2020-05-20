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
    /// The return value of CreateUserAccount()
    /// </summary>
    public enum CreateUserAccountResult
    {
        /// <summary>The account was created successfully.</summary>
        Ok,

        /// <summary>Failed to create account: another account with that name already exists.</summary>
        AccountNameExists,

        /// <summary>Failed to create account: no login name was present in User object.</summary>
        MissingLogin,

        /// <summary>Failed to create account: no password was present in the User object.</summary>
        MissingPassword,
    }

    /// <summary>
    /// Interface for a user data model.
    /// </summary>
    public interface IUserData
    {
        /// <summary>
        /// Returns the first name of the user.
        /// </summary>
        string GetFirstName();
        /// <summary>
        /// Returns the last name of the user.
        /// </summary>
        string GetLastName();
        /// <summary>
        /// Gets address line 1 of the user.
        /// </summary>
        string GetAddressLine1();
        /// <summary>
        /// Gets address line 2 of the user.
        /// </summary>
        string GetAddressLine2();
        /// <summary>
        /// Gets the city of the user.
        /// </summary>
        string GetCity();
        /// <summary>
        /// Gets the state of the user.
        /// </summary>
        string GetState();
        /// <summary>
        /// Gets the zip code of the user.
        /// </summary>
        string GetZip();
    }

    /// <summary>
    /// The result of a user query.
    /// </summary>
    public class UserQueryResultWithRevenue
    {
        /// <summary>
        /// The first term for a customer search as supplied by the user.
        /// <remarks>
        /// This value represents different query terms based on the value of
        /// IsOmniQuery.
        /// 
        /// When IsOmniQuery is false, QueryTerm1 was checked versus the last name
        /// of a customer only.
        /// 
        /// When IsOmniQuery is true, QueryTerm1 was checked versus both the first
        /// and last names of a customer.
        /// 
        /// This should be taken into account when performing highlighting on the
        /// frontend in order to highlight only the appropriate fields.
        /// </remarks>
        /// </summary>
        public string QueryTerm1 { get; set; }
        /// <summary>
        /// The second term for a customer search as supplied by the user.
        /// <remarks>
        /// This value represents different query terms based on the value of
        /// IsOmniQuery.
        /// 
        /// When IsOmniQuery is false, QueryTerm2 was checked versus the first name
        /// of a customer only.
        /// 
        /// When IsOmniQuery is true, QueryTerm2 was checked versus both the first
        /// and last names of a customer.
        /// 
        /// This should be taken into account when performing highlighting on the
        /// frontend in order to highlight only the appropriate fields.
        /// </remarks>
        /// </summary>
        public string QueryTerm2 { get; set; }

        /// <summary>
        /// Whether this query was performed by searching both first and last name
        /// of a customer with the same search term (true), or if it was performed
        /// by using specific search terms in specific name fields (false).
        /// </summary>
        public bool IsOmniQuery { get; set; }
        public IEnumerable<Tuple<User, double>> Users { get; set; }
    }

    /// <summary>
    /// Interface to query information about users.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Searches for users based on a search query.
        /// </summary>
        /// <param name="nameQuery">Query</param>
        /// <returns>Users found, if any</returns>
        public IEnumerable<User> FindUserQuery(string nameQuery);
        /// <summary>
        /// Searches for users based on a search query, and includes total revenue of the user.
        /// </summary>
        /// <param name="nameQuery">Query</param>
        /// <returns>Users found, if any.</returns>
        public UserQueryResultWithRevenue FindUserQueryIncludeRevenue(string nameQuery);
        /// <summary>
        /// Searches for users based on their first name.
        /// </summary>
        /// <param name="firstName">First name to search</param>
        /// <returns>Users found, if any.</returns>
        public IEnumerable<User> FindUserByFirstName(string firstName);
        /// <summary>
        /// Searches for users based on their last name.
        /// </summary>
        /// <param name="lastName">Last name to search</param>
        /// <returns>Users found, if any.</returns>
        public IEnumerable<User> FindUserByLastName(string lastName);
        /// <summary>
        /// Retrieves a user based on their login name.
        /// </summary>
        /// <param name="login">Login to search</param>
        /// <returns>A user, if found.</returns>
        public Task<User> GetUserByLogin(string login);
        /// <summary>
        /// Retrieves a user based on their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The User, if found.</returns>
        public Task<User> GetUserById(Guid userId);
        /// <summary>
        /// Retrieves the address of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>User address.</returns>
        public Task<Address> GetAddressByUserId(Guid userId);
        /// <summary>
        /// Determines whether a login name exists.
        /// </summary>
        /// <param name="login">The login name to check.</param>
        /// <returns>Whether the login name already exists.</returns>
        public Task<bool> LoginExists(string login);
        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>CreateUserAccountResult detailing the status of the operation.</returns>
        public Task<CreateUserAccountResult> Add(User user);
        /// <summary>
        /// Verifies if the login supplied meets requirements.
        /// </summary>
        /// <param name="login">The login name to check.</param>
        /// <returns>Whether requirements are satisfied.</returns>
        public Task<bool> VerifyUserLogin(string login);
        /// <summary>
        /// Sets the default location for a user.
        /// </summary>
        /// <param name="user">User to update.</param>
        /// <param name="location">Location to set to default.</param>
        public void SetDefaultLocation(User user, Location location);
        /// <summary>
        /// Retrieves the default location for a user.
        /// </summary>
        /// <param name="user">The user to query.</param>
        /// <returns>The default location for the user.</returns>
        public Task<Location> GetDefaultLocation(User user);
        /// <summary>
        /// Retrieves the default location for a user.
        /// </summary>
        /// <param name="userId">The user id to query.</param>
        /// <returns>The default location for the user.</returns>
        public Task<Location> GetDefaultLocation(Guid userId);
        /// <summary>
        /// Retrieves the customer's currently open order for a location.
        /// <remarks>
        /// If the customer has no open order for the location, a new one will be
        /// created, so this method will always return an order object.
        /// </remarks>
        /// </summary>
        /// <param name="user">User to get order from.</param>
        /// <param name="location">Location to check.</param>
        /// <returns>Order for the location.</returns>
        public Task<Order> GetOpenOrder(User user, Location location);
        /// <summary>
        /// Verifies that a users login name and password match.
        /// </summary>
        /// <param name="login">The login name.</param>
        /// <param name="plainPassword">The password supplied.</param>
        /// <returns>A User, if credentials match; null otherwise.</returns>
        public Task<User> VerifyCredentials(string login, string plainPassword);
        /// <summary>
        /// Updates user personal information.
        /// </summary>
        /// <param name="userId">The user ID to be updated.</param>
        /// <param name="newData">The new data to input.</param>
        /// <returns>Whether the operation was successful.</returns>
        public Task<bool> UpdateUserPersonalInfo(Guid userId, IUserData newData);
        /// <summary>
        /// Get the number of products in a user's current order.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <returns>Number of items in cart.</returns>
        public Task<int> CountProductsInCart(Guid userId);
        /// <summary>
        /// Get the number of a specific products in a user's current order.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <param name="productId">The product ID to check.</param>
        /// <returns>Number of items of the specific product in cart.</returns>
        public Task<int> CountProductInCart(Guid userId, Guid productId);
    }

    /// <summary>
    /// Implementation of IUser repository.
    /// </summary>
    public class UserRepository : Repository.IUser
    {
        private StoreContext _context;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public UserRepository(StoreContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Retrieves the default location for a user.
        /// </summary>
        /// <param name="user">The user to query.</param>
        /// <returns>The default location for the user.</returns>
        public async Task<Location> GetDefaultLocation(User user)
        {
            return await _context.Users
                                 .Include(c => c.DefaultLocation)
                                 .Where(c => c.UserId == user.UserId)
                                 .Select(c => c.DefaultLocation)
                                 .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the default location for a user.
        /// </summary>
        /// <param name="userId">The user id to query.</param>
        /// <returns>The default location for the user.</returns>
        public async Task<Location> GetDefaultLocation(Guid userId)
        {
            return await _context.Users
                .Include(c => c.DefaultLocation)
                .Where(c => c.UserId == userId)
                .Select(c => c.DefaultLocation)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>CreateUserAccountResult detailing the status of the operation.</returns>
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

        /// <summary>
        /// Retrieves a user based on their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The User, if found.</returns>
        public async Task<User> GetUserById(Guid id)
        {
            return await _context.Users
                                 .Where(c => c.UserId == id)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a user based on their login name.
        /// </summary>
        /// <param name="login">Login to search</param>
        /// <returns>A user, if found.</returns>
        public async Task<User> GetUserByLogin(string login)
        {
            login = login.ToLower();
            return await _context.Users
                                 .Where(c => c.Login.ToLower() == login)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }


        /// <summary>
        /// Retrieves the customer's currently open order for a location.
        /// <remarks>
        /// If the customer has no open order for the location, a new one will be
        /// created, so this method will always return an order object.
        /// </remarks>
        /// </summary>
        /// <param name="user">User to get order from.</param>
        /// <param name="location">Location to check.</param>
        /// <returns>Order for the location.</returns>
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

        /// <summary>
        /// Determines whether a login name exists.
        /// </summary>
        /// <param name="login">The login name to check.</param>
        /// <returns>Whether the login name already exists.</returns>
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

        /// <summary>
        /// Verifies that a users login name and password match.
        /// </summary>
        /// <param name="login">The login name.</param>
        /// <param name="plainPassword">The password supplied.</param>
        /// <returns>A User, if credentials match; null otherwise.</returns>
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

        /// <summary>
        /// Verifies if the login supplied meets requirements.
        /// </summary>
        /// <param name="login">The login name to check.</param>
        /// <returns>Whether requirements are satisfied.</returns>
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

        /// <summary>
        /// Updates user personal information.
        /// </summary>
        /// <param name="userId">The user ID to be updated.</param>
        /// <param name="newData">The new data to input.</param>
        /// <returns>Whether the operation was successful.</returns>
        public async Task<bool> UpdateUserPersonalInfo(Guid userId, IUserData newData)
        {
            // TODO: make this function less terrible

            var cityString = newData.GetCity();
            Entity.City cityEntity = null;
            if (cityString != null)
            {
                cityEntity = await _context.Addresses
                    .Where(a => a.City.Name.ToLower() == cityString.ToLower().Trim())
                    .Select(a => a.City)
                    .FirstOrDefaultAsync();
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
                    .FirstOrDefaultAsync();
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
                    .FirstOrDefaultAsync();
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
                    .FirstOrDefaultAsync();
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
                    .FirstOrDefaultAsync();
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
                .FirstOrDefaultAsync();

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
                    .FirstOrDefaultAsync();
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

        /// <summary>
        /// Retrieves the address of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>User address.</returns>
        public async Task<Address> GetAddressByUserId(Guid userId)
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

        /// <summary>
        /// Get the number of products in a user's current order.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <returns>Number of items in cart.</returns>
        public async Task<int> CountProductsInCart(Guid userId)
        {
            return await _context.OrderLineItems
                .Where(ol => ol.Order.TimeSubmitted == null)
                .Where(ol => ol.Order.User.UserId == userId)
                .SumAsync(ol => ol.Quantity);
        }

        /// <summary>
        /// Searches for a user based on a search query.
        /// </summary>
        /// <param name="nameQuery">Query</param>
        /// <returns>Users found, if any</returns>
        public IEnumerable<User> FindUserQuery(string nameQuery)
        {
            nameQuery = nameQuery.ToLower();

            // Search by either first name or last name when a space is present between two
            // search terms. Both search terms must be present in either the first or last
            // name (or both) of the customer in order to be considered a match.
            var nameComponents = nameQuery.Split(' ');
            if (nameComponents.Length == 2)
            {
                var query1 = FindUserQuery(nameComponents[0].Trim());
                var query2 = FindUserQuery(nameComponents[1].Trim());
                return query1.Intersect(query2);
            }

            // Search for names in a "lastName,firstName" fashion when a comma is present
            // in the search query.
            var lastThenFirst = nameQuery.Split(',');
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

        /// <summary>
        /// Searches for users based on a search query, and includes total revenue of the user.
        /// </summary>
        /// <param name="nameQuery">Query</param>
        /// <returns>Users found, if any.</returns>
        public UserQueryResultWithRevenue FindUserQueryIncludeRevenue(string nameQuery)
        {
            nameQuery = nameQuery.ToLower();

            // Search for names in a "lastName,firstName" fashion when a comma is present
            // in the search query.
            var lastThenFirst = nameQuery.Split(',');
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
                    QueryTerm1 = queryItem1,
                    QueryTerm2 = queryItem2,
                    IsOmniQuery = false,
                };
            }

            // Search by either first name or last name when a space is present between two
            // search terms. Both search terms must be present in either the first or last
            // name (or both) of the customer in order to be considered a match.
            var nameComponents = nameQuery.Split(' ');
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
                    QueryTerm1 = queryItem1,
                    QueryTerm2 = queryItem2,
                    IsOmniQuery = true,
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
                QueryTerm1 = nameQuery,
                QueryTerm2 = null,
                IsOmniQuery = true,
            };
        }

        /// <summary>
        /// Searches for users based on their first name.
        /// </summary>
        /// <param name="firstName">First name to search</param>
        /// <returns>Users found, if any.</returns>
        public IEnumerable<User> FindUserByFirstName(string firstName)
        {
            firstName = firstName.ToLower();
            return _context.Users
                .Where(u => u.FirstName.ToLower().Contains(firstName))
                .Select(u => u);
        }

        /// <summary>
        /// Searches for users based on their last name.
        /// </summary>
        /// <param name="lastName">Last name to search</param>
        /// <returns>Users found, if any.</returns>
        public IEnumerable<User> FindUserByLastName(string lastName)
        {
            lastName = lastName.ToLower();
            return _context.Users
                .Where(u => u.LastName.ToLower().Contains(lastName))
                .Select(u => u);
        }

        public async Task<int> CountProductInCart(Guid userId, Guid productId)
        {
            return await _context.OrderLineItems
                .Where(ol => ol.Order.TimeSubmitted == null)
                .Where(ol => ol.Order.User.UserId == userId)
                .Where(ol => ol.Product.ProductId == productId)
                .Select(ol => ol.Quantity)
                .FirstOrDefaultAsync();
        }
    }
}
