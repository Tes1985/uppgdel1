namespace dagnysbageri.Entities
{
    public class Customer
    {
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string ContactPerson { get; set; }

    public IList<CustomerAddress> CustomerAddresses { get; set; }
    public IList<Order> Orders { get; set; }
    }
}