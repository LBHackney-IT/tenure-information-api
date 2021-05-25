using AutoFixture;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace TenureInformationApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<TenureInformationDb>();
            var entity = databaseEntity.ToDomain();

            databaseEntity.Should().BeEquivalentTo(entity);
        }


        [Test]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<TenureInformation>();
            var databaseEntity = entity.ToDatabase();

            entity.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
