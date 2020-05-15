using System;
using StoreApp.Data;
using StoreApp.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        /// <summary>Failed to create account: no login name was present in <c>Customer</c> object.</summary>
        MissingLogin,

        /// <summary>Failed to create account: no password was present in the <c>Customer</c> object.</summary>
        MissingPassword,
    }

    public interface ICustomerData
    {
        string GetFirstName();
        string GetLastName();
        string GetAddressLine1();
        string GetAddressLine2();
        string GetCity();
        string GetState();
        string GetZip();
    }

    public interface ICustomer
    {
        Task<User> GetCustomerByLogin(string login);
        Task<User> GetCustomerById(Guid userId);
        Task<Address> GetAddressByuserId(Guid userId);
        Task<bool> LoginExists(string login);
        Task<CreateUserAccountResult> Add(User customer);
        Task<bool> VerifyUserLogin(string login);
        void SetDefaultLocation(User customer, Location location);
        Task<Location> GetDefaultLocation(User customer);
        Task<Location> GetDefaultLocation(Guid userId);
        Task<Order> GetOpenOrder(User customer, Location location);
        Task<User> VerifyCredentials(string login, string plainPassword);
        Task<bool> UpdateCustomerInfo(Guid userId, ICustomerData newData);
        Task<int> CountProductsInCart(Guid userId);
    }


    public class CustomerRepository : Repository.ICustomer
    {
        private StoreContext _context;

        public CustomerRepository(StoreContext context)
        {
            this._context = context;
        }

        public async Task<Location> GetDefaultLocation(User customer)
        {
            return await _context.Users
                                 .Include(c => c.DefaultLocation)
                                 .Where(c => c.UserId == customer.UserId)
                                 .Select(c => c.DefaultLocation)
                                 .SingleOrDefaultAsync();
        }

        async Task<Location> ICustomer.GetDefaultLocation(Guid userId)
        {
            return await _context.Users
                .Include(c => c.DefaultLocation)
                .Where(c => c.UserId == userId)
                .Select(c => c.DefaultLocation)
                .SingleOrDefaultAsync();
        }

        async Task<CreateUserAccountResult> ICustomer.Add(User customer)
        {
            var hashed = StoreApp.Util.Hash.Sha256(customer.Password);
            customer.Password = hashed;
            if (String.IsNullOrEmpty(customer.Login)) return CreateUserAccountResult.MissingLogin;
            if (String.IsNullOrEmpty(customer.Password)) return CreateUserAccountResult.MissingPassword;

            var loginExists = await _context.Users.Where(c => c.Login == customer.Login.ToLower()).SingleOrDefaultAsync();
            if (loginExists != null) return CreateUserAccountResult.AccountNameExists;

            await _context.AddAsync(customer);
            await _context.SaveChangesAsync();
            return CreateUserAccountResult.Ok;
        }

        async Task<User> ICustomer.GetCustomerById(Guid id)
        {
            return await _context.Users
                                 .Where(c => c.UserId == id)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }

        async Task<User> ICustomer.GetCustomerByLogin(string login)
        {
            login = login.ToLower();
            return await _context.Users
                                 .Where(c => c.Login.ToLower() == login)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }


        async Task<Order> ICustomer.GetOpenOrder(User customer, Location location)
        {
            var currentOrder = await _context.Orders
                                             .Include(o => o.Location)
                                             .Where(o => o.Customer.UserId == customer.UserId)
                                             .Where(o => o.TimeSubmitted == null)
                                             .Where(o => o.Location.LocationId == location.LocationId)
                                             .Select(o => o)
                                             .SingleOrDefaultAsync();
            if (currentOrder == null)
            {
                var newOrder = new Entity.Order(customer, location);
                _context.Add(newOrder);
                await _context.SaveChangesAsync();
                return newOrder;
            } else {
                return currentOrder;
            }
        }

        async Task<bool> ICustomer.LoginExists(string login)
        {
            login = login.ToLower();
            return await _context.Users
                           .Where(c => c.Login.ToLower() == login)
                           .Select(c => c)
                           .SingleOrDefaultAsync() != null;
        }

        async void ICustomer.SetDefaultLocation(User customer, Location location)
        {
            customer.DefaultLocation = location;
            await _context.SaveChangesAsync();
        }

        async Task<User> ICustomer.VerifyCredentials(string login, string plainPassword)
        {
            login = login.ToLower();
            var hashed = StoreApp.Util.Hash.Sha256(plainPassword);
            return await _context.Users
                                 .Where(c => c.Login.ToLower() == login)
                                 .Where(c => c.Password == hashed)
                                 .Select(c => c)
                                 .SingleOrDefaultAsync();
        }

        async Task<bool> ICustomer.VerifyUserLogin(string login)
        {
            if (String.IsNullOrEmpty(login)) return false;
            login = login.ToLower();
            // TODO: additional validation rules
            var exists = await _context.Users
                                       .Where(c => c.Login.ToLower() == login)
                                       .SingleOrDefaultAsync();
            return exists == null;
        }

        async Task<bool> ICustomer.UpdateCustomerInfo(Guid userId, ICustomerData newData)
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

            var customer = await _context.Users
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
            if (customer.Address == null)
            {
                address = new Entity.Address();
                _context.Add(address);
                customer.Address = address;
            } else {
                address = await _context.Addresses
                    .Where(a => a.AddressId == customer.Address.AddressId)
                    .Select(a => a)
                    .SingleOrDefaultAsync();
            }

            if (newData.GetFirstName() != null) customer.FirstName = newData.GetFirstName().Trim();
            else customer.FirstName = null;

            if (newData.GetLastName() != null) customer.LastName = newData.GetLastName().Trim();
            else customer.LastName = null;

            address.City = cityEntity;
            address.State = stateEntity;
            address.Zip = zipEntity;
            address.Line1 = addressLine1Entity;
            address.Line2 = addressLine2Entity;

            await _context.SaveChangesAsync();

            return true;
        }

        async Task<Address> ICustomer.GetAddressByuserId(Guid userId)
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

        async Task<int> ICustomer.CountProductsInCart(Guid userId)
        {
            return await _context.OrderLineItems
                .Where(ol => ol.Order.TimeSubmitted == null)
                .Where(ol => ol.Order.Customer.UserId == userId)
                .SumAsync(ol => ol.Quantity);
        }
    }
}