using CanvasLMS.Attributes;
using CanvasLMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CanvasLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [RedirectLogged]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [RequireSession]
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult NotAuthorized() 
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
