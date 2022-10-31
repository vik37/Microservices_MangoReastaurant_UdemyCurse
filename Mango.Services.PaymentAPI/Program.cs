using Mango.MessageBus;
using Mango.Services.PaymentAPI.Extensions;
using Mango.Services.PaymentAPI.Messaging;
using PaymentProcessor;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSingleton<IProcessPayment, ProcessPayment>();
services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
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

app.UseAzureServiceBusConsumer();
app.Run();
