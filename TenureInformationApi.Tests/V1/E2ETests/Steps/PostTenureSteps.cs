using FluentAssertions;
using Hackney.Core.Sns;
using Hackney.Core.Testing.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Factories;
using Hackney.Shared.Tenure.Infrastructure;
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
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
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

        public async Task ThenTheTenureCreatedEventIsRaised(TenureFixture tenureFixture, ISnsFixture snsFixture)
        {
            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiTenure = JsonSerializer.Deserialize<TenureResponseObject>(responseContent, CreateJsonOptions());
            var dbRecord = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(apiTenure.Id).ConfigureAwait(false);

            Action<EntityEventSns> verifyFunc = (actual) =>
            {
                actual.CorrelationId.Should().NotBeEmpty();
                actual.DateTime.Should().BeCloseTo(DateTime.UtcNow, 2000);
                actual.EntityId.Should().Be(dbRecord.Id);

                var expected = dbRecord.ToDomain();
                var actualNewData = JsonSerializer.Deserialize<TenureInformation>(actual.EventData.NewData.ToString(), CreateJsonOptions());
                actualNewData.Should().BeEquivalentTo(expected);
                actual.EventData.OldData.Should().BeNull();

                actual.EventType.Should().Be(CreateTenureEventConstants.EVENTTYPE);
                actual.Id.Should().NotBeEmpty();
                actual.SourceDomain.Should().Be(CreateTenureEventConstants.SOURCE_DOMAIN);
                actual.SourceSystem.Should().Be(CreateTenureEventConstants.SOURCE_SYSTEM);
                actual.User.Email.Should().Be("e2e-testing@development.com");
                actual.User.Name.Should().Be("Tester");
                actual.Version.Should().Be(CreateTenureEventConstants.V1_VERSION);
            };

            var snsVerifer = snsFixture.GetSnsEventVerifier<EntityEventSns>();
            (await snsVerifer.VerifySnsEventRaised<EntityEventSns>(verifyFunc)).Should().BeTrue(snsVerifer.LastException?.Message);
        }

        public async Task ThenTheTenureDetailsAreReturnedAndIdIsNotEmpty(TenureFixture tenureFixture)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiTenure = JsonSerializer.Deserialize<TenureResponseObject>(responseContent, CreateJsonOptions());

            apiTenure.Id.Should().NotBeEmpty();

            var dbRecord = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(apiTenure.Id).ConfigureAwait(false);
            dbRecord.Should().BeEquivalentTo(tenureFixture.CreateTenureRequestObject.ToDatabase(),
                                             c => c.Excluding(x => x.VersionNumber)
                                                   .Excluding(x => x.TenuredAsset.PropertyReference));

            dbRecord.TenuredAsset.PropertyReference.Should()
                                                   .Be(tenureFixture.CreateTenureRequestObject.TenuredAsset.PropertyReference);

            var domain = dbRecord.ToDomain();
            apiTenure.Should().BeEquivalentTo(domain.ToResponse());

            _cleanup.Add(async () => await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(dbRecord.Id).ConfigureAwait(false));
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
