namespace LogCentre.Api.Services
{
    /// <summary>
    /// Background Service Settings
    /// </summary>
    public class LogBackgroundServiceSettings
    {
        /// <summary>
        /// Enabled flag
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Update interval (seconds)
        /// </summary>
        public int UpdateInterval { get; set; }

        /// <summary>
        /// ToString implementation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Enabled[{Enabled}], Interval[{UpdateInterval}], {base.ToString()}";
        }
    }
}
