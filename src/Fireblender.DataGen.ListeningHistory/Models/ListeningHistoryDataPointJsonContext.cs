namespace Fireblender.DataGen.ListeningHistory.Models
{
    using System.Text.Json.Serialization;

    [JsonSerializable(typeof(ListeningHistoryDataPoint))]
    public partial class ListeningHistoryDataPointJsonContext : JsonSerializerContext
    {

    }
}
