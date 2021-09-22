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
using AutoFixture;

namespace TenureInformationApi.Tests.V1.E2ETests.Steps
{
    public class DeleteTenureDetailsStep : BaseSteps
    {
        public DeleteTenureDetailsStep(HttpClient httpClient) : base(httpClient)
        {


        }

        public async Task WhenDeletePersonFromTenureApiIsCalledAsync(DeletePersonFromTenureQueryRequest query)
        {
            var token =
                 "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImV2YW5nZWxvcy5ha3RvdWRpYW5ha2lzQGhhY2tuZXkuZ292LnVrIiwiaXNzIjoiSGFja25leSIsIm5hbWUiOiJFdmFuZ2Vsb3MgQWt0b3VkaWFuYWtpcyIsImdyb3VwcyI6WyJzYW1sLWF3cy1jb25zb2xlLW10ZmgtZGV2ZWxvcGVyIl0sImlhdCI6MTYyMzA1ODIzMn0.Jnd2kQTMiAUeKMJCYQVEVXbFc9BbIH90OociR15gfpw";

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
    }
}
