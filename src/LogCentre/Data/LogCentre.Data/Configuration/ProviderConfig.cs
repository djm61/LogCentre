using LogCentre.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogCentre.Data.Configuration
{
    public class ProviderConfig : BaseEntityConfig<Provider>
    {
        public override void Configure(EntityTypeBuilder<Provider> builder)
        {
            base.Configure(builder);

            builder.ToTable("Provider");

            builder.Property(t => t.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.NameLength)
                .HasDefaultValueSql("''");

            builder.Property(t => t.Description)
                .IsUnicode()
                .HasMaxLength(DataLiterals.DescriptionLength)
                .HasDefaultValueSql("''");

            builder.Property(t => t.Regex)
                .IsUnicode()
                .HasMaxLength(DataLiterals.RegexLength)
                .HasDefaultValueSql("''");
        }
    }
}
