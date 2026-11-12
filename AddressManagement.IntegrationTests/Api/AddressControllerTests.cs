using AddressManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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


    /// <summary>
    /// Prüft, ob eine vorhandene Adresse anhand ihrer ID
    /// über die REST-API abgerufen werden kann.
    /// </summary>
    [Fact]
    public async Task Test_WithExistingId_ReturnAddress()
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

        var createResponse = await client.PostAsJsonAsync(
            "/api/addresses",
            address);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdAddress =
            await createResponse.Content.ReadFromJsonAsync<Address>();

        Assert.NotNull(createdAddress);

        // Act
        var response = await client.GetAsync(
            $"/api/addresses/{createdAddress.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var retrievedAddress =
            await response.Content.ReadFromJsonAsync<Address>();

        Assert.NotNull(retrievedAddress);
        Assert.Equal(createdAddress.Id, retrievedAddress.Id);
        Assert.Equal("Donald", retrievedAddress.FirstName);
        Assert.Equal("Duck", retrievedAddress.LastName);
    }


    /// <summary>
    /// Prüft, ob bei einer unbekannten Adress-ID
    /// HTTP 404 mit einer Problem-Details-Antwort zurückgegeben wird.
    /// </summary>
    [Fact]
    public async Task Test_WithUnknownId_ReturnNotFoundProblemDetails()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var unknownId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync(
            $"/api/addresses/{unknownId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
        Assert.Equal("Adresse nicht gefunden", problem.Title);
        Assert.Contains(unknownId.ToString(), problem.Detail);
    }

    /// <summary>
    /// Prüft, ob alle vorhandenen Adressen über die
    /// REST-API abgerufen werden können.
    /// </summary>
    [Fact]
    public async Task Test_WithoutFilters_ReturnAllAddresses()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address1 = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            City = "Entenhausen"
        };

        var address2 = new Address
        {
            FirstName = "Micky",
            LastName = "Maus",
            City = "Maushausen"
        };

        var response1 = await client.PostAsJsonAsync(
            "/api/addresses", address1);

        var response2 = await client.PostAsJsonAsync(
            "/api/addresses", address2);

        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);

        // Act
        var response = await client.GetAsync("/api/addresses");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var addresses = await response.Content
            .ReadFromJsonAsync<List<Address>>();

        Assert.NotNull(addresses);
        Assert.Contains(addresses, a => a.FirstName == "Donald");
        Assert.Contains(addresses, a => a.FirstName == "Micky");
    }



    /// <summary>
    /// Prüft, ob der GET-Endpunkt ausschließlich Adressen
    /// der angegebenen Stadt zurückliefert.
    /// </summary>
    [Fact]
    public async Task Test_WithCityFilter_ReturnMatchingAddresses()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var matchingAddress = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            City = "Entenhausen",
            PostalCode = "12345"
        };

        var otherAddress = new Address
        {
            FirstName = "Micky",
            LastName = "Maus",
            City = "Maushausen",
            PostalCode = "54321"
        };

        var createResponse1 = await client.PostAsJsonAsync(
            "/api/addresses", matchingAddress);

        var createResponse2 = await client.PostAsJsonAsync(
            "/api/addresses", otherAddress);

        Assert.Equal(HttpStatusCode.Created, createResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createResponse2.StatusCode);

        var createdAddress = await createResponse1.Content
            .ReadFromJsonAsync<Address>();

        Assert.NotNull(createdAddress);

        // Act
        var response = await client.GetAsync(
            "/api/addresses?city=Entenhausen");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var addresses = await response.Content
            .ReadFromJsonAsync<List<Address>>();

        Assert.NotNull(addresses);
        Assert.Contains(addresses, a => a.Id == createdAddress.Id);
        Assert.All(addresses, a => Assert.Equal("Entenhausen", a.City));
    }

}
