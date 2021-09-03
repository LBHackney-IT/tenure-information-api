using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
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
                HouseholdMembers = GetListOrNull(updateTenureRequestObject.HouseholdMembers),
                LegacyReferences = null,
                Notices = null
            };
        }

        private static List<T> GetListOrNull<T>(IEnumerable<T> enumerable)
        {
            return enumerable == null ? null : enumerable.ToList();
        }
    }
}
