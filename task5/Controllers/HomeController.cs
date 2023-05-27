using System.Diagnostics;
using Application.Common.Contracts.Services;
using Application.Models.Common;
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

        [HttpGet]
        public IActionResult Index() {
            var regions = new List<RegionViewModel> {
                new RegionViewModel {
                    Country = "United States",
                    Locale = "en-US"
                },
                new RegionViewModel {
                    Country = "Russia",
                    Locale = "ru-RU"
                },
                new RegionViewModel {
                    Country = "Poland",
                    Locale = "pl-PL"
                }
            };

            return View(regions);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(string locale, double errors, int seed, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) {
            var users = await _userGenerationService.GetUsers(new GetUsersArgs {
                Locale = locale,
                Page = page,
                PageSize = pageSize,
                Seed = seed
            }, cancellationToken);

            _errorGenerationService.GenerateErrors(new GenerateErrorsArgs {
                Users = users,
                Seed = seed,
                ErrorProbability = errors,
                Locale = locale
            });
            await Task.Delay(500);

            ViewData["Locale"] = locale;
            return PartialView("_Users", users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}