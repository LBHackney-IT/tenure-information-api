using Hackney.Shared.Tenure.Domain;
using System.Collections.Generic;
using System;

namespace TenureInformationApi.V1
{
    // NB: Temporary class to prove out the concept of granular access control.
    public class EditTenureDetailsRequestObject
    {
        public DateTime? StartOfTenureDate { get; set; }

        public DateTime? EndOfTenureDate { get; set; }

        public string PaymentReference { get; set; }

        public Charges Charges { get; set; }

        public TenureType TenureType { get; set; }

        public string TenureSource { get; set; }

        public Terminated Terminated { get; set; }

        public string FundingSource { get; set; }

        public int NumberOfAdultsInProperty { get; set; }

        public int NumberOfChildrenInProperty { get; set; }

        public bool? HasOffsiteStorage { get; set; }

        public FurtherAccountInformation FurtherAccountInformation { get; set; }

        public IEnumerable<LegacyReference> LegacyReferences { get; set; }

        public TenuredAsset TenuredAsset { get; set; }
    }

}
