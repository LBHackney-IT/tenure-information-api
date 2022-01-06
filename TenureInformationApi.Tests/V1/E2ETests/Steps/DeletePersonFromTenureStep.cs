using FluentAssertions;
using Hackney.Core.Sns;
using Hackney.Core.Testing.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TenureInformationApi.Tests.V1.E2ETests.Fixtures;
using TenureInformationApi.V1.Infrastructure;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TenureInformationApi.Tests.V1.E2ETests.Steps
{
    public class DeletePersonFromTenureStep : BaseSteps
    {
        public DeletePersonFromTenureStep(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenDeletePersonFromTenureApiIsCalledAsync(DeletePersonFromTenureQueryRequest query)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";

            // setup request
            var uri = new Uri($"api/v1/tenures/{query.TenureId}/person/{query.PersonId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Method = HttpMethod.Delete;
            message.Headers.Add("Authorization", token);

            // call request
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _lastResponse = await _httpClient.SendAsync(message).ConfigureAwait(false);
        }

        public void NotFoundResponseReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void NoContentResponseReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        public async Task PersonRemovedFromTenure(Guid tenureId, Guid personId, TenureFixture fixture)
        {
            var tenure = await fixture._dbContext.LoadAsync<TenureInformationDb>(tenureId).ConfigureAwait(false);

            tenure.HouseholdMembers.Should().NotContain(x => x.Id == personId);
        }

        public async Task ThenThePersonRemovedFromTenureEventIsRaised(TenureFixture tenureFixture, ISnsFixture snsFixture)
        {
            var dbRecord = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            Action<EntityEventSns> verifyFunc = (actual) =>
            {
                actual.CorrelationId.Should().NotBeEmpty();
                actual.DateTime.Should().BeCloseTo(DateTime.UtcNow, 2000);
                actual.EntityId.Should().Be(dbRecord.Id);

                VerifyEventData(actual.EventData.OldData, tenureFixture.Tenure.HouseholdMembers);
                VerifyEventData(actual.EventData.NewData, dbRecord.HouseholdMembers);

                actual.EventType.Should().Be(PersonRemovedFromTenureConstants.EVENTTYPE);
                actual.Id.Should().NotBeEmpty();
                actual.SourceDomain.Should().Be(PersonRemovedFromTenureConstants.SOURCE_DOMAIN);
                actual.SourceSystem.Should().Be(PersonRemovedFromTenureConstants.SOURCE_SYSTEM);
                actual.User.Email.Should().Be("e2e-testing@development.com");
                actual.User.Name.Should().Be("Tester");
                actual.Version.Should().Be(PersonRemovedFromTenureConstants.V1_VERSION);
            };

            var snsVerifer = snsFixture.GetSnsEventVerifier<EntityEventSns>();
            var snsResult = await snsVerifer.VerifySnsEventRaised(verifyFunc);
            if (!snsResult && snsVerifer.LastException != null)
                throw snsVerifer.LastException;
        }

        private void VerifyEventData(object eventDataJsonObj, List<HouseholdMembers> expected)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(eventDataJsonObj.ToString(), CreateJsonOptions());
            var hmData = JsonSerializer.Deserialize<List<HouseholdMembers>>(data["householdMembers"].ToString(), CreateJsonOptions());
            hmData.Should().BeEquivalentTo(expected);
        }
    }
}
