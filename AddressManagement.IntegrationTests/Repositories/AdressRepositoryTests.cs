using AddressManagement.Domain.Entities;
using AddressManagement.Infrastucture.Persistence;
using AddressManagement.Infrastucture.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AddressManagement.IntegrationTests.Repositories
{
    /// <summary>
    /// Integrationstests für die Speicherung und Verwaltung von Adressen in der Datenbank.
    /// </summary>
    public class AdressRepositoryTests
    {
        /// <summary>
        /// Prüft, ob eine gültige Adresse im Datenbankkontext
        /// gespeichert und anschließend wieder gelesen werden kann.
        /// </summary>
        [Fact]
        public async Task Test_WithValidAddress_PersistAddress()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);

            var repository = new AddressRepository(context);

            var address = new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                Street = "Ententeich",
                HouseNumber = "11",
                PostalCode = "4711",
                City = "Entenhausen",
                Country = "Deutschland"
            };

            // Act
            await repository.CreateAsync(address);

            // Assert
            var savedAddress = await context.Addresses
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == address.Id);

            Assert.NotNull(savedAddress);
            Assert.Equal(address.FirstName, savedAddress.FirstName);
            Assert.Equal(address.LastName, savedAddress.LastName);
        }


        /// <summary>
        /// Prüft, ob eine vorhandene Adresse anhand ihrer ID
        /// aus dem Repository gelesen werden kann.
        /// </summary>
        [Fact]
        public async Task Test_WithExistingId_ReturnAddress()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var address = new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                Street = "Ententeich",
                HouseNumber = "11",
                PostalCode = "4711",
                City = "Entenhausen",
                Country = "Deutschland"
            };

            await context.Addresses.AddAsync(address);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(address.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(address.Id, result.Id);
            Assert.Equal(address.FirstName, result.FirstName);
            Assert.Equal(address.LastName, result.LastName);
        }



        /// <summary>
        /// Prüft, ob das Repository bei einer unbekannten
        /// Adress ID null zurückgibt.
        /// </summary>
        [Fact]
        public async Task Test_WithUnknownId_ReturnNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var unknownId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(unknownId);

            // Assert
            Assert.Null(result);
        }



        /// <summary>
        /// Prüft, ob das Repository ohne Filter
        /// alle gespeicherten Adressen zurückliefert.
        /// </summary>
        [Fact]
        public async Task Test_WithoutFilters_ReturnAllAddresses()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var address1 = new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                City = "Entenhausen"
            };

            var address2 = new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Dagobert",
                LastName = "Duck",
                City = "Entenhausen"
            };

            await repository.CreateAsync(address1);
            await repository.CreateAsync(address2);

            // Act
            var result = await repository.GetAllAsync(null, null);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.Id == address1.Id);
            Assert.Contains(result, a => a.Id == address2.Id);
        }



        /// <summary>
        /// Prüft, ob ausschließlich Adressen aus der angegebenen
        /// Stadt zurückgegeben werden.
        /// </summary>
        [Fact]
        public async Task Test_WithCityFilter_ReturnOnlyMatchingAddresses()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            await repository.CreateAsync(new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                City = "Entenhausen"
            });

            await repository.CreateAsync(new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Micky",
                LastName = "Maus",
                City = "Maushausen"
            });

            // Act
            var result = await repository.GetAllAsync("Entenhausen", null);

            // Assert
            Assert.Single(result);
            Assert.Equal("Donald", result[0].FirstName);
            Assert.Equal("Entenhausen", result[0].City);
        }


        /// <summary>
        /// Prüft, ob für die Stadt und die Postleitzahl die passende Adressen zurückgegeben werden.
        /// </summary>
        [Fact]
        public async Task Test_WithCityAndPostalCodeFilters_ReturnMatchingAddresses()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            await repository.CreateAsync(new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                City = "Entenhausen",
                PostalCode = "4711"
            });

            await repository.CreateAsync(new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Micky",
                City = "Entenhausen",
                PostalCode = "20095"
            });

            await repository.CreateAsync(new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Dagobert",
                City = "Hamburg",
                PostalCode = "21481"
            });

            // Act
            var result = await repository.GetAllAsync("Entenhausen", "4711");

            // Assert
            Assert.Single(result);
            Assert.Equal("Donald", result[0].FirstName);
            Assert.Equal("Entenhausen", result[0].City);
            Assert.Equal("4711", result[0].PostalCode);
        }


        /// <summary>
        /// Prüft, ob bei einer Suche ohne Treffer eine leere
        /// Liste anstelle von null zurückgegeben wird.
        /// </summary>
        [Fact]
        public async Task Test_WithNoMatchingAddresses_ShouldReturnEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            await repository.CreateAsync(new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                City = "Entenhausen",
                PostalCode = "12345"
            });

            // Act
            var result = await repository.GetAllAsync("Hamburg", null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        /// <summary>
        /// Prüft, ob eine vorhandene Adresse erfolgreich
        /// aktualisiert und gespeichert werden kann.
        /// </summary>
        [Fact]
        public async Task Test_WithExistingAddress_UpdateAddress()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var address = new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                Street = "Ententeich",
                HouseNumber = "11",
                PostalCode = "12345",
                City = "Entenhausen",
                Country = "Deutschland"
            };

            await repository.CreateAsync(address);

            var newAddress = new Address
            {
                Id = address.Id,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Street = "Geldspeicherstraße",
                HouseNumber = "1",
                PostalCode = address.PostalCode,
                City = "Duckburg",
                Country = address.Country
            };

            // Act
            var result = await repository.UpdateAsync(newAddress);

            // Assert
            Assert.True(result);

            var updatedAddress = await context.Addresses
                .AsNoTracking()
                .SingleAsync(a => a.Id == address.Id);

            Assert.Equal("Geldspeicherstraße", updatedAddress.Street);
            Assert.Equal("1", updatedAddress.HouseNumber);
            Assert.Equal("Duckburg", updatedAddress.City);
        }


        /// <summary>
        /// Prüft, ob eine vorhandene Adresse
        /// aus dem Repository gelöscht werden kann.
        /// </summary>
        [Fact]
        public async Task Test_WithExistingAddress_ShouldDeleteAddress()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var address = new Address
            {
                Id = Guid.NewGuid(),
                FirstName = "Donald",
                LastName = "Duck",
                Street = "Ententeich",
                HouseNumber = "11",
                PostalCode = "12345",
                City = "Entenhausen",
                Country = "Deutschland"
            };

            await repository.CreateAsync(address);

            // Act
            var result = await repository.DeleteAsync(address.Id);

            // Assert
            Assert.True(result);

            var deletedAddress = await context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == address.Id);

            Assert.Null(deletedAddress);
        }


        /// <summary>
        /// Prüft, ob beim Löschen einer nicht vorhandenen
        /// Adresse false zurückgegeben wird.
        /// </summary>
        [Fact]
        public async Task Test_WithUnknownAddress_ReturnFalseOnDelete()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var unknownId = Guid.NewGuid();

            // Act
            var result = await repository.DeleteAsync(unknownId);

            // Assert
            Assert.False(result);
            Assert.Empty(context.Addresses);
        }



        /// <summary>
        /// Prüft, ob die erste Seite nur die angeforderte
        /// Anzahl von Adressen enthält.
        /// </summary>
        [Fact]
        public async Task Test_GetAllWithPagination_ReturnFirstPage()
        {
            // Arrange
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            for (var i = 1; i <= 5; i++)
            {
                await repository.CreateAsync(new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Vorname{i}",
                    LastName = $"Nachname{i}",
                    Street = "Teststraße",
                    HouseNumber = "1",
                    PostalCode = "21481",
                    City = "Lauenburg",
                    Country = "Deutschland"
                });
            }

            // Act
            var addresses = await repository.GetAllAsync(
                city: null,
                postalCode: null,
                page: 1,
                pageSize: 2);

            // Assert
            Assert.Equal(2, addresses.Count);
        }



        /// <summary>
        /// Prüft, ob bei der zweiten Seite die ersten
        /// Einträge übersprungen werden.
        /// </summary>
        [Fact]
        public async Task Test_GetAllWithPagination_ReturnSecondPage()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AdressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new AdressDbContext(options);
            var repository = new AddressRepository(context);

            var addresses = new List<Address>();

            for (var i = 1; i <= 5; i++)
            {
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = $"Vorname{i}",
                    LastName = $"Nachname{i}",
                    Street = "Teststraße",
                    HouseNumber = "1",
                    PostalCode = "21481",
                    City = "Lauenburg",
                    Country = "Deutschland"
                };

                addresses.Add(address);
                await repository.CreateAsync(address);
            }

            // Act
            var result = await repository.GetAllAsync(
                city: null,
                postalCode: null,
                page: 2,
                pageSize: 2);

            // Assert
            var expectedIds = addresses
                .OrderBy(a => a.Id)
                .Skip(2)
                .Take(2)
                .Select(a => a.Id);

            Assert.Equal(expectedIds, result.Select(a => a.Id));
        }


    }
}
