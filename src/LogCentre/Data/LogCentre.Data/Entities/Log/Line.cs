using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities.Log
{
    public class Line : BaseEntity
    {
        [Required]
        public long FileId { get; set; }

        [Required]
        public DateTime LogDate { get; set; }

        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Level { get; set; } = string.Empty;

        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Thread { get; set; } = string.Empty;

        [Required]
        [StringLength(DataLiterals.DescriptionLength)]
        [MaxLength(DataLiterals.DescriptionLength)]
        public string Source { get; set; } = string.Empty;

        [Required]
        [StringLength(DataLiterals.MaxLength)]
        [MaxLength(DataLiterals.MaxLength)]
        public string LogLine { get; set; } = string.Empty;

        [Required]
        [StringLength(DataLiterals.MaxLength)]
        [MaxLength(DataLiterals.MaxLength)]
        public string FullLine { get; set; } = string.Empty;

        [Required]
        public Guid Grouping { get; set; } = Guid.Empty;

        public File File { get; set; } = new File();

        public override string ToString()
        {
            return $"FileId[{FileId}], Grouping[{Grouping}], {base.ToString()}";
        }
    }
}
