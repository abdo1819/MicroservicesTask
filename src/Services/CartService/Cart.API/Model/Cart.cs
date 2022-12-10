using System.ComponentModel.DataAnnotations;

namespace Cart.API.Model;

public class CartModel
{
    [Key]
    public Guid customerId { get; set; }
    [Required]
    public ICollection<CartLine>? Items { get; set; }
    public decimal totalPrice
    {
        get
        {
            decimal total = 0;
            if (Items == null || Items.Count == 0) return 0;
            
            foreach (var item in Items)
            {
                total += item.lineCost;
            }
            return total;
        }
    }
}
