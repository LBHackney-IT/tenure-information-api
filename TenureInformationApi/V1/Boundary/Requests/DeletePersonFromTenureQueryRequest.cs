using Microsoft.AspNetCore.Mvc;
using System;

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
