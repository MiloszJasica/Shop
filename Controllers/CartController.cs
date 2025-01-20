using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;
using System.Security.Claims;

[Authorize(Roles = "Client")]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AddToCart(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var userCart = await _context.UserCarts
            .Include(uc => uc.CartItems)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCart == null)
        {
            userCart = new UserCart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };

            _context.UserCarts.Add(userCart);
            await _context.SaveChangesAsync();
        }

        var existingItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            var cartItem = new CartItem
            {
                ProductId = id,
                Product = product,
                Quantity = 1,
                UserId = userId
            };
            userCart.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        TempData["Message"] = $"Added {product.Name} to your card!";

        return RedirectToAction("Index", "Products");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userCart = await _context.UserCarts
            .Include(uc => uc.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCart == null || userCart.CartItems.Count == 0)
        {
            return View(new List<CartItem>());
        }

        return View(userCart.CartItems);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var userCart = await _context.UserCarts
            .Include(uc => uc.CartItems)
            .FirstOrDefaultAsync(uc => uc.UserId == userId);

        if (userCart == null)
        {
            return NotFound();
        }

        var itemToRemove = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
        if (itemToRemove != null)
        {
            userCart.CartItems.Remove(itemToRemove);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Item removed from your cart.";
        }

        return RedirectToAction("Index");
    }

}
