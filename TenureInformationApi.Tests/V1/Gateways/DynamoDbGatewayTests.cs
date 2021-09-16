using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Extensions;
using Hackney.Core.JWT;
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
using TenureInformationApi.V1.Infrastructure.Exceptions;
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

        private readonly Mock<IEntityUpdater> _mockUpdater;

        public DynamoDbGatewayTests(AwsIntegrationTests<Startup> dbTestFixture)
        {
            _dynamoDb = dbTestFixture.DynamoDbContext;
            _logger = new Mock<ILogger<DynamoDbGateway>>();
            _mockUpdater = new Mock<IEntityUpdater>();
            _classUnderTest = new DynamoDbGateway(_dynamoDb, _mockUpdater.Object, _logger.Object);
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

        private UpdateTenureRequest ConstructUpdateQuery(Guid id, Guid personId)
        {
            return new UpdateTenureRequest() { Id = id, PersonId = personId };
        }

        private UpdateTenureForPersonRequestObject ConstructUpdateRequest()
        {
            var request = _fixture.Build<UpdateTenureForPersonRequestObject>()
                .With(x => x.DateOfBirth, DateTime.UtcNow.AddYears(-30))
                .Create();
            return request;
        }

        private UpdateTenureForPersonRequestObject ConstructUpdateFullNameRequest()
        {
            var request = new UpdateTenureForPersonRequestObject()
            {
                FullName = "Update"
            };
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
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var request = ConstructRequest(entity.Id);
            var response = await _classUnderTest.GetEntityById(request).ConfigureAwait(false);

            response.Should().BeEquivalentTo(entity, config => config.Excluding(y => y.VersionNumber));
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

            dbEntity.Should().BeEquivalentTo(entityRequest.ToDatabase(), config => config.Excluding(y => y.VersionNumber));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync", Times.Once());

            _cleanup.Add(async () => await _dynamoDb.DeleteAsync<TenureInformationDb>(dbEntity.Id).ConfigureAwait(false));
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
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var dbEntity = entity.ToDatabase();

            var query = ConstructUpdateQuery(entity.Id, Guid.NewGuid());
            var request = ConstructUpdateRequest();
            dbEntity.VersionNumber = 0;

            var result = await _classUnderTest.UpdateTenureForPerson(query, request, 0).ConfigureAwait(false);

            var load = await _dynamoDb.LoadAsync<TenureInformationDb>(dbEntity.Id).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync<TenureInformationDb>(load.Id).ConfigureAwait(false));

            //Updated tenure with new Household Member
            result.UpdatedEntity.Should().BeEquivalentTo(load, config => config.Excluding(y => y.VersionNumber));

            load.AgreementType.Should().BeEquivalentTo(dbEntity.AgreementType);
            load.Charges.Should().BeEquivalentTo(dbEntity.Charges);
            load.EndOfTenureDate.Should().Be(dbEntity.EndOfTenureDate);
            load.EvictionDate.Should().Be(dbEntity.EvictionDate);
            load.Id.Should().Be(dbEntity.Id);
            load.InformHousingBenefitsForChanges.Should().Be(dbEntity.InformHousingBenefitsForChanges);
            load.IsMutualExchange.Should().Be(dbEntity.IsMutualExchange);
            load.IsSublet.Should().Be(dbEntity.IsSublet);
            load.IsTenanted.Should().Be(dbEntity.IsTenanted);
            load.LegacyReferences.Should().BeEquivalentTo(dbEntity.LegacyReferences);
            load.Notices.Should().BeEquivalentTo(dbEntity.Notices);
            load.PaymentReference.Should().Be(dbEntity.PaymentReference);
            load.PotentialEndDate.Should().Be(dbEntity.PotentialEndDate);
            load.StartOfTenureDate.Should().Be(dbEntity.StartOfTenureDate);
            load.SubletEndDate.Should().Be(dbEntity.SubletEndDate);
            load.SuccessionDate.Should().Be(dbEntity.SuccessionDate);
            load.TenuredAsset.Should().BeEquivalentTo(dbEntity.TenuredAsset);
            load.TenureType.Should().BeEquivalentTo(dbEntity.TenureType);
            load.Terminated.Should().BeEquivalentTo(dbEntity.Terminated);
            load.VersionNumber.Should().Be(1);

            var expected = new HouseholdMembers()
            {
                DateOfBirth = request.DateOfBirth.Value,
                FullName = request.FullName,
                Id = query.PersonId,
                IsResponsible = request.IsResponsible.Value,
                PersonTenureType = TenureTypes.GetPersonTenureType(entity.TenureType, request.IsResponsible.Value),
                Type = request.Type.Value
            };
            load.HouseholdMembers.Should().ContainEquivalentOf(expected);
            load.HouseholdMembers.Except(load.HouseholdMembers.Where(x => x.Id == query.PersonId)).Should().BeEmpty();

            result.OldValues["householdMembers"].Should().BeEquivalentTo(entity.ToDatabase().HouseholdMembers);
            result.NewValues["householdMembers"].Should().BeEquivalentTo(result.UpdatedEntity.HouseholdMembers);
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
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);
            var dbEntity = entity.ToDatabase();
            dbEntity.VersionNumber = 0;

            var query = ConstructUpdateQuery(entity.Id, entity.HouseholdMembers.First().Id);
            var request = ConstructUpdateFullNameRequest();
            var result = await _classUnderTest.UpdateTenureForPerson(query, request, 0).ConfigureAwait(false);

            var load = await _dynamoDb.LoadAsync<TenureInformationDb>(dbEntity.Id).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync<TenureInformationDb>(load.Id).ConfigureAwait(false));

            result.UpdatedEntity.Should().BeEquivalentTo(load, config => config.Excluding(y => y.VersionNumber));


            //Updated tenure with updated HouseHold Member
            load.AgreementType.Should().BeEquivalentTo(dbEntity.AgreementType);
            load.Charges.Should().BeEquivalentTo(dbEntity.Charges);
            load.EndOfTenureDate.Should().Be(dbEntity.EndOfTenureDate);
            load.EvictionDate.Should().Be(dbEntity.EvictionDate);
            load.Id.Should().Be(dbEntity.Id);
            load.InformHousingBenefitsForChanges.Should().Be(dbEntity.InformHousingBenefitsForChanges);
            load.IsMutualExchange.Should().Be(dbEntity.IsMutualExchange);
            load.IsSublet.Should().Be(dbEntity.IsSublet);
            load.IsTenanted.Should().Be(dbEntity.IsTenanted);
            load.LegacyReferences.Should().BeEquivalentTo(dbEntity.LegacyReferences);
            load.Notices.Should().BeEquivalentTo(dbEntity.Notices);
            load.PaymentReference.Should().Be(dbEntity.PaymentReference);
            load.PotentialEndDate.Should().Be(dbEntity.PotentialEndDate);
            load.StartOfTenureDate.Should().Be(dbEntity.StartOfTenureDate);
            load.SubletEndDate.Should().Be(dbEntity.SubletEndDate);
            load.SuccessionDate.Should().Be(dbEntity.SuccessionDate);
            load.TenuredAsset.Should().BeEquivalentTo(dbEntity.TenuredAsset);
            load.TenureType.Should().BeEquivalentTo(dbEntity.TenureType);
            load.Terminated.Should().BeEquivalentTo(dbEntity.Terminated);
            load.HouseholdMembers.First(x => x.Id == query.PersonId).FullName.Should().Be(request.FullName);
            load.VersionNumber.Should().Be(1);

            result.OldValues["householdMembers"].Should().BeEquivalentTo(entity.ToDatabase().HouseholdMembers);
            result.NewValues["householdMembers"].Should().BeEquivalentTo(result.UpdatedEntity.HouseholdMembers);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(5, true)]
        public async Task UpdateTenureForPersonThrowsExceptionOnVersionConflict(int? ifMatch, bool nullTenuredAssetType)
        {
            // Arrange
            var entity = _fixture.Build<TenureInformation>()
                                            .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                            .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                            .With(x => x.SuccessionDate, DateTime.UtcNow)
                                            .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                            .With(x => x.SubletEndDate, DateTime.UtcNow)
                                            .With(x => x.EvictionDate, DateTime.UtcNow)
                                            .With(x => x.VersionNumber, (int?) null)
                                            .Create();
            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;

            var query = ConstructUpdateQuery(entity.Id, entity.HouseholdMembers.First().Id);
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);
            entity.VersionNumber = 0;

            var constructRequest = ConstructUpdateRequest();

            //Act
            Func<Task<UpdateEntityResult<TenureInformationDb>>> func = async () => await _classUnderTest.UpdateTenureForPerson(query, constructRequest, ifMatch)
                                                                                                   .ConfigureAwait(false);

            // Assert
            func.Should().Throw<VersionNumberConflictException>()
                         .Where(x => (x.IncomingVersionNumber == ifMatch) && (x.ExpectedVersionNumber == 0));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {query.Id}", Times.Never());
        }

        private async Task InsertDatatoDynamoDB(TenureInformation entity)
        {
            var entityDb = entity.ToDatabase();
            await _dynamoDb.SaveAsync<TenureInformationDb>(entityDb).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync<TenureInformationDb>(entityDb.Id).ConfigureAwait(false));
        }

        [Fact]
        public async Task EditTenureDetailsWhenTenureDoesntExistReturnsNull()
        {
            var mockQuery = _fixture.Create<TenureQueryRequest>();
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRawBody = "";

            // call gateway method
            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRawBody).ConfigureAwait(false);

            // assert result = null
            response.Should().BeNull();
        }

        [Fact]
        public async Task EditTenureDetailsWhenNoChangesAreInRequestObjectDatabaseIsntUpdated()
        {
            // create mock tenure
            var mockTenure = _fixture.Build<TenureInformation>()
                .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                .With(x => x.SuccessionDate, DateTime.UtcNow)
                .With(x => x.PotentialEndDate, DateTime.UtcNow)
                .With(x => x.SubletEndDate, DateTime.UtcNow)
                .With(x => x.EvictionDate, DateTime.UtcNow)
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            // insert mock tenure into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);

            var mockQuery = _fixture.Build<TenureQueryRequest>().With(x => x.Id, mockTenure.Id).Create(); // with tenure id
            var mockRequestObject = new EditTenureDetailsRequestObject();
            var mockRawBody = "";

            var updaterResponse = new UpdateEntityResult<TenureInformationDb>(); // with no changes

            // setup updater
            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<TenureInformationDb>(), It.IsAny<string>(), It.IsAny<EditTenureDetailsRequestObject>()))
                .Returns(updaterResponse);

            // call gateway method
            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRawBody).ConfigureAwait(false);

            // assert response is UpdateEntityResult
            response.Should().BeOfType(typeof(UpdateEntityResult<TenureInformationDb>));

            // load entity from database
            var databaseResponse = await _dynamoDb.LoadAsync<TenureInformationDb>(mockTenure.Id).ConfigureAwait(false);

            // assert entity wasnt updated (matches inserted data)
            databaseResponse.StartOfTenureDate.Should().BeCloseTo((DateTime) mockTenure.StartOfTenureDate, 1.Seconds());
            databaseResponse.EndOfTenureDate.Should().BeCloseTo((DateTime) mockTenure.EndOfTenureDate, 1.Seconds());
            databaseResponse.TenureType.Code.Should().Be(mockTenure.TenureType.Code);
        }

        [Fact]
        public async Task EditTenureDetailsWhenCalledDatabaseIsUpdated()
        {
            // create mock tenure
            var mockTenure = _fixture.Build<TenureInformation>()
                            .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                            .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                            .With(x => x.SuccessionDate, DateTime.UtcNow)
                            .With(x => x.PotentialEndDate, DateTime.UtcNow)
                            .With(x => x.SubletEndDate, DateTime.UtcNow)
                            .With(x => x.EvictionDate, DateTime.UtcNow)
                            .With(x => x.VersionNumber, (int?) null)
                            .Create();

            // insert mock tenure into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);
            mockTenure.VersionNumber = 0;

            var mockQuery = _fixture.Build<TenureQueryRequest>().With(x => x.Id, mockTenure.Id).Create(); // with tenure id
            var mockRequestObject = new EditTenureDetailsRequestObject();
            var mockRawBody = "";

            var updaterResponse = CreateUpdateEntityResultWithChanges(mockTenure);

            // setup updater
            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<TenureInformationDb>(), It.IsAny<string>(), It.IsAny<EditTenureDetailsRequestObject>()))
                .Returns(updaterResponse);

            // call gateway method
            var response = await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRawBody).ConfigureAwait(false);

            // assert response is UpdateEntityResult
            response.Should().BeOfType(typeof(UpdateEntityResult<TenureInformationDb>));

            // load entity from database
            var databaseResponse = await _dynamoDb.LoadAsync<TenureInformationDb>(mockTenure.Id).ConfigureAwait(false);

            // assert entity was updated (matches mockRequestObject data)
            databaseResponse.StartOfTenureDate.Should().BeCloseTo((DateTime) updaterResponse.UpdatedEntity.StartOfTenureDate, 2.Seconds());
            databaseResponse.EndOfTenureDate.Should().BeCloseTo((DateTime) updaterResponse.UpdatedEntity.EndOfTenureDate, 2.Seconds());
            databaseResponse.TenureType.Code.Should().Be(updaterResponse.UpdatedEntity.TenureType.Code);
        }

        [Fact]
        public async Task EditTenureDetailsWhenStartDateIsInRequestButNoEndDateIsInDatabaseNoExceptionIsThrown()
        {
            // create mock tenure - end date is null
            var mockTenure = _fixture.Build<TenureInformation>()
                                     .With(x => x.EndOfTenureDate, (DateTime?) null)
                                     .With(x => x.SuccessionDate, DateTime.UtcNow)
                                     .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                     .With(x => x.SubletEndDate, DateTime.UtcNow)
                                     .With(x => x.EvictionDate, DateTime.UtcNow)
                                     .With(x => x.VersionNumber, (int?) null)
                                     .Create();

            // insert into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);

            // setup updater to return UpdateEntityResponse with only start date
            SetupUpdaterToOnlyReturnStartOfTenureDate(DateTime.UtcNow);

            // call gateway method
            var mockQuery = new TenureQueryRequest { Id = mockTenure.Id };
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRequestBody = "";

            Func<Task> act = async () =>
            {
                await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRequestBody).ConfigureAwait(false);
            };

            // assert no exception is called
            act.Should().NotThrow<EditTenureInformationValidationException>();
        }

        [Fact]
        public async Task EditTenureDetailsWhenStartDateIsInRequestButEndDateInDatabaseIsGreaterNoExceptionIsThrown()
        {
            var tenureStartDate = _fixture.Create<DateTime>();
            var tenureEndDate = tenureStartDate.AddDays(7);

            // create mock tenure - end date that is greater
            var mockTenure = _fixture.Build<TenureInformation>()
                                     .With(x => x.EndOfTenureDate, (DateTime?) tenureEndDate)
                                     .With(x => x.SuccessionDate, DateTime.UtcNow)
                                     .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                     .With(x => x.SubletEndDate, DateTime.UtcNow)
                                     .With(x => x.EvictionDate, DateTime.UtcNow)
                                     .With(x => x.VersionNumber, (int?) null)
                                     .Create();

            // insert into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);

            // setup updater to return UpdateEntityResponse with only start date
            SetupUpdaterToOnlyReturnStartOfTenureDate(tenureStartDate);


            // call gateway method
            var mockQuery = new TenureQueryRequest { Id = mockTenure.Id };
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRequestBody = "";

            Func<Task> act = async () =>
            {
                await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRequestBody).ConfigureAwait(false);
            };

            // assert no exception is called
            act.Should().NotThrow<EditTenureInformationValidationException>();
        }

        [Fact]
        public async Task EditTenureDetailsWhenStartDateIsInRequestButEndDateInDatabaseIsLessExceptionIsThrown()
        {
            // start date passed, end date is less - throw error
            var tenureStartDate = _fixture.Create<DateTime>();
            var tenureEndDate = tenureStartDate.AddDays(-7);

            // create mock tenure - end date that is less
            var mockTenure = _fixture.Build<TenureInformation>()
                                     .With(x => x.EndOfTenureDate, (DateTime?) tenureEndDate)
                                     .With(x => x.SuccessionDate, DateTime.UtcNow)
                                     .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                     .With(x => x.SubletEndDate, DateTime.UtcNow)
                                     .With(x => x.EvictionDate, DateTime.UtcNow)
                                     .With(x => x.VersionNumber, (int?) null)
                                     .Create();

            // insert into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);

            // setup updater to return UpdateEntityResponse with only start date
            SetupUpdaterToOnlyReturnStartOfTenureDate(tenureStartDate);

            // call gateway method
            var mockQuery = new TenureQueryRequest { Id = mockTenure.Id };
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRequestBody = "";

            Func<Task> act = async () =>
            {
                await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRequestBody).ConfigureAwait(false);
            };

            // assert exception is thrown
            act.Should().Throw<EditTenureInformationValidationException>();
        }

        [Fact]
        public async Task EditTenureDetailsWhenEndDateIsInRequestButStartDateInDatabaseIsLessExceptionIsNotThrown()
        {
            var tenureEndDate = _fixture.Create<DateTime>();
            var tenureStartDate = tenureEndDate.AddDays(-7);

            // create mock tenure - start date that is less
            var mockTenure = _fixture.Build<TenureInformation>()
                                     .With(x => x.StartOfTenureDate, (DateTime?) tenureStartDate)
                                     .With(x => x.SuccessionDate, DateTime.UtcNow)
                                     .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                     .With(x => x.SubletEndDate, DateTime.UtcNow)
                                     .With(x => x.EvictionDate, DateTime.UtcNow)
                                     .With(x => x.VersionNumber, (int?) null)
                                     .Create();

            // insert into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);

            // setup updater to return UpdateEntityResponse with only start date
            SetupUpdaterToOnlyReturnEndOfTenureDate(tenureEndDate);

            // call gateway method
            var mockQuery = new TenureQueryRequest { Id = mockTenure.Id };
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRequestBody = "";

            Func<Task> act = async () =>
            {
                await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRequestBody).ConfigureAwait(false);
            };

            // assert exception is thrown
            act.Should().NotThrow<EditTenureInformationValidationException>();
        }

        [Fact]
        public async Task EditTenureDetailsWhenEndDateIsInRequestButStartDateInDatabaseIsGreaterExceptionIsThrown()
        {
            var tenureEndDate = _fixture.Create<DateTime>();
            var tenureStartDate = tenureEndDate.AddDays(7);

            // create mock tenure - start date that is less
            var mockTenure = _fixture.Build<TenureInformation>()
                                     .With(x => x.StartOfTenureDate, (DateTime?) tenureStartDate)
                                     .With(x => x.SuccessionDate, DateTime.UtcNow)
                                     .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                     .With(x => x.SubletEndDate, DateTime.UtcNow)
                                     .With(x => x.EvictionDate, DateTime.UtcNow)
                                     .With(x => x.VersionNumber, (int?) null)
                                     .Create();

            // insert into database
            await InsertDatatoDynamoDB(mockTenure).ConfigureAwait(false);

            // setup updater to return UpdateEntityResponse with only start date
            SetupUpdaterToOnlyReturnEndOfTenureDate(tenureEndDate);

            // call gateway method
            var mockQuery = new TenureQueryRequest { Id = mockTenure.Id };
            var mockRequestObject = _fixture.Create<EditTenureDetailsRequestObject>();
            var mockRequestBody = "";

            Func<Task> act = async () =>
            {
                await _classUnderTest.EditTenureDetails(mockQuery, mockRequestObject, mockRequestBody).ConfigureAwait(false);
            };

            // assert exception is thrown
            act.Should().Throw<EditTenureInformationValidationException>();
        }

        private UpdateEntityResult<TenureInformationDb> CreateUpdateEntityResultWithChanges(TenureInformation entityInsertedIntoDatabase)
        {
            var updatedEntity = entityInsertedIntoDatabase.ToDatabase();

            updatedEntity.StartOfTenureDate = DateTime.UtcNow + _fixture.Create<TimeSpan>();
            updatedEntity.EndOfTenureDate = DateTime.UtcNow + _fixture.Create<TimeSpan>();
            updatedEntity.TenureType = _fixture.Create<TenureType>();

            return new UpdateEntityResult<TenureInformationDb>
            {
                UpdatedEntity = updatedEntity,
                NewValues = new Dictionary<string, object>
                {
                     { "StartOfTenureDate", updatedEntity.StartOfTenureDate },
                     { "EndOfTenureDate", updatedEntity.EndOfTenureDate },
                     { "TenureType", updatedEntity.TenureType }
                }
            };
        }

        private void SetupUpdaterToOnlyReturnStartOfTenureDate(DateTime tenureStartDate)
        {
            var updaterResponse = new UpdateEntityResult<TenureInformationDb>
            {
                NewValues = new Dictionary<string, object>
                {
                    { "startOfTenureDate", tenureStartDate }
                }
            };

            SetupUpdater(updaterResponse);

        }

        private void SetupUpdaterToOnlyReturnEndOfTenureDate(DateTime tenureEndDate)
        {
            var updaterResponse = new UpdateEntityResult<TenureInformationDb>
            {
                NewValues = new Dictionary<string, object>
                {
                    { "endOfTenureDate", tenureEndDate }
                }
            };

            SetupUpdater(updaterResponse);
        }

        private void SetupUpdater(UpdateEntityResult<TenureInformationDb> updaterResponse)
        {
            _mockUpdater
                .Setup(x => x.UpdateEntity(It.IsAny<TenureInformationDb>(), It.IsAny<string>(), It.IsAny<EditTenureDetailsRequestObject>()))
                .Returns(updaterResponse);
        }
    }
}
