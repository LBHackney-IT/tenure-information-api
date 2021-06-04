using Microsoft.AspNetCore.Mvc;
using System;

namespace TenureInformationApi.V1.Boundary.Requests
{
    public class GetByIdRequest
    {
        [FromRoute(Name = "id")]
        public Guid? Id { get; set; }
    }
}
