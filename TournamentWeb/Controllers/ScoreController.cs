using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TournamentWeb.Controllers
{
    public class ScoreController : Controller
    {
        [Authorize]
        public async Task<IActionResult> Process()
        {
            return View();
        }
    }
}