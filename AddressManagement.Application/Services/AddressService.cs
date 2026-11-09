using AddressManagement.Application.Interfaces;
using AddressManagement.Domain.Entities;

namespace AddressManagement.Application.Services
{
    public class AddressService
    {
        private readonly IAddressRepository _repository;

        public AddressService(IAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<Address> CreateAsync(Address address)
        {
            address.Id = Guid.NewGuid();
            await _repository.AddAsync(address);
            return address;
        }

        public async Task<Address?> GetByIdAsync(Guid addressId)
        {
            throw new NotImplementedException();
        }
    }
}
