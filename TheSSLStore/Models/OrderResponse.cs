namespace TheSSLStore.Models;

public class OrderViewModel
{
    public OrderResponse[] Orders { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class OrderResponse
{
    public string TheSSLStoreOrderID { get; set; } = string.Empty;
    public string VendorOrderId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Validity { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string OrderAmount { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; }
}

public class OrderStatus
{
    public string MajorStatus { get; set; } = string.Empty;
}