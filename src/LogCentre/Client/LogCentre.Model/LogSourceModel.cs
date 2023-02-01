namespace LogCentre.Model
{
    /// <summary>
    /// Log Source
    /// </summary>
    public class LogSourceModel : BaseModel
    {
        /// <summary>
        /// Id of the Host
        /// </summary>
        public long HostId { get; set; }

        /// <summary>
        /// Id of the Provider
        /// </summary>
        public long ProviderId { get; set; }

        /// <summary>
        /// Name of the Lost Source
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Path for the Log Source
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Referenced Host
        /// </summary>
        public HostModel? Host { get; set; } = null;

        /// <summary>
        /// Referenced Provider
        /// </summary>
        public ProviderModel? Provider { get; set; } = null;

        public override string ToString()
        {
            return $"Name[{Name}], Path[{Path}], HostId[{HostId}], ProviderId[{ProviderId}], {base.ToString()}";
        }
    }
}
