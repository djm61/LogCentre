using LogCentre.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogCentre.Data.Configuration
{
    public class HostConfig : BaseEntityConfig<Host>
    {
        public override void Configure(EntityTypeBuilder<Host> builder)
        {
            base.Configure(builder);

            builder.ToTable("Host");

            builder.Property(t => t.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.NameLength)
                .HasDefaultValueSql("''");

            builder.Property(t => t.Description)
                .IsUnicode()
                .HasMaxLength(DataLiterals.DescriptionLength)
                .HasDefaultValueSql("''");
        }
    }
}
