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
            await _repository.CreateAsync(address);
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

        /// <summary>
        /// Ruft alle Adressen ab, optional gefiltert nach Stadt und/oder Postleitzahl.
        /// </summary>
        /// <param name="city">optionaler Filter nach Stadt</param>
        /// <param name="postalcode">optionaler Filter nach Postleitzahl</param>
        /// <returns>Eine Liste aller gefundenen Adressen.</returns>
        public async Task<IReadOnlyList<Address>> GetAllAsync(string? city = null, string? postalcode = null)
        {
            return await _repository.GetAllAsync(city, postalcode);
        }


        /// <summary>
        /// Gibt Adressen seitenweise zurück.
        /// Optional kann nach Stadt und Postleitzahl gefiltert werden.
        /// </summary>
        /// <param name="city">Optionaler Stadtfilter.</param>
        /// <param name="postalCode">Optionaler Postleitzahlfilter.</param>
        /// <param name="page">Seitennummer, beginnend bei 1.</param>
        /// <param name="pageSize">Anzahl der Adressen pro Seite.</param>
        /// <param name="cancellationToken">Token zum Abbrechen der Operation.</param>
        /// <returns>Die Adressen der angeforderten Seite.</returns>
        public async Task<IReadOnlyList<Address>> GetAllAsync(
            string? city,
            string? postalCode,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync(
                city,
                postalCode,
                page,
                pageSize,
                cancellationToken);
        }


        /// <summary>
        /// Aktualisert eine vorhandene Adresse.
        /// </summary>
        /// <param name="address">Adresse</param>
        /// <returns>True - wenn die Adresse erfolgreich aktualisiert wurde</returns>
        public async Task<bool> UpdateAsync(Address address)
        {
            return await _repository.UpdateAsync(address);
        }

        /// <summary>
        /// Löscht eine vorhandene Adresse anhand der angegebenen ID.
        /// </summary>
        /// <param name="addressId">Adress ID</param>
        /// <returns>True - wenn das Löschen erfolgreich war</returns>
        public async Task<bool> DeleteAsync(Guid addressId)
        {
            return await _repository.DeleteAsync(addressId);
        }
    }
}