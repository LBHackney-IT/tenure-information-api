using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using Xunit;

namespace TenureInformationApi.Tests.V1.Gateways
{
    [Collection("Aws collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<DynamoDbGateway>> _logger;
        private readonly DynamoDbGateway _classUnderTest;
        private readonly IDynamoDBContext _dynamoDb;
        private readonly List<Action> _cleanup = new List<Action>();

        public DynamoDbGatewayTests(AwsIntegrationTests<Startup> dbTestFixture)
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

        private TenureQueryRequest ConstructRequest(Guid? id = null)
        {
            return new TenureQueryRequest() { Id = id ?? Guid.NewGuid() };
        }

        private TenureQueryRequest ConstructUpdateQuery(Guid id)
        {
            return new TenureQueryRequest() { Id = id  };
        }

        private UpdateTenureRequestObject ConstructUpdateRequest(Guid id)
        {
            var request = _fixture.Build<UpdateTenureRequestObject>()
                .With(x => x.Id, id)
                .Create();

            return request;
        }

        private UpdateTenureRequestObject ConstructUpdateFullNameRequest(Guid id, IEnumerable<HouseholdMembers> householdMembers)
        {
            var request = _fixture.Build<UpdateTenureRequestObject>()
                .With(x => x.Id, id)
                .With(x => x.HouseholdMembers, householdMembers.ToList())
                .Create();
            request.HouseholdMembers.First().FullName = "Update";
            return request;
        }

        [Fact]
        public async Task GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var request = ConstructRequest();
            var response = await _classUnderTest.GetEntityById(request).ConfigureAwait(false);

            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {request.Id}", Times.Once());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetEntityByIdReturnsTheEntityIfItExists(bool nullTenuredAssetType)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var request = ConstructRequest(entity.Id);
            var response = await _classUnderTest.GetEntityById(request).ConfigureAwait(false);

            response.Should().BeEquivalentTo(entity);
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {request.Id}", Times.Once());
        }

        [Fact]
        public async Task PostNewTenureSuccessfulSaves()
        {
            // Arrange
            var entityRequest = _fixture.Build<CreateTenureRequestObject>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();

            // Act
            _ = await _classUnderTest.PostNewTenureAsync(entityRequest).ConfigureAwait(false);

            // Assert
            var dbEntity = await _dynamoDb.LoadAsync<TenureInformationDb>(entityRequest.Id).ConfigureAwait(false);

            dbEntity.Should().BeEquivalentTo(entityRequest.ToDatabase());
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync", Times.Once());

            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(dbEntity).ConfigureAwait(false));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateTenureWithNewHouseHoldMemberSuccessfullyUpdates(bool nullTenuredAssetType)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Without(x => x.HouseholdMembers)
                                 .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var query = ConstructUpdateQuery(entity.Id);
            var request = ConstructUpdateRequest(query.Id);

            var update = await _classUnderTest.UpdateTenure(query, request).ConfigureAwait(false);

            var load = await _dynamoDb.LoadAsync<TenureInformationDb>(entity.ToDatabase()).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(load).ConfigureAwait(false));

            //Updated tenure with new HouseHold Member
            load.HouseholdMembers.Should().BeEquivalentTo(request.HouseholdMembers);
            load.Id.Should().Be(entity.Id);
            load.InformHousingBenefitsForChanges.Should().Be(entity.InformHousingBenefitsForChanges);
            load.IsMutualExchange.Should().Be(entity.IsMutualExchange);
            load.IsSublet.Should().Be(entity.IsSublet);
            load.IsTenanted.Should().Be(entity.IsTenanted);
            load.LegacyReferences.Should().BeEquivalentTo(entity.LegacyReferences);
            load.StartOfTenureDate.Should().Be(entity.StartOfTenureDate);
            load.Notices.Should().BeEquivalentTo(entity.Notices);
            load.PaymentReference.Should().Be(entity.PaymentReference);
            load.PotentialEndDate.Should().Be(entity.PotentialEndDate);
            load.SubletEndDate.Should().Be(entity.SubletEndDate);
            load.SuccessionDate.Should().Be(entity.SuccessionDate);
            load.TenuredAsset.Should().BeEquivalentTo(entity.TenuredAsset);
            load.TenureType.Should().BeEquivalentTo(entity.TenureType);
            load.Terminated.Should().BeEquivalentTo(entity.Terminated);

        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateExistingHouseHoldMembersInTenureSuccessfullyUpdates(bool nullTenuredAssetType)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var query = ConstructUpdateQuery(entity.Id);
            var request = ConstructUpdateFullNameRequest(query.Id, entity.HouseholdMembers);
            var update = await _classUnderTest.UpdateTenure(query, request).ConfigureAwait(false);

            var load = await _dynamoDb.LoadAsync<TenureInformationDb>(entity.ToDatabase()).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(load).ConfigureAwait(false));

            //Updated tenure with updated HouseHold Member
            load.HouseholdMembers.Should().BeEquivalentTo(request.HouseholdMembers);
            load.Id.Should().Be(entity.Id);
            load.InformHousingBenefitsForChanges.Should().Be(entity.InformHousingBenefitsForChanges);
            load.IsMutualExchange.Should().Be(entity.IsMutualExchange);
            load.IsSublet.Should().Be(entity.IsSublet);
            load.IsTenanted.Should().Be(entity.IsTenanted);
            load.LegacyReferences.Should().BeEquivalentTo(entity.LegacyReferences);
            load.StartOfTenureDate.Should().Be(entity.StartOfTenureDate);
            load.Notices.Should().BeEquivalentTo(entity.Notices);
            load.PaymentReference.Should().Be(entity.PaymentReference);
            load.PotentialEndDate.Should().Be(entity.PotentialEndDate);
            load.SubletEndDate.Should().Be(entity.SubletEndDate);
            load.SuccessionDate.Should().Be(entity.SuccessionDate);
            load.TenuredAsset.Should().BeEquivalentTo(entity.TenuredAsset);
            load.TenureType.Should().BeEquivalentTo(entity.TenureType);
            load.Terminated.Should().BeEquivalentTo(entity.Terminated);

        }

        private async Task InsertDatatoDynamoDB(TenureInformation entity)
        {
            await _dynamoDb.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity.ToDatabase()).ConfigureAwait(false));
        }
    }
}
