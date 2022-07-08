using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusBoard
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to BusBoard!");

            //Convert Postcode to Latitude and Longitude
            string urlPost = CreateUrlForPostcode();
            using var client = new HttpClient();
            HttpResponseMessage tempPost =
                await client.GetAsync(urlPost);
            string inputPost = "";
            while (tempPost.IsSuccessStatusCode != true)
            {
                Console.WriteLine("That is not a valid postcode.");
                urlPost = CreateUrlForPostcode();
                tempPost =
                    await client.GetAsync(urlPost);
            }
            inputPost = await tempPost.Content.ReadAsStringAsync();
            
            Postcodes latLong = getLatLong(inputPost);
            
            //Convert latitude and longitude to bus stop ATCO codes
            string urlLatLong = CreateUrlForLatLong(latLong.latitude, latLong.longitude);
            var inputLatLong =
                await client.GetStringAsync(urlLatLong);
            IList<BusStops> stopList = getStops(inputLatLong);

            //Getting schedule from each stop
            for (int i = 0; i < 2; i++)
            {
                string url = CreateUrlForBuses(stopList[i].atcocode);
                var input =
                    await client.GetStringAsync(url);

                List<Buses> displayList = getSchedule(input);
                displayBuses(stopList, displayList, i);
            }

        }

        public static string CreateUrlForPostcode()
        {
            Console.WriteLine("Enter a valid postcode: ");
            string postCode = Console.ReadLine();
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

        public static string CreateUrlForBuses(string stopCode)
        {
            var u = "https://transportapi.com/v3/uk/bus/stop/" + stopCode + "///timetable.json?";
            var builder = new UriBuilder(u);
            builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route&limit=5";
            var url = builder.ToString();
            return url;
        }

        public static List<Buses> getSchedule(string input)
        {
            var busList = new DeparturesResponse();
            busList = Newtonsoft.Json.JsonConvert.DeserializeObject<DeparturesResponse>(input);

            List<Buses> displayList = busList.Departures.SelectMany(pair => pair.Value).ToList();
            displayList = displayList.OrderBy(bus => bus.aimed_departure_time).ToList();
            return displayList;
        }

        public static void displayBuses(IList<BusStops> stopList, List<Buses> displayList, int i)
        {
            Console.WriteLine("The next five buses at stop " + stopList[i].name + " are: ");
            Console.WriteLine("=============================================================================");
            Console.WriteLine("{0,-15} {1,-20} {2,-10}\n", "Line", "Direction", "Aimed departure time");

            for (int j = 0; j < 5; j++)
            {
                Console.WriteLine("{0,-15} {1,-20} {2,-10}", displayList[j].line, displayList[j].direction, displayList[j].aimed_departure_time);
            }
                
            Console.WriteLine("");
        }
    }
}