using System;

namespace TenureInformationApi.V1.Domain
{
    public class Notices
    {
        public string Type { get; set; }

        public DateTime ServedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
