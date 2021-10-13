using FluentAssertions;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Requests;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Infrastructure;
using Newtonsoft.Json;
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
    public class AddPersonToTenureStep : BaseSteps
    {
        public AddPersonToTenureStep(HttpClient httpClient) : base(httpClient)
        { }

        /// <summary>
        /// You can use jwt.io to decode the token - it is the same one we'd use on dev, etc. 
        /// </summary>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CallAPI(Guid id, Guid personId, UpdateTenureForPersonRequestObject requestObject, int? ifMatch)
        {
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";
            var uri = new Uri($"api/v1/tenures/{id}/person/{personId}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
            message.Content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);
            message.Headers.TryAddWithoutValidation(HeaderConstants.IfMatch, $"\"{ifMatch?.ToString()}\"");


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

            result.Should().BeEquivalentTo(tenureFixture.Tenure,
                                           c => c.Excluding(y => y.HouseholdMembers)
                                                 .Excluding(z => z.VersionNumber));
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

            _cleanup.Add(async () => await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(result.Id).ConfigureAwait(false));
        }

        public async Task ThenThePersonAddedToTenureEventIsRaised(TenureFixture tenureFixture, SnsEventVerifier<EntityEventSns> snsVerifer)
        {
            var dbRecord = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            Action<EntityEventSns> verifyFunc = (actual) =>
            {
                actual.CorrelationId.Should().NotBeEmpty();
                actual.DateTime.Should().BeCloseTo(DateTime.UtcNow, 2000);
                actual.EntityId.Should().Be(dbRecord.Id);

                VerifyEventData(actual.EventData.OldData, tenureFixture.Tenure.HouseholdMembers);
                VerifyEventData(actual.EventData.NewData, dbRecord.HouseholdMembers);

                actual.EventType.Should().Be(PersonAddedToTenureConstants.EVENTTYPE);
                actual.Id.Should().NotBeEmpty();
                actual.SourceDomain.Should().Be(PersonAddedToTenureConstants.SOURCE_DOMAIN);
                actual.SourceSystem.Should().Be(PersonAddedToTenureConstants.SOURCE_SYSTEM);
                actual.User.Email.Should().Be("e2e-testing@development.com");
                actual.User.Name.Should().Be("Tester");
                actual.Version.Should().Be(PersonAddedToTenureConstants.V1_VERSION);
            };

            snsVerifer.VerifySnsEventRaised(verifyFunc).Should().BeTrue(snsVerifer.LastException?.Message);
        }

        private void VerifyEventData(object eventDataJsonObj, List<HouseholdMembers> expected)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(eventDataJsonObj.ToString(), CreateJsonOptions());
            var hmData = JsonSerializer.Deserialize<List<HouseholdMembers>>(data["householdMembers"].ToString(), CreateJsonOptions());
            hmData.Should().BeEquivalentTo(expected);
        }

        public async Task ThenTheHouseholdMemberTenureDetailsAreUpdated(TenureFixture tenureFixture, Guid personId, UpdateTenureForPersonRequestObject request)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var result = await tenureFixture._dbContext.LoadAsync<TenureInformationDb>(tenureFixture.Tenure.Id).ConfigureAwait(false);

            result.Should().BeEquivalentTo(tenureFixture.Tenure,
                                           c => c.Excluding(y => y.HouseholdMembers)
                                                 .Excluding(z => z.VersionNumber));
            result.VersionNumber.Should().Be(1);
            result.HouseholdMembers.First(x => x.Id == personId).FullName.Should().Be(request.FullName);

            _cleanup.Add(async () => await tenureFixture._dbContext.DeleteAsync<TenureInformationDb>(result.Id).ConfigureAwait(false));
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
