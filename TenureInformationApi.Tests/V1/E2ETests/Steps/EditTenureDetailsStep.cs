using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Infrastructure;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TenureInformationApi.Tests.V1.E2ETests.Steps
{
    public class EditTenureDetailsStep : BaseSteps
    {
        public EditTenureDetailsStep(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task WhenEditTenureDetailsApiIsCalled(Guid id, object requestObject)
        {
            var token =
                 "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImV2YW5nZWxvcy5ha3RvdWRpYW5ha2lzQGhhY2tuZXkuZ292LnVrIiwiaXNzIjoiSGFja25leSIsIm5hbWUiOiJFdmFuZ2Vsb3MgQWt0b3VkaWFuYWtpcyIsImdyb3VwcyI6WyJzYW1sLWF3cy1jb25zb2xlLW10ZmgtZGV2ZWxvcGVyIl0sImlhdCI6MTYyMzA1ODIzMn0.Jnd2kQTMiAUeKMJCYQVEVXbFc9BbIH90OociR15gfpw";


            // setup request
            var uri = new Uri($"api/v1/tenures/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new[] { new StringEnumConverter() }
            };
            var requestJson = JsonConvert.SerializeObject(requestObject, jsonSettings);
            message.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");


            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void ThenNoContentResponseReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        public async Task ThenTheValidationErrorsAreReturned(string errorMessageName)
        {
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            JObject jo = JObject.Parse(responseContent);
            var errors = jo["errors"].Children();

            ShouldHaveErrorFor(errors, errorMessageName);
        }

        private static void ShouldHaveErrorFor(JEnumerable<JToken> errors, string propertyName, string errorCode = null)
        {
            var error = errors.FirstOrDefault(x => (x.Path.Split('.').Last().Trim('\'', ']')) == propertyName) as JProperty;
            error.Should().NotBeNull();
            if (!string.IsNullOrEmpty(errorCode))
                error.Value.ToString().Should().Contain(errorCode);
        }

        public async Task TheTenureHasntBeenUpdatedInTheDatabase(TenureFixture tenureFixture)
        {
            var databaseResponse = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.TenureId).ConfigureAwait(false);

            databaseResponse.Id.Should().Be(tenureFixture.ExistingTenure.Id);
            databaseResponse.StartOfTenureDate.Should().Be(tenureFixture.ExistingTenure.StartOfTenureDate);
            databaseResponse.EndOfTenureDate.Should().Be(tenureFixture.ExistingTenure.EndOfTenureDate);
            databaseResponse.TenureType.Code.Should().Be(tenureFixture.ExistingTenure.TenureType.Code);
        }

        public async Task ThenCustomEditTenureDetailsBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var responseEntity = JsonSerializer.Deserialize<CustomEditTenureDetailsBadRequestResponse>(responseContent, CreateJsonOptions());

            responseEntity.Should().BeOfType(typeof(CustomEditTenureDetailsBadRequestResponse));

            responseEntity.Errors.Should().HaveCountGreaterThan(0);
        }

        public async Task TheTenureHasBeenUpdatedInTheDatabase(TenureFixture tenureFixture, EditTenureDetailsRequestObject requestObject)
        {
            var databaseResponse = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.TenureId).ConfigureAwait(false);

            databaseResponse.Id.Should().Be(tenureFixture.ExistingTenure.Id);
            databaseResponse.StartOfTenureDate.Should().Be(requestObject.StartOfTenureDate);
            databaseResponse.EndOfTenureDate.Should().Be(requestObject.EndOfTenureDate);
            databaseResponse.TenureType.Code.Should().Be(requestObject.TenureType.Code);
        }
    }
}
