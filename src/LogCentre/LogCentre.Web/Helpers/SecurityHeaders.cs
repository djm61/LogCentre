namespace LogCentre.Web.Helpers
{
    public class SecurityHeaders
    {
        /// <summary>
        /// Constructor to initialize everything
        /// </summary>
        public SecurityHeaders()
        {
            StrictTransportSecurity = "false";
            XFrameOptions = "false";
            XContentTypeOptions = "false";
            ContentSecurityPolicy = "false";
            ContentSecurityPolicyOptions = new ContentSecurityPolicyOptions();
        }

        /// <summary>
        /// Strict Transport Security flag
        /// </summary>
        public string StrictTransportSecurity { get; set; }

        /// <summary>
        /// X-Frame Options flag
        /// </summary>
        public string XFrameOptions { get; set; }

        /// <summary>
        /// X-Content Type Options flag
        /// </summary>
        public string XContentTypeOptions { get; set; }

        /// <summary>
        /// Content Security Policy flag
        /// </summary>
        public string ContentSecurityPolicy { get; set; }

        /// <summary>
        /// Bool flag for <see cref="StrictTransportSecurity"/>
        /// </summary>
        public bool StrictTransportSecurityAsBool
        {
            get
            {
                if (string.IsNullOrWhiteSpace(StrictTransportSecurity))
                {
                    return false;
                }

                bool.TryParse(StrictTransportSecurity, out var bStrictTransportSecurity);
                return bStrictTransportSecurity;
            }
        }

        /// <summary>
        /// Bool flag for <see cref="XFrameOptions"/>
        /// </summary>
        public bool XFrameOptionsAsBool
        {
            get
            {
                if (string.IsNullOrWhiteSpace(XFrameOptions))
                {
                    return false;
                }

                bool.TryParse(XFrameOptions, out var bXFrameOptions);
                return bXFrameOptions;
            }
        }

        /// <summary>
        /// Bool flag for <see cref="XContentTypeOptions"/>
        /// </summary>
        public bool XContentTypeOptionsAsBool
        {
            get
            {
                if (string.IsNullOrWhiteSpace(XContentTypeOptions))
                {
                    return false;
                }

                bool.TryParse(XContentTypeOptions, out var bXContentTypeOptions);
                return bXContentTypeOptions;
            }
        }

        /// <summary>
        /// Bool flag for <see cref="ContentSecurityPolicy"/>
        /// </summary>
        public bool ContentSecurityPolicyAsBool
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ContentSecurityPolicy))
                {
                    return false;
                }

                bool.TryParse(ContentSecurityPolicy, out var bContentSecurityPolicy);
                return bContentSecurityPolicy;
            }
        }

        /// <summary>
        /// Content Security Policy Options
        /// </summary>
        public ContentSecurityPolicyOptions ContentSecurityPolicyOptions { get; set; }

        public override string ToString()
        {
            return $"StrictTransportSecurity[{StrictTransportSecurity}], XFrameOptions[{XFrameOptions}], XContentTypeOptions[{XContentTypeOptions}], ContentSecurityPolicy[{ContentSecurityPolicy}], ContentSecurityPolicyOptions[{ContentSecurityPolicyOptions}], {base.ToString}";
        }
    }

    /// <summary>
    /// Content Security Policy Options
    /// </summary>
    public class ContentSecurityPolicyOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ContentSecurityPolicyOptions()
        {
            ContentTypeOptions = string.Empty;
            FrameOptions = string.Empty;
        }

        /// <summary>
        /// Content Type Options string
        /// </summary>
        public string ContentTypeOptions { get; set; }

        /// <summary>
        /// Frame Options string
        /// </summary>
        public string FrameOptions { get; set; }

        public override string ToString()
        {
            return $"ContentTypeOptions[{ContentTypeOptions}], FrameOptions[{FrameOptions}], {base.ToString()}";
        }
    }
}
