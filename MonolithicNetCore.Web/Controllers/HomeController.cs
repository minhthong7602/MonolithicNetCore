using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using MonolithicNetCore.Models;
using MonolithicNetCore.Web.SignalHub;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MonolithicNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<HubServer> _hubServer;
        private readonly IHubContext<LogServer> _hubLog;
        public HomeController(ILogger<HomeController> logger, IHubContext<HubServer> hubServer, IHubContext<LogServer> hubLog)
        {
            _logger = logger;
            _hubServer = hubServer;
            _hubLog = hubLog;
        }

        public async Task<IActionResult> Index()
        {
            await _hubLog.Clients.Group("AdminLog").SendAsync("AdminLog", "Go to home page");
            return View();
        }

        public IActionResult Privacy()
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
