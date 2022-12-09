using Cart.API.Model;

namespace Cart.API.Repastory
{
    public interface ICartRepastory
    {
        Task<Boolean> AddToCart(Guid cartId, CartLine itemLine);
        Task<Boolean> RemoveFromCart(Guid cartId, CartLine itemLine);
        Task<CartModel?> GetCart(Guid cartId);
        Task<Boolean> ClearCart(Guid cartId);
        Task<Guid> Create();
    }
}