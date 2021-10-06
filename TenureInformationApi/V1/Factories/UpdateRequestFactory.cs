using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.V1.Factories
{
    public static class UpdateRequestFactory
    {
        public static TenureInformationDb ToDatabase(this UpdateTenureRequestObject updateTenureRequestObject)
        {
            return new TenureInformationDb()
            {
                Id = updateTenureRequestObject.Id,
                HouseholdMembers = updateTenureRequestObject.HouseholdMembers.ToListOrNull(),
                LegacyReferences = null,
                Notices = null
            };
        }
    }
}
