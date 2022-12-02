namespace LogCentre.Api.Models
{
    public class ApiVersioningSettings
    {
        public string ReportApiVersions { get; set; }
        public string AssumeDefaultVersionWhenUnspecified { get; set; }
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string HeaderApiVersionName { get; set; }

        public bool ReportApiVersionsValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ReportApiVersions))
                {
                    return false;
                }

                return bool.TryParse(ReportApiVersions, out var value) && value;
            }
        }

        public bool AssumeDefaultVersionWhenUnspecifiedValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AssumeDefaultVersionWhenUnspecified))
                {
                    return false;
                }

                return bool.TryParse(AssumeDefaultVersionWhenUnspecified, out var value) && value;
            }
        }

        public int MajorVersionValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MajorVersion))
                {
                    return 1;
                }

                return !int.TryParse(MajorVersion, out var value) ? 0 : value;
            }
        }

        public int MinorVersionValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MinorVersion))
                {
                    return 1;
                }

                return !int.TryParse(MinorVersion, out var value) ? 0 : value;
            }
        }
    }
}
