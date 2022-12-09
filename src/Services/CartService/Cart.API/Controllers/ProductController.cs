using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{


    public ProductController()
    {
        
    }

    [HttpGet(Name = "GetCart")]
    public IEnumerable<Cart> Get([FromRoute] Guid cartId)
    {
        return new List<Cart>();
    }
}
