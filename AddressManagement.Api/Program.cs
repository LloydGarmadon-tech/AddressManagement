using AddressManagement.Application.Interfaces;
using AddressManagement.Application.Services;
using AddressManagement.Infrastucture.Persistence;
using AddressManagement.Infrastucture.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Datenbankkontext
builder.Services.AddDbContext<AdressDbContext>(options =>
    options.UseInMemoryDatabase("AddressManagement"));

// Repository
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

// Application-Service
builder.Services.AddScoped<AddressService>();

// Controller
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

/// <summary>
/// Einstiegspunkt der Anwendung für Integrationstests.
/// </summary>
public partial class Program
{
}

