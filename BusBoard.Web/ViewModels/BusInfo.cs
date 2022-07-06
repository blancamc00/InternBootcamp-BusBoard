namespace BusBoard.Web.ViewModels
{
    public class BusInfo
    {
        public BusInfo(string postCode, List<DeparturesResponse> stopsAndDepartures)
        {
            PostCode = postCode;
            StopsAndDepartures = stopsAndDepartures;
        }

        public string PostCode { get; set; }
        public List<DeparturesResponse> StopsAndDepartures { get; set; }
    }
}
