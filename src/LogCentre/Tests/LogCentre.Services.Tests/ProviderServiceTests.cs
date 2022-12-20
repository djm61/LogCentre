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
    public class ProviderServiceTests
    {
        private ILoggerFactory LoggerFactory = new NullLoggerFactory();
        private readonly Random _random;

        public ProviderServiceTests()
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

            Action action = () => new ProviderService(null, mockDbContextFactory.Object);
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void Constructor_NullDbContextFactory_ThrowsArgumentNullException()
        {
            var logger = LoggerFactory.CreateLogger<ProviderService>();
            Action action = () => new ProviderService(logger, null);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var result = service.TryGet(providerId, out var provider);

                Assert.True(result);
                Assert.NotNull(provider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var result = service.TryGet(-1, out var provider);

                Assert.False(result);
                Assert.Null(provider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var result = await service.GetAsync(providerId);

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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var result = await service.GetAsync(providerId + 1);

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
        public async Task GetAsync_InvalidInput_NegativeValue_ThrowsProviderException()
        {
            LogCentreDbContext context = null;
            try
            {
                var options = new DbContextOptionsBuilder<LogCentreDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                context = new LogCentreDbContext(options, LoggerFactory);

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                await Assert.ThrowsAsync<ProviderException>(async () => await service.GetAsync(-1));
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == providerId, null, "", 1, 1);

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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var result = await service.GetAsync(x => x.Id == providerId, null, "", 10, 10);

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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var provider = new Provider
                {
                    Id = id,
                    Name = "Provider 2",
                    Description = "Provider 2",
                    Regex = "regex 2",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var result = await service.CreateAsync(provider);
                Assert.NotNull(result);

                provider = await service.GetAsync(id);
                Assert.NotNull(provider);

                Assert.Equal(result, provider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
                var provider = await service.GetAsync(providerId);
                Assert.NotNull(provider);

                provider.Name = "new provider";
                await service.UpdateAsync(provider);

                provider = await service.GetAsync(providerId);
                Assert.NotNull(provider);
                Assert.Equal("new provider", provider.Name);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var provider = await service.GetAsync(providerId);
                Assert.NotNull(provider);

                await service.RemoveAsync(provider);

                provider = await service.GetAsync(providerId);
                Assert.Null(provider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var provider = new Provider
                {
                    Id = id,
                    Name = "name",
                    Description = "description",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                await Assert.ThrowsAsync<ProviderException>(async () => await service.RemoveAsync(provider));
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var provider = await service.GetAsync(providerId);
                Assert.NotNull(provider);

                await service.DeleteAsync(provider);

                provider = await service.GetAsync(providerId);
                Assert.Null(provider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var provider = new Provider
                {
                    Id = id,
                    Name = "name",
                    Description = "description",
                    Regex = "regex",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var addResult = await service.CreateAsync(provider);
                Assert.NotNull(addResult);

                var newProvider = await service.GetAsync(id);
                Assert.NotNull(newProvider);

                var result = service.Exists(newProvider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var result = service.Exists(new Provider { Id = id });
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var provider = new Provider
                {
                    Id = id,
                    Name = "name",
                    Description = "description",
                    Regex = "regex",
                    LastUpdatedBy = "test",
                    Active = DataLiterals.Yes,
                    Deleted = DataLiterals.No,
                    RowVersion = DateTime.UtcNow
                };

                var addResult = await service.CreateAsync(provider);
                Assert.NotNull(addResult);

                var newProvider = await service.GetAsync(id);
                Assert.NotNull(newProvider);

                var result = service.IsTracked(newProvider);
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

                var providerId = _random.NextInt64();
                context = SetupData(context, providerId);

                var mockContextFactory = new Mock<IDbContextFactory<LogCentreDbContext>>();
                mockContextFactory.Setup(x => x.CreateDbContext())
                    .Returns(context);

                var service = new ProviderService(LoggerFactory.CreateLogger<ProviderService>(), mockContextFactory.Object);

                var id = _random.NextInt64();
                var result = service.IsTracked(new Provider { Id = id });
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

        private LogCentreDbContext SetupData(LogCentreDbContext context, long providerId)
        {
            var provider = new Provider
            {
                Id = providerId,
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Regex = Guid.NewGuid().ToString(),
                LastUpdatedBy = "test",
                Active = DataLiterals.Yes,
                Deleted = DataLiterals.No,
                RowVersion = DateTime.UtcNow
            };

            context.Providers.Add(provider);
            context.SaveChanges();

            return context;
        }
    }
}