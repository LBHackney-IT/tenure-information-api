using System;

namespace TenureInformationApi.V1.Domain
{
    public class HouseholdMembers
    {
        public Guid Id { get; set; }

        public HouseholdMembersType Type { get; set; }

        public string FullName { get; set; }

        public bool IsResponsible { get; set; }
    }
}
