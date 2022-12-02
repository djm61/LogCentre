using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class Provider : BaseEntity
    {
        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; }

        [StringLength(DataLiterals.DescriptionLength)]
        [MaxLength(DataLiterals.DescriptionLength)]
        public string Description { get; set; }

        [StringLength(DataLiterals.RegexLength)]
        [MaxLength(DataLiterals.RegexLength)]
        public string Regex { get; set; }

        public ICollection<LogSource> Sources { get; set; } = new HashSet<LogSource>();
    }
}