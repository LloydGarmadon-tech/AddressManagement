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
        public async Task CreateAsync(
            Address address,
            CancellationToken cancellationToken = default)
        {
            await _context.Addresses.AddAsync(address, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Liest eine Adresse anhand ihrer ID aus dem Datenbestand.
        /// Klärung : AsNoTracking, wenn die Adresse ohne Change-Tracking aus dem Datenbestand gelsen werden soll
        /// </summary>
        /// <param name="id">Adress ID</param>
        /// <returns>Null, wenn keine Adresse gefunden wird. Sonst die Adresse</returns>
        public async Task<Address?> GetByIdAsync(Guid id)
        {
            return await _context.Addresses.FindAsync(id);
        }
    }
}
