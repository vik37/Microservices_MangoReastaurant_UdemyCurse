using AutoMapper;
using Mango.Services.ProductAPI;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
var services = builder.Services;
string identityUrl = configuration["ServiceUrls:IdentityService"];

// Add services to the container.
services.AddDbContext<ApplicationDbContext>(opt =>
                    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

IMapper mapper = MappingConfig.RegisterMapp().CreateMapper();
services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddScoped<IProductRepository, ProductRepository>();
services.AddControllers();
services.AddEndpointsApiExplorer();

services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.Authority = identityUrl;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
services.AddAuthorization(opt =>
{
    opt.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "mango");
    });
});

services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token",
        Name = HeaderNames.Authorization,
        In = ParameterLocation.Header, // or magic string "Authorization"
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
