using System;
using System.Collections.Generic;
using System.Text;

namespace AddressManagement.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
    
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    
        public string? Email { get; set; }
    }
}
