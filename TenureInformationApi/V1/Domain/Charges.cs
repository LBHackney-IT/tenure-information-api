using System;

namespace TenureInformationApi.V1.Domain
{
    public class Charges
    {
        public string Rent { get; set; }

        public string CurrentBalance { get; set; }

        public string BillingFrequency { get; set; }

        public int PaymentReference { get; set; }

        public string RentGroupCode { get; set; }

        public string RentGroupDescription { get; set; }

        public string ServiceCharge { get; set; }

        public string OtherCharges { get; set; }

        public string CombinedServiceCharges { get; set; }

        public string CombinedRentCharges { get; set; }

        public string TenancyInsuranceCharge { get; set; }

        public string OriginalRentCharge { get; set; }

        public string OriginalServiceCharge { get; set; }
    }
}
