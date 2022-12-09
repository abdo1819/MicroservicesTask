using System.ComponentModel.DataAnnotations;

namespace Cart.API.Model;

public class CartLine
{
    
    public Guid CartLineId { get; set; }
    public Guid ProductId { get; set; }
    public virtual Model.CartModel? Cart { get; set; }
    public Guid CartId { get; set; }

    [Required,Range(0,int.MaxValue)]
    public decimal Price { get; set; }
    [Required,Range(0,int.MaxValue)]
    public int Quantity { get; set; } = 1;

    public decimal lineCost
    {
        get
        {
            return Price * Quantity;
        }
    }

}
