using AddressManagement.Application.Interfaces;
using AddressManagement.Application.Services;
using AddressManagement.Domain.Entities;
using Moq;

namespace AddressManagement.UnitTests.Services
{
    public class AddressServiceTests
    {
        /// <summary>
        /// Prüft, ob beim anlegen einer neuen Adresse eine neue Guid vergeben wird.
        /// </summary>
        [Fact]
        public async Task Test_WithValidAddress_AssignNewGuid()
        {
            // Arrange
            var address = new Address
            {
                FirstName = "Dagobert",
                LastName = "Duck",
                Street = "Ententeich",
                HouseNumber = "1",
                PostalCode = "4711",
                City = "Entenhausen",
                Country = "Deutschland"
            };

            var repositoryMock = new Mock<IAddressRepository>();

            var service = new AddressService(
                repositoryMock.Object);

            var originalId = Guid.NewGuid();
            address.Id = originalId;

            // Act
            var result = await service.CreateAsync(address);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        /// <summary>
        /// Prüft, ob bei einer bekannten Adress-ID die entsprechende Adresse zurückgegeben wird.
        /// </summary>
        [Fact]
        public async Task Test_WithExistingId_ReturnAddress()
        {
            // Arrange
            var addressId = Guid.NewGuid();

            var expectedAddress = new Address
            {
                Id = addressId,
                FirstName = "Dagobert",
                LastName = "Duck",
                City = "Entenhausen"
            };

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(
                    addressId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAddress);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.GetByIdAsync(addressId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(addressId, result.Id);
            Assert.Same(expectedAddress, result);
        }



        /// <summary>
        /// Prüft, ob bei einer unbekannten Adress-ID null zurückgegeben wird.
        /// </summary>
        [Fact]
        public async Task Test_WithUnknownId_ReturnNull()
        {
            // Arrange
            var addressId = Guid.NewGuid();

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(
                    addressId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Address?)null);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.GetByIdAsync(addressId);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Prüft, ob bei fehlenden Filtern alle Adressen zurückgegeben werden.
        /// </summary>        
        [Fact]
        public async Task Test_WithoutFilters_ReturnAllAddresses()
        {
            // Arrange
            IReadOnlyList<Address> expectedAddresses =
            [
                new Address { Id = Guid.NewGuid(), FirstName = "Dagobert", LastName = "Duck" },
                new Address { Id = Guid.NewGuid(), FirstName = "Donald", LastName = "Duck" }
            ];

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.GetAllAsync(
                    null,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAddresses);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedAddresses, result);
        }


        /// <summary>
        /// Prüft, ob bei Angabe eines Stadtfilters die passenden
        /// Adressen zurückgegeben werden.
        /// </summary>
        [Fact]
        public async Task Test_WithCityFilter_ReturnMatchingAddresses()
        {
            // Arrange
            const string city = "Entenhausen";

            IReadOnlyList<Address> expectedAddresses =
            [
                new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Donald",
                    LastName = "Duck",
                    City = "Entenhausen"
                },
                new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Dagobert",
                    LastName = "Duck",
                    City = "Entenhausen"
                }
            ];

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.GetAllAsync(
                    city,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAddresses);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.GetAllAsync(city: city);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedAddresses, result);

            repositoryMock.Verify(
                r => r.GetAllAsync(
                    city,
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        /// <summary>
        /// Prüft, ob bei Angabe einer Postleitzahl die passenden
        /// Adressen aus dem Repository zurückgegeben werden.
        /// </summary>
        [Fact]
        public async Task Test_WithPostalCodeFilter_ReturnMatchingAddresses()
        {
            // Arrange
            const string postalCode = "4711";

            IReadOnlyList<Address> expectedAddresses =
            [
                new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Donald",
                    LastName = "Duck",
                    PostalCode = "4711",
                    City = "Entenhausen"
                },
                new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Dagobert",
                    LastName = "Duck",
                    PostalCode = "4711",
                    City = "Entenhausen"
                }
            ];

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.GetAllAsync(
                    null,
                    postalCode,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAddresses);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.GetAllAsync(postalcode: postalCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedAddresses, result);

            repositoryMock.Verify(
                r => r.GetAllAsync(
                    null,
                    postalCode,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        /// <summary>
        /// Prüft, ob beide Filter Stadt und Postleitzahl
        /// korrekt an das Repository übergeben werden.
        /// </summary>
        [Fact]
        public async Task Test_WithCityAndPostalCodeFilters_ReturnMatchingAddresses()
        {
            // Arrange
            const string city = "Entenhausen";
            const string postalCode = "4711";

            IReadOnlyList<Address> expectedAddresses =
            [
                new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Donald",
                    LastName = "Duck",
                    City = city,
                    PostalCode = postalCode
                },
                new Address
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Dagobert",
                    LastName = "Duck",
                    City = city,
                    PostalCode = postalCode
                }
            ];

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.GetAllAsync(
                    city,
                    postalCode,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAddresses);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.GetAllAsync(
                city: city,
                postalcode: postalCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(expectedAddresses, result);

            repositoryMock.Verify(
                r => r.GetAllAsync(
                    city,
                    postalCode,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        /// <summary>
        /// Prüft, ob die Aktualisierung einer vorhandenen Adresse
        /// erfolgreich durchgeführt wird.
        /// </summary>
        [Fact]
        public async Task Test_WithExistingAddress_ReturnTrue()
        {
            // Arrange
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

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.UpdateAsync(
                    address,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.UpdateAsync(address);

            // Assert
            Assert.True(result);

            repositoryMock.Verify(
                r => r.UpdateAsync(
                    address,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        /// <summary>
        /// Prüft, ob beim Aktualisieren einer nicht vorhandenen Adresse
        /// false zurückgegeben wird.
        /// </summary>
        [Fact]
        public async Task Test_WithUnknownAddress_ReturnFalse()
        {
            // Arrange
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

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.UpdateAsync(
                    address,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.UpdateAsync(address);

            // Assert
            Assert.False(result);

            repositoryMock.Verify(
                r => r.UpdateAsync(
                    address,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        /// <summary>
        /// Prüft, ob das Löschen einer vorhandenen Adresse
        /// erfolgreich durchgeführt.
        /// </summary>
        [Fact]
        public async Task Test_DeleteWithExistingAddress_ReturnTrue()
        {
            // Arrange
            var addressId = Guid.NewGuid();

            var repositoryMock = new Mock<IAddressRepository>();

            repositoryMock
                .Setup(r => r.DeleteAsync(
                    addressId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new AddressService(repositoryMock.Object);

            // Act
            var result = await service.DeleteAsync(addressId);

            // Assert
            Assert.True(result);

            repositoryMock.Verify(
                r => r.DeleteAsync(
                    addressId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


    }
}
