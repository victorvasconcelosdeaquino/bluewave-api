namespace Bluewave.Sales.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,     // Created, waiting for payment/stock
    Approved = 2,    // Approved for shipping
    Shipped = 3,     // Shipped
    Delivered = 4,   // Delivered
    Canceled = 5     // Canceled
}