using System;

namespace StoreApp.Entity
{
    /// <summary>
    /// Contains all customer-related information such as name, phone number, and address.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// The ID for this <c>Customer</c> object.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The <c>Login</c> name for this customer.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// The first name of this customer.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of this customer.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The address of this customer.
        /// </summary>
        public virtual Entity.Address Address { get; set; }

        /// <summary>
        /// The phone number for this customer.
        /// </summary>
        public string PhoneNumber { get; set; }

        private string _Password;
        /// <summary>
        /// The password for this customer.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The default <c>Location</c> where this customer's orders should be placed.
        /// </summary>
        /// <value>If this is null, then the customer has not set a default <c>Location</c>.</value>
        public virtual Location DefaultLocation { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public Customer(){}
    }
}