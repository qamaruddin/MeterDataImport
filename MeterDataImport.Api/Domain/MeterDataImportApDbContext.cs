using Microsoft.EntityFrameworkCore;

namespace MeterDataImport.Api.Domain
{
    public class MeterDataImportApDbContext : DbContext
    {
        public MeterDataImportApDbContext(DbContextOptions<MeterDataImportApDbContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountEntityTypeConfiguration).Assembly);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}
