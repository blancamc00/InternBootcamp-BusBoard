using Newtonsoft.Json;

namespace BusBoard;

public class DeparturesResponse
{
    [JsonProperty("name")]
    public string StopName { get; set; }
    
    [JsonProperty("departures")]
    public Dictionary<string, List<Buses>> Departures { get; set; }
}