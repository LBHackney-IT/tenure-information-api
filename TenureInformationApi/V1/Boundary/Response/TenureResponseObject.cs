using System;
using System.Collections.Generic;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Response
{
    public class TenureResponseObject
    {
        public Guid Id { get; set; }
        public string PaymentReference { get; set; }
        public List<HouseholdMembers> HouseholdMembers { get; set; }
        public TenuredAsset TenuredAsset { get; set; }
        public Charges Charges { get; set; }
        public DateTime? StartOfTenureDate { get; set; }
        public DateTime? EndOfTenureDate { get; set; }
        public TenureType TenureType { get; set; }
        public bool IsActive { get; set; }
        public bool? IsTenanted { get; set; }
        public Terminated Terminated { get; set; }
        public DateTime? SuccessionDate { get; set; }
        public DateTime? EvictionDate { get; set; }
        public DateTime? PotentialEndDate { get; set; }
        public bool? IsMutualExchange { get; set; }
        public bool? InformHousingBenefitsForChanges { get; set; }
        public bool? IsSublet { get; set; }
        public DateTime? SubletEndDate { get; set; }
        public List<Notices> Notices { get; set; }
        public List<LegacyReference> LegacyReferences { get; set; }
        public AgreementType AgreementType { get; set; }
    }
}
