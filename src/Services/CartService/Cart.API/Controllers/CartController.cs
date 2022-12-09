using Cart.API.Plugin.Kafka;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using System.Text.Json;
using Cart.API.EventModel;


namespace Cart.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly IKafkaDependentProducer<Null, string> _producer;

    public CartController(IKafkaDependentProducer<Null, string> producer)
    {
        _producer = producer;
    }
    

    [HttpGet(Name = "GetCart")]
    public IEnumerable<Cart> Get([FromRoute] Guid cartId)
    {
        return new List<Cart>();
    }
    [HttpPost(Name = "AddToCart")]
    public async Task<IActionResult> Post([FromBody] CartItem item)
    {
        var message = new AddToCartMessage(item);
        await _producer.ProduceAsync("cart", message);
        return Ok();
    }
}
