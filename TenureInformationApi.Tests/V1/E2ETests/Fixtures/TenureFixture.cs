using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.Tests.V1.E2ETests.Fixtures
{
    public class TenureFixture : IDisposable
    {

        public readonly Fixture _fixture = new Fixture();
        public readonly IDynamoDBContext _dbContext;
        public TenureInformationDb Tenure { get; private set; }
        public TenureFixture(IDynamoDBContext context)
        {
            _dbContext = context;
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
                if (null != Tenure)
                    _dbContext.DeleteAsync<TenureInformationDb>(Tenure.Id).GetAwaiter().GetResult();

                _disposed = true;
            }
        }
        public CreateTenureRequestObject CreateTenureRequestObject;

        public void GivenNewTenureRequest()
        {
            var tenureRequest = _fixture.Build<CreateTenureRequestObject>()
                                        .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.SuccessionDate, DateTime.UtcNow)
                                        .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                        .With(x => x.SubletEndDate, DateTime.UtcNow)
                                        .With(x => x.EvictionDate, DateTime.UtcNow)
                                        .Create();

            CreateTenureRequestObject = tenureRequest;
        }
    }
}
