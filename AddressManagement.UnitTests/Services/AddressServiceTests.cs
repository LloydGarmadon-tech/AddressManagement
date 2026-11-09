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
        /// <returns></returns>
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

    }
}
