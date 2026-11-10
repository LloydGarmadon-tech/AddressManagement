using AddressManagement.Application.Interfaces;
using AddressManagement.Domain.Entities;
using AddressManagement.Infrastucture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AddressManagement.Infrastucture.Repositories
{
    /// <summary>
    /// Implementiert den Datenzugriff für Adressen.
    /// </summary>
    public class AddressRepository : IAddressRepository
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
        public async Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Addresses.FindAsync([id], cancellationToken);
        }

        /// <summary>
        /// Gibt alle Adressen zurück, optional kann nach Stadt und Postleitzahl gefiltert werden.
        /// </summary>
        /// <param name="city">Stadt</param>
        /// <param name="postalCode">Postleitzahl</param>
        /// <param name="cancellationToken">Token zum Abbrechen der Operation</param>
        /// <returns>
        /// Eine schreibgeschützte Liste der Adressen, die den angegebenen
        /// Filterkriterien entsprechen. Werden keine passenden Adressen gefunden,
        /// wird eine leere Liste zurückgegeben.
        /// </returns>
        public async Task<IReadOnlyList<Address>> GetAllAsync(string? city, string? postalCode,
                                                              CancellationToken cancellationToken = default)
        {
            IQueryable<Address> query = _context.Addresses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(a => a.City == city);
            }

            if (!string.IsNullOrWhiteSpace(postalCode))
            {
                query = query.Where(a => a.PostalCode == postalCode);
            }

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktualisiert die bestehende Adresse, 
        /// indem die Eigenschaften des Objektes auf die
        /// bestehende Entität in der Datenbank übertragen wird.
        /// </summary>
        /// <param name="address">zu aktualisierende Adresse</param>
        /// <param name="cancellationToken">Token zum Abbrechen der Operation</param>
        /// <returns>True - wenn die Adresse erfolgreich aktualisiert wurde, false - sonst</returns>
        public async Task<bool> UpdateAsync(Address address, CancellationToken cancellationToken = default)
        {
            var existingAddress = await _context.Addresses.FindAsync(
                new object[] { address.Id },
                cancellationToken);

            if (existingAddress is null)
            {
                return false;
            }

            _context.Entry(existingAddress).CurrentValues.SetValues(address);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <summary>
        /// Sucht die Adresse anhand der ID und löscht sie aus dem Datenbestand.
        /// </summary>
        /// <param name="id">Adress ID</param>
        /// <param name="cancellationToken">Token zum Abbrechen der Operation</param>
        /// <returns>True - wenn die Adresse erfolgreich gelöscht wurde, false - sonst</returns>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var address = await _context.Addresses.FindAsync(
                new object[] { id },
                cancellationToken);

            if (address is null)
            {
                return false;
            }

            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
