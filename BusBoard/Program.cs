using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusBoard
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to BusBoard!");
            Console.WriteLine("Enter a postcode: ");
            string postCode = Console.ReadLine();

            string urlPost = "https://api.postcodes.io/postcodes/" + postCode;
            
            
            using var clientPost = new HttpClient();
            var inputPost =
                await clientPost.GetStringAsync(urlPost);

            //Console.WriteLine(result);
            
            JObject jOresultPost = JObject.Parse(inputPost);
            JToken dataPost = jOresultPost["result"];
            Postcodes latLong = dataPost.ToObject<Postcodes>();
            
            var uLatLong = "http://transportapi.com/v3/uk/places.json?";
            var builder = new UriBuilder(uLatLong);
            builder.Query = "lat=" + latLong.latitude + "&lon=" + latLong.longitude + "&type=bus_stop&app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&limit=2";
            var urlLatLong = builder.ToString();
            using var clientLatLong = new HttpClient();
            var inputLatLong =
                await clientPost.GetStringAsync(urlLatLong);

            JObject jOresultLatLong = JObject.Parse(inputLatLong);
            IList<JToken> dataLatLong = jOresultLatLong["member"].Children().ToList();
            IList<BusStops> stopList = new List<BusStops>();
            foreach (JToken stop in dataLatLong)
            {
                BusStops tempStop = stop.ToObject<BusStops>();
                stopList.Add(tempStop);
            }
            
            Console.WriteLine(stopList[0].atcocode);
            
            string url = CreateUrl(stopList[0].atcocode);
            using var client = new HttpClient();
            var input =
                await client.GetStringAsync(url);


            JObject jOresult = JObject.Parse(input);
            List<JToken> routes = jOresult["departures"].Children().ToList();
            List<List<JToken>> data = new List<List<JToken>>();   
            foreach (JToken route in routes)
            {
                List<JToken> newRoute = routes.Children().ToList();
                data.Add(newRoute);
            }
           
            IList<Buses> busList = new List<Buses>();
            foreach (List<JToken> busRoute in data){
                foreach (JToken bus in busRoute)
                {
                    Buses tempBus = bus.ToObject<Buses>();
                    busList.Add(tempBus);
                }
                
            }
            
            //0500CCITY436
            Console.WriteLine("The next five buses are: ");

            for (int i = 0; i < busList.Count; i++)
            {
                Console.WriteLine(busList[i].line + " " + busList[i].direction + " " + busList[i].aimed_departure_time);
            }

        }
        
        public static string CreateUrl(string stopCode)
        {
            var u = "https://transportapi.com/v3/uk/bus/stop/"+ stopCode + "///timetable.json?";
            var builder = new UriBuilder(u);
            builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route&limit=5";
            var url = builder.ToString();
            return url;
        }
    }

    public class Buses
    {
        public string aimed_departure_time { get; set; }
        public string line { get; set; }
        public string direction { get; set; }
    }

    public class Postcodes
    {
        public string longitude { get; set; }
        public string latitude { get; set; }
    }

    public class BusStops
    {
        public string atcocode { get; set; }
    }
}
