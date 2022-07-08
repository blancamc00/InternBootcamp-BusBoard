using BusBoard.Api;
namespace BusBoard.Web.ViewModels
{
    public class BusInfo
    {
        public BusInfo(string postCode, IList<BusStop> stopList, List<List<Bus>> displayList)
        {
            PostCode = postCode;
            StopList = stopList;
            DisplayList = displayList;
        }

        public string PostCode { get; set; }
        public IList<BusStop> StopList { get; set; }
        
        public List<List<Bus>> DisplayList { get; set; }
    }
}
