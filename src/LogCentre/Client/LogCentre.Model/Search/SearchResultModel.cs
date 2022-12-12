namespace LogCentre.Model.Search
{
    /// <summary>
    /// Cache Item Model
    /// </summary>
    public class SearchResultModel
    {
        /// <summary>
        /// Id of the log line
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Id of the associated File
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// Date and Time of the log entry
        /// </summary>
        public DateTime LogDate { get; set; }

        /// <summary>
        /// Log Level
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// Log Source
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Log Line
        /// </summary>
        public string LogLine { get; set; } = string.Empty;

        /// <summary>
        /// Associated File
        /// </summary>
        public Log.FileModel? FileModel { get; set; }

        /// <summary>
        /// ToString implementation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Id[{Id}], LogDate[{LogDate:yyyy-MM-dd HH:mm:ss.fff}], Level[{Level}], Source[{Source}], LogLine[{LogLine}], {base.ToString()}";
        }
    }
}
