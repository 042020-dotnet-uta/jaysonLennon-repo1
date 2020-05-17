using System;
using System.Collections.Generic;

namespace StoreApp.AdminModel
{
    public class CustomerSearchResultEntry {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Revenue { get; set; }
    }

    public class CustomerSearchResult {
        public string QueryTerm1 { get; set; }
        public string QueryTerm2 { get; set; }
        public bool IsOmniQuery { get; set; }
        public List<CustomerSearchResultEntry> Users { get; set; }
    }
}