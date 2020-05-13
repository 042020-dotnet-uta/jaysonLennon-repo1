using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    public interface ICustomer
    {
        IEnumerable<Customer> FindCustomerByFirstName(string firstName);
        IEnumerable<Customer> FindCustomerByLastName(string lastName);
        IEnumerable<Customer> FindCustomerByName(string name);
        Task<Customer> GetCustomerByLogin(string login);
        Task<Customer> GetCustomerById(Guid id);
        Task<bool> LoginExists(string login);
        Task<CreateUserAccountResult> Add(Customer customer);
        Task<bool> VerifyUserLogin(string login);
        void SetDefaultLocation(Customer customer, Location location);
        Task<Location> GetDefaultLocation(Customer customer);
        IEnumerable<Order> GetOrderHistory(Customer customer);
        Task<Customer> VerifyCredentials(string login, string plainPassword);
    }


    public class CustomerRepository : Repository.ICustomer
    {
        private StoreContext _context;

        public CustomerRepository(StoreContext context)
        {
            this._context = context;
        }

        public async Task<Location> GetDefaultLocation(Customer customer)
        {
            return await _context.Customers
                                 .Include(c => c.DefaultLocation)
                                 .Select(c => c.DefaultLocation)
                                 .FirstOrDefaultAsync();
        }

        async Task<CreateUserAccountResult> ICustomer.Add(Customer customer)
        {
            var hashed = StoreApp.Util.Hash.Sha256(customer.Password);
            customer.Password = hashed;
            if (String.IsNullOrEmpty(customer.Login)) return CreateUserAccountResult.MissingLogin;
            if (String.IsNullOrEmpty(customer.Password)) return CreateUserAccountResult.MissingPassword;

            var loginExists = await _context.Customers.Where(c => c.Login == customer.Login.ToLower()).SingleOrDefaultAsync();
            if (loginExists != null) return CreateUserAccountResult.AccountNameExists;

            await _context.AddAsync(customer);
            await _context.SaveChangesAsync();
            return CreateUserAccountResult.Ok;
        }

        IEnumerable<Customer> ICustomer.FindCustomerByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Customer> ICustomer.FindCustomerByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Customer> ICustomer.FindCustomerByName(string name)
        {
            throw new NotImplementedException();
        }

        async Task<Customer> ICustomer.GetCustomerById(Guid id)
        {
            return await _context.Customers
                                 .Where(c => c.CustomerId == id)
                                 .Select(c => c)
                                 .FirstOrDefaultAsync();
        }

        async Task<Customer> ICustomer.GetCustomerByLogin(string login)
        {
            login = login.ToLower();
            return await _context.Customers
                                 .Where(c => c.Login.ToLower() == login)
                                 .Select(c => c)
                                 .FirstOrDefaultAsync();
        }

        IEnumerable<Order> ICustomer.GetOrderHistory(Customer customer)
        {
            throw new NotImplementedException();
        }

        async Task<bool> ICustomer.LoginExists(string login)
        {
            login = login.ToLower();
            return await _context.Customers
                           .Where(c => c.Login.ToLower() == login)
                           .Select(c => c)
                           .SingleOrDefaultAsync() != null;
        }

        async void ICustomer.SetDefaultLocation(Customer customer, Location location)
        {
            customer.DefaultLocation = location;
            await _context.SaveChangesAsync();
        }

        async Task<Customer> ICustomer.VerifyCredentials(string login, string plainPassword)
        {
            login = login.ToLower();
            var hashed = StoreApp.Util.Hash.Sha256(plainPassword);
            return await _context.Customers
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
            var exists = await _context.Customers
                                       .Where(c => c.Login.ToLower() == login)
                                       .SingleOrDefaultAsync();
            return exists == null;
        }
    }
}