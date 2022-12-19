using LogCentre.Data;
using LogCentre.Data.Entities;
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

        //[Fact]
        //public void TryGet_ValidId_ReturnsItem()
        //{
        //    var dataItems = new List<Host>()
        //    {
        //        new Host { Id = 1, Name = "a", Description = "a", LastUpdatedBy = "test", Active = DataLiterals.Yes, Deleted = DataLiterals.No },
        //        new Host { Id = 2, Name = "b", Description = "b", LastUpdatedBy = "test", Active = DataLiterals.Yes, Deleted = DataLiterals.No },
        //        new Host { Id = 3, Name = "c", Description = "c", LastUpdatedBy = "test", Active = DataLiterals.Yes, Deleted = DataLiterals.No },
        //    }.AsQueryable();

        //    var options = new DbContextOptionsBuilder<LogCentreDbContext>()
        //        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        //        .Options;
        //    var dbContext = new Mock<LogCentreDbContext>();
        //    dbContext = SetupValidData(dbContext.Object);

        //    //var moqDbContext = new Mock<LogCentreDbContext>().Setup(x => x.Hosts).Returns(dataItems);
        //}

        //private LogCentre
    }
}