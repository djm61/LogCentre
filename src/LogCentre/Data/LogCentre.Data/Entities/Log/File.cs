using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities.Log
{
    public class File : BaseEntity
    {
        [Required]
        public long LogSourceId { get; set; }

        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(DataLiterals.FlagLength)]
        [MaxLength(DataLiterals.FlagLength)]
        public string FileComplete { get; set; } = string.Empty;

        public LogSource LogSource { get; set; } = new LogSource();

        public ICollection<Line> Lines { get; set; } = new HashSet<Line>();

        public override string ToString()
        {
            return $"LogSourceId[{LogSourceId}], Name[{Name}], FileComplete[{FileComplete}], {base.ToString()}";
        }
    }
}
