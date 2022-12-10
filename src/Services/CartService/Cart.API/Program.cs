using Cart.API;
using Cart.API.Infrastructure.Repastories;
using Cart.API.Plugin.Kafka;
using Cart.API.Repastory;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IKafkaClientHandle, KafkaClientHandle>();
builder.Services.AddSingleton<IKafkaDependentProducer<Null,string>, KafkaDependentProducer<Null,string>>();

//By connecting here we are making sure that our service
//cannot start until redis is ready. This might slow down startup,
//but given that there is a delay on resolving the ip address
//and then creating the connection it seems reasonable to move
//that cost to startup instead of having the first request pay the
//penalty.
builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    var connestionString = builder.Configuration.GetConnectionString("RedisConnectionString");
    if (string.IsNullOrEmpty(connestionString))
    {
        throw new ArgumentException("Redis connection string is empty");
    }
    var configuration = ConfigurationOptions.Parse(connestionString, true);

    return ConnectionMultiplexer.Connect(configuration);
});


builder.Services.AddTransient<ICartRepository, RedisCartRepo>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
