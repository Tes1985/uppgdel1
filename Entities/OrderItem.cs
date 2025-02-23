namespace dagnysbageri.Entities
{
    public class OrderItem
    {
    public int OrderId { get; set; }
    public int SalesProductId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }

    public SalesProduct SalesProduct { get; set; }
    public Order Order { get; set; }   
    }
}