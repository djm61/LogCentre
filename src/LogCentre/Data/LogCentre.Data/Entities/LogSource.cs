using LogCentre.Data.Entities.Log;

using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class LogSource : BaseEntity
    {
        [Required]
        public long HostId { get; set; }

        [Required]
        public long ProviderId { get; set; }

        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(DataLiterals.PathLength)]
        [MaxLength(DataLiterals.PathLength)]
        public string Path { get; set; } = string.Empty;

        public Host Host { get; set; } = new Host();
        public Provider Provider { get; set; } = new Provider();
        public ICollection<Log.File> Files { get; set; } = new HashSet<Log.File>();

        public override string ToString()
        {
            return $"HostId[{HostId}], ProviderId[{ProviderId}], Name[{Name}], Path[{Path}], {base.ToString()}";
        }
    }
}
