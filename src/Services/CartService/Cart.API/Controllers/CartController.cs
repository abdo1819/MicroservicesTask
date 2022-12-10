using Cart.API.Plugin.Kafka;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using System.Text.Json;
using Cart.API.EventModel;
using Cart.API.Repastory;
using Cart.API.Model;
using System.Net;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IKafkaDependentProducer<Null, string> _producer;
    private readonly ICartRepository _cartRepastory;

    public CartController(IKafkaDependentProducer<Null, string> producer, ICartRepository cartRepastory)
    {
        _producer = producer;
        _cartRepastory = cartRepastory;
    }
    

    [HttpGet("{customerId}",Name = "GetCart")]
    [ProducesResponseType(typeof(CartModel), (int)HttpStatusCode.OK)]

    public async Task<ActionResult<CartModel>> Get(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            return BadRequest();
        }
        try{
            var cart = await _cartRepastory.GetCart(customerId);
            if (cart == null)
            {
                cart = await _cartRepastory.CreateCart(customerId);
            }
            return Ok(cart);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPost("{customerId}",Name = "AddToCart")]
    public async Task<IActionResult> Post(CartLine item, Guid customerId)
    {
        try
        {
            // check if cart exists or create new one
            var cart = await _cartRepastory.GetCart(customerId);
            if (cart == null)
            {
                cart = await _cartRepastory.CreateCart(customerId);
            }
            // add the item to the cart
            await _cartRepastory.SetItemInCart(customerId, item);
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
