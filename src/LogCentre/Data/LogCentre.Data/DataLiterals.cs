namespace LogCentre.Data
{
    public static class DataLiterals
    {
        public static string Yes = "Y";
        public static string No = "N";

        public const int FlagLength = 1;
        public const int NameLength = 100;
        public const int DescriptionLength = 1000;
        public const int RegexLength = 1000;
        public const int PathLength = 500;
        public const int LastUpdatedByLength = 256; //this is the length of the email column in identity framework
    }
}
