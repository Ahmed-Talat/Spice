using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel()
            {
                Category = await _db.Category
                                .ToListAsync(),

                Coupon = await _db.Coupon
                                .Where(S => S.IsActive == true)
                                .ToListAsync(),

                MenuItem = await _db.MenuItem
                                .Include(S => S.Category)
                                .Include(S => S.SubCategory)
                                .ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims != null)
            {
                var count = _db.shoppingCart.Where(s => s.ApplicationUserId == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }

            return View(indexViewModel);
        }
        //GET - DETAILS

        [Authorize]
        public async Task<IActionResult> Details(int Id)
        {
            var MenuItem = await _db.MenuItem
                                .Include(m => m.Category)
                                .Include(m => m.SubCategory)
                                .FirstOrDefaultAsync(m => m.Id == Id);

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = MenuItem,
                MenuItemId = MenuItem.Id
            };

            return View(shoppingCart);

        }

        //POST - DETAILS

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            cart.Id = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;

            if (ModelState.IsValid)
            {

                var cartFromDb = await _db.shoppingCart
                                        .Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.MenuItemId == cart.MenuItemId)
                                        .FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.shoppingCart.AddAsync(cart);
                }
                else
                {
                    cartFromDb.count += cart.count;
                }

                await _db.SaveChangesAsync();

                var count = await _db.shoppingCart.Where(c => c.ApplicationUserId == cart.ApplicationUserId).CountAsync();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
                return RedirectToAction(nameof(Index));
            }

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = await _db.MenuItem.FindAsync(cart.MenuItemId),
                MenuItemId = cart.MenuItemId
            };
            return View(shoppingCart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
