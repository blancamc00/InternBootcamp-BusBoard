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

            IList<BusStops> stopList = await BusBoard.Api.Class1.getStopList(postCode);

            //Getting schedule from each stop
            for (int i = 0; i < 2; i++)
            {
                string url = BusBoard.Api.Class1.CreateUrlForBuses(stopList[i].atcocode);
                var input = BusBoard.Api.Class1.AccessUrl(url);

                List<Buses> displayList = BusBoard.Api.Class1.getSchedule(input.Result);
                displayBuses(stopList, displayList, i);
            }
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