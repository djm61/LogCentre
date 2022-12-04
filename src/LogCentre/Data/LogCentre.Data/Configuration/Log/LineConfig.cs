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

            builder.Property(t => t.LogSourceId)
                .IsRequired();

            builder.Property(t => t.LogLine)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(DataLiterals.MaxLength)
                .HasDefaultValueSql("''");

            builder.HasOne(t => t.LogSource)
                .WithMany(t => t.LogLines)
                .HasForeignKey(t => t.LogSourceId)
                .HasConstraintName("FK_Line_LogSourceId")
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
