using AutoMapper;
using DizzlrApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DizzlrApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private Data.AppContext _context;
        private IMapper _mapper;

        public OrdersController(IMapper mapper, Data.AppContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var fileUploadViewModel = await LoadAllOrders();
            ViewBag.Message = TempData["Message"];
            return View(fileUploadViewModel);
        }

        private async Task<OrderVM> LoadAllOrders()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = new OrderVM();
            viewModel.OrderDetails =_mapper.Map<List<OrderDetailVM>>(await _context.Orders.Where(u => u.UserId == userId).Include(x=> x.Status).Include(x => x.Files).ToListAsync());
            return viewModel;
        }
    }
}
