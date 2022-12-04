using LogCentre.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogCentre.Data.Configuration
{
    public class LogSourceConfig : BaseEntityConfig<LogSource>
    {
        public override void Configure(EntityTypeBuilder<LogSource> builder)
        {
            base.Configure(builder);

            builder.ToTable("LogSource");

            builder.Property(t => t.HostId)
                .IsRequired();

            builder.Property(t => t.ProviderId)
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.NameLength)
                .HasDefaultValueSql("''");

            builder.Property(t => t.Path)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.PathLength)
                .HasDefaultValueSql("''");

            builder.HasOne(t => t.Host)
                .WithMany(t => t.Sources)
                .HasForeignKey(t => t.HostId)
                .HasConstraintName("FK_LogSource_HostId")
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(t => t.Provider)
                .WithMany(t => t.Sources)
                .HasForeignKey(t => t.ProviderId)
                .HasConstraintName("FK_LogSource_ProviderId")
                .OnDelete(DeleteBehavior.ClientSetNull);


        }
    }
}
