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
        public long LogSourceId { get; set; }

        [Required]
        public long FileId { get; set; }

        [Required]
        [StringLength(DataLiterals.MaxLength)]
        [MaxLength(DataLiterals.MaxLength)]
        public string LogLine { get; set; }

        public LogSource LogSource { get; set; }

        public File File { get; set; }
    }
}
