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
        /// Id of the File
        /// </summary>
        public long FileId { get; set; }

        /// <summary>
        /// Log Line
        /// </summary>
        public string LogLine { get; set; }

        /// <summary>
        /// Referenced Log Source
        /// </summary>
        public FileModel LogFile { get; set; }

        public override string ToString()
        {
            return $"FileId[{FileId}], Line[{LogLine}], {base.ToString}";
        }
    }
}
