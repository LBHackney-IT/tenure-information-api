using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Boundary.Requests.Validation
{
    public static class ErrorCodes
    {
        public const string XssCheckFailure = "W42";
        public const string TenureEndDate = "W26";
    }
}
