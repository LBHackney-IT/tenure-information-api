using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenureInformationApi.V1.Boundary.Requests
{
    public class DeletePersonFromTenureQueryRequest
    {
        [FromRoute(Name = "tenureId")]
        public Guid TenureId { get; set; }

        [FromRoute(Name = "personId")]
        public Guid PersonId { get; set; }
    }
}
