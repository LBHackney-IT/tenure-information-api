using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using TenureInformationApi.Tests.V1.Helper;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace TenureInformationApi.Tests.V1.Gateways
{
    //TODO: Remove this file if DynamoDb gateway not being used
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-base-api/wiki/Running-the-test-suite.
    [TestFixture]
    public class DynamoDbGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IDynamoDBContext> _dynamoDb;
        private DynamoDbGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _classUnderTest = new DynamoDbGateway(_dynamoDb.Object);
        }

        [Test]
        public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var response = _classUnderTest.GetEntityById(Guid.NewGuid());

            response.Should().BeNull();
        }

        [Test]
        public void GetEntityByIdReturnsTheEntityIfItExists()
        {
            var entity = _fixture.Create<TenureInformation>();
            var dbEntity = DatabaseEntityHelper.CreateDatabaseEntityFrom(entity);

            _dynamoDb.Setup(x => x.LoadAsync<TenureInformationDb>(entity.Id, default))
                     .ReturnsAsync(dbEntity);

            var response = _classUnderTest.GetEntityById(entity.Id);

            _dynamoDb.Verify(x => x.LoadAsync<TenureInformationDb>(entity.Id, default), Times.Once);

            entity.Id.Should().Be(response.Id);
        }
    }
}
