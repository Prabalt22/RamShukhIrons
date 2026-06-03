namespace MvcApp.Models;

public class CartItem
{
    public string IronName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice => Qty * Price;
}
