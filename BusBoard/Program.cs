using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusBoard
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //0500CCITY436
            
            Console.WriteLine("Welcome to BusBoard!");
            
            Console.WriteLine("Write stop code: ");
            string stopCode = Console.ReadLine();
            var u = "https://transportapi.com/v3/uk/bus/stop/"+ stopCode + "///timetable.json?";
            using var client = new HttpClient();

            var builder = new UriBuilder(u);
            builder.Query = "app_id=7d0ebcfd&app_key=bb3b6e2b09788d7ca770cd28ba1156bf&group=route&limit=5";
            var url = builder.ToString();
            var input =
                await client.GetStringAsync(url);
            
            //Console.WriteLine(result);
            
            
            JObject jOresult = JObject.Parse(input);

            IList<JToken> data = jOresult["departures"]["905"].Children().ToList();
            
            IList<buses> busList = new List<buses>();

            foreach (JToken bus in data)
            {
                buses tempBus = bus.ToObject<buses>();
                busList.Add(tempBus);
            }
            
            Console.WriteLine("The next five buses are: ");

            for (int i = 0; i < busList.Count; i++)
            {
                Console.WriteLine(busList[i].line + " " + busList[i].direction + " " + busList[i].aimed_departure_time);
            }

        }
    }

    public class buses
    {
        public string aimed_departure_time { get; set; }
        public string line { get; set; }
        public string direction { get; set; }
    }
}
