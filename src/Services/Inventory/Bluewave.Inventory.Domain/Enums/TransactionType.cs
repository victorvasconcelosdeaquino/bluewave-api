namespace Bluewave.Inventory.Domain.Enums;

public enum TransactionType
{
    InboundPurchase = 1,
    OutboundProduction = 2,
    OutboundSales = 3,
    TransferIn = 4,
    TransferOut = 5,
    Adjustment = 6,
    Return = 7
}