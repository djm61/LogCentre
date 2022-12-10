namespace LogCentre.Model.Search
{
    /// <summary>
    /// Search Model
    /// </summary>
    [Serializable]
    public class SearchModel
    {
        /// <summary>
        /// Start Date to search from, and including
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End Date to search to, and including
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Level - Error, Warning, Information, Debug, etc
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Source - Controller or Service usually
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Log Line to earch for - free text
        /// </summary>
        public string? LogLine { get; set; }

        public override string ToString()
        {
            return $"StartDate[{StartDate}], EndDate[{EndDate}], Level[{Level}], Source[{Source}], Line[{LogLine}], {base.ToString()}";
        }
    }
}
