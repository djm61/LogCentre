namespace LogCentre.Data.Interfaces
{
    internal interface IBaseEntity<TKey> where TKey : struct
    {
        TKey Id { get; set; }
        string Active { get; set; }
        string Deleted { get; set; }
        string LastUpdatedBy { get; set; }
        DateTime RowVersion { get; set; }
    }
}
