using LogCentre.Model;

using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace LogCentre.Web.Areas.Admin.Models
{
    public class LogSourceViewModel
    {
        public long Id { get; set; }

        [Required]
        public long HostId { get; set; }

        [Required]
        public long ProviderId { get; set; }

        [Required]
        [StringLength(ModelLiterals.NameLength)]
        [MaxLength(ModelLiterals.NameLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(ModelLiterals.PathLength)]
        [MaxLength(ModelLiterals.PathLength)]
        public string Path { get; set; } = string.Empty;

        [Required]
        [StringLength(ModelLiterals.FlagLength)]
        [MaxLength(ModelLiterals.FlagLength)]
        public string Active { get; set; } = ModelLiterals.Yes;

        [Required]
        [StringLength(ModelLiterals.FlagLength)]
        [MaxLength(ModelLiterals.FlagLength)]
        public string Deleted { get; set; } = ModelLiterals.No;

        public HostModel? Host { get; set; }
        public ProviderModel? Provider { get; set; }

        public SelectList HostSelectList { get; set; } = new SelectList(new List<string>());

        public SelectList ProviderSelectList { get; set; } = new SelectList(new List<string>());

        public override string ToString()
        {
            return $"Id[{Id}], HostId[{HostId}], ProviderId[{ProviderId}], Name[{Name}], Path[{Path}], Active[{Active}], Deleted:[{Deleted}], {base.ToString()}";
        }
    }
}
