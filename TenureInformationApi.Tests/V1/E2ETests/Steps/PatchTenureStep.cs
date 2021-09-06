using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public async Task WhenUpdateTenureApiIsCalled(Guid id, Guid personId, UpdateTenureForPersonRequestObject requestObject)
        {

            var uri = new Uri($"api/v1/tenures/{id}/person/{personId}", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");

            _lastResponse = await _httpClient.PatchAsync(uri, content).ConfigureAwait(false);
        }

        public async Task ThenANewHouseholdMemberIsAdded(TenureFixture tenureFixture, Guid personId, UpdateTenureForPersonRequestObject request)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            result.Should().BeEquivalentTo(tenureFixture.Tenure, config => config.Excluding(y => y.HouseholdMembers));

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
            result.Should().BeEquivalentTo(tenureFixture.Tenure, config => config.Excluding(y => y.HouseholdMembers));
            result.HouseholdMembers.First(x => x.Id == personId).FullName.Should().Be(request.FullName);

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
