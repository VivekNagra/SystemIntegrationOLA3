using BillingService.Api.Controllers;
using BillingService.Application.Ports;
using BillingService.Application.UseCases;
using BillingService.Infrastructure.Payments;
using BillingService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ErrorHandlingFilter>();
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ports & Adapters registrations
builder.Services.AddSingleton<IChargeRepository, InMemoryChargeRepository>();
builder.Services.AddSingleton<IPaymentProvider, FakePaymentProvider>();


builder.Services.AddScoped<CreateChargeFromRentalUseCase>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
