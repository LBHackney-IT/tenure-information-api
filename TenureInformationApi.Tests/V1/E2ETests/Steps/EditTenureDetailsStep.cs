using FluentAssertions;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.V1.Infrastructure;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TenureInformationApi.Tests.V1.E2ETests.Steps
{
    public class EditTenureDetailsStep : BaseSteps
    {
        private const string DateFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffZ";

        public EditTenureDetailsStep(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task WhenEditTenureDetailsApiIsCalled(Guid id, object requestObject)
        {
            int? defaultIfMatch = 0;
            await WhenEditTenureDetailsApiIsCalled(id, requestObject, defaultIfMatch).ConfigureAwait(false);
        }

        public async Task WhenEditTenureDetailsApiIsCalled(Guid id, object requestObject, int? ifMatch)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";

            // setup request
            var uri = new Uri($"api/v1/tenures/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);
            message.Headers.TryAddWithoutValidation(HeaderConstants.IfMatch, $"\"{ifMatch?.ToString()}\"");

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

        public async Task ThenConflictIsReturned(int? versionNumber)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            var sentVersionNumberString = (versionNumber is null) ? "{null}" : versionNumber.ToString();
            responseContent.Should().Contain($"The version number supplied ({sentVersionNumberString}) does not match the current value on the entity (0).");
        }

        public async Task TheTenureHasBeenUpdatedInTheDatabase(TenureFixture tenureFixture, EditTenureDetailsRequestObject requestObject)
        {
            var databaseResponse = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.TenureId).ConfigureAwait(false);

            databaseResponse.Id.Should().Be(tenureFixture.ExistingTenure.Id);
            databaseResponse.StartOfTenureDate.Should().Be(requestObject.StartOfTenureDate);
            databaseResponse.EndOfTenureDate.Should().Be(requestObject.EndOfTenureDate);
            databaseResponse.TenureType.Code.Should().Be(requestObject.TenureType.Code);
        }

        public async Task ThenTheTenureUpdatedEventIsRaised(TenureFixture tenureFixture, SnsEventVerifier<EntityEventSns> snsVerifer)
        {
            var dbRecord = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            Action<EntityEventSns> verifyFunc = (actual) =>
            {
                actual.CorrelationId.Should().NotBeEmpty();
                actual.DateTime.Should().BeCloseTo(DateTime.UtcNow, 2000);
                actual.EntityId.Should().Be(dbRecord.Id);

                var expectedOldData = new Dictionary<string, object>
                {
                    { "startOfTenureDate", tenureFixture.Tenure.StartOfTenureDate?.ToString(DateFormat) },
                    { "endOfTenureDate", tenureFixture.Tenure.EndOfTenureDate?.ToString(DateFormat) },
                    { "tenureType", tenureFixture.Tenure.TenureType }
                };
                var expectedNewData = new Dictionary<string, object>
                {
                    { "startOfTenureDate", dbRecord.StartOfTenureDate?.ToString(DateFormat) },
                    { "endOfTenureDate", dbRecord.EndOfTenureDate?.ToString(DateFormat) },
                    { "tenureType", dbRecord.TenureType }
                };
                VerifyEventData(actual.EventData.OldData, expectedOldData);
                VerifyEventData(actual.EventData.NewData, expectedNewData);

                actual.EventType.Should().Be(UpdateTenureConstants.EVENTTYPE);
                actual.Id.Should().NotBeEmpty();
                actual.SourceDomain.Should().Be(UpdateTenureConstants.SOURCE_DOMAIN);
                actual.SourceSystem.Should().Be(UpdateTenureConstants.SOURCE_SYSTEM);
                actual.User.Email.Should().Be("e2e-testing@development.com");
                actual.User.Name.Should().Be("Tester");
                actual.Version.Should().Be(UpdateTenureConstants.V1_VERSION);
            };

            snsVerifer.VerifySnsEventRaised(verifyFunc).Should().BeTrue(snsVerifer.LastException?.Message);
        }

        private void VerifyEventData(object eventDataJsonObj, Dictionary<string, object> expected)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(eventDataJsonObj.ToString(), CreateJsonOptions());
            DateTime.Parse(data["startOfTenureDate"].ToString()).Should().Be(DateTime.Parse(expected["startOfTenureDate"].ToString()));
            DateTime.Parse(data["endOfTenureDate"].ToString()).Should().Be(DateTime.Parse(expected["endOfTenureDate"].ToString()));

            var eventDataTenureType = JsonSerializer.Deserialize<TenureType>(data["tenureType"].ToString(), CreateJsonOptions());
            eventDataTenureType.Should().BeEquivalentTo(expected["tenureType"]);
        }
    }
}
