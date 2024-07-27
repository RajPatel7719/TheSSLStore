using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TheSSLStore.Models;

namespace TheSSLStore.Services.Order;

public class OrderService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache) : IOrderService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IMemoryCache _cache = cache;
    private readonly string _partnerCode = configuration["TheSSLStore:PartnerCode"];
    private readonly string _authToken = configuration["TheSSLStore:AuthToken"];
    private readonly string _baseUrl = configuration["TheSSLStore:BaseURL"];

    public async Task<OrderViewModel> GetOrdersAsync(int pageSize, int pageNumber)
    {
        var cacheKey = $"orders_{pageSize}_{pageNumber}";
        if (_cache.TryGetValue(cacheKey, out OrderViewModel orders))
        {
            return orders;
        }

        try
        {
            var sslOrders = await CreateRequest(pageNumber, pageSize);

            var totalPages = pageNumber;
            while (sslOrders.Length == pageSize)
            {
                totalPages++;
                
                var nextPageSslOrders = await CreateRequest(totalPages, pageSize);

                if (nextPageSslOrders.Length <= pageSize)
                {
                    break;
                }
            }

            var orderViewModel = new OrderViewModel
            {
                Orders = sslOrders,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
            _cache.Set(cacheKey, orderViewModel, cacheEntryOptions);

            return orderViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error calling API: " + ex.Message);
            throw;
        }
    }

    private async Task<OrderResponse[]> CreateRequest(int pageNumber, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}order/query");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_partnerCode}:{_authToken}")));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var requestBody = new
        {
            AuthRequest = new
            {
                PartnerCode = _partnerCode,
                AuthToken = _authToken,
                ReplayToken = "",
                UserAgent = "",
                IPAddress = ""
            },
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var json = JsonSerializer.Serialize(requestBody);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OrderResponse[]>(responseBody);
    }
}