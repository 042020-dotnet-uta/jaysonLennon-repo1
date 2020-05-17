using System;

namespace StoreApp.Entity
{
    /// <summary>
    /// Consists of the components needed to form a full address.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// The ID for this <c>Address</c>.
        /// </summary>
        public Guid AddressId { get; set; }

        /// <summary>
        /// Line1 for this <c>Address</c>.
        /// </summary>
        public virtual AddressLine1 Line1 { get; set; }

        /// <summary>
        /// Line2 for this <c>Address</c>.
        /// </summary>
        public virtual AddressLine2 Line2 { get; set; }

        /// <summary>
        /// The City for this <c>Address</c>.
        /// </summary>
        public virtual City City { get; set; }

        /// <summary>
        /// The State for this <c>Address</c>.
        /// </summary>
        public virtual State State { get; set; }

        /// <summary>
        /// The Zip Code for this <c>Address</c>.
        /// </summary>
        public virtual ZipCode Zip { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public Address(){}
    }

    /// <summary>
    /// The first line of an <c>Address</c>.
    /// Line1 information is generally a street address.
    /// </summary>
    public class AddressLine1
    {
        /// <summary>
        /// ID for this address line.
        /// </summary>
        public Guid AddressLine1Id { get; set; }

        /// <summary>
        /// The information for this address line.
        /// </summary>
        /// <value>This would generally be a street address.</value>
        public string Data { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public AddressLine1(){}
    }

    /// <summary>
    /// The second line of an <c>Address</c>.
    /// Line2 information is generally a unit/apt number or PO box.
    /// </summary>
    public class AddressLine2
    {
        /// <summary>
        /// The ID for this address line.
        /// </summary>
        public Guid AddressLine2Id { get; set; }

        /// <summary>
        /// The information for this address line.
        /// </summary>
        /// <value>This would generally be a unit number or PO box.</value>
        public string Data { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public AddressLine2(){}
    }

    /// <summary>
    /// The City of an <c>Address</c>.
    /// </summary>
    public class City
    {
        /// <summary>
        /// The ID of this <c>City</c>.
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// The name of this <c>City</c>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public City(){}
    }

    /// <summary>
    /// The State of an <c>Address</c>.
    /// </summary>
    public class State
    {
        /// <summary>
        /// The ID of this <c>State</c>.
        /// </summary>
        public Guid StateId { get; set; }

        /// <summary>
        /// The name of this <c>State</c>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public State(){}
    }

    /// <summary>
    /// The Zip Code of an <c>Address</c>.
    /// </summary>
    public class ZipCode
    {
        /// <summary>
        /// The ID of this <c>ZipCode</c>.
        /// </summary>
        public Guid ZipCodeId { get; set; }

        /// <summary>
        /// The Zip code.
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Needed for EF.
        /// </summary>
        public ZipCode(){}
    }
}