using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using System.Text.Json;

namespace MvcApp.Controllers;

public class CartController : Controller
{

    private readonly ILogger<CartController> _logger;
    public CartController(ILogger<CartController> logger)
    {
        _logger = logger;
    }


    private const string SessionKey = "ShoppingCart";

    // Action 2: View the Shop/Cart section page
    public IActionResult Index()
    {
        var cartList = GetCartFromSession();
        return View("Index",cartList);
    }


    // Action 1: Add item to session cart
    [HttpPost]
    public IActionResult CheckOut(string IronName, decimal Qty, decimal Price)
    {
        if (Qty <= 0) return BadRequest("Invalid quantity");

        // Retrieve existing cart list from session or create a new one
        var cartList = GetCartFromSession();

        // Check if product already exists in cart; if so, update quantity
        var existingItem = cartList.FirstOrDefault(x => x.IronName == IronName);
        if (existingItem != null)
        {
            existingItem.Qty += Qty;
        }
        else
        {
            cartList.Add(new CartItem { IronName = IronName, Qty = Qty, Price = Price });
        }

        // Save updated list back to session
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(cartList));

        // Redirect to the shop/cart view page to see items
        return RedirectToAction("Index");
    }

    
    // Helper method to deserialize session data safely
    private List<CartItem> GetCartFromSession()
    {
        var sessionData = HttpContext.Session.GetString(SessionKey);
        return string.IsNullOrEmpty(sessionData)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();
    }

}