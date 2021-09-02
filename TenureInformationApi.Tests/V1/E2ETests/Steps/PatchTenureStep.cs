using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TenureInformationApi.Tests.V1.E2ETests.Steps
{
    public class PatchTenureStep : BaseSteps
    {
        public PatchTenureStep(HttpClient httpClient) : base(httpClient)
        { }
        /// <summary>
        /// You can use jwt.io to decode the token - it is the same one we'd use on dev, etc. 
        /// </summary>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public async Task WhenUpdateTenureApiIsCalled(Guid id, Guid personId, UpdateTenureRequestObject requestObject)
        {

            var uri = new Uri($"api/v1/tenures/{id}/person/{personId}", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");

            _lastResponse = await _httpClient.PatchAsync(uri, content).ConfigureAwait(false);
        }

        public async Task ThenTheTenureDetailsAreUpdated(TenureFixture tenureFixture)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);
            result.HouseholdMembers.Should().BeEquivalentTo(tenureFixture.UpdateTenureRequestObject.HouseholdMembers);

            await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(result.Id).ConfigureAwait(false);
        }
        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
