using System;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Requests
{
    public class EditTenureDetailsRequestObject
    {
        public DateTime? StartOfTenureDate { get; set; }
        public DateTime? EndOfTenureDate { get; set; }
        public TenureType TenureType { get; set; }
    }
}
