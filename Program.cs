using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System_Parkingowy.Modules.AuthModule;
using System_Parkingowy.Modules.DatabaseModule;
using System_Parkingowy.Modules.NotificationModule;
using System_Parkingowy.Modules.BookingModule;
using System_Parkingowy.Modules.PaymentModule;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System_Parkingowy.Modules.SimulationModule;
using System_Parkingowy.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Ogranicz nasłuchiwanie tylko do localhost:5000
builder.WebHost.UseUrls("http://localhost:5000");

// Rejestracja serwisów w DI
//builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<INotificationFactory, StandardNotificationFactory>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingService, ReservationManager>();
builder.Services.AddScoped<ParkingSimulator>(sp => {
    var sim = new ParkingSimulator(10, 2.0);
    sim.OnSpotStatusChanged += (spotId, occupied, sensorType) => {
        PredictionController.UpdateHistory(spotId, occupied);
    };
    return sim;
});

// Dodaj DbContext EF Core
builder.Services.AddDbContext<ParkingDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ParkingDb;Trusted_Connection=True;"));

// Dodaj Swaggera
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "System Parkingowy API",
        Version = "v1",
        Description = "API do zarządzania parkingiem, rezerwacjami, użytkownikami i płatnościami.\n\n" +
            "# Architektura bazy danych\n" +
            "System korzysta z Entity Framework Core i kontekstu ParkingDbContext. Wszystkie operacje na użytkownikach, miejscach parkingowych i rezerwacjach są realizowane bezpośrednio przez serwisy domenowe (np. AuthService, ReservationManager) z użyciem ParkingDbContext.\n" +
            "\n## Moduł symulacji i sensorów\n" +
            "System posiada moduł symulacji miejsc parkingowych, który generuje zdarzenia zajęcia/zwolnienia miejsc w oparciu o rozkład Poissona i różne typy czujników (ultrasonic, magnetic, camera).\n" +
            "Każde miejsce posiada historię zmian zajętości (timestamp, zajętość, typ czujnika).\n" +
            "API umożliwia pobieranie historii zajętości dla danego miejsca lub wszystkich miejsc (do predykcji).\n" +
            "Możliwe jest ręczne wywołanie zdarzenia zmiany statusu miejsca (np. do testów).\n" +
            "\n## Predykcja zajętości\n" +
            "Dane historyczne mogą być wykorzystane do budowy modeli predykcyjnych (np. ARIMA, ML) i prognozowania dostępności miejsc w przyszłości.\n\n" +
            "## Dostęp do bazy\n" +
            "Dostęp do bazy danych odbywa się przez ParkingDbContext, który jest rejestrowany w DI i konfigurowany do pracy z SQL Server.\n"
    });

    // Przykładowe modele i requesty
    c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddControllers();

var app = builder.Build();

// Włącz Swaggera w trybie deweloperskim
app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.UseMiddleware<SimpleAuthMiddleware>();

app.MapControllers();

app.Run();