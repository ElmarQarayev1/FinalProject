using System.Text.Json.Serialization;

public class YearlyAppointmentsResponse
{
    [JsonPropertyName("months")]
    public List<string> Months { get; set; }

    [JsonPropertyName("appointments")]
    public List<int> Appointments { get; set; }
}
