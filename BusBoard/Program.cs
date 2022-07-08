using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BusBoard.Api;

namespace BusBoard
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to BusBoard!");

            //Convert Postcode to Latitude and Longitude
            Console.WriteLine("Enter a valid postcode: ");
            string postCode = Console.ReadLine();

            IList<BusStops> stopList = new List<BusStops>();
            Task t = new Task(BusBoard.Api.Class1.getStopList(postCode));
            await BusBoard.Api.Class1.getStopList(postCode, stopList);

            //Getting schedule from each stop
            for (int i = 0; i < 2; i++)
            {
                string url = CreateUrlForBuses(stopList[0].atcocode);
                using var client = new HttpClient();
                var input =
                    await client.GetStringAsync(url);

                List<Buses> displayList = getSchedule(input);
                displayBuses(stopList, displayList, i);
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
        
        static List<Buses> getSchedule(string input)
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