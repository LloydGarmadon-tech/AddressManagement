using AddressManagement.Api.DTOs;
using AddressManagement.Api.Mapping;
using AddressManagement.Application.Services;
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
    public async Task<ActionResult<AddressResponse>> CreateAsync(
        [FromBody] CreateAddressRequest request)
    {
        var address = AddressMapper.ToAddress(request);

        var createdAddress = await _addressService.CreateAsync(address);

        var response = AddressMapper.ToResponse(createdAddress);

        return Created($"/api/addresses/{response.Id}", response);
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
    public async Task<ActionResult<AddressResponse>> GetByIdAsync(Guid id)
    {
        var address = await _addressService.GetByIdAsync(id);

        if (address is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Adresse nicht gefunden",
                detail: $"Die Adresse mit der ID {id} existiert nicht.");
        }

        var response = AddressMapper.ToResponse(address);

        return Ok(response);
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
    public async Task<ActionResult<IReadOnlyList<AddressResponse>>> GetAllAsync(
        [FromQuery] string? city,
        [FromQuery] string? postalCode)
    {
        var addresses = await _addressService.GetAllAsync(city, postalCode);

        var response = addresses
            .Select(AddressMapper.ToResponse)
            .ToList();

        return Ok(response);
    }



    /// <summary>
    /// Aktualisiert eine vorhandene Adresse.
    /// </summary>
    /// <param name="id">ID der zu aktualisierenden Adresse.</param>
    /// <param name="address">Die neuen Adressdaten.</param>
    /// <returns>
    /// HTTP 204 bei erfolgreicher Aktualisierung.
    /// HTTP 404, wenn die Adresse nicht existiert.
    /// </returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
    Guid id,
    [FromBody] UpdateAddressRequest request)
    {
        var address = AddressMapper.ToAddress(request, id);

        var updated = await _addressService.UpdateAsync(address);

        if (!updated)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Adresse nicht gefunden",
                detail: $"Die Adresse mit der ID {id} existiert nicht.");
        }

        return NoContent();
    }



    /// <summary>
    /// Löscht eine vorhandene Adresse anhand ihrer ID.
    /// </summary>
    /// <param name="id">ID der zu löschenden Adresse.</param>
    /// <returns>
    /// HTTP 204 bei erfolgreicher Löschung.
    /// HTTP 404, wenn die Adresse nicht existiert.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        // Adresse über den Service löschen.
        var deleted = await _addressService.DeleteAsync(id);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Adresse nicht gefunden",
                detail: $"Die Adresse mit der ID {id} existiert nicht.");
        }

        return NoContent();
    }


}
