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
    public class HostServiceTests
    {
        private ILoggerFactory LoggerFactory = new NullLoggerFactory();
        private readonly Random _random;

        public HostServiceTests()
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

            Action action = () => new HostService(null, mockDbContextFactory.Object);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void Constructor_NullDbContextFactory_ThrowsArgumentNullException()
        {
            var logger = LoggerFactory.CreateLogger<HostService>();
            Action action = () => new HostService(logger, null);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var result = service.TryGet(hostId, out var host);

                Assert.True(result);
                Assert.NotNull(host);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var result = service.TryGet(-1, out var host);

                Assert.False(result);
                Assert.Null(host);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var result = await service.GetAsync(hostId);

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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var result = await service.GetAsync(hostId + 1);

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
        public async Task GetAsync_InvalidInput_NegativeValue_ThrowsHostException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                await Assert.ThrowsAsync<HostException>(async () => await service.GetAsync(-1));
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == hostId, null, "", 1, 1);

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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == hostId, null, "", 10, 10);

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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var host = new Host
                {
                    Id = id,
                    Name = "Host 2",
                    Description = "Host 2",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var result = await service.CreateAsync(host);
                Assert.NotNull(result);

                host = await service.GetAsync(id);
                Assert.NotNull(host);

                Assert.Equal(result, host);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
                var host = await service.GetAsync(hostId);
                Assert.NotNull(host);

                host.Name = "new host";
                await service.UpdateAsync(host);

                host = await service.GetAsync(hostId);
                Assert.NotNull(host);
                Assert.Equal("new host", host.Name);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var host = await service.GetAsync(hostId);
                Assert.NotNull(host);

                await service.RemoveAsync(host);

                host = await service.GetAsync(hostId);
                Assert.Null(host);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var host = new Host
                {
                    Id = id,
                    Name = "name",
                    Description = "description",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                await Assert.ThrowsAsync<HostException>(async () => await service.RemoveAsync(host));
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var host = await service.GetAsync(hostId);
                Assert.NotNull(host);

                await service.DeleteAsync(host);

                host = await service.GetAsync(hostId);
                Assert.Null(host);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var host = new Host
                {
                    Id = id,
                    Name = "name",
                    Description = "description",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var addResult = await service.CreateAsync(host);
                Assert.NotNull(addResult);

                var newHost = await service.GetAsync(id);
                Assert.NotNull(newHost);

                var result = service.Exists(newHost);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var result = service.Exists(new Host { Id = id });
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var host = new Host
                {
                    Id = id,
                    Name = "name",
                    Description = "description",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var addResult = await service.CreateAsync(host);
                Assert.NotNull(addResult);

                var newHost = await service.GetAsync(id);
                Assert.NotNull(newHost);

                var result = service.IsTracked(newHost);
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

                var hostId = _random.NextInt64();
                context = SetupData(context, hostId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new HostService(LoggerFactory.CreateLogger<HostService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var result = service.IsTracked(new Host { Id = id });
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

        private LogCentreDbContext SetupData(LogCentreDbContext context, long hostId)
        {
            var host = new Host
            {
                Id = hostId,
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                LastUpdatedBy = "test",
                Active = DataLiterals.Yes,
                Deleted = DataLiterals.No,
                RowVersion = DateTime.UtcNow
            };

            context.Hosts.Add(host);
            context.SaveChanges();

            return context;
        }
    }
}