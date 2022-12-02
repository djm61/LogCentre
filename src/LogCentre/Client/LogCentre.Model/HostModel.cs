namespace LogCentre.Model
{
    public class HostModel : BaseModel
    {
        /// <summary>
        /// Host
        /// </summary>
        public HostModel()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Name of the Host
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the Host
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return $"Name[{Name}], Description[{Description}], {base.ToString()}";
        }
    }
}