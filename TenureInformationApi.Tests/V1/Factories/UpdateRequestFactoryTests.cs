using AutoFixture;
using FluentAssertions;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Factories;
using Xunit;

namespace TenureInformationApi.Tests.V1.Factories
{
    public class UpdateRequestFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanMapARequestToADatabaseObject(bool nullHMs)
        {
            var request = _fixture.Create<UpdateTenureRequestObject>();
            if (nullHMs) request.HouseholdMembers = null;

            var databaseEntity = request.ToDatabase();
            databaseEntity.LegacyReferences.Should().BeNull();
            databaseEntity.Notices.Should().BeNull();
            databaseEntity.HouseholdMembers.Should().BeEquivalentTo(nullHMs? null : request.HouseholdMembers);
        }
    }
}
