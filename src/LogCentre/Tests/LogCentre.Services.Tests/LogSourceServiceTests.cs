using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

namespace LogCentre.Services.Tests
{
    public class LogSourceServiceTests
    {
        private ILoggerFactory LoggerFactory = new NullLoggerFactory();
        private readonly Random _random;

        public LogSourceServiceTests()
        {
            _random = new Random(DateTime.UtcNow.GetHashCode());
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var mockDbContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
            mockDbContextFactory.Setup(x => x.CreateDbContext())
                .Returns(() => new LogCentreDbContext(options, LoggerFactory));

            Action action = () => new LogSourceService(null, mockDbContextFactory.Object);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void Constructor_NullDbContextFactory_ThrowsArgumentNullException()
        {
            var logger = LoggerFactory.CreateLogger<LogSourceService>();
            Action action = () => new LogSourceService(logger, null);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void TryGet_ValidInput_ReturnsSingleItem()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = service.TryGet(logSourceId, out var logSource);

                Assert.True(result);
                Assert.NotNull(logSource);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public void TryGet_InvalidInput_ReturnsNoItems()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = service.TryGet(-1, out var logSource);

                Assert.False(result);
                Assert.Null(logSource);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task GetAsync_ValidInput_ReturnsSingleItem()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = await service.GetAsync(logSourceId);

                Assert.NotNull(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task GetAsync_InvalidInput_OutOfRange_ReturnsNull()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = await service.GetAsync(logSourceId + 1);

                Assert.Null(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task GetAsync_InvalidInput_NegativeValue_ThrowsLogSourceException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                await Assert.ThrowsAsync<LogSourceException>(async () => await service.GetAsync(-1));
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task Base_GetAsync_ValidInput_ReturnsSingleItem()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == logSourceId, null, "", 1, 1);

                Assert.NotNull(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task Base_GetAsync_ValidInput_RecordCount_PageSize_ReturnsNoItems()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == logSourceId, null, "", 10, 10);

                Assert.NotNull(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task Base_GetAsync_InvalidInput_ReturnsNull()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == -1);

                Assert.NotNull(result);
                Assert.Empty(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task CreateAsync_ValidInput_ReturnsNewItem()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var logSource = new LogSource
                {
                    Id = id,
                    HostId = logSourceId,
                    ProviderId = logSourceId,
                    Name = "LogSource 2",
                    Path = "path 2",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var result = await service.CreateAsync(logSource);
                Assert.NotNull(result);

                logSource = await service.GetAsync(id);
                Assert.NotNull(logSource);

                Assert.Equal(result, logSource);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task CreateAsync_NullInput_ThrowsException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                await Assert.ThrowsAsync<EntityException>(async () => await service.CreateAsync(null));
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task UpdateAsync_ValidInput_ReturnsNothing()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                var logSource = await service.GetAsync(logSourceId);
                Assert.NotNull(logSource);

                logSource.Name = "new logSource";
                await service.UpdateAsync(logSource);

                logSource = await service.GetAsync(logSourceId);
                Assert.NotNull(logSource);
                Assert.Equal("new logSource", logSource.Name);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task UpdateAsync_NullInput_ThrowsException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);
                await Assert.ThrowsAsync<EntityException>(async () => await service.UpdateAsync(null));
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task RemoveAsync_ValidInput_ReturnsNothing()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var logSource = await service.GetAsync(logSourceId);
                Assert.NotNull(logSource);

                await service.RemoveAsync(logSource);

                logSource = await service.GetAsync(logSourceId);
                Assert.Null(logSource);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task RemoveAsync_NewEntityInput_ThrowsException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var logSource = new LogSource
                {
                    Id = id,
                    HostId = logSourceId,
                    ProviderId = logSourceId,
                    Name = "name",
                    Path = "path",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                await Assert.ThrowsAsync<LogSourceException>(async () => await service.RemoveAsync(logSource));
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task DeleteAsync_ValidInput_ReturnsNothing()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var logSource = await service.GetAsync(logSourceId);
                Assert.NotNull(logSource);

                await service.DeleteAsync(logSource);

                logSource = await service.GetAsync(logSourceId);
                Assert.Null(logSource);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task DeleteAsync_NullInput_ThrowsException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                await Assert.ThrowsAsync<EntityException>(async () => await service.DeleteAsync(null));
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task Exists_ValidInput_ReturnsTrue()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var logSource = new LogSource
                {
                    Id = id,
                    HostId = logSourceId,
                    ProviderId = logSourceId,
                    Name = "name",
                    Path = "path",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var addResult = await service.CreateAsync(logSource);
                Assert.NotNull(addResult);

                var newLogSource = await service.GetAsync(id);
                Assert.NotNull(newLogSource);

                var result = service.Exists(newLogSource);
                Assert.True(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public void Exists_InvalidInput_ReturnsFalse()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var result = service.Exists(new LogSource { Id = id });
                Assert.False(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task IsTracked_ValidInput_ReturnsTrue()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var logSource = new LogSource
                {
                    Id = id,
                    HostId = logSourceId,
                    ProviderId = logSourceId,
                    Name = "name",
                    Path = "path",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var addResult = await service.CreateAsync(logSource);
                Assert.NotNull(addResult);

                var newLogSource = await service.GetAsync(id);
                Assert.NotNull(newLogSource);

                var result = service.IsTracked(newLogSource);
                Assert.True(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        [Fact]
        public async Task IsTracked_InvalidInput_ReturnsFalse()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var logSourceId = _random.NextInt64();
                context = SetupData(context, logSourceId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new LogSourceService(LoggerFactory.CreateLogger<LogSourceService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var result = service.IsTracked(new LogSource { Id = id });
                Assert.False(result);
            }
            finally
            {
                if (context != null)
                {
                    context.Database.EnsureDeleted();
                    context.Dispose();
                }
            }
        }

        private LogCentreDbContext SetupData(LogCentreDbContext context, long logSourceId)
        {
            var host = new Host
            {
                Id = logSourceId,
                Name = "host",
                Description = "description",
                LastUpdatedBy = "test",
                Active = DataLiterals.Yes,
                Deleted = DataLiterals.No,
                RowVersion = DateTime.UtcNow
            };

            context.Hosts.Add(host);

            var provider = new Provider
            {
                Id = logSourceId,
                Name = "provider",
                Description = "description",
                Regex = "regex",
                LastUpdatedBy = "test",
                Active = DataLiterals.Yes,
                Deleted = DataLiterals.No,
                RowVersion = DateTime.UtcNow
            };

            context.Providers.Add(provider);

            var logSource = new LogSource
            {
                Id = logSourceId,
                HostId = logSourceId,
                ProviderId = logSourceId,
                Name = Guid.NewGuid().ToString(),
                Path = "path",
                LastUpdatedBy = "test",
                Active = DataLiterals.Yes,
                Deleted = DataLiterals.No,
                RowVersion = DateTime.UtcNow
            };

            context.Sources.Add(logSource);
            context.SaveChanges();

            return context;
        }
    }
}