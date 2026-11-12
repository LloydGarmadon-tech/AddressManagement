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


    /// <summary>
    /// Ruft eine Adresse anhand ihrer eindeutigen ID ab.
    /// </summary>
    /// <param name="id">Die eindeutige ID der Adresse.</param>
    /// <returns>
    /// HTTP 200 mit der gefundenen Adresse oder
    /// HTTP 404, wenn die Adresse nicht existiert.
    /// </returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Address>> GetByIdAsync(Guid id)
    {
        var address = await _addressService.GetByIdAsync(id);

        if (address is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Adresse nicht gefunden",
                detail: $"Die Adresse mit der ID {id} existiert nicht.");
        }

        return Ok(address);
    }



    /// <summary>
    /// Gibt alle gespeicherten Adressen zurück.
    /// Optional kann nach Stadt und Postleitzahl gefiltert werden.
    /// </summary>
    /// <param name="city">Optionale Stadt.</param>
    /// <param name="postalCode">Optionale Postleitzahl.</param>
    /// <returns>
    /// HTTP 200 mit einer Liste der passenden Adressen.
    /// Bei fehlenden Treffern wird eine leere Liste zurückgegeben.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Address>>> GetAllAsync(
        [FromQuery] string? city,
        [FromQuery] string? postalCode)
    {
        var addresses = await _addressService.GetAllAsync(city, postalCode);

        return Ok(addresses);
    }

}
