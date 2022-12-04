namespace LogCentre.Model
{
    /// <summary>
    /// Log Source
    /// </summary>
    public class LogSourceModel : BaseModel
    {
        /// <summary>
        /// Log Source
        /// </summary>
        public LogSourceModel()
        {
            Name = string.Empty;
            Path = string.Empty;
            Host = null;
            Provider = null;
        }

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
        public string Name { get; set; }

        /// <summary>
        /// Path for the Log Source
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Referenced Host
        /// </summary>
        public HostModel? Host { get; set; }

        /// <summary>
        /// Referenced Provider
        /// </summary>
        public ProviderModel? Provider { get; set; }

        public override string ToString()
        {
            return $"Name[{Name}], Path[{Path}], HostId[{HostId}], ProviderId[{ProviderId}], {base.ToString()}";
        }
    }
}
