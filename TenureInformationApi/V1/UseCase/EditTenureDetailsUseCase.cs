using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Boundary.Requests;
using TenureInformationApi.V1.Boundary.Response;
using TenureInformationApi.V1.Domain;
using TenureInformationApi.V1.Factories;
using TenureInformationApi.V1.Gateways;
using TenureInformationApi.V1.Infrastructure;
using TenureInformationApi.V1.UseCase.Interfaces;

namespace TenureInformationApi.V1.UseCase
{
    public class EditTenureDetailsUseCase : IEditTenureDetailsUseCase
    {
        private readonly ITenureGateway _tenureGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public EditTenureDetailsUseCase(ITenureGateway tenureGateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _tenureGateway = tenureGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<TenureResponseObject> ExecuteAsync(
            TenureQueryRequest query, EditTenureDetailsRequestObject editTenureDetailsRequestObject, string requestBody, Token token)
        {
            var result = await _tenureGateway.EditTenureDetails(query, editTenureDetailsRequestObject, requestBody).ConfigureAwait(false);
            if (result == null) return null;

            if (result.NewValues.Any())
            {
                var tenureSnsMessage = _snsFactory.PersonAddedToTenure(result, token);
                var tenureTopicArn = Environment.GetEnvironmentVariable("TENURE_SNS_ARN");

                await _snsGateway.Publish(tenureSnsMessage, tenureTopicArn).ConfigureAwait(false);
            }

            return result.UpdatedEntity.ToDomain().ToResponse();
        }
    }
}
