using Cart.API.Model;

namespace Cart.API.Repastory
{
    public interface ICartRepastory
    {
        Task<Boolean> AddToCart(Guid customerId, CartLine itemLine);
        Task<Boolean> RemoveFromCart(Guid customerId, CartLine itemLine);
        Task<CartModel?> GetCart(Guid customerId);
        Task<Boolean> ClearCart(Guid customerId);
        Task<CartModel> CreateCart(Guid customerId);
    }
}