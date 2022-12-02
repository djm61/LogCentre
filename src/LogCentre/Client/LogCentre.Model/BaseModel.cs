namespace LogCentre.Model
{
    /// <summary>
    /// Base items for the model
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Base Items for the model
        /// </summary>
        public BaseModel()
        {
            Active = ModelLiterals.Yes;
            Deleted = ModelLiterals.No;
            LastUpdatedBy = string.Empty;
        }

        /// <summary>
        /// Id of the Model
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Active status of the model, either Y or N
        /// </summary>
        public string? Active { get; set; }

        /// <summary>
        /// Deleted status of the model, either Y or N
        /// </summary>
        public string? Deleted { get; set; }

        /// <summary>
        /// Username of who last updated the model
        /// </summary>
        public string LastUpdatedBy { get; set; }

        /// <summary>
        /// RowVersion value, used for concurrency
        /// </summary>
        public DateTime RowVersion { get; set; }

        public override string ToString()
        {
            return $"Id[{Id}], Active[{Active}], Deleted[{Deleted}], LastUpdatedBy[{LastUpdatedBy}], {base.ToString()}";
        }
    }
}
