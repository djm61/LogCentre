using LogCentre.Data.Interfaces;

using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class BaseEntity : IBaseEntity<long>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(DataLiterals.FlagLength)]
        [MaxLength(DataLiterals.FlagLength)]
        public string Active { get; set; } = DataLiterals.Yes;

        [Required]
        [StringLength(DataLiterals.FlagLength)]
        [MaxLength(DataLiterals.FlagLength)]
        public string Deleted { get; set; } = DataLiterals.No;

        [Required]
        [StringLength(DataLiterals.LastUpdatedByLength)]
        [MaxLength(DataLiterals.LastUpdatedByLength)]
        public string LastUpdatedBy { get; set; }

        [Required]
        public DateTime RowVersion { get; set; }
    }
}
