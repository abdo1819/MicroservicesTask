using System.ComponentModel.DataAnnotations;

namespace Cart.API;

public class Cart
{
    [Required]
    public ICollection<CartItem>? Items { get; set; }
    public decimal totalPrice
    {
        get
        {
            decimal total = 0;
            if (Items == null || Items.Count == 0) return 0;
            
            foreach (var item in Items)
            {
                total += item.Price;
            }
            return total;
        }
    }
}
