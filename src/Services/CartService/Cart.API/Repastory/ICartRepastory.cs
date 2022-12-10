using Cart.API.Model;

namespace Cart.API.Repastory
{
    public interface ICartRepository
    {
        Task<Boolean> SetItemInCart(Guid customerId, CartLine itemLine);
        Task<Boolean> RemoveFromCart(Guid customerId, Guid productId);
        Task<CartModel?> GetCart(Guid customerId);
        Task<Boolean> ClearCart(Guid customerId);
        Task<CartModel?> CreateCart(Guid customerId);
    }
}