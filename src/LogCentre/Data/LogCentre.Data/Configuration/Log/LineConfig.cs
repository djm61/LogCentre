using LogCentre.Data.Entities.Log;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogCentre.Data.Configuration.Log
{
    public class LineConfig : BaseEntityConfig<Line>
    {
        public override void Configure(EntityTypeBuilder<Line> builder)
        {
            base.Configure(builder);

            builder.ToTable("Line", "log");

            builder.Property(t => t.LogDate);

            builder.Property(t => t.Level)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.NameLength)
                .HasDefaultValue("''");

            builder.Property(t => t.Thread)
                .IsUnicode()
                .HasMaxLength(DataLiterals.NameLength)
                .HasDefaultValue("''");

            builder.Property(t => t.Source)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.DescriptionLength)
                .HasDefaultValue("''");

            builder.Property(t => t.LogLine)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.MaxLength)
                .HasDefaultValueSql("''");

            builder.Property(t => t.FullLine)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.MaxLength)
                .HasDefaultValue("''");

            builder.Property(t => t.Grouping)
                .IsRequired();

            builder.HasOne(t => t.File)
                .WithMany(t => t.Lines)
                .HasForeignKey(t => t.FileId)
                .HasConstraintName("FK_Line_FileId")
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
