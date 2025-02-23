namespace dagnysbageri.Entities
{
    public class Order
    {
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateOnly OrderDate { get; set; }

    public IList<OrderItem> OrderItems { get; set; }
    public Customer Customer { get; set; }   
    }
}