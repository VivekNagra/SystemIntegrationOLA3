using System.Net.Http.Json;
using RentalService.Application.Dtos;
using RentalService.Application.Ports;

namespace RentalService.Infrastructure.Clients;

/// <summary>
/// HTTP client adapter that calls the Billing service.
/// </summary>
public sealed class BillingHttpClient : IBillingClient
{
    private readonly HttpClient _http;

    public BillingHttpClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<CreateChargeResponseDto> CreateAndCaptureChargeAsync(CreateChargeRequestDto request, CancellationToken ct)
    {
        // Billing endpoint we will implement later:
        // POST http://localhost:8081/charges/from-rental
        var res = await _http.PostAsJsonAsync("/charges/from-rental", request, ct);

        // If billing is down, fail fast (for demo we keep it simple).
        res.EnsureSuccessStatusCode();

        var body = await res.Content.ReadFromJsonAsync<CreateChargeResponseDto>(cancellationToken: ct);
        if (body is null)
            throw new InvalidOperationException("Billing service returned an empty response.");

        return body;
    }
}
