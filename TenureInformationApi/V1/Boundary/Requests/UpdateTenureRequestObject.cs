using System;
using System.Collections.Generic;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Requests
{
    public class UpdateTenureRequestObject
    {
        public Guid Id { get; set; }
        public List<HouseholdMembers> HouseholdMembers { get; set; }
    }
}
