using System.Diagnostics;
using Application.Common.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using task5.Models;

namespace task5.Controllers {
    public class HomeController : Controller {
        private readonly IUserGenerationService _userGenerationService;

        public HomeController(IUserGenerationService userGenerationService) {
            _userGenerationService = userGenerationService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken) {
            var users = await _userGenerationService.GetUsers(new Application.Models.Common.GetUsersArgs {
                Locale = "en-US",
                Page = 1,
                PageSize = 20,
                Seed = 69420
            }, cancellationToken);
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}