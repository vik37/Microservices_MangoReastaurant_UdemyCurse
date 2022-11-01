using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Extensions;
using Mango.Services.Email.Messaging;
using Mango.Services.Email.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;
// Add services to the container.

services.AddDbContext<ApplicationDbContext>(opt =>
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
services.AddScoped<IEmailRepository,EmailRepository>();

var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
services.AddSingleton(new EmailRepository(optionBuilder.Options));
services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

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
