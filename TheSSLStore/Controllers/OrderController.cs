using Microsoft.AspNetCore.Mvc;
using TheSSLStore.Models;
using TheSSLStore.Services.Order;

namespace TheSSLStore.Controllers;

public class OrderController(IOrderService theSSLStoreApiService) : Controller
{
    private readonly IOrderService _theSSLStoreApiService = theSSLStoreApiService;

    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        const int pageSize = 20;
        try
        {
            var orders = await _theSSLStoreApiService.GetOrdersAsync(pageSize, pageNumber);
            return View(orders);
        }
        catch (Exception ex)
        {
            return View("Error", new ErrorViewModel { Message = ex.Message });
        }
    }
}