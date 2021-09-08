using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenures")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TenureInformationController : BaseController
    {
        private readonly IGetByIdUseCase _getByIdUseCase;
        private readonly IPostNewTenureUseCase _postNewTenureUseCase;
        private readonly IUpdateTenureForPersonUseCase _updateTenureForPersonUseCase;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpContextWrapper _contextWrapper;
        public TenureInformationController(IGetByIdUseCase getByIdUseCase, IPostNewTenureUseCase postNewTenureUseCase, IUpdateTenureForPersonUseCase updateTenureForPersonUseCase,
            ITokenFactory tokenFactory, IHttpContextWrapper contextWrapper)
        {
            _getByIdUseCase = getByIdUseCase;
            _postNewTenureUseCase = postNewTenureUseCase;
            _updateTenureForPersonUseCase = updateTenureForPersonUseCase;
            _tokenFactory = tokenFactory;
            _contextWrapper = contextWrapper;
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">Successfully retrieved details for the specified ID</response>
        /// <response code="404">No tenure information found for the specified ID</response>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(TenureResponseObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetByID([FromRoute] TenureQueryRequest query)
        {
            var result = await _getByIdUseCase.Execute(query).ConfigureAwait(false);
            if (result == null) return NotFound(query.Id);
            return Ok(result);
        }

        [ProducesResponseType(typeof(TenureResponseObject), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> PostNewTenure([FromBody] CreateTenureRequestObject createTenureRequestObject)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));

            var tenure = await _postNewTenureUseCase.ExecuteAsync(createTenureRequestObject, token).ConfigureAwait(false);
            return Created(new Uri($"api/v1/tenures/{tenure.Id}", UriKind.Relative), tenure);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch]
        [Route("{id}/person/{personid}")]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> UpdateTenureForPerson([FromRoute] UpdateTenureRequest query, [FromBody] UpdateTenureForPersonRequestObject updateTenureRequestObject)
        {
            var token = _tokenFactory.Create(_contextWrapper.GetContextRequestHeaders(HttpContext));

            var tenure = await _updateTenureForPersonUseCase.ExecuteAsync(query, updateTenureRequestObject, token).ConfigureAwait(false);
            if (tenure == null) return NotFound(query.Id);

            return NoContent();
        }
    }
}
