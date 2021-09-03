using Amazon.DynamoDBv2.DataModel;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Infrastructure;

namespace TenureInformationApi.Tests.V1.E2ETests.Fixtures
{
    public class TenureFixture : IDisposable
    {

        public readonly Fixture _fixture = new Fixture();
        public readonly IDynamoDBContext _dbContext;
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;

        public TenureInformationDb Tenure { get; private set; }

        public Guid TenureId { get; private set; }

        public Guid PersonId { get; private set; }

        public string InvalidTenureId { get; private set; }
        public TenureFixture(IDynamoDBContext context, IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            _dbContext = context;
            _amazonSimpleNotificationService = amazonSimpleNotificationService;

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
        public UpdateTenureRequestObject UpdateTenureRequestObject;

        public void GivenATenureExist(bool nullTenuredAssetType = false)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                 .With(x => x.SuccessionDate, DateTime.UtcNow)
                                 .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                 .With(x => x.SubletEndDate, DateTime.UtcNow)
                                 .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();

            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;

            _dbContext.SaveAsync<TenureInformationDb>(entity.ToDatabase()).GetAwaiter().GetResult();

            Tenure = entity.ToDatabase();
            TenureId = entity.Id;

        }

        public void GivenATenureDoesNotExist()
        {
            TenureId = Guid.NewGuid();
        }

        public void GivenAUpdateTenureDoesNotExist()
        {
            TenureId = Guid.NewGuid();
            PersonId = Guid.NewGuid();
            var request = _fixture.Build<UpdateTenureRequestObject>()
              .With(x => x.Id, TenureId)
              .Create();
            UpdateTenureRequestObject = request;

        }

        public void GivenAnInvalidTenureId()
        {
            InvalidTenureId = "1234567";
        }

        public void GivenNewTenureRequest()
        {
            var tenureRequest = _fixture.Build<CreateTenureRequestObject>()
                                        .With(x => x.EndOfTenureDate, DateTime.UtcNow.AddDays(1))
                                        .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.SuccessionDate, DateTime.UtcNow)
                                        .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                        .With(x => x.SubletEndDate, DateTime.UtcNow)
                                        .With(x => x.EvictionDate, DateTime.UtcNow)
                                        .Create();
            CreateSnsTopic();
            CreateTenureRequestObject = tenureRequest;

        }

        public void GivenNewTenureRequestWithValidationErrors()
        {
            var tenureRequest = _fixture.Build<CreateTenureRequestObject>()
                                        .With(x => x.EndOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.StartOfTenureDate, DateTime.UtcNow.AddDays(1))
                                        .With(x => x.SuccessionDate, DateTime.UtcNow)
                                        .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                        .With(x => x.SubletEndDate, DateTime.UtcNow)
                                        .With(x => x.EvictionDate, DateTime.UtcNow)
                                        .Create();
            CreateSnsTopic();

            CreateTenureRequestObject = tenureRequest;
        }

        public void GivenanUpdateTenureWithNewHouseholdReqeust(bool nullTenuredAssetType = false)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow.AddDays(1))
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.SuccessionDate, DateTime.UtcNow)
                                        .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                        .With(x => x.SubletEndDate, DateTime.UtcNow)
                                        .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Without(x => x.HouseholdMembers)
                                 .Create();

            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;

            _dbContext.SaveAsync<TenureInformationDb>(entity.ToDatabase()).GetAwaiter().GetResult();

            var request = _fixture.Build<UpdateTenureRequestObject>()
               .With(x => x.Id, entity.Id)
               .Create();

            Tenure = entity.ToDatabase();
            TenureId = entity.Id;
            PersonId = request.HouseholdMembers.First().Id;
            UpdateTenureRequestObject = request;
        }

        public void GivenanUpdateTenureHouseholdRequest(bool nullTenuredAssetType = false)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow.AddDays(1))
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.SuccessionDate, DateTime.UtcNow)
                                        .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                        .With(x => x.SubletEndDate, DateTime.UtcNow)
                                        .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Create();

            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;

            _dbContext.SaveAsync<TenureInformationDb>(entity.ToDatabase()).GetAwaiter().GetResult();
            var request = _fixture.Build<UpdateTenureRequestObject>()
               .With(x => x.Id, entity.Id)
               .With(x => x.HouseholdMembers, entity.HouseholdMembers.ToList())
               .Create();

            Tenure = entity.ToDatabase();
            TenureId = entity.Id;
            PersonId = request.HouseholdMembers.First().Id;
            UpdateTenureRequestObject = request;
        }

        public void GivenanUpdateTenureRequestWithValidationError(bool nullTenuredAssetType = false)
        {
            var entity = _fixture.Build<TenureInformation>()
                                 .With(x => x.EndOfTenureDate, DateTime.UtcNow.AddDays(1))
                                 .With(x => x.StartOfTenureDate, DateTime.UtcNow)
                                        .With(x => x.SuccessionDate, DateTime.UtcNow)
                                        .With(x => x.PotentialEndDate, DateTime.UtcNow)
                                        .With(x => x.SubletEndDate, DateTime.UtcNow)
                                        .With(x => x.EvictionDate, DateTime.UtcNow)
                                 .Without(x => x.HouseholdMembers)
                                 .Create();

            if (nullTenuredAssetType)
                entity.TenuredAsset.Type = null;

            _dbContext.SaveAsync<TenureInformationDb>(entity.ToDatabase()).GetAwaiter().GetResult();

            var request = _fixture.Build<UpdateTenureRequestObject>()
               .Without(x => x.Id)
               .Create();

            Tenure = entity.ToDatabase();
            TenureId = entity.Id;
            PersonId = request.HouseholdMembers.First().Id;
            UpdateTenureRequestObject = request;
        }
        private void CreateSnsTopic()
        {
            var snsAttrs = new Dictionary<string, string>();
            snsAttrs.Add("fifo_topic", "true");
            snsAttrs.Add("content_based_deduplication", "true");

            var response = _amazonSimpleNotificationService.CreateTopicAsync(new CreateTopicRequest
            {
                Name = "tenure",
                Attributes = snsAttrs
            }).Result;

            Environment.SetEnvironmentVariable("TENURE_SNS_ARN", response.TopicArn);
        }


    }
}
