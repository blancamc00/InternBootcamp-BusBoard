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

            string urlLatLong = CreateUrlForLatLong(latLong.latitude, latLong.longitude);
            
            using var clientLatLong = new HttpClient();
            var inputLatLong =
                await clientPost.GetStringAsync(urlLatLong);

            IList<BusStops> stopList = getStops(inputLatLong);
            

            for (int i = 0; i < 2; i++)
            {
                string url = CreateUrlForBuses(stopList[i].atcocode);
                using var client = new HttpClient();
                var input =
                    await client.GetStringAsync(url);

                var busList = new DeparturesResponse();

                busList = Newtonsoft.Json.JsonConvert.DeserializeObject<DeparturesResponse>(input);
                
                Console.WriteLine("The next five buses at stop n" + (i+1) + " are: ");

                List<Buses> displayList = busList.Departures.SelectMany(pair => pair.Value).ToList();
                
                //SORT BUS LIST BY TIME BEFORE PRINTING NEXT FIVE

                for (int j = 0; j < 5; j++)
                {
                    //Console.WriteLine(busList[j].line + " " + busList[j].direction + " " + busList[j].aimed_departure_time);
                    Console.WriteLine(displayList[j].line);
                }
            }

        }

        public static string CreateUrlForBuses(string stopCode)
        {
            var u = "https://transportapi.com/v3/uk/bus/stop/" + stopCode + "///timetable.json?";
            var builder = new UriBuilder(u);
            builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route&limit=5";
            var url = builder.ToString();
            return url;
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
    }
}