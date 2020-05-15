using System;
using System.Collections.Generic;

namespace StoreApp.Entity
{
    public enum Role
    {
        User,
        Admin
    }

    /// <summary>
    /// Contains all user-related information such as name, phone number, and address.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The ID for this <c>User</c> object.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The <c>Login</c> name for this user.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// The first name of this user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of this user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The address of this user.
        /// </summary>
        public virtual Entity.Address Address { get; set; }

        /// <summary>
        /// The phone number for this user.
        /// </summary>
        public string PhoneNumber { get; set; }

        private string _Password;
        /// <summary>
        /// The password for this user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The default <c>Location</c> where this user's orders should be placed.
        /// </summary>
        /// <value>If this is null, then the user has not set a default <c>Location</c>.</value>
        public virtual Location DefaultLocation { get; set; }

        public Role Role { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public User(){}
    }
}