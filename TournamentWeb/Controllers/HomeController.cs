using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TournamentWeb.Services;

namespace TournamentWeb.Controllers
{
    [Authorize(Policy = "JanOnly")]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["Status"] = "Choose action";
            return View();
        }

        public async Task<IActionResult> Download()
        {
            FakeConsole.Init();
            const string basePath = "Leagues";

            using (var dropboxClient = new DropboxClient(_configuration["Dropbox:AccessToken"]))
            {
                if (!Directory.Exists(basePath))
                    CreateDirectory(basePath);

                var list = await dropboxClient.Files.ListFolderAsync(string.Empty);

                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    await CreateFile(dropboxClient, item, basePath);
                }

                foreach (var folder in list.Entries.Where(i => i.IsFolder))
                {
                    if (!Directory.Exists($"{basePath}{folder.PathDisplay}"))
                        CreateDirectory($"{basePath}{folder.PathDisplay}");

                    var folderFiles = await dropboxClient.Files.ListFolderAsync(folder.PathDisplay);

                    foreach (var subFolder in folderFiles.Entries.Where(i => i.IsFolder))
                    {
                        if (!Directory.Exists($"{basePath}{subFolder.PathDisplay}"))
                            CreateDirectory($"{basePath}{subFolder.PathDisplay}");

                        var subFolderFiles = await dropboxClient.Files.ListFolderAsync(subFolder.PathDisplay);

                        foreach (var item in subFolderFiles.Entries.Where(i => i.IsFile))
                        {
                            if (!System.IO.File.Exists($"{basePath}{item.PathDisplay}"))
                            {
                                await CreateFile(dropboxClient, item, basePath);
                            }
                        }
                    }
                }
            }

            ViewData["Status"] = "All files synced from Dropbox";
            ViewData["Log"] = FakeConsole.Print();
            return View("Index");
        }

        private static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
            FakeConsole.WriteLine($"Created directory {path}");
        }

        private static async Task CreateFile(DropboxClient dropboxClient, Metadata item, string basePath)
        {
            var response = await dropboxClient.Files.DownloadAsync(item.PathLower);
            using (var content = await response.GetContentAsStreamAsync().ConfigureAwait(false))
            using (var fileStream = System.IO.File.Create($"{basePath}{item.PathDisplay}"))
            {
                await content.CopyToAsync(fileStream);
                FakeConsole.WriteLine($"Created file {basePath}{item.PathDisplay}");
            }
        }

        public async Task<IActionResult> Login()
        {
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            return Redirect("Index");
        }
    }
}