using System;
using System.Collections.Generic;
using StoreApp.Data;
using StoreApp.Data.Entity;

namespace StoreApp.Repository
{
    public interface ICustomerRepository
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

    public class CustomerRepository : ICustomerRepository
    {
        private StoreContext _context;

        public CustomerRepository(StoreContext context)
        {
            this._context = context;
        }

        Customer ICustomerRepository.Add(Customer customer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Customer> ICustomerRepository.FindCustomerByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Customer> ICustomerRepository.FindCustomerByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Customer> ICustomerRepository.FindCustomerByName(string name)
        {
            throw new NotImplementedException();
        }

        Customer ICustomerRepository.GetCustomerByLogin(string login)
        {
            throw new NotImplementedException();
        }

        Location ICustomerRepository.GetDefaultLocation(Customer customer)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Order> ICustomerRepository.GetOrderHistory(Customer customer)
        {
            throw new NotImplementedException();
        }

        bool ICustomerRepository.LoginExists(string login)
        {
            throw new NotImplementedException();
        }

        bool ICustomerRepository.SetDefaultLocation(Customer customer, Location location)
        {
            throw new NotImplementedException();
        }
    }
}