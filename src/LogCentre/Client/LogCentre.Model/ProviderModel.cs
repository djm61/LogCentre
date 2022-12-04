namespace LogCentre.Model
{
    /// <summary>
    /// Provider
    /// </summary>
    public class ProviderModel : BaseModel
    {
        /// <summary>
        /// Provider
        /// </summary>
        public ProviderModel()
        {
            Name = string.Empty;
            Description = string.Empty;
            Regex = string.Empty;
        }

        /// <summary>
        /// Name of the Provider
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Provider
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Regex to parse the Provider
        /// </summary>
        public string Regex { get; set; }

        public override string ToString()
        {
            return $"Name[{Name}], Description[{Description}], Regex[{Regex}], {base.ToString()}";
        }
    }
}
