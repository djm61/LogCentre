namespace LogCentre.Model.Log
{
    /// <summary>
    /// Log Line
    /// </summary>
    public class LineModel : BaseModel
    {
        /// <summary>
        /// Log Line
        /// </summary>
        public LineModel()
        {
            LogLine = string.Empty;
        }

        /// <summary>
        /// Id of the Log Source
        /// </summary>
        public long LogSourceId { get; set; }

        /// <summary>
        /// Log Line
        /// </summary>
        public string LogLine { get; set; }

        /// <summary>
        /// Referenced Log Source
        /// </summary>
        public LogSourceModel LogSource { get; set; }

        public override string ToString()
        {
            return $"LogSourceId[{LogSourceId}], Line[{LogLine}], {base.ToString}";
        }
    }
}
