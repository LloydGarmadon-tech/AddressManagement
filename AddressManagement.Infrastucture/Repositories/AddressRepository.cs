using AddressManagement.Domain.Entities;
using AddressManagement.Infrastucture.Persistence;

namespace AddressManagement.Infrastucture.Repositories
{
    /// <summary>
    /// Implementiert den Datenzugriff für Adressen.
    /// </summary>
    public class AddressRepository
    {
        private readonly AdressDbContext _context;

        /// <summary>
        /// Initialisiert das Repository mit dem Datenbankkontext.
        /// </summary>
        /// <param name="context">Der Datenbankkontext.</param>
        public AddressRepository(AdressDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fügt eine neue Adresse zum Datenbestand hinzu.
        /// </summary>
        /// <param name="address">Die zu speichernde Adresse.</param>
        /// <param name="cancellationToken">Token zum Abbrechen der Operation.</param>
        public Task AddAsync(
            Address address,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
