using AddressManagement.Domain.Entities;

namespace AddressManagement.Application.Interfaces
{
    /// <summary>
    /// Stellt die funktionale Anforderungen laut Aufgabenbeschreibung zur Verfügung. 
    /// Die Implementierung erfolgt in der Infrastuktur.
    /// </summary>
    public interface IAddressRepository
    {

        Task CreateAsync(
            Address address,
            CancellationToken cancellationToken = default);

        Task<Address?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Address>> GetAllAsync(
            string? city,
            string? postalCode,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            Address address,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

    }
}
