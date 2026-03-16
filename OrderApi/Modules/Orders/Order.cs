namespace OrderApi.Modules.Orders;

public sealed class Order
{
    public Guid Id { get; init; }
    public DateTime OrderDate { get; init; } = DateTime.UtcNow;
    public decimal Total { get; init; }
}
