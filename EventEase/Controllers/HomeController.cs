/* EventEase Elite - Navigation Logic
   Author: Joshua Marc Lourens
   Description: Manages the high-level routing for the dashboard and security protocols.
*/

using System.Diagnostics;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventEase.Controllers
{
    public class HomeController : Controller
    {
        // GET: Dashboard
        public IActionResult Index()
        {
            // Landing page for the administrative command center
            return View();
        }

        // GET: Security Protocols
        public IActionResult Privacy()
        {
            // Specialist documentation regarding data security
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Global error handler for unhandled administrative exceptions
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

/* TECHNICAL REFERENCES
   ---------------------------------------------------
   1. Response Caching: Disabling storage for error views to ensure real-time specialist debug data.
   2. Routing Patterns: Mapping standard MVC routes to the Elite dashboard.
*/