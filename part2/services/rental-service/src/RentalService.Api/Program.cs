using RentalService.Api.Controllers;
using RentalService.Application.Ports;
using RentalService.Application.UseCases;
using RentalService.Infrastructure.Clients;
using RentalService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ErrorHandlingFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IRentalRepository, InMemoryRentalRepository>();

builder.Services.AddHttpClient<IBillingClient, BillingHttpClient>(client =>
{
    var baseUrl = builder.Configuration["Billing:BaseUrl"] ?? "http://localhost:8082";
    client.BaseAddress = new Uri(baseUrl);
});


builder.Services.AddScoped<CreateBookingUseCase>();
builder.Services.AddScoped<StartRentalUseCase>();
builder.Services.AddScoped<ReturnRentalUseCase>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
