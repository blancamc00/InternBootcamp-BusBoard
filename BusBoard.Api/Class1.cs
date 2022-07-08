using Newtonsoft.Json.Linq;

namespace BusBoard.Api;

public class Class1
{
    public static async void getStopList(string postCode)
    {
        string urlPost = CreateUrlForPostcode(postCode);
        using var client = new HttpClient();
        HttpResponseMessage tempPost =
            await client.GetAsync(urlPost);
        string inputPost = "";
        while (tempPost.IsSuccessStatusCode != true)
        {
            Console.WriteLine("That is not a valid postcode.");
            Console.WriteLine("Enter a valid postcode: ");
            postCode = Console.ReadLine();
            urlPost = CreateUrlForPostcode(postCode);
            tempPost =
                await client.GetAsync(urlPost);
        }
        inputPost = await tempPost.Content.ReadAsStringAsync();
            
        Postcodes latLong = getLatLong(inputPost);
            
        //Convert latitude and longitude to bus stop ATCO codes
        string urlLatLong = CreateUrlForLatLong(latLong.latitude, latLong.longitude);
        var inputLatLong =
            await client.GetStringAsync(urlLatLong);
        stopList = getStops(inputLatLong);
    }
    
    
    public static string CreateUrlForPostcode(string postCode)
    {
        string urlPost = "https://api.postcodes.io/postcodes/" + postCode;
        return urlPost;
    }
    
    public static Postcodes getLatLong(string inputPost)
    {
        JObject jOresultPost = JObject.Parse(inputPost);
        JToken dataPost = jOresultPost["result"];
        Postcodes latLong = dataPost.ToObject<Postcodes>();
        return latLong;
    }
    
    public static string CreateUrlForLatLong(string Lat, string Long)
    {
        var uLatLong = "http://transportapi.com/v3/uk/places.json?";
        var builder = new UriBuilder(uLatLong);
        builder.Query = "lat=" + Lat + "&lon=" + Long +
                        "&type=bus_stop&app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&limit=2";
        var urlLatLong = builder.ToString();
        return urlLatLong;
    }

    public static IList<BusStops> getStops(string inputLatLong)
    {
        JObject jOresultLatLong = JObject.Parse(inputLatLong);
        IList<JToken> dataLatLong = jOresultLatLong["member"].Children().ToList();
        IList<BusStops> stopList = new List<BusStops>();
        foreach (JToken stop in dataLatLong)
        {
            BusStops tempStop = stop.ToObject<BusStops>();
            stopList.Add(tempStop);
        }

        return stopList;
    }

    static string CreateUrlForBuses(string stopCode)
    {
        var u = "https://transportapi.com/v3/uk/bus/stop/" + stopCode + "///timetable.json?";
        var builder = new UriBuilder(u);
        builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route&limit=5";
        var url = builder.ToString();
        return url;
    }

    static List<Buses> getSchedule(string input)
    {
        var busList = new DeparturesResponse();
        busList = Newtonsoft.Json.JsonConvert.DeserializeObject<DeparturesResponse>(input);
        List<Buses> displayList = busList.Departures.SelectMany(pair => pair.Value).ToList();
        displayList = displayList.OrderBy(bus => bus.aimed_departure_time).ToList();
        return displayList;
    }    
}
