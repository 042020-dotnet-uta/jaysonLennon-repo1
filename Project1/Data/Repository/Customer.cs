using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Data.Entity;

namespace StoreApp.Repository
{
    public interface ICustomer
    {
        IEnumerable<Customer> FindCustomerByFirstName(string firstName);
        IEnumerable<Customer> FindCustomerByLastName(string lastName);
        IEnumerable<Customer> FindCustomerByName(string name);
        Customer GetCustomerByLogin(string login);
        bool LoginExists(string login);
        Customer Add(Customer customer);
        bool SetDefaultLocation(Customer customer, Location location);
        Location GetDefaultLocation(Customer customer);
        IEnumerable<Order> GetOrderHistory(Customer customer);
    }

    public class CustomerRepository : ICustomer
    {
        private StoreContext _context;

        public CustomerRepository(StoreContext context)
        {
            this._context = context;
        }

        Customer ICustomer.Add(Customer customer)
        {
            throw new NotImplementedException();
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

        Customer ICustomer.GetCustomerByLogin(string login)
        {
            throw new NotImplementedException();
        }

        Location ICustomer.GetDefaultLocation(Customer customer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Order> ICustomer.GetOrderHistory(Customer customer)
        {
            throw new NotImplementedException();
        }

        bool ICustomer.LoginExists(string login)
        {
            throw new NotImplementedException();
        }

        bool ICustomer.SetDefaultLocation(Customer customer, Location location)
        {
            throw new NotImplementedException();
        }
    }
}