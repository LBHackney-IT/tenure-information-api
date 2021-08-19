using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public static class CreateRequestFactory
    {
        public static TenureInformationDb ToDatabase(this CreateTenureRequestObject createTenureRequestObject)
        {
            return new TenureInformationDb()
            {
                Id = createTenureRequestObject.Id == Guid.Empty ? Guid.NewGuid() : createTenureRequestObject.Id,
                AgreementType = createTenureRequestObject.AgreementType,
                Charges = createTenureRequestObject.Charges,
                EndOfTenureDate = createTenureRequestObject.EndOfTenureDate,
                EvictionDate = createTenureRequestObject.EvictionDate,
                HouseholdMembers = createTenureRequestObject.HouseholdMembers,
                InformHousingBenefitsForChanges = createTenureRequestObject.InformHousingBenefitsForChanges,
                IsMutualExchange = createTenureRequestObject.IsMutualExchange,
                IsSublet = createTenureRequestObject.IsSublet,
                IsTenanted = createTenureRequestObject.IsTenanted,
                LegacyReferences = GetListOrEmpty(createTenureRequestObject.LegacyReferences),
                Notices = GetListOrEmpty(createTenureRequestObject.Notices),
                PaymentReference = createTenureRequestObject.PaymentReference,
                PotentialEndDate = createTenureRequestObject.PotentialEndDate,
                StartOfTenureDate = createTenureRequestObject.StartOfTenureDate,
                SubletEndDate = createTenureRequestObject.SubletEndDate,
                SuccessionDate = createTenureRequestObject.SuccessionDate,
                TenuredAsset = createTenureRequestObject.TenuredAsset,
                TenureType = createTenureRequestObject.TenureType,
                Terminated = createTenureRequestObject.Terminated

            };

        }
        private static List<T> GetListOrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable == null ? new List<T>() : enumerable.ToList();
        }
    }
}
