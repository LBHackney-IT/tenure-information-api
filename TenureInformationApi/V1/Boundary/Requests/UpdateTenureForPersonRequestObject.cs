using System;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Requests
{
    public class UpdateTenureForPersonRequestObject
    {
        public string FullName { get; set; }
        public bool? IsResponsible { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public HouseholdMembersType? Type { get; set; }
    }
}
