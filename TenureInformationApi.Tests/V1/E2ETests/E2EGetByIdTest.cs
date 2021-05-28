using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;
using Xunit;

namespace TenureInformationApi.Tests.V1.E2ETests
{
    [Collection("DynamoDb collection")]
    public class E2EGetByIdTest : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        public TenureInformationDb Tenure { get; private set; }
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly List<Action> _cleanupActions = new List<Action>();

        public E2EGetByIdTest(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
        }

        /// <summary>
        /// Method to construct a test entity that can be used in a test
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private TenureInformation ConstructTestEntity()
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)

                                 .Create();
            return entity;
        }
        /// <summary>
        /// Method to add an entity instance to the database so that it can be used in a test.
        /// Also adds the corresponding action to remove the upserted data from the database when the test is done.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task SetupTestData(TenureInformation entity)
        {
            await _dbFixture.DynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
            _cleanupActions.Add(async () => await _dbFixture.DynamoDbContext.DeleteAsync<TenureInformationDb>(entity.Id).ConfigureAwait(false));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanupActions)
                    action();

                _disposed = true;
            }
        }

        [Fact]
        public async Task GetEntityByIdNotFoundReturns404()
        {
            var id = Guid.NewGuid();
            var uri = new Uri($"api/v1/tenures/{id}", UriKind.Relative);
            var response = await _dbFixture.Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetTenureByIdFoundReturnsResponse()
        {
            var entity = ConstructTestEntity();
            await SetupTestData(entity).ConfigureAwait(false);

            var uri = new Uri($"api/v1/tenures/{entity.Id}", UriKind.Relative);
            var response = await _dbFixture.Client.GetAsync(uri).ConfigureAwait(false);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiEntity = JsonConvert.DeserializeObject<TenureResponseObject>(responseContent);

            apiEntity.Should().BeEquivalentTo(entity.ToResponse());
        }
    }
}
