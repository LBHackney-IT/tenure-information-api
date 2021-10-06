using AutoFixture;
using FluentAssertions;
using System;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Factories;
using Xunit;

namespace TenureInformationApi.Tests.V1.Factories
{
    public class CreateRequestFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapARequestToADatabaseObject()
        {
            var request = _fixture.Create<CreateTenureRequestObject>();
            var databaseEntity = request.ToDatabase();

            databaseEntity.Id.Should().Be(request.Id);
            databaseEntity.AgreementType.Should().Be(request.AgreementType);
            databaseEntity.Charges.Should().BeEquivalentTo(request.Charges);
            databaseEntity.EndOfTenureDate.Should().Be(request.EndOfTenureDate);
            databaseEntity.EvictionDate.Should().Be(request.EvictionDate);
            databaseEntity.HouseholdMembers.Should().BeEquivalentTo(request.HouseholdMembers);
            databaseEntity.InformHousingBenefitsForChanges.Should().Be(request.InformHousingBenefitsForChanges);
            databaseEntity.IsMutualExchange.Should().Be(request.IsMutualExchange);
            databaseEntity.IsSublet.Should().Be(request.IsSublet);
            databaseEntity.IsTenanted.Should().Be(request.IsTenanted);
            databaseEntity.LegacyReferences.Should().BeEquivalentTo(request.LegacyReferences);
            databaseEntity.Notices.Should().BeEquivalentTo(request.Notices);
            databaseEntity.PaymentReference.Should().Be(request.PaymentReference);
            databaseEntity.PotentialEndDate.Should().Be(request.PotentialEndDate);
            databaseEntity.StartOfTenureDate.Should().Be(request.StartOfTenureDate);
            databaseEntity.SubletEndDate.Should().Be(request.SubletEndDate);
            databaseEntity.SuccessionDate.Should().Be(request.SuccessionDate);
            databaseEntity.TenuredAsset.Should().BeEquivalentTo(request.TenuredAsset);
            databaseEntity.TenureType.Should().Be(request.TenureType);
            databaseEntity.Terminated.Should().Be(request.Terminated);
        }

        [Fact]
        public void CanMapARequestToADatabaseObjectWithEmptyGuid()
        {
            var request = _fixture.Create<CreateTenureRequestObject>();
            request.Id = Guid.Empty;

            var databaseEntity = request.ToDatabase();
            databaseEntity.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void CanMapARequestToADatabaseObjectWithNullCollections()
        {
            var request = _fixture.Create<CreateTenureRequestObject>();
            request.HouseholdMembers = null;
            request.Notices = null;
            request.LegacyReferences = null;

            var databaseEntity = request.ToDatabase();
            databaseEntity.HouseholdMembers.Should().BeEmpty();
            databaseEntity.LegacyReferences.Should().BeEmpty();
            databaseEntity.Notices.Should().BeEmpty();
        }
    }
}
