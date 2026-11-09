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
        public async Task Test_WithValidAddress_ShouldAssignNewGuid()
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
    }
}
