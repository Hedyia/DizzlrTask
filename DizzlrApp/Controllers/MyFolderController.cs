using DizzlrApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using File = DizzlrApp.Models.File;

namespace DizzlrApp.Controllers
{
    [Authorize]
    public class MyFolderController : Controller
    {
        private readonly Data.AppContext _context;
        private IWebHostEnvironment _environment;


        public MyFolderController(IWebHostEnvironment environment, Data.AppContext context)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int maxRows = 5;

            var fileUploadViewModel = await LoadAllFiles();
            ViewBag.Message = TempData["Message"];
            double pageCount = (double)((decimal)fileUploadViewModel.FilesOnFileSystem.Count() / Convert.ToDecimal(maxRows));
            fileUploadViewModel.CurrentPageIndex = pageNumber;
            fileUploadViewModel.PageCount = (int)Math.Ceiling(pageCount);

            fileUploadViewModel.FilesOnFileSystem = fileUploadViewModel.FilesOnFileSystem.Skip((pageNumber - 1) * maxRows)
            .Take(maxRows).ToList();

            return View(fileUploadViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UploadToFileSystem(List<IFormFile> files, string description)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string wwwPath = _environment.WebRootPath;
            string contentPath = _environment.ContentRootPath;

            string path = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            List<string> uploadedFiles = new List<string>();
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                    uploadedFiles.Add(fileName);
                }

                var fileModel = new File
                {
                    Name = fileName,
                    Extension = extension,
                    CreatedOn = DateTime.UtcNow,
                    FileType = file.ContentType,
                    Description = description,
                    UserId = userId,
                };
                await _context.FilesOnFileSystem.AddAsync(fileModel);
                await _context.SaveChangesAsync();
            }
            TempData["Message"] = "File successfully uploaded to File System.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DownloadFileFromFileSystem(int id)
        {
            string path = Path.Combine(_environment.WebRootPath, "Uploads");
            var file = await _context.FilesOnFileSystem.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            var memory = new MemoryStream();
            using (var stream = new FileStream(Path.Combine(path, file.Name), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.FileType, file.Name + file.Extension);
        }
        public async Task<IActionResult> DeleteFileFromFileSystem(int id)
        {
            string path = Path.Combine(_environment.WebRootPath, "Uploads");
            var file = await _context.FilesOnFileSystem.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            if (System.IO.File.Exists(Path.Combine(path, file.Name)))
            {
                System.IO.File.Delete(Path.Combine(path, file.Name));
            }
            _context.FilesOnFileSystem.Remove(file);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Removed {file.Name + file.Extension} successfully from File System.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> SubmitSelectedFiles(IFormCollection formCollection)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<int> files = Regex.Matches(formCollection["ID"], @"\d+").Cast<Match>().Select(m => int.Parse(m.Value)).ToList();
            var orderFileItems = new List<OrderFileItem>();

            var order = new Order
            {
            OrderDate = DateTime.UtcNow,
                StatusId = 1,
                UserId = userId
            };
            foreach (var file in files)
            {
                orderFileItems.Add(new OrderFileItem
                {
                    FileId = file,
                    OrderId = order.OrderId
                });
            }
            order.Files = orderFileItems;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Your Selected Files Submited ";

            return RedirectToAction("Index");
        }

        private async Task<FileUpload> LoadAllFiles()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = new FileUpload();
            viewModel.FilesOnFileSystem = await _context.FilesOnFileSystem.Where(u => u.UserId == userId).ToListAsync();
            return viewModel;
        }
    }
}
