using System.Collections.Generic;
using System.Linq;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static TenureResponseObject ToResponse(this TenureInformation domain)
        {
            if (domain == null) return null;
            return new TenureResponseObject
            {
                Id = domain.Id,
                AccountType = domain.AccountType,
                Charges = domain.Charges,
                EndOfTenureDate = domain.EndOfTenureDate,
                EvictionDate = domain.EvictionDate,
                HouseholdMembers = domain.HouseholdMembers,
                InformHousingBenefitsForChanges = domain.InformHousingBenefitsForChanges,
                IsActive = domain.IsActive,
                IsMutualExchange = domain.IsMutualExchange,
                IsSublet = domain.IsSublet,
                IsTenanted = domain.IsTenanted,
                LegacyReferences = domain.LegacyReferences.ToList(),
                MasterAccountTenureReference = domain.MasterAccountTenureReference,
                Notices = domain.Notices.ToList(),
                PaymentReference = domain.PaymentReference,
                PotentialEndDate = domain.PotentialEndDate,
                RentCostCentre = domain.RentCostCentre,
                StartOfTenureDate = domain.StartOfTenureDate,
                SubletEndDate = domain.SubletEndDate,
                SubsidiaryAccountsReferences = domain.SubsidiaryAccountsReferences.ToList(),
                SuccessionDate = domain.SuccessionDate,
                TenuredAsset = domain.TenuredAsset,
                TenureType = domain.TenureType,
                Terminated = domain.Terminated,
                AgreementType = domain.AgreementType
            };
        }

        public static List<TenureResponseObject> ToResponse(this IEnumerable<TenureInformation> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
