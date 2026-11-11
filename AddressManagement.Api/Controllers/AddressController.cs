using AddressManagement.Application.Services;
using AddressManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AddressManagement.Api.Controllers;

/// <summary>
/// Stellt die REST-Endpunkte zur Verwaltung von Adressen bereit.
/// </summary>
[ApiController]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly AddressService _addressService;

    /// <summary>
    /// Initialisiert den Controller mit dem Adressservice.
    /// </summary>
    /// <param name="addressService">Service zur Adressverwaltung.</param>
    public AddressesController(AddressService addressService)
    {
        _addressService = addressService;
    }

    /// <summary>
    /// Erstellt eine neue Adresse.
    /// </summary>
    /// <param name="address">Die anzulegende Adresse.</param>
    /// <returns>HTTP 201 Created mit der erstellten Adresse.</returns>
    [HttpPost]
    public async Task<ActionResult<Address>> CreateAsync(
        [FromBody] Address address)
    {
        var createdAddress = await _addressService.CreateAsync(address);

        return Created(
            $"/api/addresses/{createdAddress.Id}",
            createdAddress);
    }
}
