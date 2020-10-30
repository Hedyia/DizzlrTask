using AutoMapper;
using DizzlrApp.Models;
using DizzlrApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DizzlrApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageController : Controller
    {
        private readonly IMapper _mapper;
        private readonly Data.AppContext _context;

        public ManageController(IMapper mapper, Data.AppContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, string status, int pageNumber = 1)
        {
            int maxRows = 5;

            var fileUploadViewModel = await LoadAllOrders();
            ViewBag.Message = TempData["Message"];
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            switch (sortOrder)
            {
                case "Date":
                    fileUploadViewModel.OrderDetails = fileUploadViewModel.OrderDetails.OrderBy(x => x.OrderDate).ToList();
                    break;
                case "date_desc":
                    fileUploadViewModel.OrderDetails = fileUploadViewModel.OrderDetails.OrderByDescending(x => x.OrderDate).ToList();
                    break;
                default:
                    break;

            }

            switch (status)
            {
                case "All":
                    fileUploadViewModel = await LoadAllOrders();
                    fileUploadViewModel.OrderDetails = fileUploadViewModel
                        .OrderDetails.ToList();
                    break;
                default:
                    if (_context.Statuses.Any(x => x.Name == status))
                        fileUploadViewModel.OrderDetails = fileUploadViewModel.OrderDetails.Where(x => x.Status == status).ToList();
                    break;
            }
            double pageCount = (double)((decimal)fileUploadViewModel.OrderDetails.Count() / Convert.ToDecimal(maxRows));
            fileUploadViewModel.CurrentPageIndex = pageNumber;
            fileUploadViewModel.PageCount = (int)Math.Ceiling(pageCount);

            fileUploadViewModel.OrderDetails = fileUploadViewModel.OrderDetails.Skip((pageNumber - 1) * maxRows)
            .Take(maxRows).ToList();
            return View(fileUploadViewModel);
        }

        [HttpPost]

        public async Task<JsonResult> UpdateStatus(int orderId, string status)
        {
            var statusId = (await _context.Statuses.FirstAsync(s => s.Name == status)).StatusId;

            var orderInDb = await _context.Orders.FirstAsync(o => o.OrderId == orderId);
            orderInDb.StatusId = statusId;
            await _context.SaveChangesAsync();
            return Json(new { message = $"Updated #{orderId} to {status} successfully." });
        }
        private async Task<OrderManagerVM> LoadAllOrders()
        {
            var viewModel = new OrderManagerVM();
            viewModel.OrderDetails = _mapper.Map<List<OrderManagerDetailVM>>(await _context.Orders.Include(x => x.Status).Include(x => x.Files).Include(x => x.User).ToListAsync());
            foreach (var item in viewModel.OrderDetails)
            {
                item.Statuses = new SelectList(_context.Statuses, _context.Statuses.First(x => x.Name == item.Status));
            }
            var statuses = _context.Statuses.ToList();
            statuses.Insert(0, new Status { Name = "" });
            statuses.Add(new Status { Name = "All" });

            viewModel.Statuses = new SelectList(statuses.ToList());
            return viewModel;
        }
    }
}
