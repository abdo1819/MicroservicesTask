using System.Text.Json;
using Cart.API;
using Cart.API.Model;
using Confluent.Kafka;


namespace Cart.API.EventModel;
public class AddToCartMessage:Message<Null,string>{
    public AddToCartMessage(CartLine item){
        Value = JsonSerializer.Serialize(item);
    }

}
