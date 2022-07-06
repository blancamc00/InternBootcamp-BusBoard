using Microsoft.AspNetCore.Mvc;
using BusBoard.Web.Models;
using BusBoard.Web.ViewModels;

namespace BusBoard.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TransportApi _transportApi = new TransportApi();
    private readonly PostcodeApi _postcodeApi = new PostcodeApi();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<ActionResult> BusInfo(PostcodeSelection selection)
    {
        // Add some properties to the BusInfo view model with the data you want to render on the page.
        // Write code here to populate the view model with info from the APIs.
        // Then modify the view (in Views/Home/BusInfo.cshtml) to render upcoming buses.
        var postcodeResponse = await _postcodeApi.GetLatitudeAndLongitude(selection.Postcode);
        var nearbyStops = await _transportApi.GetNearestStops(postcodeResponse.Latitude, postcodeResponse.Longitude);
        var stopsAndDepartures = new List<DeparturesResponse>();

        foreach (var stop in nearbyStops)
        {
            var departures = await _transportApi.GetDepartures(stop.StopCode);
            stopsAndDepartures.Add(departures);
        }

        var info = new BusInfo(postcodeResponse.Postcode, stopsAndDepartures);
        return View(info);
    }

    public ActionResult About()
    {
        ViewBag.Message = "Information about this site";

        return View();
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Contact us!";

        return View();
    }
}
