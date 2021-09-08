using Microsoft.AspNetCore.Mvc;
using System;

namespace TenureInformationApi.V1.Boundary.Requests
{
    public class UpdateTenureRequest
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }

        [FromRoute(Name = "personId")]
        public Guid PersonId { get; set; }
    }
}
