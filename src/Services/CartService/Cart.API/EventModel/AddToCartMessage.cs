using System.Text.Json;
using Cart.API;
using Confluent.Kafka;


namespace Cart.API.EventModel;
class AddToCartMessage:Message<Null,string>{
    public AddToCartMessage(CartItem item){
        Value = JsonSerializer.Serialize(item);
    }

}
