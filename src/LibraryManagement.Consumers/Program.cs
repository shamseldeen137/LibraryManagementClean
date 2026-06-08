using LibraryManagement.Consumers;
using LibraryManagement.Infrastructure.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<ConsumerOptions>(builder.Configuration.GetSection("Consumers"));

//builder.Services.AddHostedService<RabbitMqLibraryEventConsumer>();
builder.Services.AddHostedService<KafkaLibraryEventConsumer>();

await builder.Build().RunAsync();
