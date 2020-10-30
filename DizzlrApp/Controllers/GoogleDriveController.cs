using DizzlrApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DizzlrApp.Controllers
{
    [Authorize]
    public class GoogleDriveController : Controller
    {

        GoogleDriveAPIHelper _googleDrive;
        public GoogleDriveController(IWebHostEnvironment environment, Data.AppContext context)
        {
            _googleDrive = new GoogleDriveAPIHelper(environment, context);
        }

        [Route("/authorize")]
        [Route("GoogleDrive")]
        [HttpGet]
        public ActionResult Index()
        {
            return View(_googleDrive.GetDriveFiles());
        }

        public async Task<IActionResult> AddToMyFolder(string fileId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _googleDrive.DownloadGoogleFile(fileId, userId);

            return RedirectToAction("Index");

        }

    }
}
