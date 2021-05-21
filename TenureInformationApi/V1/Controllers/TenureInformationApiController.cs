using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TenureInformationApi.V1.Controllers
{
    [ApiController]
    //: Rename to match the APIs endpoint
    [Route("api/v1/residents")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class TenureInformationApiController : BaseController
    {
        private readonly IGetByIdUseCase _getByIdUseCase;
        public TenureInformationApiController(IGetByIdUseCase getByIdUseCase)
        {
            _getByIdUseCase = getByIdUseCase;
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="404">No ? found for the specified ID</response>
        [ProducesResponseType(typeof(TenureResponseObject), StatusCodes.Status200OK)]
        [HttpGet]
        //TODO: rename to match the identifier that will be used
        [Route("{yourId}")]
        public IActionResult ViewRecord(Guid yourId)
        {
            return Ok(_getByIdUseCase.Execute(yourId));
        }
    }
}
