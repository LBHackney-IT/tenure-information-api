using AutoFixture;
using System.Linq;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;

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
                AccountType = entity.AccountType,
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
                MasterAccountTenureReference = entity.MasterAccountTenureReference,
                Notices = entity.Notices.ToList(),
                PaymentReference = entity.PaymentReference,
                PotentialEndDate = entity.PotentialEndDate,
                RentCostCentre = entity.RentCostCentre,
                StartOfTenureDate = entity.StartOfTenureDate,
                SubletEndDate = entity.SubletEndDate,
                SubsidiaryAccountsReferences = entity.SubsidiaryAccountsReferences.ToList(),
                SuccessionDate = entity.SuccessionDate,
                TenuredAsset = entity.TenuredAsset,
                TenureType = entity.TenureType,
                Terminated = entity.Terminated
            };
        }
    }
}
