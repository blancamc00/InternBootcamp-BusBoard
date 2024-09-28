using BusBoard.Api;
using Microsoft.AspNetCore.Mvc;
using BusBoard.Web.Models;
using BusBoard.Web.ViewModels;

namespace BusBoard.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiClass _apiclass = new ApiClass();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public ActionResult BusInfo(PostcodeSelection selection)
    {
        var stopList = _apiclass.getStopList(selection.Postcode).Result;
        List<List<Bus>> displayList = new List<List<Bus>>();
        for (int i = 0; i < stopList.Count; i++)
        {
            string url = _apiclass.CreateUrlForBuses(stopList[i].atcocode);
            var busInfo = _apiclass.AccessUrl(url);
            List<Bus> busList = _apiclass.getSchedule(busInfo.Result);
            displayList.Add(busList);
        }
        
        // Add some properties to the BusInfo view model with the data you want to render on the page.
        // Write code here to populate the view model with info from the APIs.
        // Then modify the view (in Views/Home/BusInfo.cshtml) to render upcoming buses.
        var info = new BusInfo(selection.Postcode, stopList, displayList);
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
