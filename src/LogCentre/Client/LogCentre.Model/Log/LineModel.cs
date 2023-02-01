namespace LogCentre.Model.Log
{
    /// <summary>
    /// Log Line
    /// </summary>
    public class LineModel : BaseModel
    {
        /// <summary>
        /// Id of the File
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// DateTime of the log entry
        /// </summary>
        public DateTime LogDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Log Level - Debug, Info, Warn, Error, etc
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Thread Id
        /// </summary>
        public string Thread { get; set; } = string.Empty;

        /// <summary>
        /// Source from the log line
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Log Line
        /// </summary>
        public string LogLine { get; set; } = string.Empty;

        /// <summary>
        /// Full line (un parsed)
        /// </summary>
        public string FullLine { get; set; } = string.Empty;

        /// <summary>
        /// Grouping of the log line
        /// </summary>
        public Guid Grouping { get; set; } = Guid.Empty;

        /// <summary>
        /// Referenced Log Source
        /// </summary>
        public FileModel? LogFile { get; set; } = null;

        public override string ToString()
        {
            return $"FileId[{FileId}], Line[{LogLine}], {base.ToString}";
        }
    }
}
