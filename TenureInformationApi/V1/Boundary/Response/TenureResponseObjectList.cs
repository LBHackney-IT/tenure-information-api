using System.Collections.Generic;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Boundary.Response
{
    public class TenureResponseObjectList
    {
        public List<int> SubsidiaryAccountsReferences { get; set; }
        public List<Notices> Notices { get; set; }
        public List<LegacyReference> LegacyReferences { get; set; }
    }
}
