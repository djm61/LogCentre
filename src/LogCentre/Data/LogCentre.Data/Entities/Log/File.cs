﻿using System.ComponentModel.DataAnnotations;

namespace LogCentre.Data.Entities.Log
{
    public class File : BaseEntity
    {
        public File()
        {
            Name = string.Empty;
            FileComplete = DataLiterals.No;
            Lines = new HashSet<Line>();
        }

        [Required]
        public long LogSourceId { get; set; }

        [Required]
        [StringLength(DataLiterals.NameLength)]
        [MaxLength(DataLiterals.NameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(DataLiterals.FlagLength)]
        [MaxLength(DataLiterals.FlagLength)]
        public string FileComplete { get; set; }

        public LogSource LogSource { get; set; }

        public ICollection<Line> Lines { get; set; }
    }
}
