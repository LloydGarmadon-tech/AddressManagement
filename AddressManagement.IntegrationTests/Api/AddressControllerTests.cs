using AddressManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace AddressManagement.IntegrationTests.Api;

/// <summary>
/// Prüft die HTTP-Endpunkte der Adressverwaltung.
/// </summary>
public class AddressControllerTests
{
    /// <summary>
    /// Prüft, ob beim Anlegen einer Adresse
    /// HTTP 201 Created zurückgegeben wird.
    /// </summary>
    [Fact]
    public async Task Test_WithValidAddress_ReturnCreated()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Ententeich",
            HouseNumber = "11",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/addresses",
            address);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdAddress =
            await response.Content.ReadFromJsonAsync<Address>();

        Assert.NotNull(createdAddress);
        Assert.NotEqual(Guid.Empty, createdAddress.Id);

        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith(
            $"/api/addresses/{createdAddress.Id}",
            response.Headers.Location.ToString());
    }
}
