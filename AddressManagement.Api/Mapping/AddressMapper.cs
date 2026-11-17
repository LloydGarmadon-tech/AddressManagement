
using AddressManagement.Api.DTOs;
using AddressManagement.Domain.Entities;

namespace AddressManagement.Api.Mapping;

/// <summary>
/// Übernimmt die Umwandlung zwischen API-DTOs
/// und Domain-Entitäten.
/// </summary>
public static class AddressMapper
{
    /// <summary>
    /// Wandelt einen CreateAddressRequest
    /// in eine Address-Entität um.
    /// </summary>
    public static Address ToAddress(CreateAddressRequest request)
    {
        return new Address
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            PostalCode = request.PostalCode,
            City = request.City,
            Country = request.Country,
            Email = request.Email
        };
    }


    /// <summary>
    /// Wandelt einen UpdateAddressRequest in eine
    /// Address-Entität mit der angegebenen ID um.
    /// </summary>
    public static Address ToAddress(UpdateAddressRequest request, Guid id)
    {
        return new Address
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            PostalCode = request.PostalCode,
            City = request.City,
            Country = request.Country,
            Email = request.Email
        };
    }
}
