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
            var customerId = Guid.NewGuid();
            var item = new CartLine()
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            };
            _mockCartRepastory!.Setup(x => x.GetCart(customerId)).ReturnsAsync((CartModel?)null);
            _mockCartRepastory.Setup(x => x.CreateCart(customerId)).ReturnsAsync(new CartModel());
            _mockCartRepastory.Setup(x => x.AddToCart(customerId, item)).ReturnsAsync(true);
            
            _mockKafkaProducer!.Setup(x => x.ProduceAsync("cart", It.IsAny<AddToCartMessage>())).ReturnsAsync(new DeliveryResult<Null, string>());
            
            var controller = new CartController(_mockKafkaProducer.Object, _mockCartRepastory.Object);

            // Act
            var result = controller.Post(item, customerId).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            _mockCartRepastory.Verify(x => x.CreateCart(customerId), Times.Once);
            _mockCartRepastory.Verify(x => x.AddToCart(customerId, item), Times.Once);
            _mockKafkaProducer.Verify(x => x.ProduceAsync("cart", It.IsAny<AddToCartMessage>()), Times.Once);
            
        }
    }
}