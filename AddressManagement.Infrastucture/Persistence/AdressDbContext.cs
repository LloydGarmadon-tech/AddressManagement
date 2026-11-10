using AddressManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AddressManagement.Infrastucture.Persistence
{
    public class AdressDbContext : DbContext
    {
        public AdressDbContext(DbContextOptions<AdressDbContext> options) : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; } = null!;
    }
}
