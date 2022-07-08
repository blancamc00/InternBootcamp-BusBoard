using Newtonsoft.Json.Linq;

namespace BusBoard.Api;

public class ApiClass
{
    public async Task<IList<BusStop>> getStopList(string postCode)
    {
        string urlPost = CreateUrlForPostcode(postCode);
        using var client = new HttpClient();
        HttpResponseMessage tempPost =
            await client.GetAsync(urlPost);
        string inputPost = "";
        urlPost = CreateUrlForPostcode(postCode);
        tempPost =
            await client.GetAsync(urlPost);
        /*while (tempPost.IsSuccessStatusCode != true)
        {
            Console.WriteLine("That is not a valid postcode.");
            Console.WriteLine("Enter a valid postcode: ");
            postCode = Console.ReadLine();
            urlPost = CreateUrlForPostcode(postCode);
            tempPost =
                await client.GetAsync(urlPost);
        }*/
        inputPost = await tempPost.Content.ReadAsStringAsync();
        Postcode latLong = getLatLong(inputPost);
            
        //Convert latitude and longitude to bus stop ATCO codes
        string urlLatLong = CreateUrlForLatLong(latLong.latitude, latLong.longitude);
        var inputLatLong = AccessUrl(urlLatLong);
        IList<BusStop> stopList = getStops(inputLatLong.Result);
        return stopList.Take(2).ToList();
    }

    public async Task<string> AccessUrl(string url)
    {
        using var client = new HttpClient();
        var data =
            await client.GetStringAsync(url);
        return data;
    }
    public string CreateUrlForPostcode(string postCode)
    {
        string urlPost = "https://api.postcodes.io/postcodes/" + postCode;
        return urlPost;
    }
    
    public Postcode getLatLong(string inputPost)
    {
        JObject jOresultPost = JObject.Parse(inputPost);
        JToken dataPost = jOresultPost["result"];
        Postcode latLong = dataPost.ToObject<Postcode>();
        return latLong;
    }
    
    public string CreateUrlForLatLong(string Lat, string Long)
    {
        var uLatLong = "http://transportapi.com/v3/uk/places.json?";
        var builder = new UriBuilder(uLatLong);
        builder.Query = "lat=" + Lat + "&lon=" + Long +
                        "&type=bus_stop&app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&limit=2";
        var urlLatLong = builder.ToString();
        return urlLatLong;
    }

    public IList<BusStop> getStops(string inputLatLong)
    {
        JObject jOresultLatLong = JObject.Parse(inputLatLong);
        IList<JToken> dataLatLong = jOresultLatLong["member"].Children().ToList();
        IList<BusStop> stopList = new List<BusStop>();
        foreach (JToken stop in dataLatLong)
        {
            BusStop tempStop = stop.ToObject<BusStop>();
            stopList.Add(tempStop);
        }

        return stopList;
    }

    public string CreateUrlForBuses(string stopCode)
    {
        var u = "https://transportapi.com/v3/uk/bus/stop/" + stopCode + "///timetable.json?";
        var builder = new UriBuilder(u);
        builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route&limit=5";
        var url = builder.ToString();
        return url;
    }

    public List<Bus> getSchedule(string busInfo)
    {
        var busList = new DeparturesResponse();
        busList = Newtonsoft.Json.JsonConvert.DeserializeObject<DeparturesResponse>(busInfo);
        List<Bus> displayList = busList.Departures.SelectMany(pair => pair.Value).ToList();
        displayList = displayList.OrderBy(bus => bus.aimed_departure_time).ToList();
        return displayList;
    }    
}
