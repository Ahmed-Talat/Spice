using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
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
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category.ToList(),
                MenuItem = new MenuItem()
            };
        }

        //GET
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST - CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _db.SubCategory.Where(S => S.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count() > 0)
            {
                var Uploads = Path.Combine(webRootPath, "images");
                var Extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(Uploads, MenuItemVM.MenuItem.Id + Extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + Extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaulFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
                return NotFound();
            
            MenuItemVM.MenuItem = await _db.MenuItem.Include(M => M.Category).Include(M => M.SubCategory).SingleOrDefaultAsync(M => M.Id == Id);
            if (MenuItemVM == null)
                return NotFound();

            MenuItemVM.SubCategory = await _db.SubCategory.Where(M => M.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);

        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id)
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _db.SubCategory.Where(S => S.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            
            if (files.Count() > 0)
            {
                // new image has been uploaded
                var Uploads = Path.Combine(webRootPath, "images");
                var Extension_new = Path.GetExtension(files[0].FileName);

                // delete old image
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(Uploads, MenuItemVM.MenuItem.Id + Extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + Extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
            menuItemFromDb.Spiciness = MenuItemVM.MenuItem.Spiciness;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int Id)
        {
            MenuItemVM.MenuItem = await _db.MenuItem.Include(M => M.Category).Include(M => M.SubCategory).SingleOrDefaultAsync(M => M.Id == Id);
            if (MenuItemVM == null)
                return NotFound();

            MenuItemVM.SubCategory = await _db.SubCategory.Where(M => M.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);

        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return NotFound();

            MenuItemVM.MenuItem = await _db.MenuItem.Include(M => M.Category).Include(M => M.SubCategory).SingleOrDefaultAsync(M => M.Id == Id);
            if (MenuItemVM == null)
                return NotFound();

            MenuItemVM.SubCategory = await _db.SubCategory.Where(M => M.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);

        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            //delete image
            var menuItemFromDb = await _db.MenuItem.FindAsync(Id);
            string webRootPath = _hostingEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _db.MenuItem.Remove(menuItemFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
