namespace LogCentre.Model
{
    public static class ModelLiterals
    {
        /// <summary>
        /// Value for Yes
        /// </summary>
        public const string Yes = "Y";

        /// <summary>
        /// Value for No
        /// </summary>
        public const string No = "N";

        /// <summary>
        /// Length for the Flag column
        /// </summary>
        public const int FlagLength = 1;

        /// <summary>
        /// Length for the Name column
        /// </summary>
        public const int NameLength = 100;

        /// <summary>
        /// Length for the Description column
        /// </summary>
        public const int DescriptionLength = 1000;

        /// <summary>
        /// Length for the Regex column
        /// </summary>
        public const int RegexLength = 1000;

        /// <summary>
        /// Length of the Path column
        /// </summary>
        public const int PathLength = 500;

        /// <summary>
        /// Length for the Maximum column
        /// </summary>
        public const int MaxLength = 4000;

        /// <summary>
        /// Length for the LastUpdatedBy column
        /// Note: Identity framework sets it to 256 - the email column
        /// </summary>
        public const int LastUpdatedByLength = 256; //this is the length of the email column in identity framework
    }
}
