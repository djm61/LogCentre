using LogCentre.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogCentre.Data.Configuration
{
    public class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Active)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.FlagLength)
                .HasDefaultValueSql("N'Y'");

            builder.Property(t => t.Deleted)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.FlagLength)
                .HasDefaultValueSql("N'Y'");

            builder.Property(t => t.LastUpdatedBy)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.LastUpdatedByLength) //this is the length of the email column in identity framework
                .HasDefaultValueSql("''");

            builder.Property(t => t.RowVersion)
                .IsRequired()
                .IsRowVersion()
                //.ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("(getutcdate())")  //sql server utc date
                //.HasColumnType("timestamp")  //mysql server utc date
                ;

            // Column Mappings
            builder.Property(t => t.Id).HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.Active).HasColumnName("Active");
            builder.Property(t => t.Deleted).HasColumnName("Deleted");
            builder.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
