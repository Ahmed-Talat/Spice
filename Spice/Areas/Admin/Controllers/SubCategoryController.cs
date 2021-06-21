using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET
        public async Task<IActionResult> Index()
        {
            return View(await _db.SubCategory.Include(S => S.Category).ToListAsync());
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(S => S.Name).Select(S => S.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var AlreadyExists = _db.SubCategory.Include(S => S.Category)
                    .Where(S => S.Name == model.SubCategory.Name && S.Category.Id == model.SubCategory.CategoryId);

                if (AlreadyExists.Count() > 0)
                {
                    StatusMessage = "Error : SubCategory already exists under " + AlreadyExists.First().Category.Name + " category, Please use another name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(S => S.Name).Select(S => S.Name).Distinct().ToListAsync(),
                ErrorMessage = StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int Id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == Id
                                   select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
                return NotFound();

            var subCategory = await _db.SubCategory.FindAsync(Id);

            if (subCategory == null)
                return NotFound();

            CategoryAndSubCategoryViewModel model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.Select(S => S.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, CategoryAndSubCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                var AlreadyExists = await _db.SubCategory.Include(S => S.Category)
                    .Where(S => S.Name == model.SubCategory.Name && S.CategoryId == model.SubCategory.CategoryId).ToListAsync();
                if (AlreadyExists.Count() > 0)
                {
                    StatusMessage = "Error : SubCategory already exists under " + AlreadyExists.First().Category.Name + " category, Please use another name.";
                }
                else
                {
                    var subCategory = await _db.SubCategory.FindAsync(Id);

                    if (subCategory == null)
                        return NotFound();

                    subCategory.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubCategoryViewModel modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.Select(S => S.Name).Distinct().ToListAsync(),
                ErrorMessage = StatusMessage
            };
            return View(modelVM);

        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int Id)
        {
            var subCategory = await _db.SubCategory.Include(S => S.Category).SingleOrDefaultAsync(S => S.Id == Id);
            return View(subCategory);

        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return NotFound();

            var subCategory = await _db.SubCategory.Include(S => S.Category).SingleOrDefaultAsync(S => S.Id == Id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);

        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var subCategory = await _db.SubCategory.FindAsync(Id);
            if (subCategory == null)
                return NotFound();

            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
