using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using StoreApp.Repository;

namespace StoreApp.Model.Input
{
    /// <summary>
    /// Used for the "Account Management" page for users.
    /// </summary>
    // TODO: Add constraints
    public class AccountManagement : Repository.IUserData
    {
        /// <summary>The user's first name.</summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>The user's last name.</summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>The user's address line 1.</summary>
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        /// <summary>The user's address line 2.</summary>
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        /// <summary>The user's city.</summary>
        [Display(Name = "City")]
        public string City { get; set; }

        /// <summary>The user's state.</summary>
        [Display(Name = "State")]
        public string StatePicked { get; set; }

        /// <summary>The list of available state options the user may pick.</summary>
        public List<SelectListItem> States { get; } = new List<SelectListItem>()
        {
            new SelectListItem { Value = null, Text = "-- State --" },
            new SelectListItem { Value = "AL", Text = "Alabama" },
            new SelectListItem { Value = "AK", Text = "Alaska" },
            new SelectListItem { Value = "AZ", Text = "Arizona" },
            new SelectListItem { Value = "AR", Text = "Arkansas" },
            new SelectListItem { Value = "CA", Text = "California" },
            new SelectListItem { Value = "CO", Text = "Colorado" },
            new SelectListItem { Value = "CT", Text = "Connecticut" },
            new SelectListItem { Value = "DE", Text = "Delaware" },
            new SelectListItem { Value = "FL", Text = "Florida" },
            new SelectListItem { Value = "GA", Text = "Georgia" },
            new SelectListItem { Value = "HI", Text = "Hawaii" },
            new SelectListItem { Value = "ID", Text = "Idaho" },
            new SelectListItem { Value = "IL", Text = "Illinois" },
            new SelectListItem { Value = "IN", Text = "Indiana" },
            new SelectListItem { Value = "IA", Text = "Iowa" },
            new SelectListItem { Value = "KS", Text = "Kansas" },
            new SelectListItem { Value = "KY", Text = "Kentucky" },
            new SelectListItem { Value = "LA", Text = "Louisiana" },
            new SelectListItem { Value = "ME", Text = "Maine" },
            new SelectListItem { Value = "MD", Text = "Maryland" },
            new SelectListItem { Value = "MA", Text = "Massachusetts" },
            new SelectListItem { Value = "MI", Text = "Michigan" },
            new SelectListItem { Value = "MN", Text = "Minnesota" },
            new SelectListItem { Value = "MS", Text = "Mississippi" },
            new SelectListItem { Value = "MO", Text = "Missouri" },
            new SelectListItem { Value = "MT", Text = "Montana" },
            new SelectListItem { Value = "NE", Text = "Nebraska" },
            new SelectListItem { Value = "NV", Text = "Nevada" },
            new SelectListItem { Value = "NH", Text = "New Hampshire" },
            new SelectListItem { Value = "NJ", Text = "New Jersey" },
            new SelectListItem { Value = "NM", Text = "New Mexico" },
            new SelectListItem { Value = "NY", Text = "New York" },
            new SelectListItem { Value = "NC", Text = "North Carolina" },
            new SelectListItem { Value = "ND", Text = "North Dakota" },
            new SelectListItem { Value = "OH", Text = "Ohio" },
            new SelectListItem { Value = "OK", Text = "Oklahoma" },
            new SelectListItem { Value = "OR", Text = "Oregon" },
            new SelectListItem { Value = "PA", Text = "Pennsylvania" },
            new SelectListItem { Value = "RI", Text = "Rhode Island" },
            new SelectListItem { Value = "SC", Text = "South Carolina" },
            new SelectListItem { Value = "SD", Text = "South Dakota" },
            new SelectListItem { Value = "TN", Text = "Tennessee" },
            new SelectListItem { Value = "TX", Text = "Texas" },
            new SelectListItem { Value = "UT", Text = "Utah" },
            new SelectListItem { Value = "VT", Text = "Vermont" },
            new SelectListItem { Value = "VA", Text = "Virginia" },
            new SelectListItem { Value = "WA", Text = "Washington" },
            new SelectListItem { Value = "WV", Text = "West Virginia" },
            new SelectListItem { Value = "WI", Text = "Wisconsin" },
            new SelectListItem { Value = "WY", Text = "Wyoming" },

            new SelectListItem { Value = "DC", Text = "District of Columbia" },
            new SelectListItem { Value = "MH", Text = "Marshall Islands" },

            new SelectListItem { Value = "AE", Text = "Armed Forces Africa" },
            new SelectListItem { Value = "AA", Text = "Armed Forces Americas" },
            new SelectListItem { Value = "AE", Text = "Armed Forces Canada" },
            new SelectListItem { Value = "AE", Text = "Armed Forces Europe" },
            new SelectListItem { Value = "AE", Text = "Armed Forces Middle East" },
            new SelectListItem { Value = "AP", Text = "Armed Forces Pacific" },
        };

        /// <summary>The user's zip code.</summary>
        [RegularExpression(@"^[0-9]{5}(-?[0-9]{4})?$", ErrorMessage = "Zip code must be 5 or 9 digits.")]
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

        /// <summary>The default store for the user.</summary>
        [Required]
        [Display(Name = "Default Store", Description = "Store to order items from.")]
        public string StorePicked { get; set; }

        /// <summary>The list of available stores the user may choose from.</summary>
        public List<SelectListItem> Stores { get; } = new List<SelectListItem>();

        /// <summary>Returns the user's address line 1.</summary>
        string IUserData.GetAddressLine1() => this.AddressLine1;

        /// <summary>Returns the user's address line 2.</summary>
        string IUserData.GetAddressLine2() => this.AddressLine2;

        /// <summary>Returns the user's city.</summary>
        string IUserData.GetCity() => this.City;

        /// <summary>Returns the user's first name.</summary>
        string IUserData.GetFirstName() => this.FirstName;

        /// <summary>Returns the user's last name.</summary>
        string IUserData.GetLastName() => this.LastName;

        /// <summary>Returns the user's state.</summary>
        string IUserData.GetState() => this.StatePicked;

        /// <summary>Returns the user's zip code.</summary>
        string IUserData.GetZip() => this.Zip;
    }
}