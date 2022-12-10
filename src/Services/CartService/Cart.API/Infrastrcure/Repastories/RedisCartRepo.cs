

using StackExchange.Redis;
using System.Text.Json;
using Cart.API.Model;
using Cart.API.Repastory;


namespace Cart.API.Infrastructure.Repastories;


public class RedisCartRepo : ICartRepository
{
    private readonly ILogger<RedisCartRepo> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisCartRepo(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
    {
        _logger = loggerFactory.CreateLogger<RedisCartRepo>();
        _redis = redis;
        _database = redis.GetDatabase();
    }

    public async Task<bool> ClearCart(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }

    public async Task<CartModel?> GetCart(Guid customerId)
    {
        var data = await _database.StringGetAsync(customerId.ToString());

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<CartModel>(data!, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public async Task<CartModel?> CreateCart(Guid customerId)
    {
        var cart = new CartModel
        {
            CustomerId = customerId,
            Items = new List<CartLine>()
        };
        // TODO move constants to config
        const int MAX_STORING_DAYS = 30;
        var created = await _database.StringSetAsync(customerId.ToString(), 
                                                    JsonSerializer.Serialize(cart), 
                                                    TimeSpan.FromDays(MAX_STORING_DAYS));

        if (!created)
        {
            _logger.LogError($"Problem occured during the creation of the cart for the customer with id: {customerId}", customerId);
            return null;
        }

        return await GetCart(customerId);
    }

    public async Task<bool> SetItemInCart(Guid customerId, CartLine itemLine)
    {
        var cart = await GetCart(customerId) ?? await CreateCart(customerId);
        if (cart == null)
        {
            _logger.LogError($"Problem occured during adding item to cart for the customer with id: {customerId}", customerId);
            return false;
        }

        var index = cart.Items?.ToList().FindIndex(p => p.ProductId == itemLine.ProductId);

        if (cart.Items == null) cart.Items = new List<CartLine>();
        if (index >= 0)
        {
            cart.Items[index.Value].Quantity = itemLine.Quantity;
        }
        else
        {
            cart.Items.Add(itemLine);
        }

        var created = await _database.StringSetAsync(customerId.ToString(), JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        _logger.LogInformation($"Item {itemLine.ProductId} added to the cart for the customer with id: {customerId}", customerId);
        return created;
    }

    public async Task<bool> RemoveFromCart(Guid customerId, Guid productId)
    {
        var cart = await GetCart(customerId);
        if (cart == null)
        {
            _logger.LogError($"Problem occured during removing item from cart for the customer with id: {customerId} cart doesn't exist", customerId);
            return false;
        }

        var index = cart.Items?.ToList().FindIndex(p => p.ProductId == productId);

        if (index >= 0)
        {
            cart.Items?.RemoveAt(index.Value);
        }
        else{
            _logger.LogError($"Problem occured during removing item from cart for the customer with id: {customerId} item doesn't exist", customerId);
            return false;
        }

        var created = await _database.StringSetAsync(customerId.ToString(), JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        _logger.LogInformation($"Item {productId} removed from the cart for the customer with id: {customerId}", customerId);
        return created;
    }
    
    public async Task<bool> DeleteCart(Guid customerId)
    {
        return await _database.KeyDeleteAsync(customerId.ToString());
    }
    public async Task<bool> ClearCart(Guid customerId)
    {
        var cart = await GetCart(customerId);
        if (cart == null)
        {
            _logger.LogError($"customer already doesn't have cart", customerId);
            return false;
        }
        cart.Items = new List<CartLine>();
        var created = await _database.StringSetAsync(customerId.ToString(), JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        _logger.LogInformation($"Cart for the customer with id: {customerId} cleared", customerId);
        return created;
    }

    
    private IServer GetServer()
    {
        var endpoint = _redis.GetEndPoints();
        return _redis.GetServer(endpoint.First());
    }

    
}
