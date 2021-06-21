using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }
        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                byte[] Picture = null;
                using (var fileStream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        Picture = memoryStream.ToArray();
                    }
                }
                coupon.Picture = Picture;
            }

            _db.Coupon.Add(coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
                return NotFound();

            var coupon = await _db.Coupon.FindAsync(Id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var couponInDb = await _db.Coupon.FindAsync(Id);
            if (couponInDb == null)
                return NotFound();

            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                byte[] Picture = null;
                using (var fileStream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        Picture = memoryStream.ToArray();
                    }
                }
                couponInDb.Picture = Picture;
            }
            couponInDb.Name = coupon.Name;
            couponInDb.CouponType = coupon.CouponType;
            couponInDb.Discount = coupon.Discount;
            couponInDb.IsActive = coupon.IsActive;
            couponInDb.MinimumAmount = coupon.MinimumAmount;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
                return NotFound();

            var coupon = await _db.Coupon.FindAsync(Id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }


        //GET - DELETE
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return NotFound();

            var coupon = await _db.Coupon.FindAsync(Id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var couponInDb = await _db.Coupon.FindAsync(Id);
            if (couponInDb == null)
                return NotFound();

            _db.Coupon.Remove(couponInDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
