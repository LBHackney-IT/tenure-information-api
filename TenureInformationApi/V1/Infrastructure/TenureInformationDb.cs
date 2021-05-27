using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using TenureInformationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;

namespace TenureInformationApi.V1.Infrastructure
{

    [DynamoDBTable("TenureInformation", LowerCamelCaseProperties = true)]
    public class TenureInformationDb
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        [DynamoDBProperty]
        public string PaymentReference { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<HouseholdMembers>))]
        public List<HouseholdMembers> HouseholdMembers { get; set; } = new List<HouseholdMembers>();

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<TenuredAsset>))]
        public TenuredAsset TenuredAsset { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<AccountType>))]
        public AccountType AccountType { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Charges>))]
        public Charges Charges { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime StartOfTenureDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime EndOfTenureDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<TenureType>))]
        public TenureType TenureType { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbBoolConverter))]
        public bool IsActive { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbBoolConverter))]
        public bool IsTenanted { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<Terminated>))]
        public Terminated Terminated { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime SuccessionDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<AgreementType>))]
        public AgreementType AgreementType { get; set; }

        [DynamoDBProperty]
        public List<string> SubsidiaryAccountsReferences { get; set; } = new List<string>();

        [DynamoDBProperty]
        public string MasterAccountTenureReference { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime EvictionDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime PotentialEndDate { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<Notices>))]
        public List<Notices> Notices { get; set; } = new List<Notices>();

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectListConverter<LegacyReference>))]
        public List<LegacyReference> LegacyReferences { get; set; } = new List<LegacyReference>();

        [DynamoDBProperty]
        public string RentCostCentre { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbBoolConverter))]
        public bool IsMutualExchange { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbBoolConverter))]
        public bool InformHousingBenefitsForChanges { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbBoolConverter))]
        public bool IsSublet { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime SubletEndDate { get; set; }
    }
}