using Cart.API.EventModel;
using Cart.API.Controllers;
using Cart.API.Model;
using Cart.API.Plugin.Kafka;
using Cart.API.Repastory;
using Confluent.Kafka;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Cart.UnitTest
{
    [TestClass]
    public class CartControllerTest
    {
        private readonly Mock<ICartRepository>? _mockCartRepastory;
        private readonly Mock<IKafkaDependentProducer<Null, string>>? _mockKafkaProducer;
        private readonly Mock<HttpContext> _contextMock;

        public CartControllerTest()
        {
        _mockCartRepastory = new Mock<ICartRepository>();
        _mockKafkaProducer = new Mock<IKafkaDependentProducer<Null, string>>();
        _contextMock = new Mock<HttpContext>();
        }

        [ClassInitialize]
        public static void init(TestContext context)
        {
            
            
        }
        [TestMethod]
        public async Task GetCustomerCart_CreateNewCartInotexist_OKCart()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mockCartRepastory!.Setup(x => x.GetCart(customerId)).ReturnsAsync((CartModel?)null);
            _mockCartRepastory.Setup(x => x.CreateCart(customerId)).ReturnsAsync(new CartModel() { CustomerId = customerId });
            
            var controller = new CartController(_mockKafkaProducer!.Object, _mockCartRepastory.Object);
            controller.ControllerContext.HttpContext = _contextMock.Object;
            // Act
            var actionResult = await controller.Get(customerId) ;

            // Assert            
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, 200);
            Assert.IsInstanceOfType(result.Value, typeof(CartModel));
            var cart = result.Value as CartModel;
            Assert.IsNotNull(cart);
            Assert.AreEqual(cart.CustomerId, customerId);
            
            _mockCartRepastory.Verify(x => x.GetCart(customerId), Times.Once);
            _mockCartRepastory.Verify(x => x.CreateCart(customerId), Times.Once);            
        }
        [TestMethod]
        public void PostAddToCart_newCartAndValidItem_createCartAddItemOK()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var item = new CartLine()
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            };
            _mockCartRepastory!.Setup(x => x.GetCart(customerId)).ReturnsAsync((CartModel?)null);
            _mockCartRepastory.Setup(x => x.CreateCart(customerId)).ReturnsAsync(new CartModel() { CustomerId = customerId });
            _mockCartRepastory.Setup(x => x.SetItemInCart(customerId, item)).ReturnsAsync(true);
            
            _mockKafkaProducer!.Setup(x => x.ProduceAsync("cart", It.IsAny<AddToCartMessage>())).ReturnsAsync(new DeliveryResult<Null, string>());
            
            var controller = new CartController(_mockKafkaProducer.Object, _mockCartRepastory.Object);

            // Act
            var result = controller.Post(item, customerId).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            _mockCartRepastory.Verify(x => x.CreateCart(customerId), Times.Once);
            _mockCartRepastory.Verify(x => x.SetItemInCart(customerId, item), Times.Once);
            _mockKafkaProducer.Verify(x => x.ProduceAsync("cart", It.IsAny<AddToCartMessage>()), Times.Once);
            
        }
    }
}