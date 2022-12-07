using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities.Log
{
    public class Line : BaseEntity
    {
        public Line()
        {
            LogLine = string.Empty;
        }

        [Required]
        public long FileId { get; set; }

        [Required]
        [StringLength(DataLiterals.MaxLength)]
        [MaxLength(DataLiterals.MaxLength)]
        public string LogLine { get; set; }

        [Required]
        public Guid Grouping { get; set; }

        public File File { get; set; }

        public override string ToString()
        {
            return $"FileId[{FileId}], Grouping[{Grouping}], {base.ToString()}";
        }
    }
}
