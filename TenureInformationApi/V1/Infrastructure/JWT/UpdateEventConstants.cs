using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Infrastructure
{
    public static class UpdateEventConstants
    {
        // JWT TOKEN
        public const string V1_VERSION = "v1";
        public const string EVENTTYPE = "PersonAddedToTenureEvent";
        public const string SOURCE_DOMAIN = "Tenure";
        public const string SOURCE_SYSTEM = "TenureAPI";
    }
}
