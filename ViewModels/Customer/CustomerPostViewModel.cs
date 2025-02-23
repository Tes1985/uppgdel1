using dagnysbageri.ViewModels.Address;

namespace dagnysbageri.ViewModels.Customer;

public class CustomerPostViewModel
{
     public string CompanyName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string ContactPerson { get; set; }

    public IList<AddressPostViewModel> Addresses { get; set; }
}