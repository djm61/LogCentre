using LogCentre.Data.Entities.Log;

using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class LogSource : BaseEntity
    {
        public LogSource()
        {
            LogLines = new HashSet<Line>();
        }

        [Required]
        public long HostId { get; set; }

        [Required]
        public long ProviderId { get; set; }

        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(DataLiterals.PathLength)]
        [MaxLength(DataLiterals.PathLength)]
        public string Path { get; set; }

        public Host Host { get; set; }
        public Provider Provider { get; set; }
        public ICollection<Line> LogLines { get; set; }
    }
}
