using System.ComponentModel.DataAnnotations;

namespace AddressManagement.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Vorname ist erforderlich.")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nachname ist erforderlich.")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Straße ist erforderlich.")]
        public string Street { get; set; } = string.Empty;
        [Required(ErrorMessage = "Hausnummer ist erforderlich.")]
        public string HouseNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string? Email { get; set; }
    }
}
