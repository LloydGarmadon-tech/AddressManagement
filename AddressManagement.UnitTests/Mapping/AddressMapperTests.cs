using AddressManagement.Api.DTOs;

namespace AddressManagement.UnitTests.Mapping
{
    public class AddressMapperTests
    {
        /// <summary>
        /// Prüft, ob ein CreateAddressRequest vollständig
        /// in eine Address-Entität umgewandelt wird.
        /// </summary>
        [Fact]
        public void Test_CreateAddressRequest_MapToAddress()
        {
            // Arrange
            var request = new CreateAddressRequest
            {
                FirstName = "Donald",
                LastName = "Duck",
                Street = "Entenstraße",
                HouseNumber = "1",
                PostalCode = "12345",
                City = "Entenhausen",
                Country = "Deutschland",
                Email = "donald@entenhausen.de"
            };

            // Act
            var address = AddressMapper.ToAddress(request);

            // Assert
            Assert.Equal(request.FirstName, address.FirstName);
            Assert.Equal(request.LastName, address.LastName);
            Assert.Equal(request.Street, address.Street);
            Assert.Equal(request.HouseNumber, address.HouseNumber);
            Assert.Equal(request.PostalCode, address.PostalCode);
            Assert.Equal(request.City, address.City);
            Assert.Equal(request.Country, address.Country);
            Assert.Equal(request.Email, address.Email);
            Assert.Equal(Guid.Empty, address.Id);
        }
    }
}
