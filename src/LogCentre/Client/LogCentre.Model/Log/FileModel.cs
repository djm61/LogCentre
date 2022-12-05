namespace LogCentre.Model.Log
{
    /// <summary>
    /// Log File
    /// </summary>
    public class FileModel : BaseModel
    {
        /// <summary>
        /// Id of the Log Source
        /// </summary>
        public long LogSourceId { get; set; }

        /// <summary>
        /// Name of the Log File
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Referenced Log Source
        /// </summary>
        public LogSourceModel? LogSource { get; set; }

        public override string ToString()
        {
            return $"LogSourceId[{LogSourceId}], Name[{Name}], {base.ToString()}";
        }
    }
}
