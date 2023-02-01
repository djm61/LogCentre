namespace LogCentre.Model
{
    /// <summary>
    /// Provider
    /// </summary>
    public class ProviderModel : BaseModel
    {
        /// <summary>
        /// Name of the Provider
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the Provider
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Regex to parse the Provider
        /// </summary>
        public string Regex { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Name[{Name}], Description[{Description}], Regex[{Regex}], {base.ToString()}";
        }
    }
}
