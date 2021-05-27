using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using Xunit;

namespace TenureInformationApi.Tests.V1.Gateways
{
    [Collection("DynamoDb collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly DynamoDbGateway _classUnderTest;
        private readonly IDynamoDBContext _dynamoDb;
        private readonly List<Action> _cleanup = new List<Action>();

        public DynamoDbGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _dynamoDb = dbTestFixture.DynamoDbContext;
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _classUnderTest = new DynamoDbGateway(_dynamoDb, _logger.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        [Fact]
        public async Task GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var id = Guid.NewGuid();
            var response = await _classUnderTest.GetEntityById(id).ConfigureAwait(false);

            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {id}", Times.Once());

        }

        [Fact]
        public async Task GetEntityByIdReturnsTheEntityIfItExists()
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var response = await _classUnderTest.GetEntityById(entity.Id).ConfigureAwait(false);

            response.Should().BeEquivalentTo(entity);
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {entity.Id}", Times.Once());

        }

        private async Task InsertDatatoDynamoDB(TenureInformation entity)
        {
            await _dynamoDb.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity.ToDatabase()).ConfigureAwait(false));
        }
    }
}
