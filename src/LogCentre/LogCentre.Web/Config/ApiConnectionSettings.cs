namespace LogCentre.Web.Config
{
    public class ApiConnectionSettings
    {
        public ApiConnectionSettings()
        {
            Host = string.Empty;
            BasicAuthUsername = string.Empty;
            BasicAuthPassword = string.Empty;
        }

        public string Host { get; set; }
        public string BasicAuthUsername { get; set; }
        public string BasicAuthPassword { get; set; }

        public override string ToString()
        {
            return $"Host[{Host}], Username[{BasicAuthUsername}], Password[{BasicAuthPassword}], {base.ToString()}";
        }
    }
}
