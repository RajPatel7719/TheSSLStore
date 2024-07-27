using TheSSLStore.Models;

namespace TheSSLStore.Services.Order;

public interface IOrderService
{
    Task<OrderViewModel> GetOrdersAsync(int pageSize, int pageNumber);
}