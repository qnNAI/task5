using System.Diagnostics;
using Application.Common.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using task5.Models;

namespace task5.Controllers {
    public class HomeController : Controller {
        private readonly IUserGenerationService _userGenerationService;
        private readonly IErrorGenerationService _errorGenerationService;

        public HomeController(IUserGenerationService userGenerationService, IErrorGenerationService errorGenerationService) {
            _userGenerationService = userGenerationService;
            _errorGenerationService = errorGenerationService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken) {
            var users = await _userGenerationService.GetUsers(new Application.Models.Common.GetUsersArgs {
                Locale = "pl-PL",
                Page = 1,
                PageSize = 20,
                Seed = 69420
            }, cancellationToken);


            _errorGenerationService.GenerateErrors(users, 69420, 10.5, "pl-PL");


            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}