using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogCentre.Data.Configuration.Log
{
    public class FileConfig : BaseEntityConfig<Entities.Log.File>
    {
        public override void Configure(EntityTypeBuilder<Entities.Log.File> builder)
        {
            base.Configure(builder);

            builder.ToTable("File", "log");

            builder.Property(t => t.LogSourceId)
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.NameLength)
                .HasDefaultValueSql("''");

            builder.Property(t => t.FileComplete)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.FlagLength)
                .HasDefaultValueSql("'N'");

            builder.HasOne(t => t.LogSource)
                .WithMany(t => t.Files)
                .HasForeignKey(t => t.LogSourceId)
                .HasConstraintName("FK_File_LogSourceId")
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
