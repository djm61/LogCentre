using LogCentre.Data;
using LogCentre.Data.Entities;

namespace LogCentre.SeedData
{
    public static class DataInjector
    {
        public static void Initialize(LogCentreDbContext context)
        {
            context.Database.EnsureCreated();

            #region Provider

            Provider[] providers;
            if (!context.Providers.Any())
            {
                providers = new[]
                {
                    new Provider {Name = "Serilog", Description = "Provider for Serilog log files", Regex = "(?<Date>\\d{4}-\\d{2}-\\d{2}\\s\\d{2}:\\d{2}:\\d{2}.\\d{3})+\\s\\[(?<Level>\\w+)+\\]+\\s\\[+(?<Thread>\\d+)+\\]+\\s\\[(?<Source>.*)\\](?<Text>.*)", LastUpdatedBy = "Seed"}
                };

                context.Providers.AddRange(providers);
                context.SaveChanges();
            }

            #endregion
        }
    }
}