namespace LogCentre.Api.Helpers
{
    public enum CacheType
    {
        Redis, Memory, Null
    }

    public class CachingSettings
    {
        public bool Enabled { get; set; }
        public CacheType CacheType { get; set; }
        public double SlidingExpiration { get; set; }
        public double AbsoluteExpiration { get; set; }
        public string ConnectionString { get; set; }
    }
}
