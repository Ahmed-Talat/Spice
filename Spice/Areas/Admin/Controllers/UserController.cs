using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.ApplicationUser.Where(U => U.Id != claim.Value).ToListAsync());
        }

        public async Task<IActionResult> Lock(string Id)
        {
            if (Id == null)
                return NotFound();

            var user = await _db.ApplicationUser.FindAsync(Id);

            if (user == null)
                return NotFound();

            user.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string Id)
        {
            if (Id == null)
                return NotFound();

            var user = await _db.ApplicationUser.FindAsync(Id);

            if (user == null)
                return NotFound();

            user.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
