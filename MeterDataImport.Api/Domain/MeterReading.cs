using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace MeterDataImport.Api.Domain
{
    public class MeterReading
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime ReadingDateTime { get; set; }
        public int Value { get; set; }
        public Account Account { get; set; }
    }

    public class MeterReadingEntityTypeConfiguration : IEntityTypeConfiguration<MeterReading>
    {
        public void Configure(EntityTypeBuilder<MeterReading> builder)
        {
            builder.HasOne(x => x.Account)
                .WithMany(x => x.MeterReadings);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ReadingDateTime).IsRequired();
            builder.Property(x => x.Value).IsRequired();
            builder.HasIndex(x => x.AccountId);
        }
    }
}
