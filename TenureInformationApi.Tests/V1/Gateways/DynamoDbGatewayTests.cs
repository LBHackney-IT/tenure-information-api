using AutoFixture;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;
using TenureInformationApi.V1.Domain;
using System.Threading.Tasks;
using TenureInformationApi.V1.Factories;

namespace TenureInformationApi.Tests.V1.Gateways
{

    [TestFixture]
    public class DynamoDbGatewayTests : DynamoDbTests
    {
        private readonly Fixture _fixture = new Fixture();
        private DynamoDbGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new DynamoDbGateway(DynamoDbContext);
        }

        [Test]
        public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var id = Guid.NewGuid();
            var response = _classUnderTest.GetEntityById(id);

            response.Should().BeNull();
        }

        [Test]
        public void GetEntityByIdReturnsTheEntityIfItExists()
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();
            InsertDatatoDynamoDB(entity);

            var response = _classUnderTest.GetEntityById(entity.Id);

            response.Should().BeEquivalentTo(entity);

        }

        private void InsertDatatoDynamoDB(TenureInformation entity)
        {
            DynamoDbContext.SaveAsync(entity.ToDatabase()).GetAwaiter().GetResult();
        }
    }
}
