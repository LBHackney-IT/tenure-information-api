using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
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
    public class PostTenureSteps : BaseSteps
    {
        public PostTenureSteps(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenCreateTenureApiIsCalled(CreateTenureRequestObject requestObject)
        {
            var uri = new Uri($"api/v1/tenures", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
            _lastResponse = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);

        }

        public async Task ThenTheTenureDetailsAreReturnedAndIdIsNotEmpty(TenureFixture tenureFixture)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiTenure = JsonSerializer.Deserialize<TenureResponseObject>(responseContent, CreateJsonOptions());

            apiTenure.Id.Should().NotBeEmpty();

            var dbRecord = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(apiTenure.Id).ConfigureAwait(false);
            var domain = dbRecord.ToDomain();
            apiTenure.Should().BeEquivalentTo(domain.ToResponse());

            await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(dbRecord.Id).ConfigureAwait(false);
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
