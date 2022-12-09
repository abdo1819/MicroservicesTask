using Cart.API.Plugin.Kafka;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using System.Text.Json;
using Cart.API.EventModel;
using Cart.API.Repastory;
using Cart.API.Model;

namespace Cart.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly IKafkaDependentProducer<Null, string> _producer;
    private readonly ICartRepastory _cartRepastory;

    public CartController(IKafkaDependentProducer<Null, string> producer, ICartRepastory cartRepastory)
    {
        _producer = producer;
        _cartRepastory = cartRepastory;
    }
    

    [HttpGet(Name = "GetCart")]
    public IEnumerable<CartModel> Get([FromRoute] Guid cartId)
    {
        return new List<CartModel>();
    }
    [HttpPost(Name = "AddToCart")]
    public async Task<IActionResult> Post([FromBody] CartLine item, [FromRoute] Guid cartId)
    {
        try
        {
            // check if cart exists or create new one
            var cart = await _cartRepastory.GetCart(cartId);
            if (cart == null)
            {
                cartId = await _cartRepastory.Create();
            }
            // add the item to the cart
            await _cartRepastory.AddToCart(cartId, item);
            // publish the event of item addition
            var message = new AddToCartMessage(item);
            // TODO : make topic name configurable
            await _producer.ProduceAsync("cart", message);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
