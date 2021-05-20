using System.Text.Json.Serialization;

namespace TenureInformationApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HouseholdMembersType
    {
        person,
        organisation
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TenuredAssetType
    {
        dwelling,
        garage
    }
}
