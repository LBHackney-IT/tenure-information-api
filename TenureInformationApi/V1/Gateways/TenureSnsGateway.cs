using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.Tests.V1.Gateways
{
    public class TenureSnsGateway : ISnsGateway
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly IConfiguration _configuration;

        public TenureSnsGateway(IAmazonSimpleNotificationService amazonSimpleNotificationService, IConfiguration configuration)
        {
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _configuration = configuration;
        }

        public async Task Publish(TenureSns tenureSns)
        {
            string message = JsonConvert.SerializeObject(tenureSns);
            var request = new PublishRequest { Message = message, TopicArn = Environment.GetEnvironmentVariable("NEW_TENURE_SNS_ARN") };

            await _amazonSimpleNotificationService.PublishAsync(request).ConfigureAwait(false);
        }

    }
}
