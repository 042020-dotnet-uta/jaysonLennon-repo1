using System;
using System.Collections.Generic;

namespace StoreApp.AdminModel
{
    /// <summary>
    /// A row in the result table when searching for customers.
    /// </summary>
    public class CustomerSearchResultEntry {
        /// <summary>The user's ID</summary>
        public Guid UserId { get; set; }

        /// <summary>The user's first name</summary>
        public string FirstName { get; set; }

        /// <summary>The user's last name</summary>
        public string LastName { get; set; }

        /// <summary>The user's revenue</summary>
        public double Revenue { get; set; }
    }

    public class CustomerSearchResult {

        /// <summary>
        /// Query term 1 as input by the user.
        /// <remarks>
        /// See the documentation under StoreApp.Repository.UserQueryResultWithRevenue
        /// for details on what this value may represent.
        /// </remarks>
        /// </summary>
        public string QueryTerm1 { get; set; }

        /// <summary>
        /// Query term 2 as input by the user.
        /// <remarks>
        /// See the documentation under StoreApp.Repository.UserQueryResultWithRevenue
        /// for details on what this value may represent.
        /// </remarks>
        /// </summary>
        public string QueryTerm2 { get; set; }

        /// <summary>
        /// Whether this query is "lastname,firstname" query, or a "search everything" query.
        /// <remarks>
        /// See the documentation under StoreApp.Repository.UserQueryResultWithRevenue
        /// for details on how this is used.
        /// </remarks>
        /// </summary>
        public bool IsOmniQuery { get; set; }

        /// <summary>The users found in the query.</summary>
        public List<CustomerSearchResultEntry> Users { get; set; }
    }
}