﻿namespace LogCentre.Model.Cache
{
    /// <summary>
    /// Cache Item Model
    /// </summary>
    public class CacheItemModel
    {
        /// <summary>
        /// Id of the log line
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Line parts after the regex has been over it
        /// </summary>
        public IDictionary<string, string> Line { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// ToString implementation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Id[{Id}], Line.Count[{Line.Count}], {base.ToString()}";
        }
    }
}
