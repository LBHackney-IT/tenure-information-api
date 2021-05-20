using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Infrastructure
{

    [DynamoDBTable("TenureInformation", LowerCamelCaseProperties = true)]
    public class TenureInformationDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public string PaymentReference { get; set; }

        [DynamoDBProperty]
        public HouseholdMembers HouseholdMembers { get; set; }

        [DynamoDBProperty]
        public TenuredAsset TenuredAsset { get; set; }

        [DynamoDBProperty]
        public AccountType AccountType { get; set; }

        [DynamoDBProperty]
        public Charges Charges { get; set; }

        [DynamoDBProperty]
        public DateTime StartOfTenureDate { get; set; }

        [DynamoDBProperty]
        public DateTime EndOfTenureDate { get; set; }

        [DynamoDBProperty]
        public TenureType TenureType { get; set; }

        [DynamoDBProperty]
        public bool IsActive { get; set; }

        [DynamoDBProperty]
        public bool IsTenanted { get; set; }

        [DynamoDBProperty]
        public Terminated Terminated { get; set; }

        [DynamoDBProperty]
        public DateTime SuccessionDate { get; set; }

        [DynamoDBProperty]
        public AgreementType AgreementType { get; set; }

        [DynamoDBProperty]
        public List<int> SubsidiaryAccountsReferences { get; set; } = new List<int>();

        [DynamoDBProperty]
        public string MasterAccountTenureReference { get; set; }

        [DynamoDBProperty]
        public DateTime EvictionDate { get; set; }

        [DynamoDBProperty]
        public DateTime PotentialEndDate { get; set; }

        [DynamoDBProperty]
        public Notices Notices { get; set; }

        [DynamoDBProperty]
        public List<LegacyReference> LegacyReferences { get; set; } = new List<LegacyReference>();

        [DynamoDBProperty]
        public string RentCostCentre { get; set; }

        [DynamoDBProperty]
        public bool IsMutualExchange { get; set; }

        [DynamoDBProperty]
        public bool InformHousingBenefitsForChanges { get; set; }

        [DynamoDBProperty]
        public bool IsSublet { get; set; }

        [DynamoDBProperty]
        public DateTime SubletEndDate { get; set; }
    }
}
