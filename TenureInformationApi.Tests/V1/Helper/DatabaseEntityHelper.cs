using AutoFixture;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Infrastructure;
using System.Linq;

namespace TenureInformationApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static TenureInformationDb CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<TenureInformation>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static TenureInformationDb CreateDatabaseEntityFrom(TenureInformation entity)
        {
            return new TenureInformationDb
            {
                Id = entity.Id,
                AgreementType = entity.AgreementType,
                Charges = entity.Charges,
                EndOfTenureDate = entity.EndOfTenureDate,
                EvictionDate = entity.EvictionDate,
                HouseholdMembers = entity.HouseholdMembers.ToList(),
                InformHousingBenefitsForChanges = entity.InformHousingBenefitsForChanges,
                IsMutualExchange = entity.IsMutualExchange,
                IsSublet = entity.IsSublet,
                IsTenanted = entity.IsTenanted,
                LegacyReferences = entity.LegacyReferences.ToList(),
                Notices = entity.Notices.ToList(),
                PaymentReference = entity.PaymentReference,
                PotentialEndDate = entity.PotentialEndDate,
                StartOfTenureDate = entity.StartOfTenureDate,
                SubletEndDate = entity.SubletEndDate,
                SuccessionDate = entity.SuccessionDate,
                TenuredAsset = entity.TenuredAsset,
                TenureType = entity.TenureType,
                Terminated = entity.Terminated
            };
        }
    }
}
