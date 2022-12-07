namespace LogCentre.BackgroundServices
{
    public class LogBackgroundServiceSettings
    {
        public bool Enabled { get; set; }
        public int UpdateInterval { get; set; }

        public override string ToString()
        {
            return $"Enabled[{Enabled}], Interval[{UpdateInterval}], {base.ToString()}";
        }
    }
}
