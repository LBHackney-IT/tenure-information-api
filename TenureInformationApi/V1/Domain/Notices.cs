using System;

namespace TenureInformationApi.V1.Domain
{
    public class Notices
    {
        public string Type { get; set; }

        public DateTime ServedDate { get; set; }

        public string Expiry { get; set; }

        public DateTime EndDate { get; set; }
    }
}
