using System.ComponentModel.DataAnnotations;

namespace Cart.API;

public class CartItem
{
    [Required]
    public String? Name { get; set; }
    [Required,Range(0,int.MaxValue)]
    public decimal Price { get; set; }
}
