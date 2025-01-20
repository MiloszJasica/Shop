using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using Projekt.Services;

namespace Projekt.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductsController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        public IActionResult Index(string categoryFilter)
        {
            var categories = _categoryService.GetCategories();
            ViewBag.Categories = categories;

            var products = string.IsNullOrEmpty(categoryFilter)
                ? _productService.GetAllProducts()
                : _productService.GetAllProducts().Where(p => p.Category == categoryFilter).ToList();

            ViewBag.CategoryFilter = categoryFilter;
            return View(products);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            var categories = _categoryService.GetCategories();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Price,Description,Category")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productService.AddProduct(product);
                TempData["SuccessMessage"] = "Produkt został pomyślnie dodany.";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var categories = _categoryService.GetCategories();
            ViewBag.Categories = categories;
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Description,Category")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _productService.UpdateProduct(product);
                    TempData["SuccessMessage"] = $"Product '{product.Name}' updated successfully!";
                }
                catch
                {
                    if (!_productService.ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productService.GetProductById(id);
            if (product != null)
            {
                _productService.DeleteProduct(id);
                TempData["SuccessMessage"] = $"Product '{product.Name}' deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
