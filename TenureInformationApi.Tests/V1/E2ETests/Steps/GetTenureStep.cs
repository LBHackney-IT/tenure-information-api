using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.Tests.V1.E2ETests.Steps
{
    public class GetTenureStep : BaseSteps
    {
        public GetTenureStep(HttpClient httpClient) : base(httpClient)
        { }

        public async Task WhenTenureDetailsAreRequested(string id)
        {
            var uri = new Uri($"api/v1/tenures/{id}", UriKind.Relative);
            _lastResponse = await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }

        public async Task ThenTheTenureDetailsAreReturned(TenureInformationDb dbEntity)
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await _lastResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiTenure = JsonSerializer.Deserialize<TenureResponseObject>(responseContent, CreateJsonOptions());

            apiTenure.Should().BeEquivalentTo(dbEntity, config => config.Excluding(y => y.VersionNumber));

            var expectedEtagValue = $"\"{dbEntity.VersionNumber}\"";
            _lastResponse.Headers.ETag.Tag.Should().Be(expectedEtagValue);
            var eTagHeaders = _lastResponse.Headers.GetValues("ETag");
            eTagHeaders.Count().Should().Be(1);
            eTagHeaders.First().Should().Be(expectedEtagValue);
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
