using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BusBoard.Api;

namespace BusBoard
{
    class Program
    {
        private static readonly ApiClass ApiObj = new ApiClass();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to BusBoard!");
            Console.WriteLine("Enter a valid postcode: ");
            string postCode = Console.ReadLine();

            IList<BusStop> stopList = await ApiObj.getStopList(postCode);

            //Getting schedule from each stop
            for (int i = 0; i < stopList.Count; i++)
            {
                string url = ApiObj.CreateUrlForBuses(stopList[i].atcocode);
                var busInfo = ApiObj.AccessUrl(url);
                List<Bus> displayList = ApiObj.getSchedule(busInfo.Result);
                displayBuses(stopList, displayList, i);
            }
        }
        
        
        public static void displayBuses(IList<BusStop> stopList, List<Bus> displayList, int i)
        {
            Console.WriteLine("The next five buses at the " + stopList[i].name + " stop are: ");
            Console.WriteLine("=============================================================================");
            Console.WriteLine("{0,-15} {1,-20} {2,-10}\n", "Line", "Direction", "Aimed departure time");

            for (int j = 0; j < 5 & j < displayList.Count; j++)
            {
                Console.WriteLine("{0,-15} {1,-20} {2,-10}", displayList[j].line, displayList[j].direction, displayList[j].aimed_departure_time);
            }
                
            Console.WriteLine("");
        }
    }
}