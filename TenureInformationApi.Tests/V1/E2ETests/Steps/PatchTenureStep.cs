using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;

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
        public async Task<HttpResponseMessage> CallAPI(Guid id, Guid personId, UpdateTenureForPersonRequestObject requestObject, int? ifMatch)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImV2YW5nZWxvcy5ha3RvdWRpYW5ha2lzQGhhY2tuZXkuZ292LnVrIiwiaXNzIjoiSGFja25leSIsIm5hbWUiOiJFdmFuZ2Vsb3MgQWt0b3VkaWFuYWtpcyIsImdyb3VwcyI6WyJzYW1sLWF3cy1jb25zb2xlLW10ZmgtZGV2ZWxvcGVyIl0sImlhdCI6MTYyMzA1ODIzMn0.Jnd2kQTMiAUeKMJCYQVEVXbFc9BbIH90OociR15gfpw";
            var uri = new Uri($"api/v1/tenures/{id}/person/{personId}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);
            message.Headers.TryAddWithoutValidation(HeaderConstants.IfMatch, ifMatch?.ToString());


            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        public async Task WhenTheUpdateTenureApiIsCalled(Guid id, Guid personId, UpdateTenureForPersonRequestObject requestObject)
        {
            await WhenTheUpdateTenureApiIsCalled(id, personId, requestObject, 0).ConfigureAwait(false);
        }

        public async Task WhenTheUpdateTenureApiIsCalled(Guid id, Guid personId, UpdateTenureForPersonRequestObject requestObject, int? ifMatch)
        {
            _lastResponse = await CallAPI(id, personId, requestObject, ifMatch).ConfigureAwait(false);

        }

        public async Task ThenANewHouseholdMemberIsAdded(TenureFixture tenureFixture, Guid personId, UpdateTenureForPersonRequestObject request)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            //result.Should().BeEquivalentTo(tenureFixture.Tenure, config => config.Excluding(y => y.HouseholdMembers));
            result.VersionNumber.Should().Be(1);

            var expected = new HouseholdMembers()
            {
                DateOfBirth = request.DateOfBirth.Value,
                FullName = request.FullName,
                Id = personId,
                IsResponsible = request.IsResponsible.Value,
                PersonTenureType = TenureTypes.GetPersonTenureType(result.TenureType, request.IsResponsible.Value),
                Type = request.Type.Value
            };
            result.HouseholdMembers.Should().ContainEquivalentOf(expected);
            result.HouseholdMembers.Except(result.HouseholdMembers.Where(x => x.Id == personId))
                                   .Should()
                                   .BeEquivalentTo(tenureFixture.Tenure.HouseholdMembers);

            await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(result.Id).ConfigureAwait(false);
        }

        public async Task ThenTheHouseholdMemberTenureDetailsAreUpdated(TenureFixture tenureFixture, Guid personId, UpdateTenureForPersonRequestObject request)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            //result.Should().BeEquivalentTo(tenureFixture.Tenure, config => config.Excluding(y => y.HouseholdMembers));
            result.VersionNumber.Should().Be(1);
            result.HouseholdMembers.First(x => x.Id == personId).FullName.Should().Be(request.FullName);

            await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(result.Id).ConfigureAwait(false);
        }

        public async Task ThenConflictIsReturned(int? versionNumber)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var sentVersionNumberString = (versionNumber is null) ? "{null}" : versionNumber.ToString();
            responseContent.Should().Contain($"The version number supplied ({sentVersionNumberString}) does not match the current value on the entity (0).");
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
