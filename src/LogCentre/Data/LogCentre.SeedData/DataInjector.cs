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
                    new Provider {Name = "Serilog", Description = "Provider for Serilog log files", Regex = "(?<Date>\\d{4}-\\d{2}-\\d{2}\\s\\d{2}:\\d{2}:\\d{2}.\\d{3})+\\s\\[(?<Level>\\w+)+\\]+\\s\\[+(?<Thread>\\d+)+\\]+\\s\\[(?<Source>.*?)\\]\\s(?<Text>.*)", LastUpdatedBy = "Seed"},
                    new Provider {Name = "log4net", Description = "Provider for log4net log files", Regex = "(?<Level>\\w+)+\\s+(?<Date>\\d{4}-\\d{2}-\\d{2}\\s\\d{2}:\\d{2}:\\d{2},\\d{3})+\\s+\\[+(?<Thread>.{5})+\\]+\\s+(?<Source>.{30})+\\s+(?<Text>.*)", LastUpdatedBy = "Seed"}
                };

                context.Providers.AddRange(providers);
                context.SaveChanges();
            }

#if DEBUG
            Host[] hosts;
            if (!context.Hosts.Any())
            {
                hosts = new Host[]
                {
                    new Host{Name = "local", Description = "Local", LastUpdatedBy = "seed"}
                };

                context.Hosts.AddRange(hosts);
                context.SaveChanges();
            }

            LogSource[] logSources;
            if (!context.Sources.Any())
            {
                logSources = new[]
                {
                    new LogSource {HostId = 1, ProviderId = 1, Name = "local log centre", Path = "C:/ProgramData/LogCentre/local/", LastUpdatedBy = "seed"}
                };

                context.Sources.AddRange(logSources);
                context.SaveChanges();
            }

            //Data.Entities.Log.File[] files;
            //if (!context.LogFiles.Any())
            //{
            //    files = new[]
            //    {
            //        new Data.Entities.Log.File{LogSourceId = 1, Name = "LogCentre.Api-"}
            //    }
            //}
#endif

            #endregion
        }
    }
}