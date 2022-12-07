using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class Host : BaseEntity
    {
        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; }

        [StringLength(DataLiterals.DescriptionLength)]
        [MaxLength(DataLiterals.DescriptionLength)]
        public string Description { get; set; }

        public ICollection<LogSource> LogSources { get; set; } = new HashSet<LogSource>();

        public override string ToString()
        {
            return $"Name[{Name}], Description[{Description}], SourceCount[{LogSources.Count}], {base.ToString()}";
        }
    }
}
