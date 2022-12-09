using Cart.API.EventModel;
using Cart.API.Controllers;
using Cart.API.Model;
using Cart.API.Plugin.Kafka;
using Cart.API.Repastory;
using Confluent.Kafka;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace Cart.UnitTest
{
    [TestClass]
    public class CartControllerTest
    {
        private static Mock<ICartRepastory>? _mockCartRepastory;
        private static Mock<IKafkaDependentProducer<Null, string>>? _mockKafkaProducer;

        [ClassInitialize]
        public static void init(TestContext context)
        {
            _mockCartRepastory = new Mock<ICartRepastory>();
            _mockKafkaProducer = new Mock<IKafkaDependentProducer<Null, string>>();
            
        }

        [TestMethod]
        public void PostAddToCart_newCartAndValidItem_createCartAddItemOK()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var item = new CartLine()
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            };
            _mockCartRepastory!.Setup(x => x.GetCart(cartId)).ReturnsAsync((CartModel?)null);
            _mockCartRepastory.Setup(x => x.Create()).ReturnsAsync(cartId);
            _mockCartRepastory.Setup(x => x.AddToCart(cartId, item)).ReturnsAsync(true);
            
            _mockKafkaProducer!.Setup(x => x.ProduceAsync("cart", It.IsAny<AddToCartMessage>())).ReturnsAsync(new DeliveryResult<Null, string>());
            
            var controller = new CartController(_mockKafkaProducer.Object, _mockCartRepastory.Object);

            // Act
            var result = controller.Post(item, cartId).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            _mockCartRepastory.Verify(x => x.Create(), Times.Once);
            _mockCartRepastory.Verify(x => x.AddToCart(cartId, item), Times.Once);
            _mockKafkaProducer.Verify(x => x.ProduceAsync("cart", It.IsAny<AddToCartMessage>()), Times.Once);
            
        }
    }
}