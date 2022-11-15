using AutoMapper;
using CheckoutSDK.Extensions.Configuration;
using E_Commerce.Payment.Gateway;
using E_Commerce.Payment.Gateway.DataBase;
using E_Commerce.Payment.Gateway.Repository;
using E_Commerce.Payment.Gateway.Repository.Impl;
using E_Commerce.Payment.Gateway.Services;
using E_Commerce.Payment.Gateway.Services.Impl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add database context
var dbName = builder.Configuration["DataBaseName"];
if (dbName == default)
    throw new NullReferenceException("DataBaseName must be configured in settings");

builder.Services.AddDbContext<Database>(options => options
        .UseSqlite($@"data source={AppDomain.CurrentDomain.BaseDirectory}\{dbName};foreign keys=True"));

//Register services
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<IAcquiringBankService, AcquiringBankService>();
builder.Services.AddScoped<IGatewayRepository, GatewayRepository>();

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Add checkout sdk
CheckoutServiceCollection.AddCheckoutSdk(builder.Services, builder.Configuration);

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