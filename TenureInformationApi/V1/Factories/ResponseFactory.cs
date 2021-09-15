using System.Collections.Generic;
using System.Linq;
using TenureInformationApi.V1.Boundary.Response;
using Hackney.Shared.Tenure;
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
                Charges = domain.Charges,
                EndOfTenureDate = domain.EndOfTenureDate,
                EvictionDate = domain.EvictionDate,
                HouseholdMembers = domain.HouseholdMembers.ToListOrEmpty(),
                InformHousingBenefitsForChanges = domain.InformHousingBenefitsForChanges,
                IsActive = domain.IsActive,
                IsMutualExchange = domain.IsMutualExchange,
                IsSublet = domain.IsSublet,
                IsTenanted = domain.IsTenanted,
                LegacyReferences = domain.LegacyReferences.ToListOrEmpty(),
                Notices = domain.Notices.ToListOrEmpty(),
                PaymentReference = domain.PaymentReference,
                PotentialEndDate = domain.PotentialEndDate,
                StartOfTenureDate = domain.StartOfTenureDate,
                SubletEndDate = domain.SubletEndDate,
                SuccessionDate = domain.SuccessionDate,
                TenuredAsset = domain.TenuredAsset,
                TenureType = domain.TenureType,
                Terminated = domain.Terminated,
                AgreementType = domain.AgreementType,
                VersionNumber = domain.VersionNumber
            };
        }

        public static List<TenureResponseObject> ToResponse(this IEnumerable<TenureInformation> domainList)
        {
            if (null == domainList) return new List<TenureResponseObject>();
            return domainList.Select(domain => ToResponse(domain)).ToList();
        }


    }
}
