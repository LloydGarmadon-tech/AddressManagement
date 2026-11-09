using AddressManagement.Application.Interfaces;
using AddressManagement.Domain.Entities;

namespace AddressManagement.Application.Services
{
    /// <summary>
    /// Stellt die Anwendungslogik für die Verwaltung von Adressen bereit.
    /// </summary>
    public class AddressService
    {
        private readonly IAddressRepository _repository;

        public AddressService(IAddressRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Erstellt eine neue Adresse und weist ihr eine neue Guid zu.
        /// </summary>
        /// <param name="address">Die anzulegende Adresse.</param>
        /// <returns>Die erstellte Adresse.</returns>
        public async Task<Address> CreateAsync(Address address)
        {
            address.Id = Guid.NewGuid();
            await _repository.AddAsync(address);
            return address;
        }


        /// <summary>
        /// Ruft die Adresse anhand der angegebenen ID ab.
        /// </summary>
        /// <param name="addressId">Die ID der abzurufenden Adresse.</param>
        /// <returns>Die gefundende Adresse oder null, wenn keine gefunden wurde.</returns>
        public async Task<Address?> GetByIdAsync(Guid addressId)
        {
            return await _repository.GetByIdAsync(addressId);
        }
    }
}