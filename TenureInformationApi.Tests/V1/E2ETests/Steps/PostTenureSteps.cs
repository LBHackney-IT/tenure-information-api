using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Requests.Validation;
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
        private static void ShouldHaveErrorFor(JEnumerable<JToken> errors, string propertyName, string errorCode = null)
        {
            var error = errors.FirstOrDefault(x => (x.Path.Split('.').Last().Trim('\'', ']')) == propertyName) as JProperty;
            error.Should().NotBeNull();
            if (!string.IsNullOrEmpty(errorCode))
                error.Value.ToString().Should().Contain(errorCode);
        }
        /// <summary>
        /// You can use jwt.io to decode the token - it is the same one we'd use on dev, etc. 
        /// </summary>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public async Task WhenCreateTenureApiIsCalled(CreateTenureRequestObject requestObject)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImV2YW5nZWxvcy5ha3RvdWRpYW5ha2lzQGhhY2tuZXkuZ292LnVrIiwiaXNzIjoiSGFja25leSIsIm5hbWUiOiJFdmFuZ2Vsb3MgQWt0b3VkaWFuYWtpcyIsImdyb3VwcyI6WyJzYW1sLWF3cy1jb25zb2xlLW10ZmgtZGV2ZWxvcGVyIl0sImlhdCI6MTYyMzA1ODIzMn0.Jnd2kQTMiAUeKMJCYQVEVXbFc9BbIH90OociR15gfpw";
            var uri = new Uri($"api/v1/tenures", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
            message.Headers.Add("Authorization", token);

            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = await _httpClient.SendAsync(message).ConfigureAwait(false);

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

        public async Task ThenTheValidationErrorsAreReturned()
        {
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            JObject jo = JObject.Parse(responseContent);
            var errors = jo["errors"].Children();

            ShouldHaveErrorFor(errors, "EndOfTenureDate");

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
