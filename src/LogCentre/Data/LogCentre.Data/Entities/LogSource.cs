using LogCentre.Data.Entities.Log;

using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class LogSource : BaseEntity
    {
        public LogSource()
        {
            Files = new HashSet<Log.File>();
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
        public ICollection<Log.File> Files { get; set; }

        public override string ToString()
        {
            return $"HostId[{HostId}], ProviderId[{ProviderId}], Name[{Name}], Path[{Path}], {base.ToString()}";
        }
    }
}
