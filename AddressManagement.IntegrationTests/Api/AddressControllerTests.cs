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



    /// <summary>
    /// Prüft, ob der GET-Endpunkt ausschließlich Adressen
    /// mit der angegebenen Postleitzahl zurückliefert.
    /// </summary>
    [Fact]
    public async Task Test_WithPostalCodeFilter_ReturnMatchingAddresses()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var matchingAddress = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            City = "Lauenburg",
            PostalCode = "21481"
        };

        var otherAddress = new Address
        {
            FirstName = "Micky",
            LastName = "Maus",
            City = "Hamburg",
            PostalCode = "20095"
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
            "/api/addresses?postalCode=21481");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var addresses = await response.Content
            .ReadFromJsonAsync<List<Address>>();

        Assert.NotNull(addresses);
        Assert.Contains(addresses, a => a.Id == createdAddress.Id);
        Assert.All(addresses,
            a => Assert.Equal("21481", a.PostalCode));
    }


    /// <summary>
    /// Prüft, ob der GET-Endpunkt bei gleichzeitiger Angabe
    /// von Stadt und Postleitzahl nur passende Adressen zurückgibt.
    /// </summary>
    [Fact]
    public async Task Test_WithCityAndPostalCodeFilters_ReturnMatchingAddresses()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var matchingAddress = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            City = "Lauenburg",
            PostalCode = "21481"
        };

        var wrongPostalCode = new Address
        {
            FirstName = "Micky",
            LastName = "Maus",
            City = "Lauenburg",
            PostalCode = "20095"
        };

        var wrongCity = new Address
        {
            FirstName = "Dagobert",
            LastName = "Duck",
            City = "Hamburg",
            PostalCode = "21481"
        };

        var createResponse1 = await client.PostAsJsonAsync(
            "/api/addresses", matchingAddress);

        var createResponse2 = await client.PostAsJsonAsync(
            "/api/addresses", wrongPostalCode);

        var createResponse3 = await client.PostAsJsonAsync(
            "/api/addresses", wrongCity);

        Assert.Equal(HttpStatusCode.Created, createResponse1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createResponse2.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createResponse3.StatusCode);

        var createdAddress = await createResponse1.Content
            .ReadFromJsonAsync<Address>();

        Assert.NotNull(createdAddress);

        // Act
        var response = await client.GetAsync(
            "/api/addresses?city=Lauenburg&postalCode=21481");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var addresses = await response.Content
            .ReadFromJsonAsync<List<Address>>();

        Assert.NotNull(addresses);

        Assert.Contains(addresses, a => a.Id == createdAddress.Id);

        Assert.All(addresses, a =>
        {
            Assert.Equal("Lauenburg", a.City);
            Assert.Equal("21481", a.PostalCode);
        });
    }


    /// <summary>
    /// Aktualisiert eine vorhandene Adresse
    /// </summary>
    [Fact]
    public async Task Test_WithExistingAddress_UpdateAddress()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Entenstraße",
            HouseNumber = "1",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/addresses", address);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdAddress = await createResponse.Content
            .ReadFromJsonAsync<Address>();

        Assert.NotNull(createdAddress);

        var updatedAddress = new Address
        {
            Id = createdAddress.Id,
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Geldspeicherstraße",
            HouseNumber = "10",
            PostalCode = "54321",
            City = "Duckburg",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/addresses/{createdAddress.Id}",
            updatedAddress);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await client.GetAsync(
            $"/api/addresses/{createdAddress.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var savedAddress = await getResponse.Content
            .ReadFromJsonAsync<Address>();

        Assert.NotNull(savedAddress);
        Assert.Equal(createdAddress.Id, savedAddress.Id);
        Assert.Equal("Geldspeicherstraße", savedAddress.Street);
        Assert.Equal("10", savedAddress.HouseNumber);
        Assert.Equal("54321", savedAddress.PostalCode);
        Assert.Equal("Duckburg", savedAddress.City);
    }



    /// <summary>
    /// Prüft, ob beim Aktualisieren einer nicht vorhandenen
    /// Adresse HTTP 404 mit Problem Details zurückgegeben wird.
    /// </summary>
    [Fact]
    public async Task Test_UpdateWithUnknownId_ReturnNotFoundProblemDetails()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var unknownId = Guid.NewGuid();

        var address = new Address
        {
            Id = unknownId,
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Entenstraße",
            HouseNumber = "1",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/addresses/{unknownId}", address);

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
    /// Prüft, ob eine vorhandene Adresse über den
    /// DELETE-Endpunkt gelöscht werden kann.
    /// </summary>
    [Fact]
    public async Task Test_WithExistingAddress_DeleteAddress()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Entenstraße",
            HouseNumber = "1",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        var createResponse = await client.PostAsJsonAsync(
            "/api/addresses", address);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdAddress = await createResponse.Content
            .ReadFromJsonAsync<Address>();

        Assert.NotNull(createdAddress);

        // Act
        var response = await client.DeleteAsync(
            $"/api/addresses/{createdAddress.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await client.GetAsync(
            $"/api/addresses/{createdAddress.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }


    /// <summary>
    /// Prüft, ob eine Adresse ohne Vornamen
    /// mit HTTP 400 abgelehnt wird.
    /// </summary>
    [Fact]
    public async Task Test_CreateWithoutFirstName_ReturnBadRequest()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "",
            LastName = "Duck",
            Street = "Entenstraße",
            HouseNumber = "1",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("FirstName", problem.Errors.Keys);
    }


    /// <summary>
    /// Prüft, ob eine Adresse ohne Nachnamen
    /// mit HTTP 400 abgelehnt wird.
    /// </summary>
    [Fact]
    public async Task Test_CreateWithoutLastName_ReturnBadRequest()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "",
            Street = "Entenstraße",
            HouseNumber = "1",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("LastName", problem.Errors.Keys);
    }


    /// <summary>
    /// Prüft, ob eine Adresse ohne Straße
    /// mit HTTP 400 abgelehnt wird.
    /// </summary>
    [Fact]
    public async Task Test_CreateWithoutStreet_ReturnBadRequest()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            Street = "",
            HouseNumber = "1",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("Street", problem.Errors.Keys);
    }


    /// <summary>
    /// Prüft, ob eine Adresse ohne Hausnummer
    /// mit HTTP 400 abgelehnt wird.
    /// </summary>
    [Fact]
    public async Task Test_CreateWithoutHouseNumber_ReturnBadRequest()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Entenstraße",
            HouseNumber = "",
            PostalCode = "12345",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("HouseNumber", problem.Errors.Keys);
    }


    /// <summary>
    /// Prüft, ob eine Adresse ohne Postleitzahl
    /// mit HTTP 400 abgelehnt wird.
    /// </summary>
    [Fact]
    public async Task Test_CreateWithoutPostalCode_ReturnBadRequest()
    {
        // Arrange
        await using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var address = new Address
        {
            FirstName = "Donald",
            LastName = "Duck",
            Street = "Entenstraße",
            HouseNumber = "1",
            PostalCode = "",
            City = "Entenhausen",
            Country = "Deutschland"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/addresses", address);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains("PostalCode", problem.Errors.Keys);
    }


}
