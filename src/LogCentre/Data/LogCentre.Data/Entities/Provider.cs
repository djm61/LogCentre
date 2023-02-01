﻿using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities
{
    public class Provider : BaseEntity
    {
        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; } = string.Empty;

        [StringLength(DataLiterals.DescriptionLength)]
        [MaxLength(DataLiterals.DescriptionLength)]
        public string Description { get; set; } = string.Empty;

        [StringLength(DataLiterals.RegexLength)]
        [MaxLength(DataLiterals.RegexLength)]
        public string Regex { get; set; } = string.Empty;

        public ICollection<LogSource> LogSources { get; set; } = new HashSet<LogSource>();

        public override string ToString()
        {
            return $"Name[{Name}], Description[{Description}], Regex[{Regex}], SourceCount[{LogSources.Count}]";
        }
    }
}