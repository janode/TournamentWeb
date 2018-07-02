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

namespace TournamentWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Download()
        {
            const string basePath = "Leagues";

            using (var dropboxClient = new DropboxClient(_configuration["Dropbox:AccessToken"]))
            {
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                var list = await dropboxClient.Files.ListFolderAsync(string.Empty);

                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    await CreateFile(dropboxClient, item, basePath);
                }

                foreach (var folder in list.Entries.Where(i => i.IsFolder))
                {
                    if (!Directory.Exists($"{basePath}{folder.PathDisplay}"))
                        Directory.CreateDirectory($"{basePath}{folder.PathDisplay}");

                    var folderFiles = await dropboxClient.Files.ListFolderAsync(folder.PathLower);

                    foreach (var subFolder in folderFiles.Entries.Where(i => i.IsFolder))
                    {
                        if (!Directory.Exists($"{basePath}{subFolder.PathDisplay}"))
                            Directory.CreateDirectory($"{basePath}{subFolder.PathDisplay}");

                        var subFolderFiles = await dropboxClient.Files.ListFolderAsync(subFolder.PathLower);

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

            return View();
        }

        private static async Task CreateFile(DropboxClient dropboxClient, Metadata item, string basePath)
        {
            var response = await dropboxClient.Files.DownloadAsync(item.PathLower);
            using (var content = await response.GetContentAsStreamAsync().ConfigureAwait(false))
            using (var fileStream = System.IO.File.Create($"{basePath}{item.PathDisplay}"))
            {
                await content.CopyToAsync(fileStream);
            }
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