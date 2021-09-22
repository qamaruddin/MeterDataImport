using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace MeterDataImport.Api.Domain
{
    public class Account
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<MeterReading> MeterReadings { get; set; }
    }

    public class AccountEntityTypeConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasMany(x => x.MeterReadings)
                .WithOne(x => x.Account);
            builder.HasKey(x => x.AccountId);
            builder.Property(x => x.AccountId).ValueGeneratedNever();
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
        }
    }
}
