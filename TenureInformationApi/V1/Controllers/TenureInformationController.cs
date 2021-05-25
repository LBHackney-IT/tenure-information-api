using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenure")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TenureInformationController : BaseController
    {
        private readonly IGetByIdUseCase _getByIdUseCase;
        public TenureInformationController(IGetByIdUseCase getByIdUseCase)
        {
            _getByIdUseCase = getByIdUseCase;
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">Successfully retrieved details for the specified ID</response>
        /// <response code="404">No tenure information found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(TenureResponseObject), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetByID(Guid id)
        {
            try
            {
                return Ok(_getByIdUseCase.Execute(id));
            }
            catch (NotFoundException e)
            {
                return new NotFoundObjectResult(e.Message);
            }
        }
    }
}
