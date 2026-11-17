
using System.ComponentModel.DataAnnotations;

namespace AddressManagement.Api.DTOs;

/// <summary>
/// Enthält die Daten zum Erstellen einer neuen Adresse.
/// </summary>
public class CreateAddressRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Street { get; set; } = string.Empty;

    [Required]
    public string HouseNumber { get; set; } = string.Empty;

    [Required]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }
}
