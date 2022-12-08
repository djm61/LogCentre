namespace LogCentre.Api.Models
{
    /// <summary>
    /// API Versioning Settings
    /// </summary>
    public class ApiVersioningSettings
    {
        /// <summary>
        /// Report API Versions
        /// </summary>
        public string ReportApiVersions { get; set; }

        /// <summary>
        /// Assume Default Version When Unspecified
        /// </summary>
        public string AssumeDefaultVersionWhenUnspecified { get; set; }

        /// <summary>
        /// Major Version
        /// </summary>
        public string MajorVersion { get; set; }

        /// <summary>
        /// Minor Version
        /// </summary>
        public string MinorVersion { get; set; }

        /// <summary>
        /// Header API Version Name
        /// </summary>
        public string HeaderApiVersionName { get; set; }

        /// <summary>
        /// Boolean Report API Version
        /// </summary>
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

        /// <summary>
        /// Boolean Assume Default Version When Unspecified
        /// </summary>
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

        /// <summary>
        /// Int Major Version
        /// </summary>
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

        /// <summary>
        /// Int Minor Version
        /// </summary>
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

        /// <summary>
        /// ToString implemenation
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"MajorVersion[{MajorVersion}], MinorVersion[{MinorVersion}], {base.ToString()}";
        }
    }
}
