using AddressManagement.Api.DTOs;
using AddressManagement.Api.Mapping;
using AddressManagement.Domain.Entities;

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


        /// <summary>
        /// Prüft, ob ein UpdateAddressRequest vollständig
        /// in eine Address-Entität umgewandelt wird.
        /// </summary>
        [Fact]
        public void Test_UpdateAddressRequest_MapToAddress()
        {
            // Arrange
            var id = Guid.NewGuid();

            var request = new UpdateAddressRequest
            {
                FirstName = "Dagobert",
                LastName = "Duck",
                Street = "Geldspeicherstraße",
                HouseNumber = "10",
                PostalCode = "12345",
                City = "Entenhausen",
                Country = "Deutschland",
                Email = "dagobert@entenhausen.de"
            };

            // Act
            var address = AddressMapper.ToAddress(request, id);

            // Assert
            Assert.Equal(id, address.Id);
            Assert.Equal(request.FirstName, address.FirstName);
            Assert.Equal(request.LastName, address.LastName);
            Assert.Equal(request.Street, address.Street);
            Assert.Equal(request.HouseNumber, address.HouseNumber);
            Assert.Equal(request.PostalCode, address.PostalCode);
            Assert.Equal(request.City, address.City);
            Assert.Equal(request.Country, address.Country);
            Assert.Equal(request.Email, address.Email);
        }


        /// <summary>
        /// Prüft, ob eine Address-Entität vollständig
        /// in ein AddressResponse-DTO umgewandelt wird.
        /// </summary>
        [Fact]
        public void Test_Address_MapToAddressResponse()
        {
            // Arrange
            var address = new Address
            {
                Id = Guid.NewGuid(),
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
            var response = AddressMapper.ToResponse(address);

            // Assert
            Assert.Equal(address.Id, response.Id);
            Assert.Equal(address.FirstName, response.FirstName);
            Assert.Equal(address.LastName, response.LastName);
            Assert.Equal(address.Street, response.Street);
            Assert.Equal(address.HouseNumber, response.HouseNumber);
            Assert.Equal(address.PostalCode, response.PostalCode);
            Assert.Equal(address.City, response.City);
            Assert.Equal(address.Country, response.Country);
            Assert.Equal(address.Email, response.Email);
        }

    }
}
